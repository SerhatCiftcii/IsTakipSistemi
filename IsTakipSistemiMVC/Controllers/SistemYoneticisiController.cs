using IsTakipSistemiMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using IsTakipSistemiMVC.Filters;
using System.Web.Security;

namespace IsTakipSistemiMVC.Controllers
{//butun exıonlar için calışır
    [AuthFilter(3)]
    public class SistemYoneticisiController : Controller
    {
        IsTakipDBEntities entity = new IsTakipDBEntities();

        // GET: SistemYoneticisi
        [AuthFilter(3)]
        public ActionResult Index()
        {
            //bunun yerıne artık [anuthfilter()] kullanıyoruz hangi birimse ona göre girebiliıor.
            /*  var yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

              if (yetkiTurId == 3)
              {
                  return View();
              }

              else
              {
                  return RedirectToAction("Index", "Login");
              }*/

            var calisanBilgisi = CalisanBilgisi();
            ViewBag.CalisanBilgisi=calisanBilgisi;

            //chartjs  için birimleri gösterme kısmında aktif olanları getirdim.
            var birimler = (from b in entity.Birimler where b.aktiflik == true select b).ToList();

            //script kısmınada veri girmek gerekir.
            var labelBirim = "[";
            foreach (var birim in birimler)

            {
                labelBirim += "'" + birim.birimAd + "',";

            }
            labelBirim += "]";
            ViewBag.labelBirim = labelBirim;

            //personellerin işlerini toplayarak yapıcaz ve list içinde tuttcaz
            List<int> islerToplam = new List<int>();

            foreach (var birim in birimler)
            {
                int toplam = 0;
                var personeller = (from p in entity.Personeller where p.personelBirimId == birim.birimId && p.aktiflik == true select p).ToList();
                //bu personellerin içindede personellerin işini getircez

                foreach (var personel in personeller)
                {
                    var isler = (from i in entity.Isler where i.isPersonelId == personel.personelId select i).ToList();
                    toplam += isler.Count();
                }
                islerToplam.Add(toplam);

            }
            //işler toplamını chartjsde göstermek için foreachla döncez 
            string dataIs = "[";
            foreach (var i in islerToplam)
            {
                dataIs += "'" + i + "',"; //burda tek tırnak unutulunca sağdaki ', olan char çalışmadı not!!!!!!!!!!!!!!
            }
            dataIs += "]";

            ViewBag.dataIs = dataIs;

            return View();
        }

        [AuthFilter(3)]
     
        public ActionResult Birim(string filter = "Tümü", string search = "")
        {
            var calisanBilgisi = CalisanBilgisi();
            ViewBag.CalisanBilgisi = calisanBilgisi;
            //   var birimler = (from b in entity.Birimler  select b).ToList();
            IEnumerable<Birimler> birimler; // Değişkeni tanımladık

            if (filter == "active")
            {
                birimler = from b in entity.Birimler
                           where b.aktiflik == true
                           && (string.IsNullOrEmpty(search) || b.birimAd.Contains(search))
                           select b;
            }
            else if (filter == "inactive")
            {
                birimler = from b in entity.Birimler
                           where b.aktiflik == false
                           && (string.IsNullOrEmpty(search) || b.birimAd.Contains(search))
                           select b;
            }
            else
            {
                birimler = from b in entity.Birimler
                           where string.IsNullOrEmpty(search) || b.birimAd.Contains(search)
                           select b;
            }

            ViewBag.SelectedFilter = filter; // Seçili filtreyi ViewBag'e ekledik
            ViewBag.SearchQuery = search; // Arama sorgusunu ViewBag'e ekledik
            return View(birimler.ToList()); // Model olarak view'e gönderiyoruz // Model olarak view'e gönderiyoruz
        }







        // [AuthFilter(3)] yerine bu geldi
        /*  var yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);
          if (yetkiTurId == 3)
          {
              var birimler = (from b in entity.Birimler where b.aktiflik==true select b).ToList();
              //birimlerin listesini elde edicez o yüzden veri gönderilicek
              return View(birimler); //model olarak viewmızı göndercez yoksa çalışmaz
          }*/
        /*  else
          {
              return RedirectToAction("Index", "Login");
          }*/

