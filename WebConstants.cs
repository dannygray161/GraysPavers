using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraysPavers
{
    public static class WebConstants
    {
        public const string ImagePath = @"\images\product\"; // to store image path instead of writing it every time

        // we also use this in order to access our session without "magic Strings"

        public const string SessionCart = "ShoppingCartSession"; // gives access to session 

        public const string AdminRole = "Admin"; // const for the role we are creating

        public const string CustomerRole = "Customer"; // const for the role we are creating

        public const string AdminEmail = "dieseldan14@gmail.com"; // const for admin email
    }
}
