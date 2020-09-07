using facebook.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace facebook.Repository
{
    public class FacebookRepository
    {

        public readonly string Connstr = ConfigurationManager.ConnectionStrings["MyDbConn1"].ToString();
        public List<Kullanici> Kullanicilar()
        {
            var sonuc = new List<Kullanici>();
            var kullanici = new Kullanici();
            var conn = new SqlConnection();
            conn.ConnectionString = Connstr;
            conn.Open();

            SqlCommand command = new SqlCommand("Select * from dbo.kullanici ", conn);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    kullanici.Adi = reader["adi"].ToString();
                    kullanici.Soyadi = reader["soyadi"].ToString();
                    var aktif = reader["cevrimici"].ToString();
                    if (aktif == "0")
                    {
                        kullanici.AktifMi = false;
                    }
                    else
                    {
                        kullanici.AktifMi = true;
                    }
                    sonuc.Add(kullanici);
                }
            }
            conn.Close();
            return sonuc;
        }

        public bool KullaniciSifreKontrol(string mail, string pass)
        {
            var conn = new SqlConnection();
            conn.ConnectionString = Connstr;
            conn.Open();
            SqlCommand command = new SqlCommand(string.Format("Select * from dbo.kullanici WHERE mail LIKE  '%{0}%' ", mail), conn);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    var dbmail = reader["mail"].ToString();
                    var dbPass = reader["password"].ToString();

                    if (dbmail == mail && dbPass == pass)
                    {
                        conn.Close();
                        return true;
                    }
                }
            }
            conn.Close();

            return false;
        }

        public Kullanici KullaniciBilgileriniGetir(string email)
        {
            var sonuc = new Kullanici();
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Connstr;
            conn.Open();
            SqlCommand command = new SqlCommand(string.Format("Select TOP 1 * from dbo.kullanici where mail like '%{0}%' ", email), conn);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    sonuc.KullaniciId = !String.IsNullOrEmpty(reader["id"].ToString()) ? Convert.ToInt32(reader["id"].ToString()) : 0;
                    sonuc.Adi = reader["adi"].ToString();
                    sonuc.Soyadi = reader["soyadi"].ToString();
                    sonuc.Mail = reader["mail"].ToString();
                }
            }
            conn.Close();
            return sonuc;
        }

        public bool GonderiKayit(GonderiKayitRequest request)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Connstr;
            conn.Open();
            SqlCommand command = new SqlCommand(string.Format(@"INSERT INTO dbo.gonderiler
                (gonderi, paylasan_kullanici_id,gonderi_tarihi,gonderi_resim)
            VALUES
                ('{0}',{1},'{2}','{3}');", request.Description, request.KullaniciId, DateTime.Now, request.ResimAd), conn);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                return false;
            }
            conn.Close();
            return true;
        }

        public GonderiViewModel GonderileriGetir()
        {
            var sonuc = new GonderiViewModel();
            sonuc.Gonderiler = new List<GonderiModel>();
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Connstr;
            conn.Open();
            var sql = @"SELECT gonderiler.gonderi_tarihi as GonderiTarih,
            gonderiler.gonderi as Gonderi,
            gonderiler.gonderi_resim as GonderiResim,
            kullanici.adi as Adi,
            kullanici.soyadi as Soyadi
            FROM dbo.gonderiler
            LEFT JOIN dbo.kullanici ON gonderiler.paylasan_kullanici_id = kullanici.id 
            ORDER BY gonderiler.id DESC; ";

            DataSet ds = new DataSet();
            using (SqlConnection connection =
                new SqlConnection(Connstr))
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(
                    sql, connection);
                adapter.Fill(ds);

                if (HasData(ds))
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        sonuc.Gonderiler.Add(new GonderiModel
                        {
                            Gonderi = ds.Tables[0].Rows[i]["Gonderi"].ToString(),
                            GonderiZamani = Convert.ToDateTime(ds.Tables[0].Rows[i]["GonderiTarih"].ToString()),
                            GonderiResim = ds.Tables[0].Rows[i]["GonderiResim"].ToString(),
                            Ad = ds.Tables[0].Rows[i]["Adi"].ToString(),
                            Soyad = ds.Tables[0].Rows[i]["Soyadi"].ToString()
                        });
                    }

                    conn.Close();
                }
            }
            return sonuc;
        }

        public string KullaniciResimGetir(int kullaniciId)
        {
            var sonuc = string.Empty;
            var conn = new SqlConnection();
            conn.ConnectionString = Connstr;
            conn.Open();

            SqlCommand command = new SqlCommand(String.Format("select kullanici_resim as Resim from kullanici where id = {0} ", kullaniciId), conn);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    sonuc = reader["Resim"].ToString();
                }
            }
            conn.Close();
            return sonuc;
        }

        public string YeniKullaniciKaydet(YeniKullaniciKayitRequest model)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Connstr;
            conn.Open();
            SqlCommand command = new SqlCommand(string.Format(@"INSERT INTO dbo.kullanici
                (adi, soyadi,kayit_tarihi,mail,password,kullanici_resim)
            VALUES
                ('{0}','{1}','{2}','{3}',{4},'{5}');", model.Ad, model.Soyad, DateTime.Now, model.Email, model.Password, model.KullaniciResim), conn);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                return "";
            }
            conn.Close();
            return model.Email;
        }

        public bool SohbetKaydet(string gonderenId, string alanId, string mesaj)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Connstr;
            conn.Open();
            SqlCommand command = new SqlCommand(string.Format(@"INSERT INTO dbo.sohbetler
                (mesaj, gonderenId,alan_id,gonderilme_tarihi)
            VALUES
                ('{0}',{1},'{2}','{3}');", mesaj, gonderenId, alanId, DateTime.Now), conn);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                return false;
            }
            conn.Close();
            return true;
        }

        public List<Kullanici> KullaniciListesiGetir(string userId)
        {
            var sonuc = new List<Kullanici>();
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Connstr;
            conn.Open();
            var sql = @"SELECT
            kullanici.id as Id,
            kullanici.adi as Adi,
            kullanici.soyadi as Soyadi,
            kullanici.cevrimici as CevrimIci
            FROM dbo.kullanici
            ORDER BY kullanici.id DESC; ";

            DataSet ds = new DataSet();
            using (SqlConnection connection =
                new SqlConnection(Connstr))
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(
                    sql, connection);
                adapter.Fill(ds);

                if (HasData(ds))
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        if (userId == ds.Tables[0].Rows[i]["Id"].ToString()) // giriş yapan kullanıcıyı sayma
                        {
                            continue;
                        }
                        sonuc.Add(new Kullanici
                        {
                            Adi = ds.Tables[0].Rows[i]["Adi"].ToString(),
                            Soyadi = ds.Tables[0].Rows[i]["Soyadi"].ToString(),
                            AktifMi = ds.Tables[0].Rows[i]["CevrimIci"].ToString() == "1",
                            KullaniciId = Convert.ToInt32(ds.Tables[0].Rows[i]["Id"].ToString())
                        });
                    }

                    conn.Close();
                }
            }
            return sonuc;
        }

        public void KullaniciAktifPasifDurumu(int kullaniciId,bool aktif)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Connstr;
            conn.Open();
            SqlCommand command = new SqlCommand(string.Format(@"UPDATE kullanici set kullanici.cevrimici = {1} WHERE kullanici.id = {0}",kullaniciId, aktif ? 1 : 0), conn);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
               
            }
            conn.Close();
        }

        public KullaniciSohbetListesiModel SohbetKonusmalariniGetir(int alanId,string alanAdi, int gonderenId, string gonderenAdi)
        {
            var sonuc = new KullaniciSohbetListesiModel();
            sonuc.KullaniciSohbetleri = new List<SohbetViewModel>();
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Connstr;
            conn.Open();

            var sql = string.Format(@"SELECT
	                        s.mesaj AS Mesaj,
	                        k.adi AS GonderenAdi,
	                        k.id AS GonderenId,
                            s.alan_id as MesajiAlanId,
                            k2.adi as MesajiAlanAdi,
	                        s.gonderilme_tarihi AS GonderilmeTarihi,
	                        s.okundumu AS Okundumu
                        FROM
	                        sohbetler s
                        LEFT JOIN kullanici k ON k.id = s.gonderenId
                        LEFT JOIN kullanici k2 ON k2.id = s.alan_id
                        WHERE (s.alan_id ={0} and s.gonderenId = {1}) OR (s.alan_id ={1} and s.gonderenId = {0}) ", alanId, gonderenId);

            DataSet ds = new DataSet();
            using (SqlConnection connection = new SqlConnection(Connstr))
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(
                    sql, connection);
                adapter.Fill(ds);

                if (HasData(ds))
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        sonuc.KullaniciSohbetleri.Add(new SohbetViewModel
                        {
                            MesajiAlanKullaniciId = Convert.ToInt32(ds.Tables[0].Rows[i]["MesajiAlanId"].ToString()),
                            MesajiAlanKullaniciAdi = ds.Tables[0].Rows[i]["MesajiAlanAdi"].ToString(),
                            GonderenKullaniciId = Convert.ToInt32(ds.Tables[0].Rows[i]["GonderenId"].ToString()),
                            GonderenKullaniciAdi = ds.Tables[0].Rows[i]["GonderenAdi"].ToString(),
                            GonderilmeTarihi = Convert.ToDateTime(ds.Tables[0].Rows[i]["GonderilmeTarihi"].ToString()),
                            OkunduMu = ds.Tables[0].Rows[i]["Okundumu"].ToString() == "1",
                            Mesaj = ds.Tables[0].Rows[i]["Mesaj"].ToString()
                        });
                    }

                    sonuc.MesajiAlanKullaniciId = alanId;
                    sonuc.MesajiAlanKullaniciAdi = alanAdi;
                    sonuc.GonderenKullaniciId = gonderenId;
                    sonuc.GonderenKullaniciAdi = gonderenAdi;
                    conn.Close();
                }
            }
            return sonuc;
        }

        public bool HasData(DataSet ds)  // çalıştırdığımız sorgudan veri döndü mü, data var mı?
        {
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        return true; // data var
                    }
                }
            }
            return false; // data yok
        }
    }
}