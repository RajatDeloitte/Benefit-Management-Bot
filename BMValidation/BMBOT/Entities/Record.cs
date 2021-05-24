using System;
using System.Collections.Generic;
using System.Text;

namespace BMBOT.Entities
{
    [Serializable]
    public class Record1
    {
        public List<Column> Columns { get; set; }

        public int RecordId { get; set; }

        public string RecordText { get; set; }


        public List<string> Errors { get; set; }


    }
}
