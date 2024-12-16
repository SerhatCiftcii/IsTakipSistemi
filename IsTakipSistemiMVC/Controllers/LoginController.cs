using IsTakipSistemiMVC.Filters;
using IsTakipSistemiMVC.Models;
using System.Linq;
using System.Web.Mvc;

namespace IsTakipSistemiMVC.Controllers
{
    public class LoginController : Controller //control clasından miras alındı
    {
        IsTakipDBEntities entity = new IsTakipDBEntities(); //bu sayede bağlantı kurcaz
        // GET: Login
        public ActionResult Index() //actıonresult methodu view döncercek
        {
            ViewBag.mesaj = null;
            return View();
        }

        // POST: Login
        [HttpPost,ExcFilter]
        public ActionResult Index(string kullaniciAd, string parola)
        { //personel bilgilerini kullanıcıadve parolya göre çekıyoruz
            var personel = (from p in entity.Personeller
                            where p.personelKullaniciAd == kullaniciAd &&
                          p.personelParola == parola && p.aktiflik==true
                            select p).FirstOrDefault();
           
           if (personel != null)
            {
                var birim=(from b in entity.Birimler where b.birimId==personel.personelBirimId select b).FirstOrDefault();
                //Session sunucuda tutlur.(Cokkie?)
                Session["PersonelAdSoayd"] = personel.personelAdSoyad;
                Session["PersonelId"] = personel.personelId;
                Session["PersonelBirimId"] = personel.personelBirimId;
                Session["PersonelYetkiTurId"] = personel.personelYetkiTurId;

               //errorsayfasıiçin yorumsatırına alındı
            if (birim == null)
              {
                  return RedirectToAction("Index", "SistemYoneticisi");
              }


                if (birim.aktiflik == true)
                {
                    if (personel.yeniPersonel == true)
                    {
                        return RedirectToAction("Index", "SifreKontrol");

                    }
             

                    switch (personel.personelYetkiTurId)
                    {
                        case 1:
                            return RedirectToAction("Index", "Yonetici");
                       case 2:
                            return RedirectToAction("Index", "Calisan");
                       default:
                       return View();
                    }
                }
                else
                {
                    ViewBag.mesaj = "Biriminiz silindiği için giriş yapamazsınız";
                    return View();
                }
               
            }
            else
            {
                //DB kullaniciad ve parola eşit olmayanlar (sistemde yok)
                ViewBag.mesaj = "kullanıcı adı yada parola yanlış";
                return View();
            }
        }
    }
}