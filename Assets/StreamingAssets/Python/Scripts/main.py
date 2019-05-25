import os
if os.name == 'nt':  # для Windows
    import sys

    # Add vendor directory to module search path
    parent_dir = os.path.abspath(os.path.dirname(__file__))
    vendor_dir = os.path.join(parent_dir, 'python-embed-amd64/lib')

    sys.path.append(vendor_dir)
    sys.path.append(parent_dir)


from storage import Storage
from m2 import Math2
from m3 import Math3
from m4 import Math4
from m4_create_figures import Math4Figure
from report import Report

import numpy as np
import math as mt
import itertools

# TODO привести в порядок каталог проекта
# TODO проверить зависимост ИД в скрипах
# TODO добавить скрип валидации входных данных


def default(*args):  # функция заглушка для аргументов вызывающих функции
    pass


def create_figures(db_path):
    # Загрузка исходных данных
    storage = Storage(db_path)

    storage.set_progress('75%')

    planes = storage.get_planes()

    m = Math4Figure(np.array(planes), np.array([1, 3, 1]))

    storage.set_progress('85%')

    # Нормирования геометрических параметров модели
    m.normalization_model()

    storage.set_progress('95%')

    figures = m.do()

    storage.set_progress('100%')

    # запись полученных кубов в базу данных
    storage.set_figures(figures)
    print('ok')


def script_m3(db_path):
    # Загрузка исходных данных
    storage = Storage(db_path)
    materials = storage.get_materials()
    wires = storage.get_wires()
    BBAs = list(storage.get_BBAs())
    set_points = list(storage.get_set_points())
    set_points_limits, ba_limits = storage.get_limits()
    figures = storage.get_figures()

    storage.set_progress('0%')
    k_percent = 100 / len(set_points)
    v_percent = k_percent

    # Найдем минимальную частоту для вычислении 36 шагов времени
    # Для кабелей
    f_min = min(*[w.f for w in wires], *[bba.get_min_frequency() for bba in BBAs])
    t_step = (1 / f_min) / 36  # шаг времени
    time_wires = list(itertools.accumulate([t_step] * 36))
    # Для ББА
    f_max = max([(f_start + f_end) / 2 for bba in BBAs for f_start, f_end, value in bba.get_frequency_range()])
    t_step = (1 / f_max) / 36
    time_bbas = list(itertools.accumulate([t_step] * 36))

    m3 = Math3()
    m4 = Math4(figures, materials)

    res = []
    res_report = []
    for point in set_points:
        # Блок кода получения значений для шкалы времени
        time_values = []
        for t in range(36):  # шкала времени
            m3.set_t(time_wires[t])

            E = np.zeros(3)  #  напряженность электрического поля в заданной точке
            for wire in wires:  # цикл по кабелям
                m3.set_wire(wire)
                m3.set_c(point)

                m4.set_f(wire.f)
                m4.set_c(point)

                limit_a = 0
                Ew = np.zeros(3)  #  напряженность электрического поля по проводам
                for p1, p2 in wire.get_fragment():  # цикл по фрагментам кабеля
                    m3.set_fragment(p1, p2)
                    ec, limit_a = m3.do(limit_a)

                    if not wire.get_is_metallization():
                        m4.set_u((p1 + p2) / 2)
                        SEf = m4.do()
                        ec = ec * SEf

                    Ew += ec
                E += Ew

            for bba in BBAs:  # цикл по ББА
                m3.set_bba(bba)
                Ebba = 0
                for f_start, f_end, e_id in bba.get_frequency_range():
                    f_middle = (f_start + f_end) / 2
                    m3.set_range_params(f_middle, e_id)
                    er = m3.do(is_bba=True)
                    # ослабление влияние ББА на заданную точку
                    m4.set_u(bba.point)
                    SEf = m4.do()
                    er = er * SEf
                    Ebba += er * mt.sin(2 * mt.pi * f_middle * time_bbas[t])

                # Сложение напряженности по БКС с напряженностью по ББА
                Ebba /= 3 ** 0.5
                for i in range(3):
                    E[i] = mt.copysign(abs(E[i]) + Ebba, E[i])

            time_values.append((E ** 2).sum() ** 0.5)
        res.append([point.id, time_values])

        # Блок кода получения значений для отчета
        data_wires = []
        E = np.zeros(3)  #  напряженность электрического поля в заданной точке
        for wire in wires:  # цикл по кабелям
            m3.set_wire(wire)
            m3.set_c(point)
            m3.set_t((1 / wire.f) / 36 * 9)

            m4.set_f(wire.f)
            m4.set_c(point)

            limit_a = 0
            Ew = np.zeros(3)  #  напряженность электрического поля по проводам
            for p1, p2 in wire.get_fragment():  # цикл по фрагментам кабеля
                m3.set_fragment(p1, p2)
                ec, limit_a = m3.do(limit_a)

                if not wire.get_is_metallization():
                    m4.set_u((p1 + p2) / 2)
                    SEf = m4.do()
                    ec = ec * SEf

                Ew += ec

            Ew_sum = (Ew ** 2).sum() ** 0.5

            # Проверяем на превышения значения
            is_excessive = False
            point_limit = set_points_limits.get(point.id)
            if point_limit:
                if (point_limit[1] <= wire.f <= point_limit[2]) and (Ew_sum > point_limit[0]):
                    is_excessive = True

            data_wires.append([wire.id, *Ew, Ew_sum, wire.f, is_excessive])

            E += Ew

        data_bbas = []
        for bba in BBAs:  # цикл по ББА
            m3.set_bba(bba)

            data_frequencies = []
            Ebba = 0
            for f_start, f_end, e_id in bba.get_frequency_range():
                m3.set_range_params((f_start + f_end) / 2, e_id)
                er = m3.do(is_bba=True)
                # ослабление влияние ББА на заданную точку
                m4.set_u(bba.point)
                SEf = m4.do()
                er = er * SEf
                Ebba += er

                # Проверяем на превышения значения
                is_excessive = False
                point_limit = set_points_limits.get(point.id)
                if point_limit:
                    if ((point_limit[1] <= f_start <= point_limit[2]) or (point_limit[1] <= f_end <= point_limit[2])) \
                            and (abs(er) > point_limit[0]):
                        is_excessive = True

                data_frequencies.append([er, f_start, f_end, is_excessive])

            data_bbas.append([bba.id, data_frequencies])

        dct_wires = {}
        for item in data_wires:
            if dct_wires.get(item[5]) is None:
                dct_wires[item[5]] = 0
            dct_wires[item[5]] += item[4]

        dct_bbas = {}
        for item in [x for frequencies in data_bbas for x in frequencies[1]]:
            key = (item[1], item[2])
            if dct_bbas.get(key) is None:
                dct_bbas[key] = 0
            dct_bbas[key] += item[0]

        for f_bba_min, f_bba_max in dct_bbas:
            for f_wire in dct_wires:
                if f_bba_min <= f_wire <= f_bba_max:
                    dct_wires[f_wire] += dct_bbas[(f_bba_min, f_bba_max)]

        data_report = []
        for key, value in dct_wires.items():
            # Проверяем на превышения значения
            is_excessive = False
            point_limit = set_points_limits.get(point.id)
            if point_limit:
                if (point_limit[1] <= key <= point_limit[2]) and (abs(value) > point_limit[0]):
                    is_excessive = True
            data_report.append([key, value, is_excessive])
        for key, value in dct_bbas.items():
            # Проверяем на превышения значения
            is_excessive = False
            point_limit = set_points_limits.get(point.id)
            if point_limit:
                if ((point_limit[1] <= key[0] <= point_limit[2]) or (point_limit[1] <= key[1] <= point_limit[2])) \
                        and (abs(value) > point_limit[0]):
                    is_excessive = True
            data_report.append([key, value, is_excessive])

        data_report.sort(key=lambda x: x[0] if isinstance(x[0], tuple) else (x[0], x[0]))

        res_report.append([point.id, *E, (E ** 2).sum() ** 0.5, data_wires, data_bbas, data_report])

        # Запись прогресса
        storage.set_progress(f'{round(v_percent)}%')
        v_percent += k_percent

    storage.set_result_m3_times(res)
    storage.set_result_m3(res_report)
    print('ok')


