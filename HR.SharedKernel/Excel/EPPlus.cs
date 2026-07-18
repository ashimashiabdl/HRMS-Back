using HR.SharedKernel.Import;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.SharedKernel.Excel
{

    public class BatchGridModelForExcel
    {
        public long? EmployeeId { get; set; }
        public string? NationalNo { get; set; }
        public long ItemId { get; set; }
        public string? ItemDesc { get; set; }
        public string? FullName { get; set; }
        public decimal Value { get; set; }
    }

    public class BatchMappedColumnDefinition
    {
        public long ItemId { get; set; }
        public int ColumnOrder { get; set; }
        public string? ColumnLetter { get; set; }
    }

    public class BatchDateGridModelForExcel
    {
        public long? EmployeeId { get; set; }
        public string? NationalNo { get; set; }
        public string? FullName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
    public static class EPPlus
    {
        static EPPlus()
        {
            ExcelPackage.License.SetNonCommercialOrganization("HRMS");
        }

        public static List<BatchGridModelForExcel> ExcelDataToDataTable(Stream strm)
        {
            try
            {
                var dt = new DataTable();
                var xlPackage = new ExcelPackage(strm);
                List<long> ItemIdList = new List<long>();
                List<BatchGridModelForExcel> retList = new List<BatchGridModelForExcel>();
                foreach (var item in xlPackage.Workbook.Worksheets)
                {
                    ItemIdList.Add(Convert.ToInt64(item.Name));
                    dt = item.Cells[1, 1, item.Dimension.End.Row, item.Dimension.End.Column].ToDataTable(c =>
                    {
                        c.FirstRowIsColumnNames = true;
                    });
                    foreach (DataRow wageRow in dt.Rows)
                    {
                        var nationalNo = wageRow.ItemArray.Length > 0 ? wageRow.ItemArray[0]?.ToString()?.Trim() : null;
                        if (string.IsNullOrWhiteSpace(nationalNo))
                        {
                            continue;
                        }

                        if (wageRow.ItemArray.Length < 2 || wageRow.ItemArray[1] == null || wageRow.ItemArray[1] == DBNull.Value)
                        {
                            continue;
                        }

                        var rawValue = wageRow.ItemArray[1];
                        if (!TryParseDecimal(rawValue, out var value))
                        {
                            continue;
                        }

                        retList.Add(new BatchGridModelForExcel()
                        {
                            ItemId = Convert.ToInt64(item.Name),
                            NationalNo = nationalNo,
                            Value = value,
                        });
                    }
                }
                return retList;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public static List<BatchDateGridModelForExcel> ExcelDatesToDataTable(Stream strm, bool firstRowIsHeader = true)
        {
            try
            {
                var xlPackage = new ExcelPackage(strm);
                var sheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (sheet?.Dimension == null)
                {
                    return new List<BatchDateGridModelForExcel>();
                }

                var dt = sheet.Cells[1, 1, sheet.Dimension.End.Row, sheet.Dimension.End.Column].ToDataTable(c =>
                {
                    c.FirstRowIsColumnNames = firstRowIsHeader;
                });

                var retList = new List<BatchDateGridModelForExcel>();
                foreach (DataRow row in dt.Rows)
                {
                    var nationalNo = row.ItemArray.Length > 0 ? row.ItemArray[0]?.ToString()?.Trim() : null;
                    if (string.IsNullOrWhiteSpace(nationalNo))
                    {
                        continue;
                    }

                    retList.Add(new BatchDateGridModelForExcel()
                    {
                        NationalNo = nationalNo,
                        StartDate = row.ItemArray.Length > 1 ? ParseExcelDate(row.ItemArray[1]) : null,
                        EndDate = row.ItemArray.Length > 2 ? ParseExcelDate(row.ItemArray[2]) : null,
                    });
                }

                return retList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Parses Excel date cells. Accepts Gregorian or Jalali (Persian) text;
        /// always returns a Gregorian <see cref="DateTime"/> (date-only).
        /// UI (e.g. app-batch-issue) should display via Jalali pipe.
        /// </summary>
        private static DateTime? ParseExcelDate(object? value)
        {
            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            if (value is DateTime dateTime)
            {
                return dateTime.Date;
            }

            if (value is double oaDouble)
            {
                try
                {
                    return DateTime.FromOADate(oaDouble).Date;
                }
                catch
                {
                    return null;
                }
            }

            if (value is float oaFloat)
            {
                try
                {
                    return DateTime.FromOADate(oaFloat).Date;
                }
                catch
                {
                    return null;
                }
            }

            if (value is decimal oaDecimal)
            {
                try
                {
                    return DateTime.FromOADate((double)oaDecimal).Date;
                }
                catch
                {
                    return null;
                }
            }

            if (value is int or long or short)
            {
                var numericText = Convert.ToString(value, CultureInfo.InvariantCulture);
                if (ImportDateParser.TryParse(numericText, out var fromNumeric))
                {
                    return fromNumeric.Date;
                }
            }

            var text = value.ToString()?.Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            // Jalali (e.g. 1403/01/15) or Gregorian (e.g. 2024/04/03) → always Gregorian DateTime.
            if (ImportDateParser.TryParse(text, out var parsed))
            {
                return parsed.Date;
            }

            return null;
        }

        public static List<BatchGridModelForExcel> ExcelMappedGridToDataTable(
            Stream strm,
            IEnumerable<BatchMappedColumnDefinition> itemColumns,
            int nationalColumnOrder = 1)
        {
            var xlPackage = new ExcelPackage(strm);
            var sheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
            if (sheet?.Dimension == null)
            {
                return new List<BatchGridModelForExcel>();
            }

            var retList = new List<BatchGridModelForExcel>();
            var columnList = itemColumns?.Where(c => c.ItemId > 0).ToList() ?? new List<BatchMappedColumnDefinition>();
            if (!columnList.Any())
            {
                return retList;
            }

            for (var row = 2; row <= sheet.Dimension.End.Row; row++)
            {
                var nationalNo = GetWorksheetCellText(sheet, row, nationalColumnOrder)?.Trim();
                if (string.IsNullOrWhiteSpace(nationalNo))
                {
                    continue;
                }

                foreach (var column in columnList)
                {
                    var columnIndex = ResolveColumnIndex(column);
                    if (columnIndex <= 0)
                    {
                        continue;
                    }

                    var rawValue = GetWorksheetCellValue(sheet, row, columnIndex);
                    if (rawValue == null)
                    {
                        continue;
                    }

                    if (!TryParseDecimal(rawValue, out var value))
                    {
                        continue;
                    }

                    retList.Add(new BatchGridModelForExcel
                    {
                        ItemId = column.ItemId,
                        NationalNo = nationalNo,
                        Value = value,
                    });
                }
            }

            return retList;
        }

        private static int ResolveColumnIndex(BatchMappedColumnDefinition column)
        {
            if (!string.IsNullOrWhiteSpace(column.ColumnLetter))
            {
                var letters = column.ColumnLetter.Trim();
                if (letters.All(char.IsLetter))
                {
                    return ColumnLettersToIndex(letters);
                }
            }

            return column.ColumnOrder > 0 ? column.ColumnOrder : 0;
        }

        private static int ColumnLettersToIndex(string letters)
        {
            var sum = 0;
            foreach (var ch in letters.ToUpperInvariant())
            {
                if (!char.IsLetter(ch))
                {
                    return 0;
                }

                sum = (sum * 26) + (ch - 'A' + 1);
            }

            return sum;
        }

        private static object? GetWorksheetCellValue(ExcelWorksheet sheet, int row, int column)
        {
            return sheet.Cells[row, column].Value;
        }

        private static string? GetWorksheetCellText(ExcelWorksheet sheet, int row, int column)
        {
            return GetWorksheetCellValue(sheet, row, column)?.ToString();
        }

        private static bool TryParseDecimal(object rawValue, out decimal value)
        {
            value = 0;
            if (rawValue is decimal dec)
            {
                value = dec;
                return true;
            }

            if (rawValue is double dbl)
            {
                value = Convert.ToDecimal(dbl);
                return true;
            }

            if (rawValue is int intVal)
            {
                value = intVal;
                return true;
            }

            if (rawValue is long longVal)
            {
                value = longVal;
                return true;
            }

            var text = rawValue.ToString()?.Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            return decimal.TryParse(text, out value);
        }
    }
}
