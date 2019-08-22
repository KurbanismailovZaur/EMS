from functions import get_distance, get_cos
from classes import Point

import math as mt
import numpy as np
import constants as ct


class Math2:
    def __init__(self):
        self.a1 = None
        self.a2 = None
        self.b1 = None
        self.b2 = None
        self.wire_a = None
        self.wire_b = None
        self.BBA = None
        self.BBA_f = None
        self.BBA_E = None

    def set_wires(self, wire_a, wire_b):
        """
        :param wire_a: Wire
        :param wire_b: Wire
        """
        self.wire_a = wire_a
        self.wire_b = wire_b

    def set_bba(self, bba):
        self.BBA = bba

    def set_range_params(self, f, e_id):
        self.BBA_f = f
        self.BBA_E = e_id

    def set_fragments(self, a1, a2, b1, b2):
        """
        :param a1: начала фрагмента a
        :param a2: конец фрагмента a
        :param b1: начала фрагмента b
        :param b2: конец фрагмента b
        """
        self.a1 = a1
        self.a2 = a2
        self.b1 = b1
        self.b2 = b2

    def do(self, is_bba=False):
        if is_bba:
            C = (self.b1 + self.b2) / 2
            p_GC = C - self.BBA.point
            p_b1b2 = self.b2 - self.b1
            d_GC = get_distance(self.BBA.point, C)
            d_current = get_distance(self.b1, self.b2)
            cos_alfa = get_cos(p_GC, p_b1b2, d_GC, d_current)
            sin_alfa = mt.sin(mt.acos(cos_alfa))
            r = get_distance(self.BBA.point, C)
            w = 2 * mt.pi * self.BBA_f
            er = self.BBA_E * ((1 / (w * r ** 3) - w / (r * ct.CC ** 2) + 1 / (ct.CC * r ** 2))
                               / (1 / w - w / ct.CC ** 2 + 1 / ct.CC))

            Uf = mt.copysign(d_current / 2 * er * sin_alfa, cos_alfa)

            return Uf

        # вычисление векторов фрагментов
        p_a1a2 = self.a2 - self.a1
        p_b1b2 = self.b2 - self.b1
        # вычисление длин отрезков провода (AB, BC, CD, etc.)
        d_compare = round(get_distance(self.a1, self.a2), 3)
        d_current = round(get_distance(self.b1, self.b2), 3)
        # вычисление длин отрезков между проводами (a1b1, a1b2, a1C2, etc.)
        d_a1b1 = round(get_distance(self.a1, self.b1), 3)
        d_a2b1 = round(get_distance(self.a2, self.b1), 3)
        d_a1b2 = round(get_distance(self.a1, self.b2), 3)
        d_a2b2 = round(get_distance(self.a2, self.b2), 3)
        # вычисляем косинус угла между отрезками
        cos_segment = get_cos(p_a1a2, p_b1b2, d_compare, d_current)

        p1x, p1y, p1z = p_a1a2
        p2x, p2y, p2z = p_b1b2

        CE = 0  # величина электрической емкости между проводами

        a_uid = ''
        for ch in self.wire_a.id:
            a_uid += f"\\suka{str(ord(ch)).rjust(4, '0')}"

        b_uid = ''
        for ch in self.wire_b.id:
            b_uid += f"\\suka{str(ord(ch)).rjust(4, '0')}"

        # Проверяем на пересечение узлами отрезков
        if d_a1b1 + d_a2b1 == d_compare \
                or d_a1b2 + d_a2b2 == d_compare \
                or d_a1b1 + d_a1b2 == d_current \
                or d_a2b1 + d_a2b2 == d_current:
            raise Exception(f"Cable crossing: {a_uid} with {b_uid}")

        """
        Определения положения двух отрезков относительно друг друга:
        - соосно CE = 0
        - параллельно
        - общий случай
        """
        if cos_segment in (1, -1):  # два отрезка распаложенны параллельно либо соосно
            half_perimeter = (d_a1b1 + d_a2b1 + d_compare) / 2
            max_sides_triangle = max(d_a1b1, d_a2b1, d_compare)
            if half_perimeter == max_sides_triangle:  # соблюдение критерия соосности
                # отрезки расположенны относительно друг друга соосно
                # расстояние перекрытия между проекциями отрезков
                d = min(d_a1b1, d_a2b1, d_a1b2, d_a2b2)

                M = ct.NU / (4 * mt.pi) \
                    * ((d_compare + d_current + d)
                       * mt.log(d_compare + d_current + d) + d * mt.log(d)
                       - (d_compare + d) * mt.log(d_compare + d)
                       - (d_current + d) * mt.log(d_current + d)) * cos_segment
            else:
                # отрезки расположенны относительно друг друга паралельно
                s = (half_perimeter
                     * (half_perimeter - d_compare)
                     * (half_perimeter - d_a2b1)
                     * (half_perimeter - d_a1b1)) ** 0.5
                # расстояние между проводами
                h = round(2 * s / d_compare, 3)

                if cos_segment == 1:
                    h_a2b1 = round((d_a2b1 ** 2 - h ** 2) ** 0.5, 3)
                    h_a2b2 = round((d_a2b2 ** 2 - h ** 2) ** 0.5, 3)

                    if d_current + h_a2b1 == h_a2b2:
                        d = h_a2b1
                    else:
                        d = -h_a2b1
                else:
                    h_a1b1 = round((d_a1b1 ** 2 - h ** 2) ** 0.5, 2)
                    h_a1b2 = round((d_a1b2 ** 2 - h ** 2) ** 0.5, 2)

                    if d_current + h_a1b1 == h_a1b2:
                        d = h_a1b1
                    else:
                        d = -h_a1b1

                # алфа, бэта, гамма, дельта - совокупность отрезков для упрощения
                af = d_compare + d_current + d
                bt = d_compare + d
                gm = d_current + d
                dt = d

                M = ct.NU / (4 * mt.pi) \
                    * (af * mt.log(af + mt.hypot(af, h))
                       - bt * mt.log(bt + mt.hypot(bt, h))
                       - gm * mt.log(gm + mt.hypot(gm, h))
                       + dt * mt.log(dt + mt.hypot(dt, h))
                       - mt.hypot(af, h) + mt.hypot(bt, h)
                       + mt.hypot(gm, h) - mt.hypot(dt, h)) * cos_segment
        else:  # общий случай
            # Проверяем на пересечения кабелей
            complan = (self.a1.x - self.b1.x) * (p2z * p1y - p2y * p1z) \
                      - (self.a1.y - self.b1.y) * (p2z * p1x - p2x * p1z) \
                      + (self.a1.z - self.b1.z) * (p2y * p1x - p2x * p1y)
            if complan == 0:
                zero_xyz = np.array([0, 0, 0])

                ab1_x = (self.b1.z - self.a1.z) * p1y - (self.b1.y - self.a1.y) * p1z
                ab1_y = -((self.b1.z - self.a1.z) * p1x - (self.b1.x - self.a1.x) * p1z)
                ab1_z = (self.b1.y - self.a1.y) * p1x - (self.b1.x - self.a1.x) * p1y
                ab2_x = (self.b2.z - self.a1.z) * p1y - (self.b2.y - self.a1.y) * p1z
                ab2_y = -((self.b2.z - self.a1.z) * p1x - (self.b2.x - self.a1.x) * p1z)
                ab2_z = (self.b2.y - self.a1.y) * p1x - (self.b2.x - self.a1.x) * p1y
                ab1 = np.array([ab1_x, ab1_y, ab1_z])
                ab2 = np.array([ab2_x, ab2_y, ab2_z])
                p_ab1 = ab1 - zero_xyz
                p_ab2 = ab2 - zero_xyz
                d_ab1 = get_distance(zero_xyz, ab1)
                d_ab2 = get_distance(zero_xyz, ab2)
                cosab = get_cos(p_ab1, p_ab2, d_ab1, d_ab2)

                ba1_x = (self.a1.z - self.b1.z) * p2y - (self.a1.y - self.b1.y) * p2z
                ba1_y = -((self.a1.z - self.b1.z) * p2x - (self.a1.x - self.b1.x) * p2z)
                ba1_z = (self.a1.y - self.b1.y) * p2x - (self.a1.x - self.b1.x) * p2y
                ba2_x = (self.a2.z - self.b1.z) * p2y - (self.a2.y - self.b1.y) * p2z
                ba2_y = -((self.a2.z - self.b1.z) * p2x - (self.a2.x - self.b1.x) * p2z)
                ba2_z = (self.a2.y - self.b1.y) * p2x - (self.a2.x - self.b1.x) * p2y
                ba1 = np.array([ba1_x, ba1_y, ba1_z])
                ba2 = np.array([ba2_x, ba2_y, ba2_z])
                p_ba1 = ba1 - zero_xyz
                p_ba2 = ba2 - zero_xyz
                d_ba1 = get_distance(zero_xyz, ba1)
                d_ba2 = get_distance(zero_xyz, ba2)
                cosba = get_cos(p_ba1, p_ba2, d_ba1, d_ba2)

                if cosab == -1 and cosba == -1:
                    raise Exception(f"Cable crossing: {a_uid} with {b_uid}")

            # вычисление вектора нормали
            nx = p1y * p2z - p1z * p2y
            ny = -(p1x * p2z - p1z * p2x)
            nz = p1x * p2y - p1y * p2x

            # параметры плоскостей для каждого отрезка
            apl_compare = apl_current = nx
            bpl_compare = bpl_current = ny
            cpl_compare = cpl_current = nz
            dpl_compare = -nx * self.a1.x - ny * self.a1.y - nz * self.a1.z
            dpl_current = -nx * self.b1.x - ny * self.b1.y - nz * self.b1.z

            # параметр прямой заданной парамметрически которая является
            t1 = (-dpl_current - apl_current * self.a1.x - bpl_current * self.a1.y
                  - cpl_current * self.a1.z) / (apl_current ** 2 + bpl_current ** 2
                                                + cpl_current ** 2)
            # координаты точни a1нов
            a1bx = self.a1.x + t1 * apl_current
            a1by = self.a1.y + t1 * bpl_current
            a1bz = self.a1.z + t1 * cpl_current

            xy = xz = yz = 0
            if p1y * p2x - p2y * p1x != 0:
                xy = 100
            if p1z * p2x - p2z * p1x != 0:
                xz = 10
            if p1z * p2y - p2z * p1y != 0:
                yz = 1
            xyz = xy + xz + yz

            # находим координаты точки o2
            if xyz >= 100:
                o2x = (a1bx * p1y * p2x - self.b1.x * p2y * p1x - a1by * p1x * p2x
                       + self.b1.y * p1x * p2x) / (p1y * p2x - p2y * p1x)

                if p1x == 0:
                    ty = (o2x - self.b1.x) / p2x
                    o2y = p2y * ty + self.b1.y
                    o2z = p2z * ty + self.b1.z
                else:
                    tn = (o2x - a1bx) / p1x
                    o2y = p1y * tn + a1by
                    o2z = p1z * tn + a1bz
            elif xyz >= 10:
                o2z = (a1bz * p1x * p2z - self.b1.z * p2x * p1z - a1bx * p1z * p2z
                       + self.b1.x * p1z * p2z) / (p1x * p2z - p2x * p1z)

                if p1z == 0:
                    ty = (o2z - self.b1.z) / p2z
                    o2x = p2x * ty + self.b1.x
                    o2y = p2y * ty + self.b1.y
                else:
                    tn = (o2z - a1bz) / p1z
                    o2x = p1x * tn + a1bx
                    o2y = p1y * tn + a1by

            elif xyz >= 1:
                o2y = (a1by * p1z * p2y - self.b1.y * p2z * p1y - a1bz * p1y * p2y
                       + self.b1.z * p1y * p2y) / (p1z * p2y - p2z * p1y)

                if p1y == 0:
                    ty = (o2y - self.b1.y) / p2y
                    o2x = p2x * ty + self.b1.x
                    o2z = p2z * ty + self.b1.z
                else:
                    tn = (o2y - a1by) / p1y
                    o2x = p1x * tn + a1bx
                    o2z = p1z * tn + a1bz

            t2 = (-dpl_compare - apl_compare * o2x - bpl_compare * o2y
                  - cpl_compare * o2z) / (apl_current ** 2 + bpl_current ** 2 + cpl_current ** 2)

            o1x = o2x + t2 * apl_compare
            o1y = o2y + t2 * bpl_compare
            o1z = o2z + t2 * cpl_compare

            point_o1 = Point(np.array([o1x, o1y, o1z]))
            point_o2 = Point(np.array([o2x, o2y, o2z]))
            # расстояние между плоскостями
            h = get_distance(point_o1, point_o2)

            x1 = get_distance(point_o1, self.a1)
            x2 = get_distance(point_o1, self.a2)

            y1 = get_distance(point_o2, self.b1)
            y2 = get_distance(point_o2, self.b2)

            if round(abs(d_compare - x2) - x1, 3) != 0:
                x2 = -x2
            x1 = x2 - d_compare

            if round(abs(d_current - y2) - y1, 3) != 0:
                y2 = -y2
            y1 = y2 - d_current

            fi = mt.acos(cos_segment)

            if h != 0:
                AM = mt.atan((x1 + y1 + d_a1b1) / h * mt.tan(fi / 2)) \
                     + mt.atan((x2 + y2 + d_a2b2) / h * mt.tan(fi / 2)) \
                     - mt.atan((x1 + y2 + d_a1b2) / h * mt.tan(fi / 2)) \
                     - mt.atan((x2 + y1 + d_a2b1) / h * mt.tan(fi / 2))
            else:
                AM = 0

            M = 2 * ct.NU / (4 * mt.pi) * cos_segment \
                * (x2 * mt.atanh(d_current / (d_a2b2 + d_a2b1))
                   + y2 * mt.atanh(d_compare / (d_a2b2 + d_a1b2))
                   - x1 * mt.atanh(d_current / (d_a1b1 + d_a1b2))
                   - y1 * mt.atanh(d_compare / (d_a1b1 + d_a2b1))
                   + h / mt.sin(fi) * AM)

        # Блок вычисления Емкосного воздействия
        # Если метализация есть хотябы на одном проводе то Uc12 = 0
        Uc12 = 0
        if not (self.wire_a.get_is_metallization() or self.wire_b.get_is_metallization()) and cos_segment != 0:
            if cos_segment > 0:
                cos_alfa1 = get_cos(p_a1a2, self.b1 - self.a1, d_compare, d_a1b1)
                cos_alfa2 = get_cos(self.a1 - self.a2, self.b2 - self.a2, d_compare, d_a2b2)

                if (cos_alfa1 >= 0 and cos_alfa2 >= 0) or (cos_alfa1 <= 0 and cos_alfa2 <= 0):
                    Up = 1
                else:  # если оба косинуса имеют разные знаки
                    Up = max(mt.sin(mt.acos(cos_alfa1)), mt.sin(mt.acos(cos_alfa2)))

            else:  # cos_segment < 0
                cos_alfa1 = get_cos(p_a1a2, self.b2 - self.a1, d_compare, d_a1b2)
                cos_alfa2 = get_cos(self.a1 - self.a2, self.b1 - self.a2, d_compare, d_a2b1)

                if cos_alfa1 * cos_alfa2 < 0:
                    Up = max(mt.sin(mt.acos(cos_alfa1)), mt.sin(mt.acos(cos_alfa2)))
                else:
                    Up = 1

            # длина влияния расматриваемых отрезков проводов
            de = min(d_compare, d_current) * Up * cos_segment
            # найдем растояние между фрагментами
            he = get_distance((self.a1 + self.a2) / 2, (self.b1 + self.b2) / 2)
            # найдем среднеарифиметическое значение диаметров жил взаимовлияющих проводов
            diam = (self.wire_a.diam + self.wire_b.diam) / 2
            # найдем виличину электрической емкости между отрезками проводов
            if he != 0:
                CE = (mt.pi * ct.EPS) / mt.log((2 * he) / diam + ((he / diam) ** 2 - 1) ** 0.5) * de
                # найдем емкостное сопротивление
                Zc = abs(1 / (self.wire_a.w * CE))
                # найдем сумарное сопротивление на портах провода b
                Zb = self.wire_b.R1 + self.wire_b.R2
                # найдем напряжение переданное от провода a к проводу b
                Uc12 = self.wire_a.U * Zb / (Zb + Zc)
            else:
                Uc12 = 0

        return Uc12, M


if __name__ == "__main__":
    import xlrd

    wires = []
    book = xlrd.open_workbook("data_import/TestLeads2.xls")
    # Цикл по листам
    for sh_i in range(book.nsheets):
        sh = book.sheet_by_index(sh_i)
        lst_rx = range(5, sh.nrows, 1)
        nodes = []
        # Цикл по строкам начиная с 6
        for rx_i, rx in enumerate(lst_rx):
            nodes.append(sh.row_values(rx))

        wires.append(nodes)

    [print(wire) for wire in wires]

    m2 = Math2(wires)

    print(m2.do())
