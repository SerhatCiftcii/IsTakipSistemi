using IsTakipSistemiMVC.Filters;
using IsTakipSistemiMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IsTakipSistemiMVC.Controllers
{
    public class PersonelController : Controller
    {
        IsTakipDBEntities entity = new IsTakipDBEntities();
        // GET: Personel

        [AuthFilter(1,3)]
        public ActionResult Index()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);
            ViewBag.yetkiTurId=yetkiTurId;
            var calisanBilgisi = CalisanBilgisi();
            ViewBag.CalisanBilgisi = calisanBilgisi;

            //&& p.aktiflik == true aktiflik olanlarda gözükcek pasif olanlarda biz bastığımızda aktif bpasif bilgisi gözükücek zaten vieewda belli kodu kaldı

            //index içine personelleri çağıralım ksıtlamayıda yaaplımki başka sistem yonetıcısı başkası adına değişiklik yapmasın göremesin aynı zamanda **yeni olarak silmeişlemınde true olanlar gözüksümü ekledik
            var personeller = (from p in entity.Personeller where p.personelYetkiTurId != 3  select p).ToList();

            return View(personeller);
        }
        [AuthFilter(1,3)]
        public ActionResult Olustur()
        {
           int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);
            ViewBag.yetkiTurId = yetkiTurId;
          var calisanBilgisi = CalisanBilgisi();
            ViewBag.CalisanBilgisi = calisanBilgisi;

            BirimYetkiTurler by = birimYetkiTurlerDoldur();

            //bir birimde 2.yönetici atanmak isterse hata mesajını göndermemiz lazımşuanlık herhangı mesaj göndermıyoruz
            ViewBag.mesaj = null;
           return View(by);
        }

        [HttpPost, ActFilter("Yeni Personel Eklendi")]
        public ActionResult Olustur(FormCollection fc)
        {
            //nameden aldığımızkullanici ad
            string personelKullaniciAd = fc["kullaniciAd"];
            var personel = (from p in entity.Personeller where p.personelKullaniciAd == personelKullaniciAd select p)
                .FirstOrDefault();
            //birimin sadece 1tane yöneticisi olur onun kontrolunu yapıyoruz
            var birimId = Convert.ToInt32(fc["birim"]);
            if (Convert.ToInt32(fc["yetkiTur"]) == 1)
            {
                var birimYoneticiKontrol = (from p in entity.Personeller
                                            where p.personelBirimId == birimId &&
                  p.personelYetkiTurId == 1
                                            select p).FirstOrDefault();
                if (birimYoneticiKontrol != null)
                {
                    BirimYetkiTurler by = birimYetkiTurlerDoldur();
                    //bir birimde 2.yönetici atanmak isterse hata mesajını göndermemiz lazım
                    ViewBag.mesaj = "Bu birimin sadece bir yöneticisi olabilir";
                    TempData["bilgi"] = null;
                    return View(by);

                }
            }

            if (personel == null)
            {
                Personeller yeniPersonel = new Personeller();
                yeniPersonel.personelAdSoyad = fc["adSoyad"];
                yeniPersonel.personelKullaniciAd = personelKullaniciAd;
                yeniPersonel.personelParola = fc["parola"];
                yeniPersonel.personelBirimId = Convert.ToInt32(fc["birim"]);
                yeniPersonel.personelYetkiTurId = Convert.ToInt32(fc["yetkiTur"]); //burdakı hata 3saat sürdü.....
                yeniPersonel.yeniPersonel = true;

                yeniPersonel.aktiflik = true;

                entity.Personeller.Add(yeniPersonel);
                entity.SaveChanges();
                //actfilterkuulanıyyorsak tempdata bilgiye ihtiyaç var
                TempData["bilgi"] = yeniPersonel.personelKullaniciAd;
                return RedirectToAction("Index");

            }
            else
            {
                BirimYetkiTurler by = birimYetkiTurlerDoldur();

                //bir birimde 2.yönetici atanmak isterse hata mesajını göndermemiz lazım
                ViewBag.mesaj = "Kullanıcı Adı Bulunmaktadır";
                TempData["bilgi"] = null;
                return View(by);
            }
        }
        [AuthFilter(1, 3)]
        public ActionResult Guncelle(int id)
        {
            var calisanBilgisi = CalisanBilgisi();
            ViewBag.CalisanBilgisi = calisanBilgisi;

            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);
            ViewBag.yetkiTurId = yetkiTurId;

            var personel = (from p in entity.Personeller where p.personelId == id select p).FirstOrDefault();

            return View(personel);

        }


        [HttpPost, ActFilter("Personel Güncellendi")]
        public ActionResult Guncelle(int id, string adSoyad)
        {
            Personeller personel = (from p in entity.Personeller where p.personelId == id select p).FirstOrDefault();
            personel.personelAdSoyad = adSoyad;
            entity.SaveChanges();
            TempData["bilgi"] = personel.personelKullaniciAd;
            return RedirectToAction("Index");
        }
        [AuthFilter(1,3)]
        public ActionResult Pasif(int id)
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);
            ViewBag.yetkiTurId= yetkiTurId; 

            var calisanBilgisi = CalisanBilgisi();
            ViewBag.CalisanBilgisi = calisanBilgisi;

            Personeller personel = (from p in entity.Personeller where p.personelId == id select p).FirstOrDefault();
            return View(personel);
        }

        [HttpPost, ActFilter("Personel Pasife Alındı")]
        public ActionResult Pasif (FormCollection fc)
        {
            //hiidenfordan gelen  personelId
            int personelId = Convert.ToInt32(fc["personelId"]);

            var personel = (from p in entity.Personeller where p.personelId == personelId select p).FirstOrDefault();
            //şimdi aktifliği güncellicez
            personel.aktiflik = false;
            entity.SaveChanges();

            TempData["bilgi"] = personel.personelKullaniciAd;

            return RedirectToAction("Index");

        }


        public BirimYetkiTurler birimYetkiTurlerDoldur()
        {
            BirimYetkiTurler by = new BirimYetkiTurler();

            by.birimlerList = (from b in entity.Birimler where b.aktiflik == true select b).ToList();
            by.yetkiTurlerList = (from y in entity.YetkiTurler where y.yetkiTurId != 3 select y).ToList();
            return by;
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

        [AuthFilter(1, 3)]
        public ActionResult Aktif(int id)
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);
            ViewBag.yetkiTurId = yetkiTurId;

            var calisanBilgisi = CalisanBilgisi();
            ViewBag.CalisanBilgisi = calisanBilgisi;

            Personeller personel = (from p in entity.Personeller where p.personelId == id select p).FirstOrDefault();
            return View(personel);
        }

        [HttpPost, ActFilter("Personel Aktife Alındı")]
        public ActionResult Aktif (FormCollection fc)
        {
            //hiidenfordan gelen  personelId
            int personelId = Convert.ToInt32(fc["personelId"]);

            var personel = (from p in entity.Personeller where p.personelId == personelId select p).FirstOrDefault();
            //şimdi aktifliği güncellicez
            personel.aktiflik = true;
            entity.SaveChanges();

            TempData["bilgi"] = personel.personelKullaniciAd;

            return RedirectToAction("Index");

        }


    }

}