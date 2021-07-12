using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraysPavers.Models
{
    public class ShoppingCart
    {
         public int ProductId { get; set; }//this could be simplified by using list of integer, but long term more helpful in case we want to add more props to session

    }
}
