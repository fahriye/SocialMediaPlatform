using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace facebook.Models
{
    public class Kullanici
    {
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public int KullaniciId { get; set; }
        public bool AktifMi { get; set; }
        public string Mail { get; set; }
    }
}