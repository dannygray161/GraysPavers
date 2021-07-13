using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using GraysPavers_DataAccess.Data;
using GraysPavers_Models;
using GraysPavers_Utility;

namespace GraysPavers.Controllers
{
    [Authorize(Roles = WebConstants.AdminRole)]

    public class AppTypeController : Controller
    {
        private readonly ApplicationDbContext _db; //create this so that we have an obj of applicationdbcontext

        public AppTypeController(ApplicationDbContext db)
        {
            _db = db; // allows access to private readonly, .category method
        }

        public IActionResult Index()
        {
            IEnumerable<AppType> objList = _db.AppType;

            return View(objList);
        }

        #region Create Actions

        // get for create
        public IActionResult Create()
        {
            IEnumerable<AppType> objList = _db.AppType;

            #region Retreiving a list of categories

            /*
             * Here, we use IEnumerable<> because we want to iterate
             * through this list in order to get all categories.
             * so we create an IEnumerable of type categories
             * and assign it to our instance obj
             * _db then .category in order to retrieve the categories.
             * then, all we do is return the objList. Done
             */

            #endregion

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken] // built in security
        //Post for Create
        public IActionResult Create(AppType obj)
        {
            if (ModelState.IsValid) // server side validation
            {
                _db.AppType.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(obj);


            #region Retreiving a list of categories

            /*
             * Here, we use IEnumerable<> because we want to iterate
             * through this list in order to get all categories.
             * so we create an IEnumerable of type categories
             * and assign it to our instance obj
             * _db then .category in order to retrieve the categories.
             * then, all we do is return the objList. Done
             */

            #endregion

        }


        #endregion


        #region Edit Actions

        //Get for Edit
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var obj = _db.AppType.Find(id); // looks for pkey value id (find only works with pkeys)
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);

        }


        [HttpPost]
        [ValidateAntiForgeryToken] // built in security
        //Post for Create
        public IActionResult Edit(AppType obj)
        {
            if (ModelState.IsValid)
            {
                _db.AppType.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");

            }

            return View(obj);



        }

        #endregion


        #region Delete Actions
         // Get For Delete
        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }

            var obj = _db.AppType.Find(id);
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _db.AppType.Find(id);
            if (id == null || id == 0)
            {
                return NotFound();
            }

            _db.AppType.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        #endregion
























    }


}

