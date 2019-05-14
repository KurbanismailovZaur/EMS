import math as mt
import numpy as np
import constants as ct


class Math1:
    def __init__(self, amperage=2):
        # сила тока в проводнике
        self.amperage = amperage
        self.nodes = None
        self.nodes_count = 0
        self.c = None

    def set_nodes(self, nodes):
        self.nodes = nodes
        self.nodes_count = len(nodes) - 1

    def set_c(self, c):
        self.c = c

    def set_amperage(self, amperage):
        self.amperage = amperage

    def do(self):
        """
        Нахождение длин отрезков АВ, АС, ВС
        """
        ab = np.zeros(self.nodes_count)
        ac = np.zeros(self.nodes_count)
        bc = np.zeros(self.nodes_count)
        # Вычисление длины АВ
        for i in range(self.nodes_count):
            s = 0
            for j in range(3):
                s += pow((self.nodes[i + 1, j] - self.nodes[i, j]), 2)
            ab[i] = mt.sqrt(s)
        # Вычисление длины AC
        for i in range(self.nodes_count):
            s = 0
            for j in range(3):
                s += pow((self.c[j] - self.nodes[i, j]), 2)
            ac[i] = mt.sqrt(s)
        # Вычисление длины BC
        for i in range(self.nodes_count):
            s = 0
            for j in range(3):
                s += pow((self.c[j] - self.nodes[i + 1, j]), 2)
            bc[i] = mt.sqrt(s)

        """
        Вычисляем косинус углов CAB и ABC
        """
        # Косинус угла CAB
        alfa1 = np.zeros(self.nodes_count)
        for i in range(self.nodes_count):
            s = 0
            for j in range(3):
                s += (self.c[j] - self.nodes[i, j]) * (self.nodes[i + 1, j] - self.nodes[i, j])
            # Проверяем критерий касания точки С и отрезка АB
            if ab[i] != ac[i] + bc[i]:
                alfa1[i] = mt.degrees(mt.acos(round(s / (ab[i] * ac[i]), 8)))
        # 180 - угл ABC
        alfa180 = np.zeros(self.nodes_count)
        for i in range(self.nodes_count):
            s = 0
            for j in range(3):
                s += (self.c[j] - self.nodes[i + 1, j]) * (self.nodes[i, j] - self.nodes[i + 1, j])
            # Проверяем критерий касания точки С и отрезка АВ
            if ab[i] != ac[i] + bc[i]:
                alfa180[i] = mt.degrees(mt.acos(round(s / (ab[i] * bc[i]), 8)))
        # Косинус угла ABC
        alfa2 = np.zeros(self.nodes_count)
        for i in range(self.nodes_count):
            alfa2[i] = 180 - alfa180[i]

        """
        Вычисляем r0
        """
        r0 = np.zeros(self.nodes_count)
        for i in range(self.nodes_count):
            if abs(ac[i]) >= 0.0000000001:
                r0[i] = ac[i] * mt.sin(mt.radians(alfa1[i]))

        """
        Вычисляем B(Тл)
        """
        btl = np.zeros(self.nodes_count)
        for i in range(self.nodes_count):
            if r0[i] != 0:
                btl[i] = ct.NU / (4 * mt.pi) * self.amperage / \
                         r0[i] * (mt.cos(mt.radians(alfa1[i])) - mt.cos(mt.radians(alfa2[i])))

        """
        Получение координат вектора V
        """
        xv = np.zeros(self.nodes_count)
        yv = np.zeros(self.nodes_count)
        zv = np.zeros(self.nodes_count)
        for i in range(self.nodes_count):
            xv[i] = (self.nodes[i + 1, 1] - self.nodes[i, 1]) * (self.c[2] - self.nodes[i, 2]) - \
                    (self.nodes[i + 1, 2] - self.nodes[i, 2]) * (self.c[1] - self.nodes[i, 1])
            yv[i] = (self.nodes[i + 1, 2] - self.nodes[i, 2]) * (self.c[0] - self.nodes[i, 0]) - \
                    (self.nodes[i + 1, 0] - self.nodes[i, 0]) * (self.c[2] - self.nodes[i, 2])
            zv[i] = (self.nodes[i + 1, 0] - self.nodes[i, 0]) * (self.c[1] - self.nodes[i, 1]) - \
                    (self.nodes[i + 1, 1] - self.nodes[i, 1]) * (self.c[0] - self.nodes[i, 0])

        """
        Расчет длины AB x AC
        """
        ab_ac = np.zeros(self.nodes_count)
        for i in range(self.nodes_count):
            ab_ac[i] = mt.sqrt(pow(xv[i], 2) + pow(yv[i], 2) + pow(zv[i], 2))

        """
        Нахождение координат векторов магнитной индукции
        """
        xb = np.zeros(self.nodes_count)
        yb = np.zeros(self.nodes_count)
        zb = np.zeros(self.nodes_count)
        for i in range(self.nodes_count):
            # Вектор по оси Х
            if xv[i] != 0:
                xb[i] = xv[i] / ab_ac[i] * btl[i]
            # Вектор по оси Y
            if yv[i] != 0:
                yb[i] = yv[i] / ab_ac[i] * btl[i]
            # Вектор по оси Z
            if zv[i] != 0:
                zb[i] = zv[i] / ab_ac[i] * btl[i]

        """
        Нахождение координат вектора В
        """
        bsum = np.zeros(3)
        bsum[0] = xb.sum()
        bsum[1] = yb.sum()
        bsum[2] = zb.sum()

        return bsum


if __name__ == "__main__":
    import xlrd
    m1 = Math1()

    book = xlrd.open_workbook("data_import/TestLeads.xls")
    # Цикл по листам
    for sh_i in range(book.nsheets):
        sh = book.sheet_by_index(sh_i)
        lst_rx = range(5, sh.nrows, 1)
        nodes = np.empty((len(lst_rx), 3))
        # Цикл по строкам начиная с 6
        for rx_i, rx in enumerate(lst_rx):
            nodes[rx_i] = sh.row_values(rx)

        m1.set_nodes(nodes)
        m1.set_c([6., 10., 0.])

        bsum = m1.do()
        print(bsum)
