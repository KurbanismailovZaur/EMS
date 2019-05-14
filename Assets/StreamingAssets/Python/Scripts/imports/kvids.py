from settings import DB_PATH

import sqlite3
import xlrd
import json


class Import:
    def kvid1(self, path):
        print('=> импорт 1КВИД...')

        table_name = 'KVID1_REF'
        conn = sqlite3.connect(DB_PATH)
        cursor = conn.cursor()
        cursor.execute(f"DELETE FROM {table_name}")  # удаление данных из БД
        conn.commit()

        book = xlrd.open_workbook(path)
        sh = book.sheet_by_index(0)  # данная форма состоит из одного листа
        # Цикл по строкам начиная с 4
        for rx in range(3, sh.nrows):
            row = sh.row_values(rx)
            cursor.execute(f"INSERT into {table_name} values ({int(row[0])}, '{row[1]}', "
                           f"{', '.join(str(x if x != '' else 'NULL') for x in row[2:])})")

        conn.commit()

        conn.close()

        print('всего материалов:', len(range(3, sh.nrows)))
        print()

    def kvid2(self, path):
        print('=> импорт 2КВИД...')

        table_name = 'KVID2'
        conn = sqlite3.connect(DB_PATH)
        cursor = conn.cursor()
        cursor.execute(f"DELETE FROM {table_name}")  # удаление данных из БД
        conn.commit()

        book = xlrd.open_workbook(path)
        # Цикл по листам
        for sh_i in range(book.nsheets):
            sh = book.sheet_by_index(sh_i)
            data = json.dumps([[int(sh.row_values(rx)[0]), sh.row_values(rx)[1]] for rx in range(6, sh.nrows)])

            cursor.execute(f"INSERT into {table_name} values ({', '.join('?' * 6)})",  # генерация 6-ти колонок
                           [sh.name, sh.row_values(0)[1], *[str(x) for x in sh.row_values(4)], data])

        conn.commit()

        conn.close()

        print('всего блоков:', book.nsheets)
        print()

    def kvid3(self, path):
        print('=> импорт 3КВИД...')

        table_name = 'KVID3'
        conn = sqlite3.connect(DB_PATH)
        cursor = conn.cursor()
        cursor.execute(f"DELETE FROM {table_name}")  # удаление данных из БД
        conn.commit()

        book = xlrd.open_workbook(path)
        # Цикл по листам
        for sh_i in range(book.nsheets):
            sh = book.sheet_by_index(sh_i)
            type_wire = sh.row_values(1)[1]
            source, recipient = sh.row_values(4)[:2]
            data = json.dumps([sh.row_values(rx) for rx in range(7, sh.nrows)])

            cursor.execute(f"INSERT into {table_name} values ({', '.join('?' * 5)})",  # генерация 5-ти колонок
                           [sh.name, type_wire, source, recipient, data])

        conn.commit()

        conn.close()

        print('всего проводов:', book.nsheets)
        print()

    def kvid4(self, path):
        print('=> импорт 4КВИД...')

        table_name = 'KVID4_REF'
        conn = sqlite3.connect(DB_PATH)
        cursor = conn.cursor()
        cursor.execute(f"DELETE FROM {table_name}")  # удаление данных из БД
        conn.commit()

        book = xlrd.open_workbook(path)
        sh = book.sheet_by_index(0)  # данная форма состоит из одного листа
        # Цикл по строкам начиная с 3
        for rx in range(2, sh.nrows):
            row = sh.row_values(rx)
            cursor.execute(f"INSERT into {table_name} values ({', '.join('?' * 12)})",  # генерация 12-ть колонок
                           row)

        conn.commit()

        conn.close()

        print('всего типов кабелей:', len(range(2, sh.nrows)))
        print()

    def kvid5(self, path):
        print('=> импорт 5КВИД...')

        table_name = 'KVID5'
        conn = sqlite3.connect(DB_PATH)
        cursor = conn.cursor()
        cursor.execute(f"DELETE FROM {table_name}")  # удаление данных из БД
        conn.commit()

        book = xlrd.open_workbook(path)
        sh = book.sheet_by_index(0)  # данная форма состоит из одного листа
        # Цикл по строкам начиная с 3
        for rx in range(2, sh.nrows):
            row = sh.row_values(rx)
            cursor.execute(f"INSERT into {table_name} values ({', '.join('?' * 10)})",  # генерация 10-ти колонок
                           row)

        conn.commit()

        conn.close()

        print('всего портов:', len(range(2, sh.nrows)))
        print()

    def kvid6(self, path):
        print('=> импорт 6КВИД...')

        table_name = 'KVID6'
        conn = sqlite3.connect(DB_PATH)
        cursor = conn.cursor()
        cursor.execute(f"DELETE FROM {table_name}")  # удаление данных из БД
        conn.commit()

        book = xlrd.open_workbook(path)
        sh = book.sheet_by_index(0)  # данная форма состоит из одного листа
        # Цикл по строкам начиная с 3
        for rx in range(2, sh.nrows):
            row = sh.row_values(rx)
            cursor.execute(f"INSERT into {table_name} values ({', '.join('?' * 4)})",  # генерация 4-х колонок
                           row)

        conn.commit()

        conn.close()

        print('всего заданных точек:', len(range(2, sh.nrows)))
        print()

    def kvid7(self, path):
        print('=> импорт 7КВИД...')

        table_name = 'KVID7'
        conn = sqlite3.connect(DB_PATH)
        cursor = conn.cursor()
        cursor.execute(f"DELETE FROM {table_name}")  # удаление данных из БД
        conn.commit()

        book = xlrd.open_workbook(path)
        sh = book.sheet_by_index(0)  # данная форма состоит из одного листа
        # Цикл по строкам начиная с 3
        for rx in range(2, sh.nrows):
            row = sh.row_values(rx)
            cursor.execute(f"INSERT into {table_name} values ({', '.join('?' * 5)})",  # генерация 5-ти колонок
                           row)

        conn.commit()

        conn.close()

        print('всего соединений:', len(range(2, sh.nrows)))
        print()

    def kvid8(self, path):
        print('=> импорт 8КВИД...')

        table_name_1 = 'KVID8_1'
        table_name_2 = 'KVID8_2'
        conn = sqlite3.connect(DB_PATH)
        cursor = conn.cursor()
        cursor.execute(f"DELETE FROM {table_name_1}")  # удаление данных из БД
        cursor.execute(f"DELETE FROM {table_name_2}")
        conn.commit()

        book = xlrd.open_workbook(path)
        data = (
            (0, table_name_1, 4),
            (1, table_name_2, 5)
        )
        for index, table_name, col in data:
            sh = book.sheet_by_index(index)
            # Цикл по строкам начиная с 3
            for rx in range(2, sh.nrows):
                row = sh.row_values(rx)
                cursor.execute(f"INSERT into {table_name} values ({', '.join('?' * col)})", row)

            print('всего значений в элемнтах БКС:' if index else 'всего значений в точках мониторинга:',
                  len(range(2, sh.nrows)))

        conn.commit()

        conn.close()

        print()


if __name__ == "__main__":
    # Elapsed time: 0:01:58.574 - 0:01:15.942
    # Memory used: 293.738mb - 318.859mb

    import time_and_memory

    im = Import()
    im.kvid1('../data_import/1КВИД_ref.xls')
    im.kvid2('../data_import/2КВИД.xls')
    im.kvid3('../data_import/3КВИДv2.xls')
    im.kvid4('../data_import/4КВИД_ref.xls')
    im.kvid5('../data_import/5КВИД.xls')
    im.kvid6('../data_import/6КВИД.xls')
    # im.kvid7('../data_import/7КВИД.xls') упразнен
    im.kvid8('../data_import/8КВИД.xls')

    time_and_memory.endlog()