def script_m2(db_path):
    # Загрузка исходных данных
    storage = Storage(db_path)
    materials = storage.get_materials()
    wires = storage.get_wires()
    BBAs = list(storage.get_BBAs())
    set_points_limits, ba_limits = storage.get_limits()
    figures = storage.get_figures()

    storage.set_progress('0%')
    k_percent = 100 / len(wires) / 2
    v_percent = k_percent

    # Проверка на кол-во проводов, их должно быть больше чем 1
    if len(wires) < 2:
        raise Exception("The number of wires must be greater than 1")

    m2 = Math2()
    m4 = Math4(figures, materials)

    # Взаимодейтсвием между кабелями БКС
    dct_sources = {}
    for wire_a in wires:  # кабель источник
        for wire_b in wires:  # кабель приемник
            # провод сам с собой не сравниваем и ветвящиеся провода между собой не сравниваем
            if wire_a == wire_b or wire_a.get_id_real() == wire_b.get_id_real():
                continue

            m2.set_wires(wire_a, wire_b)
            m4.set_f(wire_b.f)
            E = H = 0
            for b1, b2 in wire_b.get_fragment():
                m4.set_c((b1 + b2) / 2)
                for a1, a2 in wire_a.get_fragment():
                    m2.set_fragments(a1, a2, b1, b2)
                    Ucf, Hf = m2.do()

                    m4.set_u((a1 + a2) / 2)

                    Hf *= m4.do(is_magnetic=True)
                    if not (wire_a.get_is_metallization() or wire_b.get_is_metallization()):
                        Ucf *= m4.do()
                    E += Ucf
                    H += Hf

            Uc = E * wire_a.SE * wire_b.SE

            Uh = H * wire_b.w * wire_b.I
            Uh *= wire_a.SH * wire_b.SH

            # Создаю словаря проводов со списком значений влияния на данный провод других проводов
            if dct_sources.get(wire_b.id) is None:
                dct_sources[wire_b.id] = [[], []]  # [0] - кабеля, [1] - ББА
            dct_sources[wire_b.id][0].append([wire_a.id, wire_a.f, Uc, Uh])

        # Запись прогресса
        storage.set_progress(f'{round(v_percent)}%')
        v_percent += k_percent

    # Взаимодействие между БКС и ББА
    for wire in wires:
        for bba in BBAs:  # цикл по ББА
            m2.set_bba(bba)
            m4.set_u(bba.point)

            data_frequencies = []
            for f_start, f_end, e_id in bba.get_frequency_range():
                m2.set_range_params((f_start + f_end) / 2, e_id)

                Ebba = 0
                for b1, b2 in wire.get_fragment():
                    m2.set_fragments(None, None, b1, b2)
                    m4.set_c((b1 + b2) / 2)

                    Uf = m2.do(is_bba=True)
                    # ослабление влияние ББА на заданную точку
                    SEf = m4.do()
                    Uf *= SEf
                    Ebba += Uf

                data_frequencies.append([Ebba, f_start, f_end])

            dct_sources[wire.id][1].append([bba.id, data_frequencies])

        # Запись прогресса
        storage.set_progress(f'{round(v_percent)}%')
        v_percent += k_percent

    res = []
    for wire_id, values in dct_sources.items():
        wires_sum = bbas_sum = 0

        dct_wires = {}
        for item in values[0]:
            if dct_wires.get(item[1]) is None:
                dct_wires[item[1]] = 0
            dct_wires[item[1]] += item[-2] + item[-1]
            wires_sum += item[-2] + item[-1]

        dct_bbas = {}
        for item in [x for frequencies in values[1] for x in frequencies[1]]:
            key = (item[1], item[2])
            if dct_bbas.get(key) is None:
                dct_bbas[key] = 0
            dct_bbas[key] += item[0]
            bbas_sum += item[0]

        for f_bba_min, f_bba_max in dct_bbas:
            for f_wire in dct_wires:
                if f_bba_min <= f_wire <= f_bba_max:
                    dct_wires[f_wire] += dct_bbas[(f_bba_min, f_bba_max)]

        # Проверяем на превышения значения
        data_report = []
        for key, value in dct_wires.items():
            is_excessive = False
            wire_limit = ba_limits.get(wire_id)
            if wire_limit:
                if (wire_limit[1] <= key <= wire_limit[2]) and (abs(value) > wire_limit[0]):
                    is_excessive = True
            data_report.append([value, key, is_excessive])

        for key, value in dct_bbas.items():
            is_excessive = False
            wire_limit = ba_limits.get(wire_id)
            if wire_limit:
                if ((wire_limit[1] <= key[0] <= wire_limit[2]) or (wire_limit[1] <= key[1] <= wire_limit[2])) \
                        and (abs(value) > wire_limit[0]):
                    is_excessive = True
            data_report.append([value, key, is_excessive])

        data_report.sort(key=lambda x: x[1] if isinstance(x[1], tuple) else (x[1], x[1]))

        res.append([wire_id, values[0], values[1], data_report, wires_sum + bbas_sum])

    storage.set_result_m2(res)
    print('ok')


