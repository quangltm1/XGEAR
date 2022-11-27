using XGEAR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XGEAR.Controllers
{
    public class OrderDetailController : Controller
    {
        XgearEntities database = new XgearEntities();
        // GET: OrderDetail
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GroupByTop5()
        {
            List<OrderDetail> orderD = database.OrderDetails.ToList();
            List<Product> proList = database.Products.ToList();
            var query = from od in orderD
                        join p in proList on od.IDProduct equals p.ProductID into tbl
                        group od by new
                        {
                            idPro = od.IDProduct,
                            namePro = od.Product.NamePro,
                            imagePro = od.Product.ImagePro,
                            price = od.Product.Price,
                        } into gr
                        orderby gr.Sum(s => s.Quantity) descending
                        select new ViewModel
                        {
                            IDPro = gr.Key.idPro,
                            NameCate = gr.Key.namePro,
                            ImgPro = gr.Key.imagePro,
                            pricePro = (decimal)gr.Key.price,
                            Sum_Quantity = gr.Sum(s => s.Quantity)
                        };
            return View(query.Take(5).ToList());
        }
    }
}