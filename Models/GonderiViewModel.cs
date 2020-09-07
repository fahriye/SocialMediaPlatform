using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace facebook.Models
{
    public class GonderiViewModel
    {
        public List<GonderiModel> Gonderiler { get; set; }
        public string KullaniciResim { get; set; }
        public List<Kullanici> Kullanicilar { get; set; }

    }
}