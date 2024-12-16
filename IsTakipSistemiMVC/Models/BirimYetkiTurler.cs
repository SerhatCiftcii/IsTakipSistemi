using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IsTakipSistemiMVC.Models
{
    public class BirimYetkiTurler
    {
        public List<Birimler>birimlerList {  get; set; }
        public List<YetkiTurler> yetkiTurlerList { get; set; }
    }
}