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
    public static class KVID3DataReader
    {
        public static List<(string name, float wireLenght, string wireType, string iEsID, string pEsID, List<Wire.Point> points)> ReadFromFile(string pathToXLS)
        {
            var tabs = new List<(string name, float wireLenght, string wireType, string iEsID, string pEsID, List<Wire.Point> points)>();

            HSSFWorkbook workbook;

            using (FileStream stream = new FileStream(pathToXLS, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                workbook = new HSSFWorkbook(stream);
            }
            
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                ISheet sheet = workbook.GetSheetAt(i);

                var l = 0; //(float)sheet.GetRow(0).GetCell(1).NumericCellValue;
                var wT = sheet.GetRow(1).GetCell(1).StringCellValue;
                var iID = sheet.GetRow(4).GetCell(0).StringCellValue;
                var pID = sheet.GetRow(4).GetCell(1).StringCellValue;

                tabs.Add((sheet.SheetName, l, wT, iID, pID, ReadPoints(sheet)));
            }

            return tabs;
        }

        private static List<Wire.Point> ReadPoints(ISheet sheet)
        {
            var points = new List<Wire.Point>();

            for (int j = 7; j <= sheet.LastRowNum; j++)
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

            point.metallization1 = GetNullableFloat(metallization1Cell);
            point.metallization2 = GetNullableFloat(metallization2Cell);

            return point;
        }

        private static bool IsCorrectNodeRow(IRow row)
        {
            for (int i = 0; i < 3; i++)
                if (!IsFloat(row.GetCell(i)))
                    return false;

            for (int i = 3; i < 5; i++)
            {
                var cell = row.GetCell(i);

                if (!IsNullableFloat(cell))
                    return false;
            }

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