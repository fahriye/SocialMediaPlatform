using facebook.Models;
using facebook.Repository;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace facebook.Controllers
{
    public class HomeController : Controller
    {
        FacebookRepository rep = new FacebookRepository();

        public ActionResult Index()
        {
            //var kullaniciList = rep.Kullanicilar();
            if (Session["KullaniciId"] != null)
            {
                rep.KullaniciAktifPasifDurumu(Convert.ToInt32(Session["KullaniciId"]), false); // kullanıcı durumu pasif olsun.
            }
            Session.Clear();
            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var sonuc = rep.KullaniciSifreKontrol(model.Email,model.Password);
            if (sonuc)
            {
                var kullanici = rep.KullaniciBilgileriniGetir(model.Email);
                Session["KullaniciId"] = kullanici.KullaniciId;
                Session["KullaniciAdi"] =kullanici.Adi;
                Session["KullaniciSoyadi"] = kullanici.Soyadi;
                Session["Mail"] = kullanici.Mail;

                rep.KullaniciAktifPasifDurumu(kullanici.KullaniciId,true); // kullanıcı durumu aktif olsun.
                
                return RedirectToAction("Anasayfa", "Home");
            }
            else
            {
                ModelState.AddModelError("Uyarı", "Kullanıcı adı veya şifre hatalı! Lütfen kontrol ediniz!");
                return View(model);
            }
        }


        public ActionResult Anasayfa(GonderiViewModel model)
        {
            if (Session["KullaniciId"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            model =  rep.GonderileriGetir();
            model.KullaniciResim = rep.KullaniciResimGetir(Convert.ToInt32(Session["KullaniciId"]));
            model.Kullanicilar = rep.KullaniciListesiGetir(Session["KullaniciId"].ToString());
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GonderiKaydet(GonderiKayitRequest request, HttpPostedFileBase Resim)
        {
            if (Session["KullaniciId"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var resimkayitSonuc = ResimKaydet(Resim, HttpContext, true);

            request.ResimAd = resimkayitSonuc;
            request.KullaniciId = Convert.ToInt32(Session["KullaniciId"]);
            var kayit = rep.GonderiKayit(request);
            return RedirectToAction("Anasayfa", "Home");
        }

        public static string ResimKaydet(HttpPostedFileBase Resim, HttpContextBase ctx, bool gonderi)
        {
            string benzersizAd = Path.GetFileNameWithoutExtension(Resim.FileName) + "-" + Guid.NewGuid() +
                                 Path.GetExtension(Resim.FileName);

            int resYukseklik = gonderi ? Convert.ToInt32(ConfigurationManager.AppSettings["resH"]) : Convert.ToInt32(ConfigurationManager.AppSettings["kullaniciY"]);
            int resGenislik = gonderi ? Convert.ToInt32(ConfigurationManager.AppSettings["resW"]) : Convert.ToInt32(ConfigurationManager.AppSettings["kullaniciG"]);
            Image orj = Image.FromStream(Resim.InputStream);
            Bitmap res = new Bitmap(orj, resGenislik, resYukseklik);
            res.Save(ctx.Server.MapPath("~/Content/Resimler/" + benzersizAd));
            var kaydedilenAd = "/Content/Resimler/" + benzersizAd;
            return kaydedilenAd;
        }

        public ActionResult SohbetPenceresi(KullaniciSohbetListesiModel model)
        {
            if (Session["KullaniciId"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var kullanici = rep.KullaniciBilgileriniGetir(Session["Mail"].ToString());
                model.GonderenKullaniciAdi = kullanici.Adi;
                model.GonderenKullaniciId = kullanici.KullaniciId;

                var sohbet = rep.SohbetKonusmalariniGetir(model.MesajiAlanKullaniciId, model.MesajiAlanKullaniciAdi, model.GonderenKullaniciId, model.GonderenKullaniciAdi);
                if (!String.IsNullOrEmpty(sohbet.GonderenKullaniciAdi) ||
                    !String.IsNullOrEmpty(sohbet.MesajiAlanKullaniciAdi))
                {
                    model.KullaniciSohbetleri = sohbet.KullaniciSohbetleri;
                }
            }       

            return View(model);
        }
        public ActionResult YeniUyeEkrani()
        {
            return View();
        }

        [HttpPost]
        public ActionResult YeniUyeEkrani(YeniKullaniciKayitRequest request, HttpPostedFileBase Resim)
        {
            //if (Session["KullaniciId"] == null)
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            var resimkayitSonuc = ResimKaydet(Resim, HttpContext, false);

            request.KullaniciResim = resimkayitSonuc;
            var sonuc = rep.YeniKullaniciKaydet(request);


            if (!String.IsNullOrEmpty(sonuc))
            {
                return RedirectToAction("Index", "Home");
            }
            // return View();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult KullaniciListesi(GonderiViewModel model)
        {
            return View(model);
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}