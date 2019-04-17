using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Data.XLS
{
	public static class WiringDataReader 
	{
        public static bool ReadWiringFromFile(string pathToXLS)
        {
            //Wiring.Factory wiringFactory = new Wiring.Factory();
            //wiring = wiringFactory.Create();

            HSSFWorkbook workbook;

            using (FileStream stream = new FileStream(pathToXLS, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                workbook = new HSSFWorkbook(stream);
            }

            //Wire.Factory wireFactory = new Wire.Factory();

            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                ISheet sheet = workbook.GetSheetAt(i);

                IRow amplitudeRow = sheet.GetRow(0);
                if (!IsNumericCell(amplitudeRow, 1))
                {
                    return false;
                }

                float amplitude = (float)amplitudeRow.GetCell(1).NumericCellValue;

                IRow frequencyRow = sheet.GetRow(1);
                if (!IsNumericCell(frequencyRow, 1))
                {
                    return false;
                }

                float frequency = (float)frequencyRow.GetCell(1).NumericCellValue;

                IRow amperageRow = sheet.GetRow(2);
                if (!IsNumericCell(frequencyRow, 1))
                {
                    return false;
                }

                float amperage = (float)amperageRow.GetCell(1).NumericCellValue;

                //Wire wire = wiring.CreateWire(sheet.SheetName, amplitude, frequency, amperage);

                for (int j = 5; j <= sheet.LastRowNum; j++)
                {
                    IRow row = sheet.GetRow(j);

                    if (!IsCorrectNodeRow(row))
                    {
                        break;
                    }

                    //wire.Add(ReadNode(row), Space.Self);
                }
            }

            return true;
        }

        private static Vector3 ReadNode(IRow row)
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