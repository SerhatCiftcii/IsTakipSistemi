using IsTakipSistemiMVC.Filters;
using IsTakipSistemiMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IsTakipSistemiMVC.Controllers
{
    public class SifreKontrolController : Controller
    {
        IsTakipDBEntities entity = new IsTakipDBEntities();
        // GET: SifreKontrol

        public ActionResult Index()
        {
            var calisanBilgisi = CalisanBilgisi();
            ViewBag.CalisanBilgisi = calisanBilgisi;

            var personelId = Convert.ToInt32(Session["PersonelId"]);

            if(personelId==0)   return RedirectToAction("Index","Login");

            var personel=(from p in entity.Personeller where p.personelId==personelId select p).FirstOrDefault();
            ViewBag.mesaj = null;
            ViewBag.sitil = null;
            ViewBag.yetkiTurId = null;
            return View(personel);
        }

        [HttpPost,ActFilter("Parola Değiştirildi")]
        public ActionResult Index(int personelId , string yeniParola, string yeniParolaKontrol,string eskiParola)
        {
            var personel=(from p in entity.Personeller where p.personelId == personelId select p).FirstOrDefault();
            if (eskiParola != personel.personelParola)
            {
                ViewBag.mesaj = "Eski parolanızı yanlış girdiniz";
                ViewBag.sitil = "alert alert-danger";

                return View(personel);
            }

            if(yeniParola != yeniParolaKontrol)
            {
                ViewBag.mesaj = "Yeni parola ve Yeni parola tekrarı eşleşmedi";
                ViewBag.sitil = "alert alert-danger";

                return View(personel);
            }
            if (yeniParola.Length<6  || yeniParola.Length>15)
            {
                ViewBag.mesaj = "Yeni parola en az 6 karakter en çok 15 karakter olmalıdır";
                ViewBag.sitil = "alert alert-danger";

                return View(personel);
            }
            //Yeni parolaya geçiş 
            personel.personelParola = yeniParola;
            //yeni bir kullanıcı değiştirdiyse artık eski kullanıcı olucak false döncek
            //yeni personel değiştirseydi false olmasını beklicektik şifresini değiştirdiği için.
            personel.yeniPersonel = false;
            entity.SaveChanges();
            //log kaydı
            TempData["bilgi"]=personel.personelKullaniciAd;
            //yönlendirme js ile olucak
            ViewBag.mesaj = "Parolanız başarıyla değiştirildi. Anasayfaya yönlendiriliyorsunuz";
            ViewBag.sitil = "alert alert-success";
            ViewBag.yetkiTurId = personel.personelYetkiTurId;
            return View(personel);
        }
        public string CalisanBilgisi()
        {
            var personelId = Convert.ToInt32(Session["PersonelId"]);
            var personelAdSoyad = (from p in entity.Personeller where p.personelId == personelId select p.personelAdSoyad).FirstOrDefault();

            int birimId = Convert.ToInt32(Session["PersonelBirimId"]);
            var birimAd = (from b in entity.Birimler where b.birimId == birimId select b.birimAd).FirstOrDefault();

            var calisanBilgisi = personelAdSoyad + "-" + birimAd;

            return calisanBilgisi;
        }


    }
}