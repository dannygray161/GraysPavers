using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GraysPavers.Models
{
    public class AppType
    {
        [Key]
        public int AppId { get; set; }
        [DisplayName("App Type")]
        [Required]
        public string AppName { get; set; }



    }
}
