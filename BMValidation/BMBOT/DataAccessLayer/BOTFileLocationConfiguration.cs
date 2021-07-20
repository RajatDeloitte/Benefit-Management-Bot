using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BMBOT.DataAccessLayer
{
    public class BOTFileLocationConfiguration
    {
       [Key]
       public int BOTFileLocationConfigurationId { get; set; }
       public string FileNameStartsWith { get; set; }
       public string FileLocation { get; set; }
       public string Frequency { get; set; }
       public int? DayOfTheWeek { get; set; }
       public DateTime? ValidationDate { get; set; }
       public TimeSpan? ValidationTime { get; set; }
       public string ValidationType { get; set; }
    }
}
