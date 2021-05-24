using System;
using System.Collections.Generic;
using System.Text;

namespace BMBOT.Entities
{
    public class Column
    {
        public string ColumnName { set; get; }

        public string DataType { set; get; }

        public int MinPosition { set; get; }

        public int MaxPosition { set; get; }

        public decimal MinValue { set; get; }

        public decimal MaxValue { set; get; }

        public string RegularExpression { set; get; }

        public string DBQuery { set; get; }

        public string QueryType { get; set; }

        public string ValidValues { set; get; }

        public string ValidValuesReferenceTable { get; set; }

        public string TextValue { get; set; }

        public decimal? DecimalValue { get; set; }

        public List<string> Errors { get; set; }

        public int BOTTemplateColumnId { get; set; }
        public string Identifier { get; set; }
    }
}
