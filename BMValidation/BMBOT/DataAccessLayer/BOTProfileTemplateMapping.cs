using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BMBOT.DataAccessLayer
{
    public class BOTProfileTemplateMapping
    {
        [Key]
        public int BOTProfileTemplateMappingId { get;set;}

        public int ProfileId { get; set; }

        public int TemplateId { get; set;}

        public string OutputFileLocation { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; } 

        public string IsActive { get; set; }

        public string CommaSepereatedEmail { get; set; }

       
    }
}
