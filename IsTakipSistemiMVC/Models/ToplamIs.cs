using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;


namespace IsTakipSistemiMVC.Models
{
    public class ToplamIs
    {
        public string personelAdSoyad { get; set; }
        public int toplamIs { get; set; }    // Toplam iş sayısı
        public int okunacakIs { get; set; }  // Okunacak işlerin sayısı
        public int yapiliyorIs { get; set; }  // Yapılıyor durumda olan işlerin sayısı
        public int yapilacakIs { get; set; } // Yapılmayı bekleyen işlerin sayısı

    }
}