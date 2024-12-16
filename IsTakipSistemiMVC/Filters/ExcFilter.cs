using IsTakipSistemiMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace IsTakipSistemiMVC.Filters
{       
    //hataayıklama
    public class ExcFilter : FilterAttribute, IExceptionFilter
    {
          
            IsTakipDBEntities entity = new IsTakipDBEntities();

            public void OnException(ExceptionContext filterContext)
            {
                // Hata işleme
                filterContext.ExceptionHandled = true;

                // Log kaydı ekleme
                Loglar log = new Loglar
                {
                    logAciklama = filterContext.Exception.Message,
                    actionAd = filterContext.RouteData.Values["action"]?.ToString() ?? "N/A",
                    controllerAd = filterContext.RouteData.Values["controller"]?.ToString() ?? "N/A",
                    tarih = DateTime.Now,
                    personelId = filterContext.HttpContext.Session["PersonelId"] != null ? Convert.ToInt32(filterContext.HttpContext.Session["PersonelId"]) : (int?)null,
                    // isHata: true false olcak loglarda hata exp olduğunda aynı loglar gibi bir sayfa olsun hata alınanlar true loglardada falsee olucak gözükmücek 
                    isHata = true // Hata logu olduğu için true olarak işaretleniyor

                   
                };

                entity.Loglar.Add(log);
                entity.SaveChanges();

                // Hata sayfasına yönlendirme
                filterContext.Controller.TempData["error"] = filterContext.Exception;
                filterContext.Result = new RedirectResult("/Error/Index");
            }
        }
    }