def script_report(db_path, xlsx_path):
    # Загрузка данных
    storage = Storage(db_path)
    results_m3 = storage.get_result_m3()
    results_m2 = storage.get_result_m2()
    select_points = storage.get_select_points()
    select_wires = storage.get_select_wires()

    report = Report(results_m3, results_m2, select_points, select_wires, path=xlsx_path)
    report.do()

    print('ok')


if __name__ == "__main__":
    from settings import DB_PATH

    import argparse

    parser = argparse.ArgumentParser()
    parser.add_argument('-db', action='store', dest='db_path', default=DB_PATH, help='Путь к файлу базы данных')
    parser.add_argument('-xlsx', action='store', dest='xlsx_path', default='report.xlsx', help='Путь к файлу отчета')
    parser.add_argument('--create_figures', action='store_const', const=create_figures, default=default,
                        help='Преобразование плоскостей в фигуры')
    parser.add_argument('--script_m3', action='store_const', const=script_m3, default=default,
                        help='Расчет напряженности электрического поля в заданных точках')
    parser.add_argument('--script_m2', action='store_const', const=script_m2, default=default,
                        help='Расчет взаимного воздействия кабелей БКС и БА на БКС')
    parser.add_argument('--script_report', action='store_const', const=script_report, default=default,
                        help='Генерация отчета')

    results = parser.parse_args()
    results.create_figures(results.db_path)
    results.script_m3(results.db_path)
    results.script_m2(results.db_path)
    results.script_report(results.db_path, results.xlsx_path)
    # script_m3('./db/ems.bytes')
    # script_report('./db/ems.bytes', './report.xlsx')
