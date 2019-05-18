import xlsxwriter


class Report:
    def __init__(self, results_m3, results_m2, points, wires, path='reports.xlsx'):
        self.results_m3 = results_m3
        self.results_m2 = results_m2
        self.select_points = points
        self.select_wires = wires
        self.path = path

    def do(self):
        is_points = len(self.select_points) > 0  # если заданные точки выбраны
        is_wires = len(self.select_wires) > 0  # если кабеля выбраны

        if is_points or is_wires:
            workbook = xlsxwriter.Workbook(self.path)

            if is_points:
                worksheet_1kv = workbook.add_worksheet('1КВ')
                worksheet_2kv = workbook.add_worksheet('2КВ')
                worksheet_3kv = workbook.add_worksheet('3КВ')
                #
                # for point_id in self.select_points:
                #     pass

            if is_wires:
                worksheet_4kv = workbook.add_worksheet('4КВ')
                worksheet_5kv = workbook.add_worksheet('5КВ')
                worksheet_6kv = workbook.add_worksheet('6КВ')

            workbook.close()
