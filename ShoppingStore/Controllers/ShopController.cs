﻿using ShoppingStore.Areas.Admin.Models.Data;
using ShoppingStore.Areas.Admin.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingStore.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index","Pages");
        }

        public ActionResult CategoryMenuPartial()
        {
            //Declare list of CategoryVM
            List<CategoryVM> categoryVMlist;

            //Init the list
            using (Dbase db =new Dbase())
            {
                categoryVMlist = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
            }


            //Return partial with list
            return PartialView(categoryVMlist);
        }


        //GET:Shop/Category/name
        public ActionResult Category(string name)
        {
            //Declare a list of ProductVM
            List<ProductVM> productVMList;

            using (Dbase db = new Dbase())
            {
                //Get category id
                CategoryDTO categoryDTO = db.Categories.Where(x => x.Slug == name).FirstOrDefault();
                int catId = categoryDTO.Id;

                //Init the list
                productVMList = db.Products.ToArray().Where(x => x.CategoryId ==catId).Select(x => new ProductVM(x)).ToList();

                //Get category name
                var productCat = db.Products.Where(x => x.CategoryId == catId).FirstOrDefault();
                ViewBag.CategoryName = productCat.CategoryName;
            }

            //Return view with list
            return View(productVMList);
        }
    }
}