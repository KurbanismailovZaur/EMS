from pyxlsb import open_workbook
from console_progressbar import ProgressBar

pb = ProgressBar(total=100, prefix='Status', suffix='', decimals=2, length=50, fill='X', zfill='-')

with open_workbook('data_import/SortedVertices.xlsx') as wb:
    count_sheets = len(wb.sheets)
    result = []

    for sh_i, sh_name in enumerate(wb.sheets):
        with wb.get_sheet(sh_name) as sheet:
            for i, row in enumerate(sheet.rows()):
                result.append(row[2].v)

        pb.print_progress_bar(((sh_i + 1) * 100) / count_sheets)

    print()
    print(result[:10], result[-1], sep=' - ')
    # for key, value in dct_adjacent.items():
    #     if len(value) > 1:
    #         count_adjacent += 1
    #         print(key, '-', end=' ')
    #         [print(x, end=' ') for x in value]
    #         print()
    # print('Всего листов:', count_sheets)
    # print('Кол-во смежных вершин:', count_adjacent)