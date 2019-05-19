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

            props_base = {
                'font_name': 'Times New Roman',
                'text_wrap': True,
                'align': 'center',
                'font_size': 10,
                'border': 1,
            }

            props_exceed = props_base.copy()
            props_exceed['bg_color'] = '#FFC7CE'

            props_bold = props_base.copy()
            props_bold['bold'] = True

            props_header = props_bold.copy()
            props_header['font_size'] = 12

            format_base = workbook.add_format(props_base)
            format_exceed = workbook.add_format(props_exceed)
            format_bold = workbook.add_format(props_bold)
            format_header = workbook.add_format(props_header)

            cell_size = 20  # размер в пикселях одной ячейки

            if is_points:
                #  Для динамического отображения графиков
                count_f = len(self.results_m3[self.select_points[0]][-1])
                count_rows_add = count_f - 15
                if count_rows_add < 0:
                    chart_height = cell_size * 15
                    count_rows_add = abs(count_rows_add)
                else:
                    chart_height = cell_size * count_f
                    count_rows_add = 1
                """
                Форма 1КВ
                """
                worksheet = workbook.add_worksheet('1КВ')

                # Настройки листа
                worksheet.freeze_panes(4, 0)
                worksheet.repeat_rows(1, 3)
                worksheet.set_paper(9)
                worksheet.fit_to_pages(1, 0)
                worksheet.set_margins(0.3, 0.3, 0.3, 0.3)
                worksheet.center_horizontally()

                # Заголовок листа
                worksheet.merge_range('A1:I1', 'СВЕДЕНИЯ\n о подробном табличном отображении напряженности '
                                               'электрического поля в\n заданных точках Изделия и оценка '
                                               'превышения предельных значений\n (отчетные данные)', format_header)
                worksheet.set_row(0, 65)

                # Заголовки таблицы
                worksheet.merge_range('A2:A3', 'ID точки', format_bold)
                worksheet.merge_range('B2:B3', 'Влияющий кабель/ББА', format_bold)
                worksheet.merge_range('C2:F2', 'Напряженность электрополя по осям (В/м)', format_bold)
                worksheet.merge_range('G2:H2', 'Диапазон частот (Гц)', format_bold)
                worksheet.write('C3', 'x', format_bold)
                worksheet.write('D3', 'y', format_bold)
                worksheet.write('E3', 'z', format_bold)
                worksheet.write('F3', 'результат', format_bold)
                worksheet.write('G3', 'НГД', format_bold)
                worksheet.write('H3', 'ВГД', format_bold)
                worksheet.merge_range('I2:I3', 'Оценка превышения', format_bold)

                worksheet.set_column(0, 0, 7)
                worksheet.set_column(1, 1, 12)
                worksheet.set_column(2, 5, 10)
                worksheet.set_column(6, 8, 12)

                # Нумерация столбцов
                [worksheet.write(3, x, x + 1, format_bold) for x in range(9)]

                row = 4
                col = 0
                for point_id in self.select_points:
                    if row >= 1000000:
                        break
                    item = self.results_m3[point_id]
                    for wire in item[4]:
                        if wire[6]:
                            set_format = format_exceed
                        else:
                            set_format = format_base
                        worksheet.write(row, col, point_id, set_format)
                        worksheet.write(row, col + 1, wire[0], set_format)
                        worksheet.write(row, col + 2, wire[1], set_format)
                        worksheet.write(row, col + 3, wire[2], set_format)
                        worksheet.write(row, col + 4, wire[3], set_format)
                        worksheet.write(row, col + 5, wire[4], set_format)
                        worksheet.merge_range(row, col + 6, row, col + 7, wire[5], set_format)
                        worksheet.write(row, col + 8, 'Превышение!' if wire[6] else '', set_format)
                        row += 1

                    for bba in item[5]:
                        for f_range in bba[1]:
                            if f_range[3]:
                                set_format = format_exceed
                            else:
                                set_format = format_base
                            worksheet.write(row, col, point_id, set_format)
                            worksheet.write(row, col + 1, bba[0], set_format)
                            worksheet.write(row, col + 2, '', set_format)
                            worksheet.write(row, col + 3, '', set_format)
                            worksheet.write(row, col + 4, '', set_format)
                            worksheet.write(row, col + 5, f_range[0], set_format)
                            worksheet.write(row, col + 6, f_range[1], set_format)
                            worksheet.write(row, col + 7, f_range[2], set_format)
                            worksheet.write(row, col + 8, 'Превышение!' if f_range[3] else '', set_format)
                            row += 1

                worksheet.print_area(f'A1:I{row}')

                """
                Форма 2КВ
                """
                worksheet = workbook.add_worksheet('2КВ')

                # Настройки листа
                worksheet.freeze_panes(4, 0)
                worksheet.repeat_rows(1, 3)
                worksheet.set_paper(9)
                worksheet.fit_to_pages(1, 0)
                worksheet.set_margins(0.3, 0.3, 0.3, 0.3)
                worksheet.center_horizontally()

                # Заголовок листа
                worksheet.merge_range('A1:E1', 'СВЕДЕНИЯ\n о сводном табличном отображении напряженности '
                                               'электрического поля в заданных точках Изделия\n '
                                               '(отчетные данные)', format_header)
                worksheet.set_row(0, 65)

                # Заголовки таблицы
                worksheet.merge_range('A2:A3', 'ID точки', format_bold)
                worksheet.merge_range('B2:E2', 'Напряженность электрополя по осям (В/м)', format_bold)
                worksheet.write('B3', 'x', format_bold)
                worksheet.write('C3', 'y', format_bold)
                worksheet.write('D3', 'z', format_bold)
                worksheet.write('E3', 'результат', format_bold)

                worksheet.set_column(0, 4, 10)

                # Нумерация столбцов
                [worksheet.write(3, x, x + 1, format_bold) for x in range(5)]

                row = 4
                col = 0
                for point_id in self.select_points:
                    if row >= 1000000:
                        break
                    item = self.results_m3[point_id]
                    worksheet.write(row, col, point_id, format_base)
                    worksheet.write(row, col + 1, item[0], format_base)
                    worksheet.write(row, col + 2, item[1], format_base)
                    worksheet.write(row, col + 3, item[2], format_base)
                    worksheet.write(row, col + 4, item[3], format_base)
                    row += 1

                worksheet.print_area(f'A1:E{row}')

                """
                Форма 3КВ
                """
                worksheet = workbook.add_worksheet('3КВ')

                # Настройки листа
                worksheet.freeze_panes(3, 0)
                worksheet.repeat_rows(1, 2)
                worksheet.set_paper(9)
                worksheet.fit_to_pages(1, 0)
                worksheet.set_margins(0.3, 0.3, 0.3, 0.3)
                worksheet.center_horizontally()

                # Заголовок листа
                worksheet.merge_range('A1:L1', 'СВЕДЕНИЯ\n о сводном табличном и графическом отображении напряженности '
                                               'электрического поля в заданных точках Изделия\n '
                                               '(отчетные данные)', format_header)
                worksheet.set_row(0, 50)

                # Заголовки таблицы
                worksheet.write('A2', 'ID точки', format_bold)
                worksheet.write('B2', 'Суммарная напряженность (В/м)', format_bold)
                worksheet.write('C2', 'НГД (Гц) - ВГД (Гц)', format_bold)
                worksheet.write('D2', 'Оценка превышения', format_bold)
                worksheet.merge_range('E2:L2', 'Графическое отображение', format_bold)

                worksheet.set_column(0, 0, 7)
                worksheet.set_column(1, 1, 12)
                worksheet.set_column(2, 2, 20)
                worksheet.set_column(3, 3, 10)

                # Нумерация столбцов
                [worksheet.write(2, x, x + 1, format_bold) for x in range(4)]
                worksheet.merge_range('E3:L3', '5', format_bold)

                row = 3
                col = 0
                for point_id in self.select_points:
                    if row + count_f >= 1000000:
                        break
                    item = self.results_m3[point_id]
                    row_point = row
                    for r in item[-1]:
                        if r[2]:
                            set_format = format_exceed
                        else:
                            set_format = format_base
                        worksheet.write(row, col, point_id, set_format)
                        worksheet.write(row, col + 1, abs(r[1]), set_format)
                        if isinstance(r[0], list):
                            worksheet.write(row, col + 2, f'{r[0][0]:.2e} - {r[0][1]:.2e}', set_format)
                        else:
                            worksheet.write(row, col + 2, f'{r[0]:.2e}', set_format)
                        worksheet.write(row, col + 3, 'Превышение!' if r[2] else '', set_format)
                        row += 1
                    # Добавление графика
                    chart = workbook.add_chart({'type': 'bar'})
                    chart.add_series({
                        'categories': f'=3КВ!$C${row_point + 1}:$C${row}',
                        'values': f'=3КВ!$B${row_point + 1}:$B${row}',
                        'points': [{'fill': {'color': '#CD5C5C' if x[2] else '#4682B4'}} for x in item[-1]]
                    })
                    chart.set_x_axis({
                        'name': 'Напряженность поля (В/м)',
                        'num_font':  {'rotation': 45}
                    })
                    chart.set_y_axis({'name': 'Частота (Гц)'})
                    chart.set_legend({'none': True})
                    chart.set_size({'width': 512, 'height': chart_height})
                    worksheet.insert_chart(row_point, col + 4, chart)
                    row += count_rows_add

                worksheet.print_area(f'A1:L{row}')

            if is_wires:
                #  Для динамического отображения графиков
                count_f = len(self.results_m2[self.select_wires[0]][-2]) + 1
                count_rows_add = count_f - 15
                if count_rows_add < 0:
                    chart_height = cell_size * 15
                    count_rows_add = abs(count_rows_add)
                else:
                    chart_height = cell_size * count_f
                    count_rows_add = 1

                """
                Форма 4КВ
                """
                worksheet = workbook.add_worksheet('4КВ')

                # Настройки листа
                worksheet.freeze_panes(4, 0)
                worksheet.repeat_rows(1, 3)
                worksheet.set_paper(9)
                worksheet.fit_to_pages(1, 0)
                worksheet.set_margins(0.3, 0.3, 0.3, 0.3)
                worksheet.center_horizontally()

                # Заголовок листа
                worksheet.merge_range('A1:G1', 'СВЕДЕНИЯ\n о подробном табличном отображении взаимного воздействия '
                                               'кабелей БКС а\n также воздействия БА на БКС\n '
                                               '(отчетные данные)', format_header)
                worksheet.set_row(0, 65)

                # Заголовки таблицы
                worksheet.merge_range('A2:B2', 'Пары кабель/кабель кабель/БА (П-В)', format_bold)
                worksheet.merge_range('C2:G2', 'Воздействие кабеля/БА (В) на кабель (П) ', format_bold)
                worksheet.write('A3', 'П', format_bold)
                worksheet.write('B3', 'В', format_bold)
                worksheet.write('C3', 'Емкостное воздействие (В)', format_bold)
                worksheet.write('D3', 'Индуктивное воздействие (В)', format_bold)
                worksheet.write('E3', 'Суммарное воздействие (В)', format_bold)
                worksheet.write('F3', 'НГД (Гц)', format_bold)
                worksheet.write('G3', 'ВГД (Гц)', format_bold)

                worksheet.set_column(0, 1, 7)
                worksheet.set_column(2, 4, 12)
                worksheet.set_column(5, 6, 10)

                # Нумерация столбцов
                [worksheet.write(3, x, x + 1, format_bold) for x in range(7)]

                row = 4
                col = 0
                for wire_id in self.select_wires:
                    if row >= 1000000:
                        break
                    item = self.results_m2[wire_id]
                    for wire in item[0]:
                        worksheet.write(row, col, wire_id, format_base)
                        worksheet.write(row, col + 1, wire[0], format_base)
                        worksheet.write(row, col + 2, wire[2], format_base)
                        worksheet.write(row, col + 3, wire[3], format_base)
                        worksheet.write(row, col + 4, wire[2] + wire[3], format_base)
                        worksheet.merge_range(row, col + 5, row, col + 6, wire[1], format_base)
                        row += 1

                    for bba in item[1]:
                        for f_range in bba[1]:
                            worksheet.write(row, col, wire_id, format_base)
                            worksheet.write(row, col + 1, bba[0], format_base)
                            worksheet.write(row, col + 2, f_range[0], format_base)
                            worksheet.write(row, col + 3, '', format_base)
                            worksheet.write(row, col + 4, f_range[0], format_base)
                            worksheet.write(row, col + 5, f_range[1], format_base)
                            worksheet.write(row, col + 6, f_range[2], format_base)
                            row += 1

                worksheet.print_area(f'A1:G{row}')

                """
                Форма 5КВ
                """
                worksheet = workbook.add_worksheet('5КВ')

                # Настройки листа
                worksheet.freeze_panes(3, 0)
                worksheet.repeat_rows(1, 2)
                worksheet.set_paper(9)
                worksheet.fit_to_pages(1, 0)
                worksheet.set_margins(0.3, 0.3, 0.3, 0.3)
                worksheet.center_horizontally()

                # Заголовок листа
                worksheet.merge_range('A1:L1', 'СВЕДЕНИЯ\n о сводном табличном и графическом отображении получаемом '
                                               'воздействии каждого кабеля БКС в частотном распределении\n '
                                               '(отчетные данные)', format_header)
                worksheet.set_row(0, 50)

                # Заголовки таблицы
                worksheet.write('A2', 'ID кабеля', format_bold)
                worksheet.write('B2', 'Суммарное воздействие (В)', format_bold)
                worksheet.write('C2', 'НГД (Гц) - ВГД (Гц)', format_bold)
                worksheet.write('D2', 'Оценка превышения', format_bold)
                worksheet.merge_range('E2:L2', 'Графическое отображение', format_bold)

                worksheet.set_column(0, 0, 7)
                worksheet.set_column(1, 1, 12)
                worksheet.set_column(2, 2, 20)
                worksheet.set_column(3, 3, 10)

                # Нумерация столбцов
                [worksheet.write(2, x, x + 1, format_bold) for x in range(4)]
                worksheet.merge_range('E3:L3', '5', format_bold)

                row = 3
                col = 0
                for wire_id in self.select_wires:
                    if row + count_f >= 1000000:
                        break
                    item = self.results_m2[wire_id]
                    row_point = row
                    for r in item[-2]:
                        if r[2]:
                            set_format = format_exceed
                        else:
                            set_format = format_base
                        worksheet.write(row, col, wire_id, set_format)
                        worksheet.write(row, col + 1, abs(r[0]), set_format)
                        if isinstance(r[1], list):
                            worksheet.write(row, col + 2, f'{r[1][0]:.2e} - {r[1][1]:.2e}', set_format)
                        else:
                            worksheet.write(row, col + 2, f'{r[1]:.2e}', set_format)
                        worksheet.write(row, col + 3, 'Превышение!' if r[2] else '', set_format)
                        row += 1

                    # Добавление итогового значения по кабелю
                    worksheet.write(row, col, wire_id, format_bold)
                    worksheet.write(row, col + 1, abs(item[-1]), format_bold)
                    worksheet.write(row, col + 2, '', format_bold)
                    worksheet.write(row, col + 3, '', format_bold)
                    worksheet.write(row, col + 4, '', format_bold)
                    row += 1

                    # Добавление графика
                    chart = workbook.add_chart({'type': 'bar'})
                    chart.add_series({
                        'categories': f'=5КВ!$C${row_point + 1}:$C${row - 1}',
                        'values': f'=5КВ!$B${row_point + 1}:$B${row - 1}',
                        'points': [{'fill': {'color': '#CD5C5C' if x[2] else '#4682B4'}} for x in item[-2]]
                    })
                    chart.set_x_axis({
                        'name': 'Напряжение (В)',
                        'num_font': {'rotation': 45}
                    })
                    chart.set_y_axis({'name': 'Частота (Гц)'})
                    chart.set_legend({'none': True})
                    chart.set_size({'width': 512, 'height': chart_height})
                    worksheet.insert_chart(row_point, col + 4, chart)
                    row += count_rows_add

                worksheet.print_area(f'A1:L{row}')

            workbook.close()
