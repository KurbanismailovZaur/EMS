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
    public static class KVID8DataReader
    {
        public static (List<(string pointID, float maxVoltage, int fMin, int fMax)> tab0, List<(string idES, string wireID, float maxVoltage, int fMin, int fMax)> tab1) ReadFromFile(string pathToXLS)
        {
            HSSFWorkbook workbook;

            using (FileStream stream = new FileStream(pathToXLS, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                workbook = new HSSFWorkbook(stream);
            }

            ISheet sheet0 = workbook.GetSheetAt(0);
            ISheet sheet1 = workbook.GetSheetAt(1);



            return (ReadTab0Data(sheet0), ReadTab1Data(sheet1));
        }

        private static List<(string pointID, float maxVoltage, int fMin, int fMax)> ReadTab0Data(ISheet sheet)
        {
            var result = new List<(string pointID, float maxVoltage, int fMin, int fMax)>();

            for (int j = 2; j <= sheet.LastRowNum; j++)
            {
                IRow row = sheet.GetRow(j);

                if (!IsCorrectNodeRowTab0(row)) break;

                result.Add(ReadTab0Row(row));
            }

            return result;
        }

        private static (string pointID, float maxVoltage, int fMin, int fMax) ReadTab0Row(IRow row)
        {

            var id = row.GetCell(0).StringCellValue;
            var mV = (float)row.GetCell(1).NumericCellValue;
            var fMin = (int)row.GetCell(2).NumericCellValue;
            var fMax = (int)row.GetCell(3).NumericCellValue;

            return (id, mV, fMin, fMax);
        }


        private static List<(string idES, string wireID, float maxVoltage, int fMin, int fMax)> ReadTab1Data(ISheet sheet)
        {
            var result = new List<(string idES, string wireID, float maxVoltage, int fMin, int fMax)>();

            for (int j = 2; j <= sheet.LastRowNum; j++)
            {
                IRow row = sheet.GetRow(j);

                if (!IsCorrectNodeRowTab1(row)) break;

                result.Add(ReadTab1Row(row));
            }

            return result;
        }

        private static (string idES, string wireID, float maxVoltage, int fMin, int fMax) ReadTab1Row(IRow row)
        {
            var id = row.GetCell(0).StringCellValue;
            var wId = row.GetCell(1).StringCellValue;
            var mV = (float)row.GetCell(2).NumericCellValue;
            var fMin = (int)row.GetCell(3).NumericCellValue;
            var fMax = (int)row.GetCell(4).NumericCellValue;

            return (id, wId, mV, fMin, fMax);
        }



        private static bool IsCorrectNodeRowTab0(IRow row)
        {
            if (row.GetCell(0).CellType != CellType.String)
                return false;

            for (int i = 1; i < 4; i++)
                if (!IsFloat(row.GetCell(i)))
                    return false;

            return true;
        }

        private static bool IsCorrectNodeRowTab1(IRow row)
        {
            if (row.GetCell(0).CellType != CellType.String)
                return false;

            if (row.GetCell(1).CellType != CellType.String)
                return false;

            for (int i = 2; i < 5; i++)
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