    public ActionResult Olustur()
        {
            var calisanBilgisi = CalisanBilgisi();
            ViewBag.CalisanBilgisi = calisanBilgisi;

            var yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);
            if (yetkiTurId == 3)
            {

                return View(); //model olarak viewmızı göndercez yoksa çalışmaz
            }

            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [ActFilter("Yeni Birim Eklendi")]
        [HttpPost]
        public ActionResult Olustur(string birimAd)
        {
            Birimler yeniBirim = new Birimler();
            //YUKARDA TANIMLADIK başharfleri büyük yapar küçük girsen bile en yukarda System.Globalization;
            string yeniAd = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(birimAd);
            yeniBirim.birimAd = yeniAd;
            yeniBirim.aktiflik = true;

            entity.Birimler.Add(yeniBirim);
            entity.SaveChanges();

            TempData["bilgi"] = yeniBirim.birimAd;

            return RedirectToAction("Birim");

        }
        public ActionResult Guncelle(int id)
        {
            var calisanBilgisi = CalisanBilgisi();
            ViewBag.CalisanBilgisi = calisanBilgisi;

            var yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);
            if (yetkiTurId == 3)
            {
                var birim = (from b in entity.Birimler where b.birimId == id select b).FirstOrDefault(); //güncellemk istedğimi birimin bütün bilgileri elimizde
                return View(birim); //model olarak viewmızı göndercez yoksa çalışmaz
            }

            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [HttpPost, ActFilter("Birim Güncellendi")]
        public ActionResult Guncelle(FormCollection fc)
        {
            var calisanBilgisi = CalisanBilgisi();
            ViewBag.CalisanBilgisi = calisanBilgisi;

            int birimId = Convert.ToInt32(fc["birimId"]);
            string yeniAd = fc["birimAd"];

            var birim = (from b in entity.Birimler where b.birimId == birimId select b).FirstOrDefault();//birime eriştik

            birim.birimAd = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(yeniAd);
            entity.SaveChanges();

            TempData["bilgi"] = birim.birimAd;

            return RedirectToAction("Birim");

        }
        [ActFilter("Birim Pasife Alındı")]
        public ActionResult Pasif(int id)
        {
            var calisanBilgisi = CalisanBilgisi();
            ViewBag.CalisanBilgisi = calisanBilgisi;

            var yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);
            if (yetkiTurId == 3)
            {
                var birim = (from b in entity.Birimler where b.birimId == id select b).FirstOrDefault(); //güncellemk istedğimi birimin bütün bilgileri elimizde
                birim.aktiflik = false; //aktifliksutununu ekledikten sonra silme işlemi silince false olucak
                /*     // //  entity.Birimler.Remove(birim); //birimi silme// artık aktifliği kontrol edicez */

                entity.SaveChanges();

                TempData["bilgi"] = birim.birimAd;
                return RedirectToAction("Birim");
            }

            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [ActFilter("Birim Aktife Alındı")]
        public ActionResult Aktif(int id)
        {
            var calisanBilgisi = CalisanBilgisi();
            ViewBag.CalisanBilgisi = calisanBilgisi;

            var yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);
            if (yetkiTurId == 3)
            {
                var birim = (from b in entity.Birimler where b.birimId == id select b).FirstOrDefault(); //güncellemk istedğimi birimin bütün bilgileri elimizde
                birim.aktiflik = true; //aktifliksutununu ekledikten sonra silme işlemi silince false olucak
                /*     // //  entity.Birimler.Remove(birim); //bunu kullanıesak tehlikeli **/ 
                entity.SaveChanges();

                TempData["bilgi"] = birim.birimAd;
                return RedirectToAction("Birim");
            }

            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [AuthFilter(3)]
        public ActionResult Loglar()
        {
            var calisanBilgisi = CalisanBilgisi();
            ViewBag.CalisanBilgisi = calisanBilgisi;
            int personelYetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);
            // Logları tarihe göre azalan sırada alma
            var loglar = entity.Loglar
        .Where(l => !l.isHata.HasValue || l.isHata.Value == false) // Hata logları hariç
        .OrderByDescending(l => l.tarih)
        .ToList();

            // View'e logları gönderme
            return View(loglar);

        }
        [AuthFilter(3)]
        public ActionResult HataIndex()
        {
                 var calisanBilgisi=CalisanBilgisi();
                    ViewBag.CalisanBilgisi=calisanBilgisi;

               int personelYetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            // Sadece hata loglarını al
            var hataLoglar = (from l in entity.Loglar
                              join p in entity.Personeller on l.personelId equals p.personelId
                              where p.personelYetkiTurId == personelYetkiTurId
                              && l.isHata.HasValue && l.isHata.Value // Sadece hata logları
                              orderby l.tarih descending
                              select l).ToList();
            return View(hataLoglar);
        }

        public string CalisanBilgisi()
        {
            var personelId = Convert.ToInt32(Session["PersonelId"]);
            var personelAdSoyad = (from p in entity.Personeller where p.personelId == personelId select p.personelAdSoyad).FirstOrDefault();

            // birimId nullable int olarak tanımlanmalı
            //birimId değişkenini int? olarak tanımladık, bu sayede null değeri alabilir.
            int? birimId = Session["PersonelBirimId"] != null ? Convert.ToInt32(Session["PersonelBirimId"]) : (int?)null;

            string birimAd;

            if (birimId == null)
            {
                birimAd = "Sistem Yöneticisi";
            }
            else
            {
                birimAd = (from b in entity.Birimler where b.birimId == birimId select b.birimAd).FirstOrDefault();
            }

            var calisanBilgisi = personelAdSoyad + "-" + birimAd;
            ViewBag.birimAd = birimAd;

            return calisanBilgisi;
        }

    }

}
