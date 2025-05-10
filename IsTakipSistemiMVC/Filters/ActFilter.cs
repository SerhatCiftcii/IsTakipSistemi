using IsTakipSistemiMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IsTakipSistemiMVC.Filters
{
    public class ActFilter : FilterAttribute, IActionFilter
    {
        IsTakipDBEntities entity=new IsTakipDBEntities();

        protected string aciklama;
        public ActFilter(string actAciklama)
        { 
       this.aciklama = actAciklama;
        }
        //çalıştıktan hemen sonra yapılıcak işlemler
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Controller.TempData["bilgi"] != null)
            {
                // Log kaydını alacağız
                Loglar log = new Loglar
                {
                    logAciklama = this.aciklama + "(" + filterContext.Controller.TempData["bilgi"] + ")",
                    actionAd = filterContext.RouteData.Values["action"]?.ToString() ?? "N/A",
                    controllerAd = filterContext.RouteData.Values["controller"]?.ToString() ?? "N/A",
                    tarih = DateTime.Now,
                    personelId = filterContext.HttpContext.Session["PersonelId"] != null ? Convert.ToInt32(filterContext.HttpContext.Session["PersonelId"]) : (int?)null
                };

                entity.Loglar.Add(log);
                entity.SaveChanges();
            }
        }
        //çalışmadan önce yapılıcak işlemler 
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
          
        }
    }
}