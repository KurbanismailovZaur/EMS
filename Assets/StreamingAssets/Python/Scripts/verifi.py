from pyxlsb import open_workbook
from console_progressbar import ProgressBar

pb = ProgressBar(total=100, prefix='Status', suffix='', decimals=2, length=50, fill='X', zfill='-')

with open_workbook('data_import/Geometric_centers.xlsb') as wb:
    count_sheets = len(wb.sheets)
    count_adjacent = 0
    dct_adjacent = {}

    for sh_i, sh_name in enumerate(wb.sheets):
        with wb.get_sheet(sh_name) as sheet:
            for i, row in enumerate(sheet.rows()):
                if i == 0:
                    continue
                point = (row[0].v, row[1].v, row[2].v)

                d = dct_adjacent.get(point, None)
                if d is None:
                    dct_adjacent[point] = {row[3].v}
                else:
                    dct_adjacent[point].add(row[3].v)

        pb.print_progress_bar(((sh_i + 1) * 100) / count_sheets)

    print()
    for key, value in dct_adjacent.items():
        if len(value) > 1:
            count_adjacent += 1
            print(key, '-', end=' ')
            [print(x, end=' ') for x in value]
            print()
    print('Всего листов:', count_sheets)
    print('Кол-во смежных вершин:', count_adjacent)
