using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BMBOT.DataAccessLayer
{
    public class BOTTemplateValidationMapping
    {
        [Key]
        public int TemplateValidationMappingId { get ;set;}

        public int TemplateId { get ;set;}

        public string ValidRecordFileLocation { get; set; }

        public string FailedRecordFileLocation { get; set; }

        public string IsRecordToBeRemoved { get; set; }

        public string IsNotificationRequiredOnValidationFail { get; set; }

        public string IsNotificationRequriedOnValiidationPass { get; set; }

        public string CommaSeperatedEmailIdForFailNotification { get; set; }

        public string CommaSeperatedEmailIdForPassNotification { get; set; }

        public string IsProcessMarkToBeFail { get; set; }

        public string ColumnNames { get; set; }

        public string AdditonalAttributes { get; set; }
        
        public string ValidationName { get; set; }

        public string EmailTemplateForValidationFail { get; set; }

        public string EmailTemplateForValidationPass { get; set; }

     
    }
}
