from functions import get_distance
from classes import Point

import math as mt
import numpy as np
import constants as ct


class Math2:
    def __init__(self):
        self.A1 = None
        self.B1 = None
        self.A2 = None
        self.B2 = None
        
    def set_fragments(self, A1, B1, A2, B2):
        self.A1 = A1
        self.B1 = B1
        self.A2 = A2
        self.B2 = B2

    def do(self):
        # вычисление векторов фрагментов
        p_A1B1 = self.B1 - self.A1
        p_A2B2 = self.B2 - self.A2
        # вычисление длин отрезков провода (AB, BC, CD, etc.)
        d_compare = get_distance(self.A1, self.B1)
        d_current = get_distance(self.A2, self.B2)
        # вычисление длин отрезков между проводами (A1A2, A1B2, A1C2, etc.)
        d_A1A2 = get_distance(self.A1, self.A2)
        d_B1A2 = get_distance(self.B1, self.A2)
        d_A1B2 = get_distance(self.A1, self.B2)
        d_B1B2 = get_distance(self.B1, self.B2)
        # вычисляем косинус угла между отрезками
        cos_segment = round((p_A1B1 * p_A2B2).sum() / (d_compare * d_current), 3)

        p1x, p1y, p1z = p_A1B1
        p2x, p2y, p2z = p_A2B2

        """
        Определения положения двух отрезков относительно друг друга:
        - соосно
        - параллельно
        - общий случай
        """
        if cos_segment in (1, -1):  # два отрезка распаложенны параллельно либо соосно
            half_perimeter = (d_A1A2 + d_B1A2 + d_compare) / 2
            max_sides_triangle = max(d_A1A2, d_B1A2, d_compare)
            if half_perimeter == max_sides_triangle:  # соблюдение критерия соосности
                # TODO CE = 0
                # отрезки расположенны относительно друг друга соосно
                # расстояние перекрытия между проекциями отрезков
                d = min(d_A1A2, d_B1A2, d_A1B2, d_B1B2)

                M = ct.NU / 4 * mt.pi \
                    * ((d_compare + d_current + d)
                       * mt.log(d_compare + d_current + d) + d * mt.log(d)
                       - (d_compare + d) * mt.log(d_compare + d)
                       - (d_current + d) * mt.log(d_current + d)
                       * cos_segment)
            else:
                # отрезки расположенны относительно друг друга паралельно
                s = (half_perimeter
                     * (half_perimeter - d_compare)
                     * (half_perimeter - d_B1A2)
                     * (half_perimeter - d_A1A2)) ** 0.5
                # расстояние между проводами
                h = round(2 * s / d_compare, 3)

                if cos_segment == 1:
                    h_b1a2 = round((d_B1A2 ** 2 - h ** 2) ** 0.5, 3)
                    h_b1b2 = round((d_B1B2 ** 2 - h ** 2) ** 0.5, 3)

                    if d_current + h_b1a2 == h_b1b2:
                        d = h_b1a2
                    else:
                        d = -h_b1a2
                else:
                    h_a1a2 = round((d_A1A2 ** 2 - h ** 2) ** 0.5, 2)
                    h_a1b2 = round((d_A1B2 ** 2 - h ** 2) ** 0.5, 2)

                    if d_current + h_a1a2 == h_a1b2:
                        d = h_a1a2
                    else:
                        d = -h_a1a2

                if d >= 0:
                    pass  # TODO CE = 0
                else:
                    pass  # de = abs(d)
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
            # вычисление вектора нормали
            nx = p1y * p2z - p1z * p2y
            ny = -(p1x * p2z - p1z * p2x)
            nz = p1x * p2y - p1y * p2x

            # параметры плоскостей для каждого отрезка
            apl_compare = apl_current = nx
            bpl_compare = bpl_current = ny
            cpl_compare = cpl_current = nz
            dpl_compare = -nx * self.A1.x - ny * self.A1.y - nz * self.A1.z
            dpl_current = -nx * self.A2.x - ny * self.A2.y - nz * self.A2.z

            # параметр прямой заданной парамметрически которая является
            t1 = (-dpl_current - apl_current * self.A1.x - bpl_current * self.A1.y
                  - cpl_current * self.A1.z) / (apl_current ** 2 + bpl_current ** 2
                                                     + cpl_current ** 2)
            # координаты точни a1нов
            a1bx = self.A1.x + t1 * apl_current
            a1by = self.A1.y + t1 * bpl_current
            a1bz = self.A1.z + t1 * cpl_current

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
                o2x = (a1bx * p1y * p2x - self.A2.x * p2y * p1x - a1by * p1x * p2x
                       + self.A2.y * p1x * p2x) / (p1y * p2x - p2y * p1x)

                if p1x == 0:
                    ty = (o2x - self.A2.x) / p2x
                    o2y = p2y * ty + self.A2.y
                    o2z = p2z * ty + self.A2.z
                else:
                    tn = (o2x - a1bx) / p1x
                    o2y = p1y * tn + a1by
                    o2z = p1z * tn + a1bz
            elif xyz >= 10:
                o2z = (a1bz * p1x * p2z - self.A2.z * p2x * p1z - a1bx * p1z * p2z
                       + self.A2.x * p1z * p2z) / (p1x * p2z - p2x * p1z)

                if p1z == 0:
                    ty = (o2z - self.A2.z) / p2z
                    o2x = p2x * ty + self.A2.x
                    o2y = p2y * ty + self.A2.y
                else:
                    tn = (o2z - a1bz) / p1z
                    o2x = p1x * tn + a1bx
                    o2y = p1y * tn + a1by

            elif xyz >= 1:
                o2y = (a1by * p1z * p2y - self.A2.y * p2z * p1y - a1bz * p1y * p2y
                       + self.A2.z * p1y * p2y) / (p1z * p2y - p2z * p1y)

                if p1y == 0:
                    ty = (o2y - self.A2.y) / p2y
                    o2x = p2x * ty + self.A2.x
                    o2z = p2z * ty + self.A2.z
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

            x1 = get_distance(point_o1, self.A1)
            x2 = get_distance(point_o1, self.B1)

            y1 = get_distance(point_o2, self.A2)
            y2 = get_distance(point_o2, self.B2)

            if round(abs(d_compare - x2) - x1, 3) != 0:
                x2 = -x2
            x1 = x2 - d_compare

            if round(abs(d_current - y2) - y1, 3) != 0:
                y2 = -y2
            y1 = y2 - d_current

            fi = mt.acos(cos_segment)

            if h != 0:
                AM = mt.atan((x1 + y1 + d_A1A2) / h * mt.tan(fi / 2)) \
                    + mt.atan((x2 + y2 + d_B1B2) / h * mt.tan(fi / 2)) \
                    - mt.atan((x1 + y2 + d_A1B2) / h * mt.tan(fi / 2)) \
                    - mt.atan((x2 + y1 + d_B1A2) / h * mt.tan(fi / 2))
            else:
                AM = 0

            M = 2 * ct.NU / 4 * mt.pi * cos_segment \
                * (x2 * mt.tanh(d_current / (d_B1B2 + d_B1A2))
                   + y2 * mt.tanh(d_compare / (d_B1B2 + d_A1B2))
                   - x1 * mt.tanh(d_current / (d_A1A2 + d_A1B2))
                   - y1 * mt.tanh(d_compare / (d_A1A2 + d_B1A2))
                   + h / mt.sin(fi) * AM)

        return M

    # TODO CE в случае соосности равна 0


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
