using IsTakipSistemiMVC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IsTakipSistemiMVC.Controllers
{
    public class IsDurum //kendi modelimizi oluşturuyoruz iş takip için tabi burası class
    {
        internal string yapiliyorİsYorum;

        public string İsBaslik { get; set; }
        public string isAciklama { get; set; }
        public DateTime? iletilenTarih { get; set; }
        public DateTime? yapilanTarih { get; set; }
        public string durumAd { get; set; }
        public string isYorum { get; set; }
        public string isOkunmaTarihi { get; set; }

        public DateTime? YapilicakTarih {  get; set; }    
        public string AciliyetSeviyesi { get; set; }
        public byte[] Image { get; set; } // Resim verisi ekle

        public  string yapiliyorisYorum { get; set; }

    }

    public class CalisanController : Controller
    {
        IsTakipDBEntities entity = new IsTakipDBEntities();//bu unutulmamlıdır dikkat edin ilk yapılıcak controlden sonra

        // GET: Calisan
        public ActionResult Index()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            //fonksiyonumuzu burda tanımladık
            var calisanBilgisi = CalisanBilgisi();
            ViewBag.CalisanBilgisi = calisanBilgisi;


            if (yetkiTurId == 2)
            {
                // Sadece kendi işlerimizi görmek için
                int personelId = Convert.ToInt32(Session["PersonelId"]);
                

                // Yapılacak, Okunan, Tamamlanan ve Okunmayı Bekleyen iş sayısını al
                var yapilacakIsSayisi = entity.Isler
                    .Count(i => i.isPersonelId == personelId && i.isDurumId == 2 );
               
                var okunanIsSayisi = entity.Isler
                    .Count(i => i.isPersonelId == personelId && i.isOkunma == true);

                var tamamlananIsSayisi = entity.Isler
                    .Count(i => i.isPersonelId == personelId && i.isDurumId == 3);

                var okunmayanIsSayisi = entity.Isler
                    .Count(i => i.isPersonelId == personelId && i.isOkunma == false);

                var bugun = DateTime.Now;
                var bitimTarihGecmis = entity.Isler
                    .Count(i => i.isPersonelId == personelId && i.YapilicakTarih < bugun && (i.isDurumId == 2 || i.isDurumId == 1));


                ViewBag.YapilacakIsSayisi = yapilacakIsSayisi;
                ViewBag.OkunanIsSayisi = okunanIsSayisi;
                ViewBag.TamamlananIsSayisi = tamamlananIsSayisi;
                ViewBag.OkunmayanIsSayisi = okunmayanIsSayisi;
                ViewBag.bitimTarihGecmis=bitimTarihGecmis;

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult Yap(string aciliyetSeviyesi)
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            var calisanBilgisi = CalisanBilgisi();
            ViewBag.CalisanBilgisi = calisanBilgisi;

            if (yetkiTurId == 2)
            {
                int personelId = Convert.ToInt32(Session["PersonelId"]);
                var isler = (from i in entity.Isler where i.isPersonelId == personelId && i.isDurumId == 2 select i).ToList();

                // Yeni eklediğiniz sorgu
                var yoneticiler = (from p in entity.Personeller
                                   where p.personelYetkiTurId == 1
                                   select p.personelAdSoyad).ToList();
                ViewBag.yonetciler = yoneticiler;

                var tamamlanmayibekleyenissayısı = entity.Isler
                    .Count(i => i.isPersonelId == personelId && i.isDurumId == 2);
                ViewBag.TamamlanmayiBekleyenİsSayısı = tamamlanmayibekleyenissayısı;

                // Aciliyet seviyesine göre filtreleme
                if (!string.IsNullOrEmpty(aciliyetSeviyesi))
                {
                    // Eğer 'aciliyetSeviyesi' boş değilse, filtreleme yapılır.
                    isler = isler.Where(i => i.AciliyetSeviyesi == aciliyetSeviyesi).ToList();
                }

                // Sıralama işlemi
                isler = isler.OrderByDescending(i => i.iletilenTarih).ToList();
                ViewBag.isler = isler;

                if (!isler.Any())
                {
                    ViewBag.Mesaj = "Tamamlanıcak  Durumunda iş kalmadı Atanacak işi bekleyin .....";
                }

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [HttpPost]
        public ActionResult Yap(int isId, string actionType, string isYorum, HttpPostedFileBase Image,string yapiliyorisYorum)
        {
            var tekIs = (from i in entity.Isler where i.isId == isId select i).FirstOrDefault();
            // Eğer yorum boşsa, varsayılan bir yorum belirle
            if (isYorum == "") isYorum = "çalışanımız yorum yapmadı";

            if (actionType == "Tamamla")
            {
                tekIs.yapilanTarih = DateTime.Now;
                tekIs.isDurumId = 3;
                tekIs.isYorum = isYorum;
                tekIs.YapilicakTarih = DateTime.Now;
                tekIs.AciliyetSeviyesi = tekIs.AciliyetSeviyesi;
                

                // Eğer bir resim yüklenmişse
                if (Image != null && Image.ContentLength > 0)
                {
                    byte[] imageData;
                    using (var binaryReader = new BinaryReader(Image.InputStream))
                    {
                        imageData = binaryReader.ReadBytes(Image.ContentLength); // Resmi byte dizisine çeviriyoruz
                    }
                    tekIs.Image = imageData; // Fotoğrafı güncelle
                }

                entity.SaveChanges();
            }
            else if (actionType == "Yapiliyor")
            {
                tekIs.isDurumId = 2;
                tekIs.yapiliyorisYorum = yapiliyorisYorum;
                entity.SaveChanges();

            }

            return RedirectToAction("Yap", "Calisan");
        }

        public ActionResult Yapiliyor(string aciliyetSeviyesi)
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            var calisanBilgisi = CalisanBilgisi();
            ViewBag.CalisanBilgisi = calisanBilgisi;

            if (yetkiTurId == 2)
            {
                int personelId = Convert.ToInt32(Session["PersonelId"]);
                var isler = (from i in entity.Isler where i.isPersonelId == personelId && i.isDurumId == 1 && i.isOkunma == true select i).ToList();

                var yapilmayibekleyenissayisi = entity.Isler
                    .Count(i => i.isPersonelId == personelId && i.isDurumId == 1 && i.isOkunma==true );
                ViewBag.YapilmayıBekleyenİsSayisi = yapilmayibekleyenissayisi;

                // Aciliyet seviyesine göre filtreleme
                if (!string.IsNullOrEmpty(aciliyetSeviyesi))
                {
                    // Eğer 'aciliyetSeviyesi' boş değilse, yani kullanıcı bir aciliyet seviyesi seçmişse
                    // Bu durumda filtreleme yapılır.
                    isler = isler.Where(i => i.AciliyetSeviyesi == aciliyetSeviyesi).ToList();
                }
                ViewBag.isler = isler;

                // Sıralama işlemi
                isler = isler.OrderByDescending(i => i.iletilenTarih).ToList();
              

                //henüz iş verilmediyse (yapılıyor sayfasında iş kalmadıysa yanı)
                if (!isler.Any())
                {
                    ViewBag.Mesaj = "Yapılıyor Durumunda iş kalmadı Atanacak işi bekleyin .....";
                }
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [HttpPost]
        public ActionResult Yapiliyor(int isId, string actionType, string yapiliyorisYorum, string isYorum)
        {

            var tekIs = (from i in entity.Isler where i.isId == isId select i).FirstOrDefault();
            if (yapiliyorisYorum == "") yapiliyorisYorum = "çalışanımız yorum yapmadı";
            if (actionType == "Tamamla")
            {
                tekIs.isDurumId = 3;
                entity.SaveChanges();
            }

            else if (actionType == "Yapiliyor")
                if (yapiliyorisYorum == "") yapiliyorisYorum = "çalışanımız yorum yapmadı";
            {
                //yorum boş döncekse yazıdrıcak burdan
                tekIs.YapimaBaslama = DateTime.Now;
                
                tekIs.isDurumId = 2;
                tekIs.YapilicakTarih=DateTime.Now;
                tekIs.AciliyetSeviyesi = tekIs.AciliyetSeviyesi;
               tekIs.yapiliyorisYorum = yapiliyorisYorum;
                entity.SaveChanges();

            }
            return RedirectToAction("Yapiliyor", "Calisan");
        }
        //farklı olarkda @modelsi kullanıcaz (viewda) list kullancaz çünkiyapıcaz 
        //kedni işinin listesini görmeli çalışan yapıldı ve atandı gibi ekleyebilirsem yapılıyır                                     
        public ActionResult Takip(string aciliyetSeviyesi)
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            var calisanBilgisi = CalisanBilgisi();
            ViewBag.CalisanBilgisi = calisanBilgisi;

            if (yetkiTurId == 2) // Kullanıcı yetkisi "çalışan" ise
            {
                int personelId = Convert.ToInt32(Session["PersonelId"]); // Çalışan ID'sini al

                // İşlerin ve durumların listesini al
                var isler = from i in entity.Isler
                            join d in entity.Durumlar on i.isDurumId equals d.durumId
                            where i.isPersonelId == personelId
                            select new
                            {
                                i.isBaslik,
                                i.isAciklama,
                                i.iletilenTarih,
                                i.yapilanTarih,
                                i.YapilicakTarih,
                                i.AciliyetSeviyesi,
                                d.durumAd,
                                i.isYorum,
                                i.Image,
                                i.yapiliyorisYorum,
                            };

                // Aciliyet seviyesine göre filtreleme
                if (!string.IsNullOrEmpty(aciliyetSeviyesi))
                {
                    // Eğer 'aciliyetSeviyesi' boş değilse, yani kullanıcı bir aciliyet seviyesi seçmişse
                    // Bu durumda filtreleme yapılır.
                    isler = isler.Where(i => i.AciliyetSeviyesi == aciliyetSeviyesi);
                }

                var islerList = isler.ToList().OrderByDescending(i => i.iletilenTarih); // İşleri tarihine göre sıralama

                var tamamlananIsSayisi = entity.Isler
                   .Count(i => i.isPersonelId == personelId && i.isDurumId == 3);
                ViewBag.TamamlananIsSayisi = tamamlananIsSayisi;

                IsDurumModel model = new IsDurumModel(); // Gönderilecek model nesnesi oluştur
                List<IsDurum> list = new List<IsDurum>(); // İş durumlarını tutacak liste oluştur

                // Her iş için IsDurum nesnesi oluştur
                foreach (var i in islerList)
                {
                    IsDurum isDurum = new IsDurum();
                    isDurum.İsBaslik = i.isBaslik;
                    isDurum.isAciklama = i.isAciklama;
                    isDurum.iletilenTarih = i.iletilenTarih;
                    isDurum.yapilanTarih = i.yapilanTarih;
                    isDurum.durumAd = i.durumAd;
                    isDurum.isYorum = i.isYorum;
                    isDurum.YapilicakTarih = i.YapilicakTarih;
                    isDurum.AciliyetSeviyesi = i.AciliyetSeviyesi;
                    isDurum.Image = i.Image; // Resim verisini modelin özelliğine ata
                    isDurum.yapiliyorisYorum = i.yapiliyorisYorum; //neden algılamıyor burayu anlamadım


                    list.Add(isDurum); // Listeye ekle
                }

                model.isdurumlar = list; // Modelin isdurumlar özelliğine listeyi ata
                return View(model); // Model ile view'ı döndür
            }
            else
            {
                return RedirectToAction("Index", "Login"); // Yetki uygun değilse login sayfasına yönlendir
            }
        }


        public ActionResult OkunacakIsler(string aciliyetSeviyesi)
        {
            var calisanBilgisi = CalisanBilgisi();
            ViewBag.CalisanBilgisi = calisanBilgisi;

            


            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]); //en başta logginde tanımaldığımız sesson hala kullanılıyor mesela. 
            if (yetkiTurId == 2)
            {
                //sadece knedi işlerimizi görmek için yaptım.   
                int personelId = Convert.ToInt32(Session["PersonelId"]);
                var isler = (from i in entity.Isler where i.isPersonelId == personelId && i.isOkunma == false orderby i.iletilenTarih descending select i).ToList();
                ViewBag.isler = isler;
                ///////////////////////
                ///
                var okunmayanIsSayisi = entity.Isler
                   .Count(i => i.isPersonelId == personelId && i.isOkunma == false);
                ViewBag.OkunmayanIsSayisi = okunmayanIsSayisi;

                if (!isler.Any())
                {
                    ViewBag.Mesaj = "Atanıcak  Durumunda iş kalmadı Atanacak işi bekleyin .....";
                }
                //
                //aciliyte göre filtreleme
                if (!string.IsNullOrEmpty(aciliyetSeviyesi))
                {
                    // Eğer 'aciliyetSeviyesi' boş değilse, yani kullanıcı bir aciliyet seviyesi seçmişse
                    // Bu durumda filtreleme yapılır.
                    isler = isler.Where(i => i.AciliyetSeviyesi == aciliyetSeviyesi).ToList();
                }
                ViewBag.isler=isler;

                return View();
            }
            else
            {
                return RedirectToAction("Index","Login");
            }
        }

        [HttpPost]
        public ActionResult OkunacakIsler(int isId)
        {
            var tekIs = (from i in entity.Isler where i.isId == isId select i).FirstOrDefault();
            //index sayfamızda falsları çağırmıştık true olanlar gelmıcek ve okunmuş olucak 
            tekIs.isOkunma = true;
            tekIs.isOkunmaTarihi = DateTime.Now;
            tekIs.YapilicakTarih = DateTime.Now;
            tekIs.AciliyetSeviyesi = tekIs.AciliyetSeviyesi;
            entity.SaveChanges();

            return RedirectToAction("OkunacakIsler", "Calisan");
        }

        //kod fazlaığından fonksiyon oluşturarak kurtulduk artık  var calisanBilgisi = CalisanBilgisi(); --  ViewBag.CalisanBilgisi = calisanBilgisi; dedğimizde tanımlanmış olucak.
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

