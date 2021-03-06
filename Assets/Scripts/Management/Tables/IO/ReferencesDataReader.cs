﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.IO;
using System.Linq;
using UnityEngine.UI;

namespace Management.Tables.IO
{
    public static class ReferencesDataReader
    {
        public static (List<Material> materials, List<WireMark> wireMarks) ReadFromFile(string pathToXLS)
        {
            HSSFWorkbook workbook;

            using (FileStream stream = new FileStream(pathToXLS, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                workbook = new HSSFWorkbook(stream);
            }

            List<Material> materials = ReadMaterials(workbook.GetSheetAt(0));
            List<WireMark> wireMarks = ReadWireMarks(workbook.GetSheetAt(1), materials);

            return (materials, wireMarks);
        }

        private static List<Material> ReadMaterials(ISheet sheet)
        {
            var materials = new List<Material>();

            for (int i = 3; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);

                if (!IsCorrectMaterial(row)) break;

                materials.Add(ReadMaterial(row));
            }

            return materials;
        }

        private static Material ReadMaterial(IRow row)
        {
            return new Material
            {
                Code = GetInt(row, 0),
                Name = GetString(row, 1),
                Conductivity = GetNullableFloat(row, 2),
                MagneticPermeability = GetNullableFloat(row, 3),
                DielectricConstant = GetNullableFloat(row, 4)
            };
        }

        private static bool IsCorrectMaterial(IRow row)
        {
            return IsIntCell(row, 0) && IsNotEmptyStringCell(row, 1) && IsNullableFloatCells(row, 2, 3, 4);
        }

        private static List<WireMark> ReadWireMarks(ISheet sheet, List<Material> materials)
        {
            var wireMarks = new List<WireMark>();

            for (int i = 2; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);

                if (!IsCorrectWireMark(row, materials)) break;

                wireMarks.Add(ReadWireMark(row, materials));
            }

            return wireMarks;
        }

        private static WireMark ReadWireMark(IRow row, List<Material> materials)
        {
            return new WireMark
            {
                Code = GetString(row, 0),
                CoreMaterial = GetMaterial(row, 1, materials),
                CoreDiameter = GetFloat(row, 2),

                Screen1 = new WireMark.Screen
                {
                    Material = GetMaterial(row, 3, materials),
                    InnerRadius = GetFloat(row, 4),
                    Thresold = GetFloat(row, 5),
                    IsolationMaterial = GetMaterial(row, 6, materials)
                },

                Screen2 = new WireMark.Screen
                {
                    Material = GetMaterial(row, 7, materials),
                    InnerRadius = GetNullableFloat(row, 8),
                    Thresold = GetNullableFloat(row, 9),
                    IsolationMaterial = GetMaterial(row, 10, materials)
                },

                CrossSectionDiameter = GetFloat(row, 11)
            };
        }

        private static bool IsCorrectWireMark(IRow row, List<Material> materials)
        {
            return IsNotEmptyStringCell(row, 0) && IsMaterialCode(row, 1, materials) && IsFloatCell(row, 2)
                && IsMaterialCode(row, 3, materials) && IsFloatCells(row, 4, 5) && IsMaterialCodeOrNull(row, 6, materials) && IsMaterialCodeOrNull(row, 7, materials)
                && IsNullableFloatCells(row, 8, 9) && IsMaterialCodeOrNull(row, 10, materials) && IsFloatCell(row, 11);
        }
        
        #region Cell type checkers
        private static bool IsIntCell(IRow row, int cellIndex)
        {
            if (row.GetCell(cellIndex).CellType != CellType.Numeric) return false;

            var value = row.GetCell(cellIndex).NumericCellValue;

            return value == (int)value;
        }

        private static bool IsNullableIntCell(IRow row, int cellIndex)
        {
            var cell = row.GetCell(cellIndex);

            if (cell.CellType == CellType.Blank) return true;
            if (cell.CellType != CellType.Numeric) return false;

            var value = cell.NumericCellValue;

            return value == (int)value;
        }

        private static bool IsStringCell(IRow row, int cellIndex)
        {
            var cell = row.GetCell(cellIndex);

            return cell.CellType == CellType.Blank || cell.CellType == CellType.String;
        }

        private static bool IsNotEmptyStringCell(IRow row, int cellIndex)
        {
            var cell = row.GetCell(cellIndex);

            return cell.CellType == CellType.String && !string.IsNullOrWhiteSpace(cell.StringCellValue);
        }

        private static bool IsFloatCell(IRow row, int cellIndex)
        {
            return row.GetCell(cellIndex).CellType == CellType.Numeric;
        }

        private static bool IsFloatCells(IRow row, params int[] cellsIndexes)
        {
            if (cellsIndexes.Length == 0) return false;

            return cellsIndexes.All(c => IsFloatCell(row, c));
        }

        private static bool IsNullableFloatCell(IRow row, int cellIndex)
        {
            return row.GetCell(cellIndex).CellType == CellType.Blank || row.GetCell(cellIndex).CellType == CellType.Numeric;
        }

        private static bool IsNullableFloatCells(IRow row, params int[] cellsIndexes)
        {
            if (cellsIndexes.Length == 0) return false;

            return cellsIndexes.All(c => IsNullableFloatCell(row, c));
        }

        private static bool IsMaterialCode(IRow row, int cellIndex, List<Material> materials)
        {
            return materials.Find(m => m.Code == GetInt(row, cellIndex)) != null;
        }

        private static bool IsMaterialCodeOrNull(IRow row, int cellIndex, List<Material> materials)
        {
            var code = GetNullableInt(row, cellIndex);

            if (code == null) return true;

            return materials.Find(m => m.Code == code) != null;
        }
        #endregion

        #region Cell value readers
        private static int GetInt(IRow row, int cellIndex) => (int)row.GetCell(cellIndex).NumericCellValue;

        private static int? GetNullableInt(IRow row, int cellIndex)
        {
            var cell = row.GetCell(cellIndex);

            return cell.CellType == CellType.Blank ? null : (int?)cell.NumericCellValue;
        }

        private static string GetString(IRow row, int cellIndex)
        {
            var cell = row.GetCell(cellIndex);

            return cell.CellType == CellType.Blank ? null : cell.StringCellValue;
        }

        private static float GetFloat(IRow row, int cellIndex) => (float)row.GetCell(cellIndex).NumericCellValue;

        private static float? GetNullableFloat(IRow row, int cellIndex)
        {
            var cell = row.GetCell(cellIndex);

            return cell.CellType == CellType.Blank ? null : (float?)cell.NumericCellValue;
        }

        private static Material GetMaterial(IRow row, int cellIndex, List<Material> materials)
        {
            var code = GetNullableInt(row, cellIndex);

            return code == null ? null : materials.Find(m => m.Code == code);
        }
        #endregion
    }
}