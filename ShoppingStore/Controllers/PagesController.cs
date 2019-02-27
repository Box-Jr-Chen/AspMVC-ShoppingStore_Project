using ShoppingStore.Areas.Admin.Models.Data;
using ShoppingStore.Areas.Admin.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingStore.Controllers
{
    public class PagesController : Controller
    {
        // GET: Index/{page}
        public ActionResult Index(string page = "")
        {
            // Get/set page slug
            if (page == "")
                page = "home";

            // Declare model and DTO
            PageVM model;
            PageDTO dto;

            // Check if page exists
            using (Dbase db = new Dbase())
            {
                if (!db.Pages.Any(x => x.Slug.Equals(page)))
                {
                    return RedirectToAction("Index", new { page = "" });
                }
            }

            // Get page DTO
            using (Dbase db = new Dbase())
            {
                dto = db.Pages.Where(x => x.Slug == page).FirstOrDefault();
            }

            // Set page title
            ViewBag.PageTitle = dto.Title;

            // Check for sidebar
            if (dto.HasSlidebar == true)
            {
                ViewBag.Sidebar = "Yes";
            }
            else
            {
                ViewBag.Sidebar = "No";
            }

            // Init model
            model = new PageVM(dto);

            // Return view with model
            return View(model);
        }

        public ActionResult PagesMenuPartial()
        {
            // Declare a list of PageVM
            List<PageVM> pageVMList;

            // Get all pages except home
            using (Dbase db = new Dbase())
            {
                pageVMList = db.Pages.ToArray().OrderBy(x => x.Sorting).Where(x => x.Slug != "home").Select(x => new PageVM(x)).ToList();
            }
            // Return partial view with list
            return PartialView(pageVMList);
        }

        public ActionResult SidebarPartial()
        {
            // Declare model
            SidebarVM model;

            // Init model
            using (Dbase db = new Dbase())
            {
                SidebarDTO dto = db.Sidebar.Find(1);

                model = new SidebarVM(dto);
            }

            // Return partial view with model
            return PartialView(model);
        }
    }
}