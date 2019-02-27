using ShoppingStore.Areas.Admin.Models.Data;
using ShoppingStore.Areas.Admin.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingStore.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PageController : Controller
    {
        // GET: Admin/Page
        public ActionResult Index()
        {
            List<PageVM> pagesList;

            using (Dbase db = new Dbase())
            {
                //Init the List
                pagesList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }

            return View(pagesList);
        }
        //GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }
        //POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            //Check Model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            using (Dbase db =new Dbase())
            {
                //Declare slug
                string slug;

                //Init pageDTO
                PageDTO dto = new PageDTO();

                //DTO  title
                dto.Title = model.Title;

                //Check for and set slug if need be
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }
                //Make sure title and slug are unique
                if (db.Pages.Any(x => x.Title == model.Title) || db.Pages.Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("","That title or slug already exists.");
                    return View(model);
                }


                //DTO the rest
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSlidebar = model.HasSlidebar;
                dto.Sorting = 100;

                //Save DTO
                db.Pages.Add(dto);
                db.SaveChanges();
            }


            //Set TempData message
            TempData["SM"] = "You have added a new page!";

            //Redirect
            return RedirectToAction("AddPage");
        }
        //GET: Admin/Pages/EditPage/id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            try
            {
                //Declare pageVM
                PageVM model;

                using (Dbase db = new Dbase())
                {
                    //Get the page
                    PageDTO dto = db.Pages.Find(id);

                    //Confirm page exists
                    if (dto == null)
                    {
                        return Content("The page does not exist.");
                    }

                    //Init pageVM
                    model = new PageVM(dto);

                }

                //Return view with model
                return View(model);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
        //POST: Admin/Pages/EditPage/id
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            //check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Dbase db =new Dbase())
            {

                //Get page id
                int id = model.Id;

                //Init slug
                string slug ="home";

                //Get the page
                PageDTO dto = db.Pages.Find(id);

                //DTO the title
                dto.Title = model.Title;

                //Check for slug and set it if need be
                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }

                //Make sure title and slug are unique
                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title) || 
                    db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "That title or slug already exists.");
                    return View(model);
                }

                //DTO the rest
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSlidebar = model.HasSlidebar;


                //Save the DTO
                db.SaveChanges();
            }
            //Set TempData message
            TempData["SM"] ="You have edited the page!";

            //Redirect
            return RedirectToAction("PageDetails/"+ model.Id);
        }
        //GET: Admin/Pages/PageDetails/id
        public ActionResult PageDetails(int id)
        {
            //Declare PageVM
            PageVM model;

            using (Dbase db = new Dbase())
            {
                //Get the page
                PageDTO dto = db.Pages.Find(id);

                //confirm page exists
                if (dto == null)
                {
                    return Content("The page does not exist.");
                }

                //Init PageVM
                model = new PageVM(dto);
             }

                //Return view with model
                return View(model);
        }
        //GET: Admin/Pages/DeletePage/id
        public ActionResult DeletePage(int id)
        {
            using (Dbase db = new Dbase())
            {
                //Get the page
                PageDTO dto = db.Pages.Find(id);

                //Remove the page
                db.Pages.Remove(dto);
                //Save
                db.SaveChanges();
            }

            //Redirect
            return RedirectToAction("Index");
        }

        //POST: Admin/Pages/RecorderPages
        [HttpPost]
        public void ReorderPages(int[] id)
        {
            using (Dbase db=new Dbase())
            {
                //Set initial count
                int count = 1;

                // Declare PageDTO
                PageDTO dto;

                //Set sorting for each page
                foreach (var pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;

                    db.SaveChanges();
                    count++;
                }
            }
        }

        //GET: Admin/Pages/EditSidebar
        [HttpGet]
        public ActionResult EditSidebar()
        {
            //Declare model
            SidebarVM model;

            using (Dbase db =new Dbase())
            {
                //Get the DTO
                SidebarDTO dto = db.Sidebar.Find(1);

                //Init model
                model = new SidebarVM(dto);
            }
            //Return view with model
            return View(model);
        }

        //POST: Admin/Pages/EditSidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using (Dbase db = new Dbase())
            {
                //Get the DTO
                SidebarDTO dto = db.Sidebar.Find(1);

                //DTO the body
                dto.Body = model.Body;

                //Save
                db.SaveChanges();
            }

            //Set TempData message
            TempData["SM"] = "You have edited the sidebar!";

            //Redirect
            return RedirectToAction("EditSidebar");
        }
    }
}