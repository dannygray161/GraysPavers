using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GraysPavers.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace GraysPavers.Data
{

    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            #region DbContext Implementation

            /*
             *created a ctor, implemented dbcontextOptions<ApplicationDbContext> and name it options
             * then specify it implements base(options)
             */

            #endregion

        }

        #region Creating and Pushing Tables to the Database

        /*
         *Here, we created a prop of type dbset and passed the entity we are trying to create
         * We then give the table a meaningful name
         * then we go to startup class file.
         *
         * now that start up is done, we head over to package manager console to create the database with scaffolding
         * then update the database, then add initial migration
         */

        #endregion
        public DbSet<Category> Category { get; set; }
        public DbSet<AppType> AppType { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }










    }
}
