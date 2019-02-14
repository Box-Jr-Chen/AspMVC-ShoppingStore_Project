using ShoppingStore.Areas.Admin.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoppingStore.Areas.Admin.Models.Data;
namespace ShoppingStore.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop/Categories
        public ActionResult Categories()
        {
            //Declare a list of models
            List<CategoryVM> categoryVMList;

            using (Dbase db =new Dbase())
            {
                //Init the list
                categoryVMList = db.Categories
                                .ToArray()
                                .OrderBy(x => x.Sorting)
                                .Select(x => new CategoryVM(x))
                                .ToList();
            }
            //Return view with list
            return View(categoryVMList);
        }

        //POST: Admin/shop/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            //Declare id
            string id;


            using (Dbase db = new Dbase())
            {
                //Check that the category name is unique
                if (db.Categories.Any(x => x.Name == catName))
                    return "titletaken";

                //Init DTO
                CategoryDTO dto = new CategoryDTO();

                //Add to DTO
                dto.Name = catName;
                dto.Slug = catName.Replace(" ", "-").ToLower();
                dto.Sorting = 100;

                //Save DTO
                db.Categories.Add(dto);
                db.SaveChanges();

                //Get the id
                id = dto.Id.ToString();
             }

            //Return id
            return id;
        }

        //POST: Admin/shop/ReorderCategories
        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            using (Dbase db = new Dbase())
            {
                //Set initial count
                int count = 1;

                // Declare CategoryDTO
                CategoryDTO dto;

                //Set sorting for each category
                foreach (var catId in id)
                {
                    dto = db.Categories.Find(catId);
                    dto.Sorting = count;

                    db.SaveChanges();
                    count++;
                }
            }
        }

        //GET: Admin/shop/DeleteCategory/id
        public ActionResult DeleteCategory(int id)
        {
            using (Dbase db = new Dbase())
            {
                //Get the category
                CategoryDTO dto = db.Categories.Find(id);

                //Remove the Category
                db.Categories.Remove(dto);
                //Save
                db.SaveChanges();
            }

            //Redirect
            return RedirectToAction("Categories");
        }

        //POST:Admin/Shop/RenameCategory
        [HttpPost]
        public string RenameCategory(string newCatName,int id)
        {
            
            using (Dbase db =new Dbase())
            {
                //Check category name is unique
                if (db.Categories.Any(x => x.Name ==newCatName))
                {
                    return "titletaken";
                }

                //Get  DTO
                CategoryDTO dto = db.Categories.Find(id);

                //Edit DTO
                dto.Name = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower();
                //Save
                db.SaveChanges();
            }
            //Return
            return "ok";
        }

        //GET: Admin/shop/AddProduct
        public ActionResult AddProduct()
        {
            //Init model
            ProductVM model = new ProductVM();

            //Add select list of categories 
            using (Dbase db = new Dbase())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }

            //Return view with model
            return View(model);
        }
    }
}