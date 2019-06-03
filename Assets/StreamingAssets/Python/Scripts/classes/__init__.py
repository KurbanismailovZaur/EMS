from functions import get_distance

import math as mt
import constants as ct


class Point:
    def __init__(self, point, point_id=None, metal1=None, metal2=None):
        """
        :param point: np.array([x, y, z])
        :param point_id: идентификатор
        :param metal1: значение метализации по первому экрану
        :param metal2: значение метализации по второму экрану
        """
        self.point = point
        self.x, self.y, self.z = point
        self.id = point_id
        self.metal1 = metal1
        self.metal2 = metal2

    def __add__(self, other):
        return Point(self.point + other.point)

    def __sub__(self, other):
        return Point(self.point - other.point)

    def __mul__(self, other):
        try:
            val = other.point
        except AttributeError:
            val = other
        return Point(self.point * val)

    def __rmul__(self, other):
        return Point(self.point * other)

    def __truediv__(self, other):
        try:
            val = other.point
        except AttributeError:
            val = other
        return Point(self.point / val)

    def __rtruediv__(self, other):
        return Point(self.point / other)

    def __pow__(self, power, modulo=None):
        return Point(self.point ** power)

    def __str__(self):
        return f"<{self.__class__.__name__} x:{self.x} y:{self.y} z:{self.z}>"

    def __getitem__(self, item):
        return self.point[item]

    def __setitem__(self, key, value):
        self.point[key] = value

    def sum(self):
        return self.point.sum()


class Figure:
    def __init__(self, point, r, material_id):
        """
        :param point: точка центра окружности
        :param r: радиус
        :param material_id: идентификатор материала
        """
        self.point = point
        self.r = r
        self.material_id = material_id

    def __str__(self):
        return f"<{self.__class__.__name__} material_id:{self.material_id}>"


class Wire:
    def __init__(self, wire_id, points, f, U, R1, R2, materials, type_wire):
        """
        :param wire_id: идентификатор кабеля
        :param points: точки изгибов
        :param f: рабочая частота
        :param I: сила тока
        :param materials: справочник материалов
        :param type_wire: параметры типа кабеля
        """
        self.id = wire_id
        self.points = points
        self.f = f
        self.U = U
        self.R1 = R1
        self.R2 = R2
        self.I = U / (R1 + R2)
        # Определим для дальнейших расчетов ослабления угловую частоту
        self.w = f * 2 * mt.pi
        # Определение метализирован ли кабель
        m_indexes = [[], []]  # индексы точек метализации по экранам
        hs = 0  # среднее значение метализации экранов
        for ind, point in enumerate(points):
            if point.metal1:
                m_indexes[0].append(ind)
                hs += point.metal1
            if point.metal2:
                m_indexes[1].append(ind)
                hs += point.metal2
        self.is_metallization = any(m_indexes)
        if self.is_metallization:
            hs /= len(m_indexes[0]) + len(m_indexes[1])
        # Диаметр жилы кабеля
        self.diam = type_wire[1]

        # Находим коэффициент ослабление эраном провода (стр 43)
        SEs = 1
        SHs = 1

        shields = []
        if type_wire[2]:  # если 1-й экран существует
            shields.append(type_wire[2:6])
        if type_wire[6]:  # если 2-й экран существует
            shields.append(type_wire[6:10])

        for shield, r, d, insulation in shields:
            ec = materials[shield][1]
            nu = materials[shield][2]
            ea = materials[insulation][3]

            K = (self.w * ec * nu) ** 0.5
            Zm = (self.w * nu / ec) ** 0.5

            Zde = 1 / (self.w * ea * r)
            Zdm = self.w * nu * r

            # Коэффициент ослобляющий электрическую состовляющую за счет экрана (не актуален если есть метализация)
            if self.is_metallization:
                SEs *= 0
            else:
                SEs *= 1 / mt.cosh(K * d) * 1 / (1 + 0.5 * (Zde / Zm + Zm / Zde) * mt.tanh(K * d))
            # Коэффициент ослобляющий магнитную состовляющую за счет экрана
            SHs *= 1 / mt.cosh(K * d) * 1 / (1 + 0.5 * (Zdm / Zm + Zm / Zdm) * mt.tanh(K * d))

        # Находим коэффициент ослабления за счет метализации (SE здесь нет впринципе)
        SHm = 1

        dS = Wire.get_distance(points)  # длина провода
        for shield_i, lst_index in enumerate(m_indexes):
            if not lst_index:  # если метализация отсутсвует
                continue
            shield, r, d, insulation = shields[shield_i]
            ec = materials[shield][1]
            nu = materials[shield][2]

            if len(lst_index) > 1:
                points_metal = [0] + lst_index + [len(points) - 1]  # добавляем индексы точек x0 и x1
                ls = 0  # находим среднее расстояние точек изгибов до ближайшей точки метализации
                # пары x0m1, m1m2, m2x1
                points_metal_comba = [(points_metal[i], points_metal[i + 1]) for i in range(len(points_metal) - 1)]
                for i, m_i in enumerate(points_metal_comba):
                    # если первая или последняя точка метализации
                    if i == 0 or i == len(points_metal_comba) - 1:
                        ls += (Wire.get_distance(points[m_i[0]:m_i[1] + 1]) ** 2) / 2
                    else:  # если точка метализации находится в середине
                        ls += (Wire.get_distance(points[m_i[0]:m_i[1] + 1]) ** 2) / 4
                ls /= dS

                # довычисление средней суммы значения по колонке метализации
                nu /= ct.NU_CU
                V = 4 * 10 ** -6 * d * (nu * ec * f) ** 0.5

                # Сопротивления экрана на заданной частоте
                R = 0.318 * ls / r * (mt.sinh(V) + mt.sin(V)) / (mt.cosh(V) - mt.cos(V)) * ((nu * f) / ec) ** 0.5

                L = 2 * 10 ** -7 * dS * mt.log(2 * hs / r + 1)
                # Коэффициент ослобляющий магнитную состовляющую
                SHm *= R / (R + self.w * L)

        self.SE = SEs
        self.SH = SHs * SHm

    def __str__(self):
        return f"<{self.__class__.__name__} wire_id:{self.id}>"

    def get_fragment(self):
        for i in range(len(self.points) - 1):
            yield self.points[i], self.points[i + 1]

    def get_is_metallization(self):
        return self.is_metallization

    def get_id_real(self):
        return self.id.split('_')[0]

    @staticmethod
    def get_distance(points):
        """
        :param points: часть провода для которого вы должны вычислить расстояние
        :return: длина части провода
        """
        res = 0
        for i in range(len(points) - 1):
            res += get_distance(points[i], points[i + 1])
        return res


class BBA:
    def __init__(self, bba_id, name, point, frequencies):
        """
        Класс - блок бортовой аппаратуры
        :param bba_id: идентификатор ББА
        :param name: наименование
        :param point: геометрический центр
        :param frequencies: диапазоны частот со значениями напряженности
        """
        self.id = bba_id
        self.name = name
        self.point = point
        self.frequencies = frequencies

    def __str__(self):
        return f"<{self.__class__.__name__} name:{self.name}>"

    def get_min_frequency(self):
        return min([f[0] for f in self.frequencies])

    def get_frequency_range(self):
        for i in range(len(self.frequencies) - 1):
            yield self.frequencies[i][0], self.frequencies[i + 1][0], self.frequencies[i][1]
