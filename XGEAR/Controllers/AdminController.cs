using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XGEAR.Models;
using PagedList;
using PagedList.Mvc;


namespace XGEAR.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        XgearEntities database = new XgearEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();

        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            //gán giá trị người dùng nhập liệu
            var username = collection["username"];
            var password = collection["password"];
            //kiểm tra đăng nhập
            if (String.IsNullOrEmpty(username))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(password))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }
            else
            {
                AdminUser ad = database.AdminUsers.SingleOrDefault(n => n.NameUser == username && n.PasswordUser == password);
                if (ad != null)
                {
                    Session["TaikhoanAdmin"] = ad;
                    return RedirectToAction("Index", "Admin");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
                
            }
            return View();
        }
        //check role admin sau đó login vào trang admin
        public ActionResult Admin()
        {
            if (Session["TaikhoanAdmin"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                AdminUser ad = (AdminUser)Session["TaikhoanAdmin"];
                if (ad.RoleUser == "Admin")
                {
                   
                    return View();
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }
        }

        public ActionResult Category(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 5;
            return View(database.Categories.ToList().OrderBy(n => n.Id).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult CreateCate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateCate(Category cate)
        {
            try
            {
                database.Categories.Add(cate);
                database.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return Content("Error Create New");
            }

        }
        public ActionResult DetailsCate(int id)
        {
            return View(database.Categories.Where(s => s.Id == id).FirstOrDefault());
        }
        public ActionResult EditCate(int id)
        {
            return View(database.Categories.Where(s => s.Id == id).FirstOrDefault());

        }
        [HttpPost]
        public ActionResult EditCate(int id, Category cate)
        {
            database.Entry(cate).State = System.Data.Entity.EntityState.Modified;
            database.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult DeleteCate(int id)
        {
            return View(database.Categories.Where(s => s.Id == id).FirstOrDefault());

        }
        [HttpPost]
        public ActionResult DeleteCate(int id, Category cate)
        {
            try
            {
                cate = database.Categories.Where(s => s.Id == id).FirstOrDefault();
                database.Categories.Remove(cate);
                database.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {

                return Content("This data is using in other table, Error Delete!");
            }
        }
        public ActionResult Product(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            return View(database.Products.ToList().OrderBy(n => n.ProductID).ToPagedList(pageNumber, pageSize));
        }
       
        public ActionResult CreatePro()
        {
            List<Category> list = database.Categories.ToList().OrderBy(n => n.IDCate).ToList();
            ViewBag.Category = new SelectList(list, "IDCate", "NameCate", "");
            Product pro = new Product();
            return View(pro);
        }
        [HttpPost]
        public ActionResult CreatePro(Product pro)
        {
            List<Category> list = database.Categories.ToList().OrderBy(n => n.IDCate).ToList();
            try
            {
                if (pro.UploadImage != null)
                {
                    string filename = Path.GetFileNameWithoutExtension(pro.UploadImage.FileName);
                    string extent = Path.GetExtension(pro.UploadImage.FileName);
                    filename = filename + extent;
                    pro.ImagePro = "~/Content/images/" + filename;
                    pro.UploadImage.SaveAs(Path.Combine(Server.MapPath("~/Content/images/"), filename));
                }
                ViewBag.Category = new SelectList(list, "IDCate", "NameCate", "");

                database.Products.Add(pro);
                database.SaveChanges();
                return RedirectToAction("Index");
            }

            catch
            {

                return View();
            }
        }
        
        public ActionResult DetailsPro(int id)
        {
            return View(database.Products.Where(s => s.ProductID == id).FirstOrDefault());
        }
        public ActionResult EditPro(int id)
        {
            List<Category> list = database.Categories.ToList().OrderBy(n => n.IDCate).ToList();

            ViewBag.Category = new SelectList(list, "IDCate", "NameCate", "");
            return View(database.Products.Where(s => s.ProductID == id).FirstOrDefault());
            

        }
        [HttpPost]
        public ActionResult EditPro(int id, Product pro)
        {
            List<Category> list = database.Categories.ToList();
            ViewBag.listCategory = new SelectList(list, "IDCate", "NameCate", "");
            database.Entry(pro).State = System.Data.Entity.EntityState.Modified;
            database.SaveChanges();
            return RedirectToAction("Product");
        }
        public ActionResult DeletePro(int id)
        {
            return View(database.Products.Where(s => s.ProductID == id).FirstOrDefault());

        }
        [HttpPost]
        public ActionResult DeletePro(int id, Product pro)
        {
            try
            {
                pro = database.Products.Where(s => s.ProductID == id).FirstOrDefault();
                database.Products.Remove(pro);
                database.SaveChanges();
                return RedirectToAction("Product");
            }
            catch
            {

                return Content("This data is using in other table, Error Delete!");
            }
        }

    }
}