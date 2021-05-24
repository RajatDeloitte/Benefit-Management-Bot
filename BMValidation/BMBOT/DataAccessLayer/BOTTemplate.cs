using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BMBOT.DataAccessLayer
{
    public class BOTTemplate
    {
        [Key]
        public int TemplateId { get; set; }

       public string TemplateName { get; set; }
	
	   public string OutBoundFileLocation { get; set; }

        public int? NoOfLineToBeExcludedFromTop { get; set; }

        public int? NoOfLineToBeExcludedFromBottom { get; set; }

        public string ExcludeLineStartText { get; set; }




    }
}
