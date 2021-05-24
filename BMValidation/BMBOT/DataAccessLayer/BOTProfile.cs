using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BMBOT.DataAccessLayer
{
    public class BOTProfile
    {
        [Key]
        public int ProfileId { get; set; }

        public string ProfileName { get; set; }
        public string IsActive { get;  set; }
    }
}
