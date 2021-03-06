using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GraysPavers_DataAccess.Data;
using GraysPavers_DataAccess.Repository.IRepository;
using GraysPavers_Models;
using GraysPavers_Models.ViewModels;
using GraysPavers_Utility;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GraysPavers.Controllers
{

    [Authorize(Roles = WebConstants.AdminRole)]
    public class ProductController : Controller
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
 * now we can use _db throughout our controller.
 * 
 */

        #endregion

        private readonly IProductRepository _prodRepo; //create this so that we have an obj of applicationdbcontext
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IProductRepository prodRepo, IWebHostEnvironment webHostEnvironment)
        {
            _prodRepo = prodRepo; // allows access to private readonly, .category method
            _webHostEnvironment = webHostEnvironment;
        }


        // Get for Index
        public IActionResult Index()
        {
            IEnumerable<Product> objList = _prodRepo.GetAll(includeProperties:"Category,AppType");
            //     ^^^^ this is eager loading. Much better than the way loading from the database is demonstrated below (foreach)

            //foreach (var obj in objList)
            //{
            //    obj.Category = _db.Category.FirstOrDefault(c => c.CategoryId == obj.CategoryId); // configure the foreign key relationship to display related data in index
            //    obj.AppType = _db.AppType.FirstOrDefault(c => c.AppId == obj.AppId); // configure the foreign key relationship to display related data in index

            //}

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

            return View(objList);
            //return View();
        }

        #region Upsert Action

        //Get for Upsert
        public IActionResult Upsert(int? id)
        {
            //IEnumerable<SelectListItem> CategoryDropDown = _db.Category.Select(i => new SelectListItem
            //{
            //    Text = i.CategoryName,
            //    Value = i.CategoryId.ToString()
            //});

            //ViewBag.CategoryDropDown = CategoryDropDown;

            Product product = new Product();

            ProductViewModel productVM = new ProductViewModel()
            {
                Product = new Product(),
                CategorySelectList = _prodRepo.GetAllDropDown(WebConstants.CategoryName),
                AppTypeSelectList = _prodRepo.GetAllDropDown(WebConstants.AppTypeName)

            }; // this loads category drop down list

            if (id == null)
            {
                //this establishes we want to create. 
                return View(productVM);
            }
            else
            {
                productVM.Product = _prodRepo.Find(id.GetValueOrDefault());
                if (productVM.Product == null)
                {
                    return NotFound();
                }

                return View(productVM);

            }

        }



        [HttpPost]
        [ValidateAntiForgeryToken] // built in security
        [ActionName("Upsert")]
        //Post for Create
        public IActionResult Upsert(ProductViewModel productVM)
        {
            //server side validation
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files; // allows us to store image
                string webRootPath = _webHostEnvironment.WebRootPath;

                if (productVM.Product.Id == 0)
                {
                    //creating and storing the file 
                    string upload = webRootPath + WebConstants.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream); // file will be stored in upload 
                    }

                    productVM.Product.Image = fileName + extension; // copies new path to image (new guid name and extension) not the constant path

                    _prodRepo.Add(productVM.Product);
                    TempData[WebConstants.Success] = "Product Added Successfully";


                }
                else
                {
                    //updating
                    var objFromDb = _prodRepo.FirstOrDefault(u => u.Id == productVM.Product.Id, isTracking:false); // as no tracking to avoid error of tracking mult objs
                    if (files.Count > 0)
                    {
                        string upload = webRootPath + WebConstants.ImagePath; // save image path to upload
                        string fileName = Guid.NewGuid().ToString(); // generate a guid name
                        string extension = Path.GetExtension(files[0].FileName); // get extension of file

                        var oldFile = Path.Combine(upload, objFromDb.Image); // get old file from db
                        if (System.IO.File.Exists(oldFile)) // check if exists
                        {
                            System.IO.File.Delete(oldFile); // if exists delete it
                        }


                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create)) //use filestream and append all three vars above
                        {
                            files[0].CopyTo(fileStream); // file will be stored in upload moved over to folder. 
                        }

                        productVM.Product.Image = fileName + extension; // saves new image


                    }
                    else
                    {
                        productVM.Product.Image = objFromDb.Image; // keeps old image if image wasnt updated but something else was
                    }

                    _prodRepo.Update(productVM.Product); // this updates everything that was changed in product
                    TempData[WebConstants.Success] = "Product Updated Successfully";

                }

                _prodRepo.Save();
                return RedirectToAction("Index");


            }
            TempData[WebConstants.Error] = "Error Please try again";


            productVM.CategorySelectList = _prodRepo.GetAllDropDown(WebConstants.CategoryName);
            productVM.AppTypeSelectList = _prodRepo.GetAllDropDown(WebConstants.AppTypeName);



            return View(productVM); // state is invalid 

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


        #region Delete Action

            // get for Delete 
            public IActionResult Delete(int? id)
            {
                if (id == null || id == 0)
                {
                    return NotFound();
                }

                Product product =
                    _prodRepo.FirstOrDefault(u => u.Id == id, includeProperties: "Category,AppType");
                    
                                                                                                                                        ////product.Category = _db.Category.Find(product.CategoryId); // load category associated with product ^^ this does it more efficiently

            if (product == null)
            {
                return NotFound();
            }

            return View(product);

            //return View();
            }


            // post for Delete

            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken] // built in security
           
            public IActionResult DeletePost(int? id)
            {
                //server side validation
                    var obj = _prodRepo.Find(id.GetValueOrDefault()); // looks for pkey value id (find only works with pkeys)
                    if (obj == null)
                    {
                        return NotFound();
                    }

                    string upload = _webHostEnvironment.WebRootPath + WebConstants.ImagePath; // get image from db

                    var oldFile = Path.Combine(upload, obj.Image); // get old file from db
                    if (System.IO.File.Exists(oldFile)) // check if exists
                    {
                        System.IO.File.Delete(oldFile); // if exists delete it
                        _prodRepo.Remove(obj);
                        _prodRepo.Save();
                        TempData[WebConstants.Success] = "Deleted Successfully";




                    }
                    TempData[WebConstants.Error] = "Error Please try again";






                 return RedirectToAction("Index");


                

            }


        #endregion



    }
} 
