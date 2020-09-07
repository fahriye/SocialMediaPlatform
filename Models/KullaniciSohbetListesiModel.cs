using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace facebook.Models
{
    public class KullaniciSohbetListesiModel
    {
        public List<SohbetViewModel> KullaniciSohbetleri { get; set; }
        public int GonderenKullaniciId { get; set; }
        public int MesajiAlanKullaniciId { get; set; }
        public string GonderenKullaniciAdi { get; set; }
        public string MesajiAlanKullaniciAdi { get; set; }
    }
}