using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using GraysPavers.Models;

namespace GraysPavers.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        [DisplayName(" Product Name")]
        public string ProductName { get; set; }

        public string ShortDesc { get; set; }

        [DisplayName(" Product Description")]
        public string ProductDescription { get; set; }
        [Required]
        [Range(1, double.MaxValue)]
        [DisplayName(" Price")]
        public double ProductPrice { get; set; }
        public string Image { get; set; } // to store the image path string
        [Display(Name = " Category Type")]
        public int CategoryId { get; set; } // foreign key piece


        [ForeignKey("CategoryId")] // marked as fk 
        public virtual Category Category { get; set; } // to create a relationship between category and product

        [Display(Name = " Application Type")]
        public int AppId { get; set; } // foreign key piece


        [ForeignKey("AppId")] // marked as fk 
        public virtual AppType AppType { get; set; } // to create a relationship between category and product






        /* ONCE YOU ADD A NEW MODEL YOU MUST ADD THAT MODEL TO THE DBCONTEXT OTHERWISE IT WILL NOT BE CREATED. */

    }
}
