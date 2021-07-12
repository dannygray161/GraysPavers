using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace GraysPavers.Models
{

    public class Category
    {
        [Key]

        #region Key Data Annotation Explanation

        /*This tells entity framework that this column
 needs to be an ID column and it needs to be set to primary key

 The Inclusion of Id in your prop name makes it so that it defaults 
to Primary Key and ID, However It is always better to be explicit*/


        #endregion
        public int CategoryId { get; set; }
        [Required]
        public string CategoryName { get; set; }
        [DisplayName("Display Order")]
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Must Be Greater Than Zero")]
        public int DisplayOrder { get; set; }
        public int Count { get; set; }

    }
}
