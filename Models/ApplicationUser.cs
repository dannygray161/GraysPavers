using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace GraysPavers.Models
{
    public class ApplicationUser : IdentityUser
    {
        // inherits from IdentityUser so that our properties get added to the aspnet users default table

        public string FirstName { get; set; }
        public string LastName { get; set; }

    }
}
