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
        public static Wiring ReadWiringFromFile(string pathToXLS)
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

                IRow amplitudeRow = sheet.GetRow(0);
                if (!IsNumericCell(amplitudeRow, 1))
                    throw new FormatException("Amplitude must be a numeric value.");

                float amplitude = (float)amplitudeRow.GetCell(1).NumericCellValue;

                IRow frequencyRow = sheet.GetRow(1);
                if (!IsNumericCell(frequencyRow, 1))
                    throw new FormatException("Frequency must be a numeric value.");

                float frequency = (float)frequencyRow.GetCell(1).NumericCellValue;

                IRow amperageRow = sheet.GetRow(2);
                if (!IsNumericCell(frequencyRow, 1))
                    throw new FormatException("Amperage must be a numeric value.");

                float amperage = (float)amperageRow.GetCell(1).NumericCellValue;

                var points = ReadPoints(sheet);

                wires.Add(Wire.Factory.Create(sheet.SheetName, amplitude, frequency, amperage, points));
            }

            return Wiring.Factory.Create(wires);
        }

        private static List<Vector3> ReadPoints(ISheet sheet)
        {
            var points = new List<Vector3>();

            for (int j = 5; j <= sheet.LastRowNum; j++)
            {
                IRow row = sheet.GetRow(j);

                if (!IsCorrectNodeRow(row)) break;

                points.Add(ReadPoint(row));
            }

            return points;
        }

        private static Vector3 ReadPoint(IRow row)
        {
            Vector3 node;

            node.x = (float)row.GetCell(0).NumericCellValue;
            node.y = (float)row.GetCell(1).NumericCellValue;
            node.z = (float)row.GetCell(2).NumericCellValue;

            return node;
        }

        private static bool IsCorrectNodeRow(IRow row)
        {
            for (int i = 0; i < 3; i++)
            {
                ICell cell = row.GetCell(i);

                if (cell.CellType != CellType.Numeric)
                {
                    return false;
                }
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