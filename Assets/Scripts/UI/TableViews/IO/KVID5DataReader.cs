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
    public static class KVID5DataReader
    {
        public static List<(string code, Vector3 point, string type, int? iR, int? oV, int? oF, string bBA, string conType)> ReadFromFile(string pathToXLS)
        {
            HSSFWorkbook workbook;

            using (FileStream stream = new FileStream(pathToXLS, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                workbook = new HSSFWorkbook(stream);
            }

            if (workbook.NumberOfSheets != 1)
                throw new FormatException("Number of sheers must be a one");

            ISheet sheet = workbook.GetSheetAt(0);

            return ReadData(sheet);
        }

        private static List<(string code, Vector3 point, string type, int? iR, int? oV, int? oF, string bBA, string conType)> ReadData(ISheet sheet)
        {
            var data = new List<(string code, Vector3 point, string type, int? iR,int? oV, int? oF, string bBA, string conType)>();

            for (int j = 2; j <= sheet.LastRowNum; j++)
            {
                IRow row = sheet.GetRow(j);

                if (!IsCorrectNodeRow(row)) break;

                data.Add(ReadDataRow(row));
            }

            return data;
        }

        private static (string code, Vector3 point, string type, int? iR, int? oV, int? oF, string bBA, string conType) ReadDataRow(IRow row)
        {
            var point = new Vector3
            {
                x = (float)row.GetCell(1).NumericCellValue,
                y = (float)row.GetCell(2).NumericCellValue,
                z = (float)row.GetCell(3).NumericCellValue
            };

            var type = row.GetCell(4).StringCellValue;
            var iR = GetNullableInt(row.GetCell(5));
            var oV = GetNullableInt(row.GetCell(6));
            var oF = GetNullableInt(row.GetCell(7));
            var bBA = row.GetCell(8).StringCellValue;
            var conType = row.GetCell(9).StringCellValue;


            return (row.GetCell(0).StringCellValue, point, type, iR, oV, oF, bBA, conType);
        }

        private static bool IsCorrectNodeRow(IRow row)
        {
            if (row.GetCell(0).CellType != CellType.String)
                return false;

            for (int i = 1; i < 4; i++)
                if (!IsFloat(row.GetCell(i)))
                    return false;

            if (row.GetCell(4).CellType != CellType.String)
                return false;


            for (int i = 5; i < 8; i++)
                if (!IsNullableFloat(row.GetCell(i)))
                    return false;

            if (!IsNullableString(row.GetCell(8)))
                return false;

            if (!IsNullableString(row.GetCell(9)))
                return false;


            return true;
        }

        private static bool IsFloat(ICell cell) => cell.CellType == CellType.Numeric;

        private static bool IsNullableFloat(ICell cell)
        {
            return cell.CellType == CellType.Blank || cell.CellType == CellType.Numeric;
        }

        private static bool IsNullableString(ICell cell)
        {
            return cell.CellType == CellType.Blank || cell.CellType == CellType.String;
        }

        private static float? GetNullableFloat(ICell cell)
        {
            return cell.CellType == CellType.Blank ? null : (float?)cell.NumericCellValue;
        }

        private static int? GetNullableInt(ICell cell)
        {
            return cell.CellType == CellType.Blank ? null : (int?)cell.NumericCellValue;
        }
    }
}