from functions import get_distance
from numba import jit

import numpy as np


class Math4Figure:
    def __init__(self, planes, r_xyz):
        """
        :param planes: плоскости из которых состоит модель - np.array(<три точки>, материал)
        :param r_xyz: ренальные размеры, где  X, Z - радиусы, а Y - высота изделия
        """
        self.planes = planes
        self.planes_length = self.planes.shape[0]

        # TODO на данный момент ниже описанная длина задается константой относительно отдельной 3D модели
        # Значения по осям X, Y, Z должны лежать в пределах r_xyz (исходя из размеров модели)
        # где r_xyz[i] - ренальные размеры (радиусы) изделия по осям
        if r_xyz.all():  # реальные значения должны быть больше нуля
            self.r_xyz = np.abs(r_xyz)
        else:
            raise ValueError('real sizes should not be zero')

    @staticmethod
    @jit(nopython=True)
    def relocation(pl, mid_xyz, k_xyz, pl_length):
        """
        Функция нормирования отдельной плоскости
        :param pl: плоскость состоящая из трех точек
        :param mid_xyz: значения центрирования множества точек по осям
        :param k_xyz: масштабирующий коэффициент
        :param pl_length: тех.параметр - длина массива определяющий плоскость (10)
        :return: плоскость с нормированными геометрическими параметрами
        """
        for i in range(3):  # цикл по трем осям
            for j in range(i, pl_length - 1, 3):  # цикл по x<i>, y<i> z<i>
                pl[j] = (pl[j] - mid_xyz[i]) * k_xyz[i]
        return pl

    def normalization_model(self):
        """
        Нормирования геометрических параметров модели
        """

        # Нахождение максимальных и минимальных значений по осям
        max_all = self.planes.max(axis=0)
        min_all = self.planes.min(axis=0)

        max_xyz = np.empty(3)
        min_xyz = np.empty(3)
        for i in range(3):
            max_xyz[i] = max_all[i:-1:3].max()  # i - x, y, z | -1 - не учитывая material_id | 3 - шаг по колонкам осей
            min_xyz[i] = min_all[i:-1:3].min()

        if np.all(np.abs(max_xyz) <= self.r_xyz) and np.all(np.abs(min_xyz) <= self.r_xyz):
            return False

        mid_xyz = np.empty(3)
        k_xyz = np.empty(3)  # масштабирующий коэффициент
        for i in range(3):
            if i != 1:  # если ось не Y
                # Для центрирования множества точек по осям
                # определяется половина сумм полученных величин по каждой оси
                mid_xyz[i] = (max_xyz[i] + min_xyz[i]) / 2
            else:
                mid_xyz[i] = min_xyz[i]
            # Для масштабирования множества точек по осям, определяется масштабирующий коэффициент
            k_xyz[i] = self.r_xyz[i] / (max_xyz[i] - mid_xyz[i])

        # Нормирование геометрических параметров точечной модели
        self.planes = np.array([self.relocation(pl, mid_xyz, k_xyz, pl.shape[0]) for pl in self.planes])
        # Округление значений по осям до 2-х знаков
        self.planes = np.around(self.planes, decimals=2)
        # Удаление дубликатов
        self.planes = np.unique(self.planes, axis=0)

        return True

    @staticmethod
    @jit(nopython=False)
    def find_figures(planes):
        figures = []

        # Находим фигуры
        for i in range(planes.shape[0]):
            plane = planes[i]
            a = plane[:3]
            b = plane[3:6]
            c = plane[6:9]
            material_id = plane[-1]

            # Определяем длинны сторон трехугольника
            ab = get_distance(a, b)
            bc = get_distance(b, c)
            ac = get_distance(a, c)

            # Исключаем деление на 0
            if bc == 0 or ac == 0:
                continue

            # Находим радиус вписанной окружности
            r = (((-ab + bc + ac) * (ab - bc + ac) * (ab + bc - ac)) / (4 * (ab + bc + ac))) ** 0.5
            # Отсекаем фигуры в ходе вычисления которых возникла ошибка связанная с плавующей запятой
            # и фигуры радиус которых равен 0
            if np.isnan(r) or r == 0:
                continue

            # Находим соотношение АВ/ВС
            l1 = ab / bc

            # Находим координаты точки М
            m = np.empty(3)
            for i in range(3):
                m[i] = (a[i] + l1 * c[i]) / (1 + l1)

            # Точка пересечения биссектрис (инцентр) делит биссектрису ВМ по соотношению:
            l2 = (ab + bc) / ac

            # Находим координаты точки О (центр вписанной окружности)
            o = np.empty(3)
            for i in range(3):
                o[i] = (b[i] + l2 * m[i]) / (1 + l2)

            # Записываем:
            # -	три координаты геометрического центра (Ox, Oy, Oz)
            # -	длина действующего радиуса (R)
            # -	код материала
            figures.append([o[0], o[1], o[2], r, material_id])

        return figures

    def do(self):

        # Нормирования геометрических параметров модели
        if self.normalization_model():
            pass
            # print('==> данные нормированны')

        # Находим фигуры
        figures = self.find_figures(self.planes)
        # Удаление дубликатов
        figures = np.unique(figures, axis=0)

        return figures


if __name__ == "__main__":
    # Elapsed time: 0:01:01.784
    # Memory used: 167.211mb

    from settings import DB_PATH

    import sqlite3
    import time_and_memory

    conn = sqlite3.connect(DB_PATH)
    cursor = conn.cursor()

    print('=> чтение данных...')
    cursor.execute("SELECT * FROM ModelPoint")

    m = Math4Figure(np.array(cursor.fetchall()), np.array([1, 3, 1]))
    print('=> обработка данных...')
    res = m.do()

    # запись полученных кубов в базу данных
    cursor.execute("DELETE FROM ModelFigure")  # удаление данных из БД
    conn.commit()

    count_figures = sum(1 for _ in
                        map(lambda item:
                            cursor.execute(f"INSERT into ModelFigure values ({', '.join(str(x) for x in item)})"), res))

    print()
    print('Всего фигур: %s' % count_figures)  # 219574

    conn.commit()

    conn.close()

    time_and_memory.endlog()
