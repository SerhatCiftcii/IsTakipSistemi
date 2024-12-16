using IsTakipSistemiMVC.Filters;
using IsTakipSistemiMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IsTakipSistemiMVC.Controllers
{
    public class YoneticiController : Controller
    {
        IsTakipDBEntities entity = new IsTakipDBEntities(); //bunu unutmayalım 


        // GET: Yonetici
        public ActionResult Index()
        {
            var calisanBilgisi = CalisanBilgisi();
            ViewBag.CalisanBilgisi = calisanBilgisi;
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]); // Kullanıcının yetki türü
            
            if (yetkiTurId == 1)
            {
                var personelId = Convert.ToInt32(Session["PersonelId"]);

                var birimId = Convert.ToInt32(Session["PersonelBirimId"]);

                var personeller = from p in entity.Personeller
                                  join i in entity.Isler on p.personelId equals i.isPersonelId into isler
                                  where p.personelBirimId == birimId && p.personelYetkiTurId != 1
                                  select new
                                  {
                                      personelAd = p.personelAdSoyad,
                                      isler = isler
                                  };

                List<ToplamIs> list = new List<ToplamIs>();

                foreach (var personel in personeller)
                {
                    ToplamIs toplamIs = new ToplamIs
                    {
                        personelAdSoyad = personel.personelAd,
                        toplamIs = personel.isler.Count(),
                        okunacakIs = personel.isler.Count(i => !i.isOkunma.HasValue || !i.isOkunma.Value), // Okunacak işler, 
                        yapiliyorIs = personel.isler.Count(i => i.isDurumId == 1),  // Yapılıyor durumda olan işler (aslında yapıma başlanmamış iş tıklayınca tamamlanmayı bekleyen işe düşücek)
                        yapilacakIs = personel.isler.Count(i => i.isDurumId == 2)   // Yapılmayı bekleyen işler
                    };

                    list.Add(toplamIs);
                }

                IEnumerable<ToplamIs> siraListe = list.OrderByDescending(i => i.toplamIs);
                return View(siraListe);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult Ata()
        {
           //artık giriş yapan kullanıcının yetkituru elimizde

            //fonksiyonumuzu burda tanımladık
            var calisanBilgisi = CalisanBilgisi();
            ViewBag.CalisanBilgisi = calisanBilgisi;
              int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);
            if (yetkiTurId == 1)  //indexe bastığımızda diret sayfanın açılmamsı lazım yoneticiysek yonetici sayfası çalışcak, çalışansa çalışan sayfası 
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);
                var calisanlar = (from p in entity.Personeller where p.personelBirimId == birimId && p.personelYetkiTurId == 2 select p).ToList(); //birden fazla çalışana iş atıcamız için to list gönderdik
                ViewBag.personeller = calisanlar; //yakaldığımız calışanları yooladık viewbage //foreach dongulerınde kullandımız yer viewda

                //giriş yapılınca orn: muhasabe için yonetici gibi yonetci ındexde yazmayı unutma @ViewBag

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login"); //değilse logine tekrar dönder 
            }
        }
        ///YOL 1
       /* [HttpPost]
        public ActionResult Ata(string isBaslik, string isAciklama, string selectPer)
        {
            return View();
        }*/ // 1.YOLUMUZ Alternatif 2..yoluda deneylim

        //2.yol
        [HttpPost]
        public ActionResult Ata(FormCollection formCollection)
        {

            string isBaslik = formCollection["isBaslik"];
            string isAciklama = formCollection["isAciklama"];
            DateTime YapilacakTarih = Convert.ToDateTime(formCollection["YapilacakTarih"]);
            string AciliyetSeviyesi = formCollection["AciliyetSeviyesi"];
            int secilenPersonelId = Convert.ToInt32(formCollection["selectPer"]);



            Isler yeniIs = new Isler();  //İŞ EKLEME VERİLERİ
            yeniIs.isBaslik = isBaslik; //  
            yeniIs.isAciklama = isAciklama;
            yeniIs.isPersonelId = secilenPersonelId;
            yeniIs.YapilicakTarih= YapilacakTarih;
            yeniIs.AciliyetSeviyesi= AciliyetSeviyesi;
            yeniIs.iletilenTarih = DateTime.Now;
            yeniIs.isOkunma = false;
            yeniIs.isDurumId = 1;
            
 
                entity.Isler.Add(yeniIs);
                entity.SaveChanges(); // veri tabanı tetikler veri tabanına ekler
            return RedirectToAction("Takip", "Yonetici");
        }

        public ActionResult Takip() // get Ata() içindekileri aldık aslında itiyacımız calısanları aktarcaz
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]); //artık giriş yapan kullanıcının yetkituru elimizde

            //fonksiyonumuzu burda tanımladık
            var calisanBilgisi = CalisanBilgisi();
            ViewBag.CalisanBilgisi = calisanBilgisi;

            if (yetkiTurId == 1)  //indexe bastığımızda diret sayfanın açılmamsı lazım yoneticiysek yonetici sayfası çalışcak, çalışansa çalışan sayfası 
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);
                var calisanlar = (from p in entity.Personeller where p.personelBirimId == birimId && p.personelYetkiTurId == 2 && p.aktiflik==true select p).ToList(); //birden fazla çalışana iş atıcamız için to list gönderdik
                ViewBag.personeller = calisanlar; //yakaldığımız calışanları yooladık viewbage //foreach dongsunde kulldnımız personeller



                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login"); //değilse logine tekrar dönder 
            }
        }

        [HttpPost]
        public ActionResult Takip(int selectPer)
        {

            //fonksiyonumuzu burda tanımladık
            var calisanBilgisi = CalisanBilgisi();
            ViewBag.CalisanBilgisi = calisanBilgisi;

            var secilenPersonel = (from p in entity.Personeller where p.personelId == selectPer select p).FirstOrDefault();
            TempData["secilen"] = secilenPersonel;  //tkip methodundan listele metouna veri göndercez 2 aşamada veri gönderme VievBag Tek Aşama

            return RedirectToAction("Listele", "Yonetici"); // viewbagde önceden direkt methottan viewe veri göndermeyddi, temdata ise  takip methodundan  listelemethoduna  veri gödercex
            //***
            //Not: takip post edildikten sonra listeleactıon tetiklenecek  tempdatada secilen personel bilgiler yer alıcak*** ///
        }

        [HttpGet]
        public ActionResult Listele(string aciliyetSeviyesi)
        {
            var calisanBilgisi = CalisanBilgisi();
            ViewBag.CalisanBilgisi = calisanBilgisi;
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            //fonksiyonumuzu burda tanımladık
           

            if (yetkiTurId != 1)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                Personeller secilenPersonel = (Personeller)TempData["secilen"];

                try
                {
                    var isler = (from i in entity.Isler
                                 where i.isPersonelId == secilenPersonel.personelId
                                 orderby i.iletilenTarih descending
                                 select i).ToList();
                    ViewBag.isler = isler;
                    ViewBag.personel = secilenPersonel;
                    ViewBag.isSayisi = isler.Count();
                   
                   

                    return View();
                }
                catch (Exception)
                {
                    return RedirectToAction("Takip", "Yonetici");
                }
            }

        }
        public ActionResult AyinElemani()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);
            var calisanBilgisi = CalisanBilgisi();
            ViewBag.CalisanBilgisi = calisanBilgisi;

            if (yetkiTurId == 1)
            {
                int simdikiYil = DateTime.Now.Year;
                int simdikiAy = DateTime.Now.Month;

                List<int> yillar = new List<int>();
                for (int i = simdikiYil; i >= 2024; i--)
                {
                    yillar.Add(i);
                }
                ViewBag.yillar = yillar;
                ViewBag.ayinElemani = null;
                ViewBag.SelectedAy = new DateTime(simdikiYil, simdikiAy, 1).ToString("MMMM");
                ViewBag.SelectedYil = simdikiYil;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult AyinElemani(int aylar, int yillar)
        {
            int birimId = Convert.ToInt32(Session["PersonelBirimId"]);
            var personeller = entity.Personeller.Where(p => p.personelBirimId == birimId)
                                                 .Where(p => p.personelYetkiTurId != 1);

            DateTime baslangicTarih = new DateTime(yillar, aylar, 1);
            DateTime bitisTarih = baslangicTarih.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);//1.ay  23 :59 :59

            var isler = entity.Isler.Where(i => i.yapilanTarih >= baslangicTarih && i.yapilanTarih <= bitisTarih);

            var groupJoin = personeller.GroupJoin(isler, p => p.personelId, i => i.isPersonelId, (p, group) => new
            {
                sonucIsler = group,
                personelAd = p.personelAdSoyad
            });

            List<ToplamIs> list = new List<ToplamIs>();
            foreach (var personel in groupJoin)
            {
                ToplamIs toplamIs = new ToplamIs
                {
                    personelAdSoyad = personel.personelAd,
                    toplamIs = personel.sonucIsler.Count(i => i.yapilanTarih != null)
                };
                list.Add(toplamIs);
            }

            var siraliListe = list.OrderByDescending(i => i.toplamIs);
            ViewBag.ayinElemani = siraliListe;
            int simdikiYil = DateTime.Now.Year;

            List<int> sonucyillar = new List<int>();
            for (int i = simdikiYil; i >= 2024; i--)
            {
                sonucyillar.Add(i);
            }
            ViewBag.yillar = sonucyillar;
            ViewBag.SelectedAy = new DateTime(yillar, aylar, 1).ToString("MMMM");
            ViewBag.SelectedYil = yillar;
            return View();
        }
        [AuthFilter(1)]
        public ActionResult Loglar()
        {
            int personelId = Convert.ToInt32(Session["PersonelId"]);
            int personelYetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            var calisanBilgisi = CalisanBilgisi();
            ViewBag.CalisanBilgisi = calisanBilgisi;

            // Yöneticinin sadece kendi loglarını filtreleme
            var loglar = (from l in entity.Loglar
                          join p in entity.Personeller on l.personelId equals p.personelId
                          where p.personelYetkiTurId == personelYetkiTurId && l.personelId == personelId
                          orderby l.tarih descending
                          select l).ToList();

            ViewBag.Loglar = loglar;

            return View(loglar);
        }
        [AuthFilter(1)]
        public ActionResult HataIndex()
        {
          
            // Sadece hata loglarını al
            var hataLoglar = entity.Loglar
                .Where(l => l.isHata.HasValue && l.isHata.Value) // sadece isHata = true olanları getir
                .OrderByDescending(l => l.tarih)
                 .ToList();
            return View(hataLoglar);
        }

        //kod fazlaığından fonksiyon oluşturarak kurtulduk artık  var calisanBilgisi = CalisanBilgisi(); --  ViewBag.CalisanBilgisi = calisanBilgisi; dedğimizde tanımlanmış olucak.
        public string CalisanBilgisi()
        {
            var personelId = Convert.ToInt32(Session["PersonelId"]);
            var personelAdSoyad = (from p in entity.Personeller where p.personelId == personelId select p.personelAdSoyad).FirstOrDefault();

            var birimId = Convert.ToInt32(Session["PersonelBirimId"]);
            var birimAd = (from b in entity.Birimler where b.birimId == birimId select b.birimAd).FirstOrDefault();

            var calisanBilgisi = personelAdSoyad + "-" + birimAd;
            ViewBag.birimAd = birimAd;
            return calisanBilgisi;
        }


    }
}