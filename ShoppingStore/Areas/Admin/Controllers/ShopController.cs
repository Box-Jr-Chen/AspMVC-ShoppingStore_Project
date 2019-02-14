using ShoppingStore.Areas.Admin.Models.ViewModels.Shop;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoppingStore.Areas.Admin.Models.Data;
using System.Web.Helpers;
using System.Diagnostics;
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
        [HttpGet]
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

        //POST: Admin/shop/AddProduct
        [HttpPost]
        public ActionResult AddProduct(ProductVM model, HttpPostedFileBase file)
        {
            //Check model state
            if (! ModelState.IsValid)
            {
                using (Dbase db = new Dbase())
                {
                    model.Categories = new SelectList(db.Categories.ToList(),"Id","Name");
                    return View(model);
                }
            }

            //Make sure product name is unique
            using (Dbase db = new Dbase())
            {
                if (db.Products.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("","That product name is taken!");
                    return View(model);
                }
            }

            //Declare product id
            int id;
            //Initialize and save productDTO
            using (Dbase db = new Dbase())
            {
                ProductDTO product = new ProductDTO();

                product.Name        = model.Name;
                product.Slug        = model.Name.Replace(" ", "-").ToLower();
                product.Description = model.Description;
                product.Price       = model.Price;
                product.CategoryId = model.CategoryId;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                product.CategoryName = catDTO.Name;

                db.Products.Add(product);
                db.SaveChanges();

                //Get the id
                id = product.Id;
            }

            //Set TempData message
            TempData["SM"] = "You have added a product";

            #region Upload Image

            //Create necessary directories
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads",Server.MapPath(@"\")));


            //Check if a file was uploaded
            var pathString1 = Path.Combine(originalDirectory.ToString(),"Products");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() );
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

            if (!Directory.Exists(pathString1))
                Directory.CreateDirectory(pathString1);

            if (!Directory.Exists(pathString2))
                Directory.CreateDirectory(pathString2);

            if (!Directory.Exists(pathString3))
                Directory.CreateDirectory(pathString3);

            if (!Directory.Exists(pathString4))
                Directory.CreateDirectory(pathString4);

            if (!Directory.Exists(pathString5))
                Directory.CreateDirectory(pathString5);

            if (file == null)
                Debug.Write("==============NULL");

            //Check if a file was uploaded
            if (file != null && file.ContentLength > 0)
            {
                //Get file extension
                string ext = file.ContentType.ToLower();

                //Verify extension
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png" )
                {
                    using (Dbase db = new Dbase())
                    {
                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "The image was not uploaded -wrong image extension.");
                        return View(model);
                    }
                }

                //Init image name
                string imageName = file.FileName;

                //Save image name to DTO
                using (Dbase db = new Dbase())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }

                //Set original and thumb image paths
                var path  = string.Format("{0}\\{1}",pathString2,imageName);
                var path2 = string.Format("{0}\\{1}", pathString3, imageName);


                TempData["SM"] = path2;
               // Console.WriteLine(" == > " + path);
              //  Console.WriteLine(" == > " + path2);
                //Save original
                file.SaveAs(path);

                //Create and save thumb
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);
            }

            #endregion

            //Redirect
            return RedirectToAction("AddProduct");
        }
    }
}