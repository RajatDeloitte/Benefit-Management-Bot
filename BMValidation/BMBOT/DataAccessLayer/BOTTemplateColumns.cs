using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BMBOT.DataAccessLayer
{
    public class BOTTemplateColumns
    {
        
        [Key]
        public int BOTTemplateColumnId { set; get; }

        public int TemplateId { set; get; }

        public string ColumnName { set; get; }

        public string DataType { set; get; }

        public int MinPosition { set; get; }

        public int MaxPosition { set; get; }

        public int? MinValue { set; get; }

        public int? MaxValue { set; get; }

        public string RegularExpression { set; get; }

        public string DBQuery { set; get; }

        public string QueryType { get; set; }

        public string ParameterName { get; set; }

        public string ParameterValue { get; set; }

        public string ParameterType { get; set; }

        public string ValidValues { set; get; }

        public string ValidValuesReferenceTable { get; set; }

        public string IsIdentifier { get; set; }



    }
}
