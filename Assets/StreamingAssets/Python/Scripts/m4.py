from classes import Point, Figure
from functions import get_distance

import numpy as np
import constants as ct
import math


class Math4:
    def __init__(self, figures, materials):
        """
        :param figures: итератор по всем фигурам определяющих конструктив изделия
        :param materials: материалы
        """
        self.figures = figures
        self.materials = materials
        self.X0 = None
        self.X1 = None
        self.U = None
        self.C = None
        self.f = None

    def set_u(self, u):
        """
        Задаем точку генерации
        :param u: Point
        """
        self.U = u

    def set_c(self, c):
        """
        Задание точки в которой ищеться напряженность
        :param c: Point
        """
        self.C = c

    def set_f(self, f):
        """
        Задание частот излучения
        :param f: int
        """
        self.f = f

    def do(self, is_magnetic=False):
        """
        Основной алгоритм действий
        :param is_magnetic: False - расчет для электрической состовляющей, True - расчет для магнитной состовляющей
        """
        # Находим разницу
        p = self.C - self.U
        # Находим параметры квадратного уравнения ax2 + bx + c = 0
        a = (p ** 2).sum()

        f = self.f  # для сокращения

        res = 0  # значения ослабления магнитной или электрической составляющей
        for figure in self.figures:
            o = figure.point
            r = figure.r
            po = self.C - o

            b = (po * p).sum() * 2
            c = (po ** 2).sum() - r ** 2

            # Найдем дискриминант
            d = b ** 2 - 4 * a * c

            if d > 0:
                # Находим корни квадратного уравнения
                t1 = (-b - d ** 0.5) / (2 * a)
                t2 = (-b + d ** 0.5) / (2 * a)

                # Найдем точки пересечения сферы с прямой
                T1 = t1 * p + self.C
                T2 = t2 * p + self.C

                # Найдем толщину экрана
                dT = get_distance(T1, T2)

                # Найдем радиус экрана
                re = min(get_distance(self.U, T1), get_distance(self.U, T2))

                # TODO если в справочнике такого материала нет берется материал по умолчанию
                # TODO не учитываем то что значения может и не быть в нужном столбце
                # Найдем проводимость материала фигуры, относительно меди

                try:
                    ec = self.materials.get(figure.material_id, ct.MATERIAL_DEFAULT)[1] / ct.EC_CU

                    # Найдем относительную магнитную проницаемость материала фигуры, относительно меди
                    nu = self.materials.get(figure.material_id, ct.MATERIAL_DEFAULT)[2] / ct.NU_CU
                except Exception:
                    ec = ct.MATERIAL_DEFAULT[1] / ct.EC_CU
                    nu = ct.MATERIAL_DEFAULT[2] / ct.NU_CU

                if is_magnetic:  # расчет для магнитной состовляющей
                    res += 14.6 + 10 * math.log10((f * re ** 2 * ec) / nu) + 131.4 * dT * (f * nu * ec) ** 0.5
                else:  # расчет для электрической состовляющей
                    res += 332 + 10 * math.log10(ec / (re ** 2 * f ** 3 * nu)) + 131.4 * dT * (f * nu * ec) ** 0.5

        # Перевод полученного значения в разы
        res = 10 ** -(res / 20)

        return res


if __name__ == "__main__":
    from storage import Storage
    #
    # import time_and_memory as tm
    #
    # data = np.array([[1, 2, 6, 0.0001, 4]])
    # figures = map(lambda x: Figure(Point(np.array(x[:3])), x[-2], x[-1]), data)
    # materials = Load.get_materials()
    # m = Math4(figures, materials, is_magnetic=True)
    # m.set_u(Point(np.array([1, 2, 0])))
    # m.set_f(500000)
    # m.set_c(Point(np.array([1, 2, 2])))
    # print(m.do())

    # import csv
    # with open("cubes.csv", "w") as f:
    #     writer = csv.writer(f)
    #     writer.writerows(data)

    # tm.log('данные прочитаны')

    # x[:-1] - 8 вершин куба
    # x[-1] - метериал куба
    # преобразуем данные к Cube(Point1, Point2, ..., Point8)
    # cubes = map(lambda x: Cube(np.array(x[:-1]).reshape(8, 3), x[-1]), data)
    #
    # print('=> вычисления...')
    # m = Math4(cubes)
    # m.set_U(Point(np.array([0.012, 0.03, 0.043])), Point(np.array([0.015, 0.03, 0.044])))
    # m.set_c(Point(np.array([0.02, 0.032, 0.04])))
    # m.do()
    # tm.log('вычисления завершины')

    # print('=> обработка данных...')
    # print('Всего кубов: %s' % len(res))
    #
    # # запись полученных кубов в базу данных
    # cursor.execute("DELETE FROM ModelCube")  # удаление данныз и БД
    # conn.commit()
    #
    # for item in res:
    #     cursor.execute(f"INSERT into ModelCube values ({', '.join(str(x) for x in item)})")
    #
    # conn.commit()

    # conn.close()
    #
    # tm.endlog()
