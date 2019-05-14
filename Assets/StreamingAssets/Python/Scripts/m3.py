from classes import Point

import math as mt
import numpy as np
import constants as ct


class Math3:
    def __init__(self):
        self.wire = None
        self.X0 = None
        self.X1 = None
        self.C = None
        # время
        self.t = 0

    def set_wire(self, wire):
        """
        :param wire: Wire
        """
        self.wire = wire

    def set_fragment(self, p1, p2):
        self.X0 = p1
        self.X1 = p2

    def set_c(self, c):
        self.C = c

    def set_t(self, t):
        self.t = t

    def do(self, limit_a=0):
        """
        Основной алгоритм действий
        """
        if self.wire.get_is_metallization():  # если присутсвует на кабеле метализация то мы не рассчитываем алгоритм
            return np.zeros(3), 0

        # Вычисляем волновое число
        wk = self.wire.w / ct.CC

        # TODO Добавить вариант просчета БА
        # G = Point([])
        # r = get_distance(G, self.C)


        """
        Вычисления:
        - вектора прямой
        - r0-rC
        - (r0-rC)*p
        - pp
        """
        # вычисления вектора прямой
        pxyz = self.X1 - self.X0
        # вычисление r0-rC
        r0rc = self.X0 - self.C
        # вычисление (r0-rC)*p
        r0rc_p = (r0rc * pxyz).sum()
        # вычисление p*p
        pp = (pxyz ** 2).sum()

        """
        Вычисление rb
        """
        rb = self.X0 - r0rc_p / pp * pxyz

        """
        Вычисления:
        - пределов a b
        - проекции С
        """
        # итерационный блок вычисления пределов a b
        limit_b = pp
        # итерационный блок вычисления проекции С
        n = ((rb - self.X0) ** 2).sum()
        h = ((self.C - rb) ** 2).sum()
        # блок вычисления пределов a b
        limit_b = limit_b ** 0.5 + limit_a
        # блок вычисления проекции С
        n = n ** 0.5 + limit_a
        h = h ** 0.5

        """
        Алгоритм Start
        """
        count_iter = 100

        dx = (limit_b - limit_a) / count_iter

        # искомые элементы напряженности
        erx = 0
        ery = 0
        ettx = 0
        etty = 0
        for it in range(count_iter):
            x = it * dx + dx / 2 + limit_a
            xt = it * dx + limit_a
            bx = n - x
            r = (bx ** 2 + h ** 2) ** 0.5

            costt = bx / r
            sintt = h / r

            dl = self.wire.I * (1 / wk * (mt.cos(self.wire.w * (self.t - (xt + dx) / ct.CC)) - mt.cos(self.wire.w * (self.t - xt / ct.CC))) - dx * mt.sin(self.wire.w * self.t)) / 2

            erx += wk * dl / (2 * mt.pi * ct.EPS * self.wire.w * r ** 2) * (mt.sin(self.wire.w * self.t - wk * r) / (wk * r) + mt.cos(self.wire.w * self.t - wk * r)) * costt ** 2
            ery += wk * dl / (2 * mt.pi * ct.EPS * self.wire.w * r ** 2) * (mt.sin(self.wire.w * self.t - wk * r) / (wk * r) + mt.cos(self.wire.w * self.t - wk * r)) * costt * sintt

            ettx -= wk ** 2 * dl / (4 * mt.pi * ct.EPS * self.wire.w * r) * (mt.cos(self.wire.w * self.t - wk * r) / (wk * r) + (1 / ((wk * r) ** 2) - 1) * mt.sin(self.wire.w * self.t - wk * r)) * sintt ** 2
            etty += wk ** 2 * dl / (4 * mt.pi * ct.EPS * self.wire.w * r) * (mt.cos(self.wire.w * self.t - wk * r) / (wk * r) + (1 / ((wk * r) ** 2) - 1) * mt.sin(self.wire.w * self.t - wk * r)) * sintt * costt

        ex = erx + ettx
        ey = ery + etty

        """
        Алгоритм End
        """

        """
        Переход от локальных к базовым координатам
        """

        lbx = pxyz / (limit_b - limit_a)
        lby = Point(np.array([0, 0, 0]))
        if h != 0:
            lby = (self.C - rb) / h

        # финальные значения по координатам
        ec = ex * lbx + ey * lby

        # ослабление электрической составляющей экраном провода
        ec = ec * self.wire.SE

        return ec.point, limit_b


if __name__ == "__main__":
    pass
    # 1 / frequency / 9
    # from classes import Point
    #
    # m = Math3(500000, 70, 200, 500)
    # m.set_fragment(Point(np.array([-0.95, 2.40, -0.95])), Point(np.array([-0.95, 2.40, 0.95])))
    # m.set_c(Point(np.array([0.95, 2.40, 0.00])))
    #
    # print(m.do())

    # # Генерируем точки
    # X = np.arange(0.00, 20.00, 0.3)
    # Y = np.arange(0.00, 20.00, 0.3)
    # res = np.zeros((len(X), len(Y)))
    #
    # for i, x in enumerate(X):
    #     for j, y in enumerate(Y):
    #         m3_new.set_c([x, y, 0])
    #         ee = m3_new.do()
    #         temp = 0
    #         for j in range(3):
    #             temp += pow(ee[j], 2)
    #         res[i][j] = mt.sqrt(temp)
    # print(res)







