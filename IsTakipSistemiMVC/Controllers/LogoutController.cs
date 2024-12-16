using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IsTakipSistemiMVC.Controllers
{
    public class LogoutController : Controller
    {
        // GET: Logout
        public ActionResult Index() 
        {
            Session.Abandon(); //çıkış yapılması için sessıonların bosaltılması gerekıyor
            return RedirectToAction("Index","Login");
          
        }
    }
}