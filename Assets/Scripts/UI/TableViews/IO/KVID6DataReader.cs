using Management.Wires;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UI.TableViews.IO
{
    public static class KVID6DataReader
    {
        public static List<(string code, Vector3 position)> ReadFromFile(string pathToXLS)
        {
            HSSFWorkbook workbook;

            using (FileStream stream = new FileStream(pathToXLS, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                workbook = new HSSFWorkbook(stream);
            }

            if (workbook.NumberOfSheets != 1)
                throw new FormatException("Number of sheers must be a one");

            ISheet sheet = workbook.GetSheetAt(0);

            return ReadPoints(sheet);
        }

        private static List<(string code, Vector3 position)> ReadPoints(ISheet sheet)
        {
            var points = new List<(string code, Vector3 point)>();

            for (int j = 2; j <= sheet.LastRowNum; j++)
            {
                IRow row = sheet.GetRow(j);

                if (!IsCorrectNodeRow(row)) break;

                points.Add(ReadPoint(row));

                if (j == 103823 + 2) break; 
            }

            return points;
        }

        private static (string code, Vector3 position) ReadPoint(IRow row)
        {
            var point = new Vector3
            {
                x = (float)row.GetCell(1).NumericCellValue,
                y = (float)row.GetCell(2).NumericCellValue,
                z = (float)row.GetCell(3).NumericCellValue
            };

            return (row.GetCell(0).StringCellValue, point);
        }

        private static bool IsCorrectNodeRow(IRow row)
        {
            if (row.GetCell(0).CellType != CellType.String)
                return false;

            for (int i = 1; i < 4; i++)
                if (!IsFloat(row.GetCell(i)))
                    return false;

            return true;
        }

        private static bool IsFloat(ICell cell) => cell.CellType == CellType.Numeric;

        private static bool IsNullableFloat(ICell cell)
        {
            return cell.CellType == CellType.Blank || cell.CellType == CellType.Numeric;
        }

        private static float? GetNullableFloat(ICell cell)
        {
            return cell.CellType == CellType.Blank ? null : (float?)cell.NumericCellValue;
        }
    }
}