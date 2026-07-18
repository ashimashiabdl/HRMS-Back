using System.Collections.Generic;

namespace HR.Payroll.Core.DTOs
{
    public class DbfFileDataDTO
    {
        public List<string> ColumnNames { get; set; } = new List<string>();
        public List<Dictionary<string, object>> Rows { get; set; } = new List<Dictionary<string, object>>();
        public int TotalRecords { get; set; }
    }
}

