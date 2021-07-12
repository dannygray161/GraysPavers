using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraysPavers.Models.ViewModels
{
    public class DetailsViewModel
    {
        public DetailsViewModel()
        {
            Product = new Product(); // ctor to initialize product to a new object inside the view model, so that whenever the view model is called, product is initialized to new. 
        }
        public Product Product { get; set; }
        public bool ExistsInCart { get; set; }

    }
}
