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
    public static class KVID2DataReader
    {
        public static List<(string tabName, string productName, Vector3 center, List<(float? x, float? y)> voltage)> ReadFromFile(string pathToXLS)
        {
            HSSFWorkbook workbook;

            using (FileStream stream = new FileStream(pathToXLS, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                workbook = new HSSFWorkbook(stream);
            }

            var result = new List<(string tabName, string productName, Vector3 center, List<(float?, float?)> voltage)>();

            for (int i = 0; i < workbook.NumberOfSheets; ++i)
            {
                ISheet sheet = workbook.GetSheetAt(i);

                var sheetData = ReadSheetData(sheet);

                result.Add((sheet.SheetName, sheetData.productName, sheetData.center, sheetData.voltage));
            }

            return result;
        }

        private static (Vector3 center, string productName, List<(float?, float?)> voltage) ReadSheetData(ISheet sheet)
        {
            var voltages = new List<(float?, float?)>();

            IRow pNameRow = sheet.GetRow(0);
            if (pNameRow.GetCell(1).CellType != CellType.String) throw new ApplicationException("Incorrect table data");
            var pName = pNameRow.GetCell(1).StringCellValue;

            IRow pointRow = sheet.GetRow(4);
            if (!IsCorrectPointNodeRow(pointRow)) throw new ApplicationException("Incorrect table data");


            var center = ReadPoint(pointRow);


            for (int j = 6; j <= sheet.LastRowNum; j++)
            {
                IRow vRow = sheet.GetRow(j);

                if (!IsCorrectVoltageNodeRow(vRow)) break;

                voltages.Add(ReadVoltage(vRow));
            }

            return (center, pName, voltages);
        }

        private static Vector3 ReadPoint(IRow row)
        {
            var point = new Vector3
            {
                x = (float)row.GetCell(0).NumericCellValue,
                y = (float)row.GetCell(1).NumericCellValue,
                z = (float)row.GetCell(2).NumericCellValue
            };

            return point;
        }

        private static (float?, float?) ReadVoltage(IRow row)
        {

            var x = GetNullableFloat(row.GetCell(0));
            var y = GetNullableFloat(row.GetCell(1));


            return (x, y);
        }

        private static bool IsCorrectPointNodeRow(IRow row)
        {
            for (int i = 0; i < 3; i++)
                if (!IsFloat(row.GetCell(i)))
                    return false;

            return true;
        }

        private static bool IsCorrectVoltageNodeRow(IRow row)
        {
            for (int i = 0; i < 2; i++)
                if (!IsNullableFloat(row.GetCell(i)))
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