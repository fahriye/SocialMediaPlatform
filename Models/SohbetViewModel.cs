using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace facebook.Models
{
    public class SohbetViewModel
    {
        public int GonderenKullaniciId { get; set; }
        public string GonderenKullaniciAdi { get; set; }
        public int MesajiAlanKullaniciId { get; set; }
        public string MesajiAlanKullaniciAdi { get; set; }
        public DateTime GonderilmeTarihi { get; set; }
        public bool OkunduMu { get; set; }
        public string Mesaj { get; set; }
    }
}