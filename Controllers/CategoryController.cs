using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using GraysPavers_DataAccess.Data;
using GraysPavers_DataAccess.Repository.IRepository;
using GraysPavers_Models;
using GraysPavers_Utility;

namespace GraysPavers.Controllers
{
    [Authorize(Roles = WebConstants.AdminRole)]

    public class CategoryController : Controller
    {


        //dependency Injection Example

        #region Dependency Injection

        /*
 * Below, we are using the categorycontroller ctor
 * and passing into it in order to access or pull
 * something from that container in the service
 * applicationdbcontext.
 * so, in order to populate _obj
 *we have to inject into a ctor the class or interface name.
 *then we set _obj = obj creating the dependency and allowing us to access
 * an instance of the applicationdbcontext.
 * now we can use _catRepo throughout our controller.
 * 
 */

        #endregion

        private readonly ICategoryRepository _catRepo; //create this so that we have an obj of applicationdbcontext

        public CategoryController(ICategoryRepository catRepo)
        {
            _catRepo = catRepo; // allows access to private readonly, .category method
        }


        // Get for Index
        public IActionResult Index()
        {
            IEnumerable<Category> objList = _catRepo.GetAll();

            #region Retreiving a list of categories

            /*
             * Here, we use IEnumerable<> because we want to iterate
             * through this list in order to get all categories.
             * so we create an IEnumerable of type categories
             * and assign it to our instance obj
             * _catRepo then .category in order to retrieve the categories.
             * then, all we do is return the objList. Done
             */

            #endregion

            return View(objList);
        }

        #region Create Action

        //Get for Create
        public IActionResult Create()
        {
            //IEnumerable<Category> objList = _catRepo.Category;

            #region Retreiving a list of categories

            /*
             * Here, we use IEnumerable<> because we want to iterate
             * through this list in order to get all categories.
             * so we create an IEnumerable of type categories
             * and assign it to our instance obj
             * _catRepo then .category in order to retrieve the categories.
             * then, all we do is return the objList. Done
             */

            #endregion

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken] // built in security
        //Post for Create
        public IActionResult Create(Category obj)
        {
            //server side validation
            if (ModelState.IsValid)
            {
                _catRepo.Add(obj);
                _catRepo.Save();
                return RedirectToAction("Index");


            }

            return View(obj);

            #region Retreiving a list of categories

            /*
             * Here, we use IEnumerable<> because we want to iterate
             * through this list in order to get all categories.
             * so we create an IEnumerable of type categories
             * and assign it to our instance obj
             * _catRepo then .category in order to retrieve the categories.
             * then, all we do is return the objList. Done
             */

            #endregion

        }


        #endregion

        #region Edit Action

        // get for edit 
            public IActionResult Edit(int? id)
            {
                if (id == null || id == 0) // looks for id if 0 or null, not found
                {
                    return NotFound();
                }

                var obj = _catRepo.Find(id.GetValueOrDefault()); // looks for pkey value id (find only works with pkeys)
                if (obj == null)
                {
                    return NotFound();
                }

                return View(obj); // found pkey value, so return view. 
            }


            // post for edit

            [HttpPost]
            [ValidateAntiForgeryToken] // built in security
            //Post for Create
            public IActionResult Edit(Category obj)
            {
                //server side validation
                if (ModelState.IsValid)
                {
                    _catRepo.Update(obj);
                    _catRepo.Save();
                    return RedirectToAction("Index");


                }

                return View(obj);
            }

        #endregion

        #region Delete Action

            // get for Delete 
            public IActionResult Delete(int? id)
            {
                if (id == null || id == 0) // looks for id if 0 or null, not found
                {
                    return NotFound();
                }

                var obj = _catRepo.Find(id.GetValueOrDefault()); // looks for pkey value id (find only works with pkeys)
                if (obj == null)
                {
                    return NotFound();
                }

                return View(obj); // found pkey value, so return view. 
            }


            // post for Delete

            [HttpPost]
            [ValidateAntiForgeryToken] // built in security
           
            public IActionResult DeletePost(int? id)
            {
                //server side validation
                    var obj = _catRepo.Find(id.GetValueOrDefault()); // looks for pkey value id (find only works with pkeys)
                    if (obj == null)
                    {
                        return NotFound();
                    }
                    _catRepo.Remove(obj);
                    _catRepo.Save();
                    return RedirectToAction("Index");


                

            }


        #endregion



    }
} 
