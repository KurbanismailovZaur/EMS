from classes import Point, Figure, Wire, BBA

import sqlite3
import json
import numpy as np


class Storage:
    def __init__(self, db_path):
        self.db_path = db_path

    def get_materials(self):
        """
        Загрузка материалов (1КВИД)
        :return: {0: ('вакуум', None, 1.257e-06, 8.842e-12), 1: ('серебро', 62500000.0, 1.257e-06, None), ...}
        """
        conn = sqlite3.connect(self.db_path)
        cursor = conn.cursor()

        cursor.execute("SELECT * FROM KVID1_REF")
        res = {row[0]: row[1:] for row in cursor.fetchall()}

        conn.close()

        return res

    def get_types_wire(self):
        """
        Загрузка материалов (4КВИД)
        :return: {'м1': (2, 0.002, 2, 0.003, 0.0003, 27, 2, 0.006, 0.0003, 27, 0.0146), ...}
        """
        conn = sqlite3.connect(self.db_path)
        cursor = conn.cursor()

        cursor.execute("SELECT * FROM KVID4_REF")
        res = {row[0]: row[1:] for row in cursor.fetchall()}

        conn.close()

        return res

    def get_wires(self):
        """
        Загрузка 5КВИД
        :return: [Wire, Wire ..., Wire]
        """
        conn = sqlite3.connect(self.db_path)
        cursor = conn.cursor()

        cursor.execute("SELECT * FROM KVID5")
        ports = {row[0]: row[1:] for row in cursor.fetchall()}

        cursor.execute("SELECT * FROM KVID3")
        data_kvid3 = cursor.fetchall()

        # создаем словарь портов приемников со списком (пустым) физических портов если порт это порт ветвящийся
        # и значение внутреннего сопротивления если порт физический
        dct_ports_recipient = {}
        for wire in data_kvid3:
            port_recipient = wire[3]
            if port_recipient.split('_')[-1].lower() == 'у':
                dct_ports_recipient[port_recipient] = []
            else:
                dct_ports_recipient[port_recipient] = ports[port_recipient][4]
        # заполняем списки по ветвящимся портам
        for wire in data_kvid3:
            port_source = wire[2]
            port_recipient = wire[3]
            if port_source.split('_')[-1].lower() == 'у':
                dct_ports_recipient[port_source].append(port_recipient)

        # рекурсивный поиск сопротивления по принципу параллельного соединения резисторов
        def find_R2(lst_p):
            res = []
            for p in lst_p:
                val = dct_ports_recipient[p]
                if isinstance(val, list):
                    res.append(find_R2(val))
                else:
                    res.append(val)

            return round(1 / sum([1 / r for r in res]), 2)
        # замена всех списков на соответветствюющие вычислинные значения
        for port, value in dct_ports_recipient.items():
            if isinstance(value, list):
                dct_ports_recipient[port] = find_R2(value)

        res = []
        wires = {}  # справочник реальных проводов
        for wire in data_kvid3:
            wire_id = wire[0]
            port_source = wire[2]
            port_recipient = wire[3]
            points = list(map(lambda x: Point(np.array(x[:3]), metal1=x[-2], metal2=x[-1]), json.loads(wire[-1])))
            w_real = wire_id.split('_')[0]  # берем название провода до символа подчеркивания ("_")
            temp_wire = wires.get(w_real, None)
            if temp_wire:  # в случае если мы рассматриваем ветвлящийся провод
                f, U, R1, R2 = temp_wire
            else:  # в случае основного провода
                f = ports[port_source][6]
                U = ports[port_source][5]
                R1 = ports[port_source][4]
                R2 = dct_ports_recipient[port_recipient]

                wires[w_real] = [f, U, R1, R2]

            materials = self.get_materials()  # загрузка материалов
            types_wire = self.get_types_wire()  # загрузка типов проводов
            type_wire = types_wire[wire[1]]  # получаем параметры типа кабеля

            res.append(Wire(wire_id, points, f, U, R1, R2, materials, type_wire))

        conn.close()

        return res

    def get_BBAs(self):
        """
        Загрузка блоков аппаратуры (2КВИД)
        :return: итерационнай объект BBA
        """
        conn = sqlite3.connect(self.db_path)
        cursor = conn.cursor()

        cursor.execute("SELECT * FROM KVID2")
        res = map(lambda x: BBA(*x[:2], Point(np.array(x[2:-1])), json.loads(x[-1])), cursor.fetchall())

        conn.close()

        return res

    def get_set_points(self):
        """
        Загрузка заданных точек (6КВИД)
        :return: итерационнай объект Point
        """
        conn = sqlite3.connect(self.db_path)
        cursor = conn.cursor()

        cursor.execute("SELECT * FROM KVID6")
        res = map(lambda x: Point(np.array(x[1:]), point_id=x[0]), cursor.fetchall())

        conn.close()

        return res

    def get_limits(self):
        """
        Загрузка пределов (8КВИД)
        :return: ({}, {})
        """
        conn = sqlite3.connect(self.db_path)
        cursor = conn.cursor()

        cursor.execute("SELECT * FROM KVID8_1")
        data_8_1 = {row[0]: row[1:] for row in cursor.fetchall()}

        cursor.execute("SELECT * FROM KVID8_2")
        data_8_2 = {row[1]: row[2:] for row in cursor.fetchall()}  # отбрасываем первую колонку, т.к. она не нужна

        conn.close()

        return data_8_1, data_8_2

    def get_figures(self):
        """
        Загрузка фигур
        :return: итерационнай объект Figure(Point, r, material_id)
        """
        conn = sqlite3.connect(self.db_path)
        cursor = conn.cursor()

        cursor.execute("SELECT * FROM ModelFigure")
        res = map(lambda x: Figure(Point(np.array(x[:3])), x[-2], x[-1]), cursor.fetchall())

        conn.close()

        return res

    def get_planes(self):
        """
        Загрузка плоскостей
        :return: list
        """
        conn = sqlite3.connect(self.db_path)
        cursor = conn.cursor()

        cursor.execute("SELECT * FROM ModelPoint")
        res = cursor.fetchall()

        conn.close()

        return res

    def get_model_sizes(self):
        """
        Загрузка размеров (радиусов) модели
        :return: list
        """
        conn = sqlite3.connect(self.db_path)
        cursor = conn.cursor()

        cursor.execute("SELECT * FROM ModelSizes")
        res = cursor.fetchone()  # читаем только одну строку

        conn.close()

        return res

    def get_result_m3(self):
        """
        Загрузка результатов по M3
        :return: итерационнай объект list
        """
        conn = sqlite3.connect(self.db_path)
        cursor = conn.cursor()

        cursor.execute("SELECT * FROM ResultM3")
        res = {row[0]: [*row[1:5], json.loads(row[5]), json.loads(row[6]),
                        json.loads(row[7])] for row in cursor.fetchall()}

        conn.close()

        return res

    def get_result_m2(self):
        """
        Загрузка результатов по M2
        :return: итерационнай объект list
        """
        conn = sqlite3.connect(self.db_path)
        cursor = conn.cursor()

        cursor.execute("SELECT * FROM ResultM2")
        res = {row[0]: [json.loads(row[1]), json.loads(row[2]), json.loads(row[3]),
                        row[-1]] for row in cursor.fetchall()}

        conn.close()

        return res

    def get_select_points(self):
        """
        Загрузка выбранный точек
        :return: итерационнай объект str
        """
        conn = sqlite3.connect(self.db_path)
        cursor = conn.cursor()

        cursor.execute("SELECT * FROM SelectPoint")
        res = [x[0] for x in cursor.fetchall()]

        conn.close()

        return res

    def get_select_wires(self):
        """
        Загрузка выбранный кабелей
        :return: итерационнай объект list
        """
        conn = sqlite3.connect(self.db_path)
        cursor = conn.cursor()

        cursor.execute("SELECT * FROM SelectWire")
        res = [x[0] for x in cursor.fetchall()]

        conn.close()

        return res

    def set_figures(self, data):
        """
        Запись сформированных фигур
        :param data: np.array
        :return: кол-во сформированных фигур
        """
        conn = sqlite3.connect(self.db_path)
        cursor = conn.cursor()

        cursor.execute("DELETE FROM ModelFigure")  # удаление данных из БД
        conn.commit()

        count = sum(1 for _ in
                    map(lambda item:
                        cursor.execute(f"INSERT into ModelFigure values ({', '.join(str(x) for x in item)})"),
                        data))

        conn.commit()

        conn.close()

        return count  # всего фигур

    def set_result_m3_times(self, data):
        """
        Запись результата работы m3 с шагом по времени для моделя визуализации
        :param data: list
        :return: кол-во вычислинных точек относительно 36 промежутков времени
        """
        conn = sqlite3.connect(self.db_path)
        cursor = conn.cursor()

        cursor.execute("DELETE FROM ResultM3Times")  # удаление данных из БД
        conn.commit()

        count = sum(1 for _ in
                    map(lambda item:
                        cursor.execute(f"INSERT into ResultM3Times values ({', '.join('?' * 2)})",
                                       [item[0], json.dumps(item[1])]),
                        data))

        conn.commit()

        conn.close()

        return count

    def set_result_m3(self, data):
        """
        Запись результата работы m3 для отчета
        :param data: lst
        :return: кол-во записанных строк
        """
        conn = sqlite3.connect(self.db_path)
        cursor = conn.cursor()

        cursor.execute("DELETE FROM ResultM3")  # удаление данных из БД
        conn.commit()

        count = sum(1 for _ in
                    map(lambda item:
                        cursor.execute(f"INSERT into ResultM3 values ({', '.join('?' * 8)})",
                                       [*item[:-3], json.dumps(item[-3]), json.dumps(item[-2]), json.dumps(item[-1])]),
                        data))

        conn.commit()

        conn.close()

        return count

    def set_result_m2(self, data):
        """
        Запись результата работы m2
        :param data: lst
        :return: кол-во записанных строк
        """
        conn = sqlite3.connect(self.db_path)
        cursor = conn.cursor()

        cursor.execute("DELETE FROM ResultM2")  # удаление данных из БД
        conn.commit()

        count = sum(1 for _ in
                    map(lambda item:
                        cursor.execute(f"INSERT into ResultM2 values ({', '.join('?' * 5)})",
                                       [item[0],
                                        json.dumps(item[1]), json.dumps(item[2]), json.dumps(item[3]), item[4]]),
                        data))

        conn.commit()

        conn.close()

        return count

    def set_progress(self, percent):
        """
        Запись прогресса завершения определенной задачи в процентах
        :param percent: int
        """
        conn = sqlite3.connect(self.db_path)
        cursor = conn.cursor()

        cursor.execute(f"UPDATE Progress SET percent = '{percent}'")

        conn.commit()

        conn.close()


if __name__ == "__main__":
    storage = Storage('../db/ems.bytes')
    # print(list(storage.get_BBAs()))
