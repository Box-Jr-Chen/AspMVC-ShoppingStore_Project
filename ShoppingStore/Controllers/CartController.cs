﻿using ShoppingStore.Areas.Admin.Models.Data;
using ShoppingStore.Areas.Admin.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingStore.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            //Init the cart list
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();
            //Check if cart is empty
            if (cart.Count ==0 || Session["cart"] == null)
            {
                ViewBag.Message = "Yout cart is empty.";
                return View();
            }

            //Calculate total and save to ViewBag

            decimal total = 0m;

            foreach (var item in cart)
            {
                total += item.Total;
            }

            ViewBag.GrandTotal = total;

            //Return view with list
            return View(cart);
        }

        public ActionResult CartPartial()
        {
            //Init CartVM
            CartVM model = new CartVM();

            //Init quantity
            int qty = 0;

            //Init price
            decimal price = 0m;

            //Check for cart session
            if (Session["cart"] != null)
            {
                //Get total qty and price
                var list = (List<CartVM>)Session["cart"];

                foreach (var item in list)
                {
                    qty += item.Quantity;
                    price += item.Quantity * item.Price;
                }

                model.Quantity = qty;
                model.Price = price;
            }
            else
            {
                //Or set qty and price to 0
                model.Quantity = 0;
                model.Price = 0m;
            }

            //Return partial view with model
            return PartialView(model);
        }

        public ActionResult AddToCartPartial(int id)
        {
            //Init CartVM list
            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            //Init CartVM
            CartVM model = new CartVM();

            using (Dbase db = new Dbase())
            {
                //Get the product
                ProductDTO product = db.Products.Find(id);

                //Check uf product is already in cart
                var productInCart = cart.FirstOrDefault(x =>x.ProductId ==id);

                //if not,add now
                if (productInCart == null)
                {
                    cart.Add(new CartVM()
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = 1,
                        Price = product.Price,
                        Image = product.ImageName

                    });
                }
                else
                {
                    //if it is,increment
                    productInCart.Quantity++;
                }
            }
            //Get total aty and price and add to model

            int qty = 0;
            decimal price = 0m;

            foreach (var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }

            model.Quantity = qty;
            model.Price    = price;

            //Save cart back to session
            Session["cart"] = cart;


            //REturn partial view wirh model
            return PartialView(model);
        }

        //Get :/Cart/IncrementProduct
        public JsonResult IncrementProduct(int productId)
        {
            //Init cart init
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Dbase db = new Dbase())
            {

                //Get CartVM form list
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);


                //Increment qty
                model.Quantity++;

                //Store needed data
                var result = new { qty = model.Quantity,price = model.Price};

                //Return json with data
                return Json(result,JsonRequestBehavior.AllowGet);
            }
        }

        //Get :/Cart/DecrementProduct
        public ActionResult DecrementProduct(int productId)
        {
            //Init cart
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Dbase db = new Dbase())
            {
                //Get model drom list
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                //Decrement qty
                if (model.Quantity > 1)
                {
                    model.Quantity--;
                }
                else
                {
                    model.Quantity =0;
                    cart.Remove(model);
                }
                //Store needed data
                var result = new { qty = model.Quantity, price = model.Price };

                //Return json
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }

        //Get :/Cart/RemoveProduct
        public void RemoveProduct(int productId)
        {
            //Init cart list
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Dbase db = new Dbase())
            {
                //Get model from list
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                //Remove model form list
                cart.Remove(model);
            }
        }
    }
}