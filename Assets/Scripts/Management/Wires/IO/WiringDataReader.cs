using Management.Wires;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Management.Wires.IO
{
    public static class WiringDataReader
    {
        public static Wiring ReadFromFile(string pathToXLS)
        {
            HSSFWorkbook workbook;

            using (FileStream stream = new FileStream(pathToXLS, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                workbook = new HSSFWorkbook(stream);
            }

            List<Wire> wires = new List<Wire>();

            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                ISheet sheet = workbook.GetSheetAt(i);

                var points = ReadPoints(sheet);

                wires.Add(Wire.Factory.Create(sheet.SheetName, points));
            }

            return Wiring.Factory.Create(wires);
        }

        private static List<Wire.Point> ReadPoints(ISheet sheet)
        {
            var points = new List<Wire.Point>();

            for (int j = 3; j <= sheet.LastRowNum; j++)
            {
                IRow row = sheet.GetRow(j);

                if (!IsCorrectNodeRow(row)) break;

                points.Add(ReadPoint(row));
            }

            return points;
        }

        private static Wire.Point ReadPoint(IRow row)
        {
            Wire.Point point = new Wire.Point();

            point.position.x = (float)row.GetCell(0).NumericCellValue;
            point.position.y = (float)row.GetCell(1).NumericCellValue;
            point.position.z = (float)row.GetCell(2).NumericCellValue;

            var metallization1Cell = row.GetCell(3);
            var metallization2Cell = row.GetCell(4);

            point.metallization1 = metallization1Cell.CellType == CellType.Blank ? null : (float?)metallization1Cell.NumericCellValue;
            point.metallization2 = metallization1Cell.CellType == CellType.Blank ? null : (float?)metallization2Cell.NumericCellValue;

            return point;
        }

        private static bool IsCorrectNodeRow(IRow row)
        {
            for (int i = 0; i < 3; i++)
                if (row.GetCell(i).CellType != CellType.Numeric)
                    return false;

            for (int i = 3; i < 5; i++)
            {
                var cell = row.GetCell(i);

                if (cell.CellType != CellType.Blank && cell.CellType != CellType.Numeric)
                    return false;
            }


            return true;
        }

        private static bool IsNumericCell(IRow row, int columnIndex)
        {
            ICell cell = row.GetCell(columnIndex);

            return cell.CellType == CellType.Numeric;
        }
    }
}