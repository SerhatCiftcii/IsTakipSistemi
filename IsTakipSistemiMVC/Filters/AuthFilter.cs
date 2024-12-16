using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IsTakipSistemiMVC.Filters
{
    public class AuthFilter : FilterAttribute, IAuthorizationFilter
    {
        protected int[] yetkiTurId;
        //hangi yetkıtureaitse ona göre gircek ornyonetici:1,calısan2:gibi 
        //1 veya 2 alması için parametre alıcak şekilde düzenledik o yüzden
        public AuthFilter(params int[] yetkiTur)
        {
            this.yetkiTurId = yetkiTur;

        }

        //filterContextile Session Bilgisine Erişlebilcek
        public void OnAuthorization(AuthorizationContext filterContext)
        {

            int yetkiTurId = Convert.ToInt32(filterContext.HttpContext.Session["PersonelYetkiTurId"]);
            //parametreden gelen yetkuTur ile YetkiTurId karşılaştıralım birbirine eşit değilse yönlenmeyi logine tekrar dönder 
            if (!this.yetkiTurId.Contains(yetkiTurId))
            {
                filterContext.Result = new RedirectResult("/Login/Index");
            }
        }
    }
}