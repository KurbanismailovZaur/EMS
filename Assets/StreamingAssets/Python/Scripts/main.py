from storage import Storage
from m2 import Math2
from m3 import Math3
from m4 import Math4
from m4_create_figures import Math4Figure

import numpy as np
import itertools

# TODO добавить емкостное взаимоедйствие проводов в m2
# TODO добавить БА
# TODO привести в порядок каталог проекта
# TODO генерация отчетов
# TODO проверить зависимост ИД в скрипах


def default(*args):  # функция заглушка для аргументов вызывающих функции
    pass


def create_figures(db_path):
    # Загрузка исходных данных
    storage = Storage(db_path)
    planes = storage.get_planes()

    m = Math4Figure(np.array(planes), np.array([1, 3, 1]))
    figures = m.do()

    # запись полученных кубов в базу данных
    print(storage.set_figures(figures))


def script_m3(db_path):
    # Загрузка исходных данных
    storage = Storage(db_path)
    materials = storage.get_materials()
    wires = storage.get_wires()
    BAs = storage.get_BAs()
    set_points = storage.get_set_points()
    set_points_limits, ba_limits = storage.get_limits()
    figures = storage.get_figures()

    # Найдем минимальную частоту для вычислении 36 шагов времени
    f_min = min(*[w.f for w in wires], *[f[0] for ba in BAs for f in ba[-1]])
    t_step = (1 / f_min) / 36  # шаг времени

    m3 = Math3()
    m4 = Math4(figures, materials)

    res = []
    res_report = []
    for point in set_points:
        # Блок кода получения значений для шкалы времени
        time_values = []
        for t in itertools.accumulate([t_step] * 36):  # шкала времени
            m3.set_t(t)

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

            data_wires.append([wire.id, *Ew, Ew_sum, wire.f, wire.f, is_excessive])

            E += Ew
        res_report.append([point.id, *E, (E ** 2).sum() ** 0.5, data_wires])
    storage.set_result_m3_times(res)
    storage.set_result_m3(res_report)
    print('ok')


def script_m2(db_path):
    # Загрузка исходных данных
    storage = Storage(db_path)
    materials = storage.get_materials()
    wires = storage.get_wires()
    figures = storage.get_figures()

    # Проверка на кол-во проводов, их должно быть больше чем 1
    if len(wires) < 2:
        raise Exception("The number of wires must be greater than 1")

    m2 = Math2()
    m4 = Math4(figures, materials)

    dct_wires = {}
    for index_compare, wire_compare in enumerate(wires[:-1]):  # кабель источник
        for index_current, wire_current in enumerate(wires[index_compare + 1:]):  # кабель приемник
            m4.set_f(wire_current.f)
            H = 0
            for A2, B2 in wire_current.get_fragment():
                m4.set_u((A2 + B2) / 2)
                for A1, B1 in wire_compare.get_fragment():
                    m2.set_fragments(A1, B1, A2, B2)
                    H += m2.do()

                    m4.set_c((A1 + B1) / 2)
                    H *= m4.do(is_magnetic=True)

            Uh = H * wire_current.w * wire_current.I

            Uh *= wire_compare.SH * wire_current.SH

            # Создаю словаря проводов со списком значений влияния на данный провод других проводов
            if dct_wires.get(wire_current.id) is None:
                dct_wires[wire_current.id] = []
            dct_wires[wire_current.id].append([wire_compare.id, wire_compare.f, Uh])

            if dct_wires.get(wire_compare.id) is None:
                dct_wires[wire_compare.id] = []
            dct_wires[wire_compare.id].append([wire_current.id, wire_current.f, Uh])

    res = []
    for wire_id, values in dct_wires.items():
        res.append([wire_id, values, sum([x[-1] for x in values])])

    storage.set_result_m2(res)
    print('ok')


if __name__ == "__main__":
    from settings import DB_PATH

    import argparse

    parser = argparse.ArgumentParser()
    parser.add_argument('-db', action='store', dest='db_path', default=DB_PATH, help='Путь к файлу базы данных')
    parser.add_argument('--create_figures', action='store_const', const=create_figures, default=default,
                        help='Преобразование плоскостей в фигуры')
    parser.add_argument('--script_m3', action='store_const', const=script_m3, default=default,
                        help='Расчет напряженности электрического поля в заданных точках')
    parser.add_argument('--script_m2', action='store_const', const=script_m2, default=default,
                        help='Расчет взаимного воздействия кабелей БКС и БА на БКС')

    results = parser.parse_args()
    results.create_figures(results.db_path)
    results.script_m3(results.db_path)
    results.script_m2(results.db_path)
