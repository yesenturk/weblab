using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using liftmarket.Models;
using liftmarket.Models.Repository;

namespace liftmarket.Controllers
{
    public class BayiController : Controller
    {
        private DatabaseContext db = new DatabaseContext();
        BaseRepository<Siparisler> siparisRepo = new BaseRepository<Siparisler>();
        BaseRepository<ServisBakim> bakimRepo = new BaseRepository<ServisBakim>();
        BaseRepository<Urun> urunRepo = new BaseRepository<Urun>();

        // GET: Bayi
        public ActionResult Index()
        {
            return View(db.Urun.ToList());
        }
        public ActionResult Basket()
        {
            ViewBag.Title = "Sepet";

            List<SepetUrun> SepettekiUrunler = new List<SepetUrun>();

            if (Session["SepettekiUrunler"] != null)
            {
                SepettekiUrunler = (List<SepetUrun>)Session["SepettekiUrunler"];
            }

            if (SepettekiUrunler.Count > 0)
            {
                return View("Basket", SepettekiUrunler);
            }
            else
            {
                return View("EmptyBasket");
            }
        }

        public ActionResult AddToBasket(int? id)
        {
            List<SepetUrun> SepettekiUrunler = new List<SepetUrun>();

            if (Session["SepettekiUrunler"] != null)
            {
                SepettekiUrunler = (List<SepetUrun>)Session["SepettekiUrunler"];
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SepetUrun tempUrun = SepettekiUrunler.Find(x => x.UrunId == id);

            if (tempUrun != null)
            {
                tempUrun.Adet++;
                tempUrun.UrunToplamFiyati = tempUrun.UrunFiyati * tempUrun.Adet;

            }
            else
            {
                Urun urun = db.Urun.Find(id);
                SepetUrun sepetUrun = new SepetUrun();
                sepetUrun.UrunId = urun.Id;
                sepetUrun.UrunAdi = urun.UrunAdi;
                sepetUrun.UrunFiyati = urun.BayiFiyati;
                sepetUrun.BakimFiyati = urun.BakimFiyati;
                sepetUrun.BakimSure = urun.BakimSure;
                sepetUrun.Adet = 1;
                sepetUrun.UrunToplamFiyati = urun.BayiFiyati * sepetUrun.Adet;

                SepettekiUrunler.Add(sepetUrun);

            }
            Session["SepettekiUrunler"] = SepettekiUrunler;

            if (SepettekiUrunler.Count > 0)
            {
                return RedirectToAction("Basket", "Bayi");
            }
            else
            {
                return RedirectToAction("Basket", "Bayi");
            }
        }
        public ActionResult SiparisVer()
        {
            List<SepetUrun> SepettekiUrunler = new List<SepetUrun>();
            int BayiId = 0;
            if (Session["SepettekiUrunler"] != null)
            {
                SepettekiUrunler = (List<SepetUrun>)Session["SepettekiUrunler"];
            }

            if (Session["BayiId"] != null)
                BayiId = (int)Session["BayiId"];
            Random random = new Random();
            string spNo = "20210000" + random.Next(1000, 10000);
            DateTime today = DateTime.Now;
            foreach (var item in SepettekiUrunler)
            {

                Siparisler siparisler = new Siparisler();
                siparisler.BayiId = BayiId;
                siparisler.UrunId = item.UrunId;
                siparisler.SiparisTarihi = today;
                siparisler.ToplamTutar = item.UrunToplamFiyati;
                siparisler.OdemeYontemleriId = 2;
                siparisler.OnaylandiMi = false;
                siparisler.SiparisNo = spNo;
                siparisler.Adet = item.Adet;

                siparisRepo.Add(siparisler);

                ServisBakim servisBakim = new ServisBakim();
                servisBakim.BayiId = BayiId;
                servisBakim.BakimMasrafi = item.BakimFiyati;
                servisBakim.sonrakiBakimTarihi = today.AddDays(item.BakimSure);
                servisBakim.BakimTarihi = today;
                servisBakim.SiparisNo = spNo;
                servisBakim.UrunId = item.UrunId;

                bakimRepo.Add(servisBakim);
            }
            Session.Remove("SepettekiUrunler");
            SiparisGeldiEmailGonder();
            return RedirectToAction("Index");
        }
        public ActionResult Siparisler()
        {
            int BayiId = 0;
            if (Session["BayiId"] != null)
                BayiId = (int)Session["BayiId"];

            var mymodel = (from s in siparisRepo.Query<Siparisler>()
                           join
                           u in siparisRepo.Query<Urun>() on s.UrunId equals u.Id
                           //join
                           //b in siparisRepo.Query<ServisBakim>() on u.Id equals b.UrunId
                           where (s.BayiId == BayiId) && s.IsDeleted == false
                           orderby s.CreateDate descending
                           select new SiparisDetay()
                           {
                               SiparisNo = s.SiparisNo,
                               UrunAdi = u.UrunAdi,
                               SiparisTarihi = s.SiparisTarihi,
                               Adet = s.Adet,
                               //SonBakimTarihi = b.BakimTarihi,
                               //SonrakiBakimTarihi = b.BakimTarihi,
                               BakimMasrafi = u.BakimFiyati,
                               BakimSuresi = u.BakimSure,
                               OnayDurumu = s.OnaylandiMi == true ? "Siparişiniz onaylandı" : "Siparişiniz onay bekliyor",
                               ToplamTutar = s.ToplamTutar
                           }).Distinct().ToList();
            //    var newmodel= mymodel
            //    .GroupBy(p => p.SiparisNo,
            //             (k, c,d) => new SiparisDetay()
            //             {
            //                 SiparisNo = k,
            //                 Urunler = c.Select(cs => cs.Urun).ToList(),.
            //                 Adet=d.Sum(x => x.ad),
            //             };
            //}
            //            ).ToList();

            var orders =
    from h in mymodel
    group h by h.SiparisNo into r
    select new SiparisDetay()
    {
        SiparisNo = r.Key,
        Adet = r.Sum(x => x.Adet),
        ToplamTutar = r.Sum(x => x.ToplamTutar),
    };
            //mymodel.Reverse();
            return View(orders);
        }
        public ActionResult OrderDetail(string SiparisNo)
        {
            int BayiId = 0;
            if (Session["BayiId"] != null)
                BayiId = (int)Session["BayiId"];

            var mymodel = (from s in siparisRepo.Query<Siparisler>()
                           join
                           u in siparisRepo.Query<Urun>() on s.UrunId equals u.Id
                           join
                           b in siparisRepo.Query<ServisBakim>() on u.Id equals b.UrunId
                           where (s.BayiId == BayiId && s.IsDeleted == false && s.SiparisNo == SiparisNo && b.SiparisNo == SiparisNo)
                           orderby s.CreateDate descending
                           select new SiparisDetay()
                           {
                               SiparisNo = s.SiparisNo,
                               UrunAdi = u.UrunAdi,
                               SiparisTarihi = s.SiparisTarihi,
                               Adet = s.Adet,
                               SonBakimTarihi = b.BakimTarihi != null ? b.BakimTarihi : s.SiparisTarihi,
                               SonrakiBakimTarihi = b.sonrakiBakimTarihi,
                               BakimMasrafi = u.BakimFiyati,
                               BakimSuresi = u.BakimSure,
                               OnayDurumu = s.OnaylandiMi == true ? "Siparişiniz onaylandı" : "Siparişiniz onay bekliyor"
                           }).Distinct().ToList();

            //mymodel.Reverse();
            return View(mymodel);
        }

        // GET: Bayi/Edit/5
        public ActionResult UrunAdetGuncelle(int urunAdet, int urunId)
        {
            if (urunAdet == 0)
            {
                SepettenCikarUrunu(urunId);
            }
            List<SepetUrun> SepettekiUrunler = new List<SepetUrun>();

            if (Session["SepettekiUrunler"] != null)
            {
                SepettekiUrunler = (List<SepetUrun>)Session["SepettekiUrunler"];
            }

            if (SepettekiUrunler.Count > 0)
            {
                SepetUrun tempUrun = SepettekiUrunler.Find(x => x.UrunId == urunId);
                tempUrun.Adet = urunAdet;
                tempUrun.UrunToplamFiyati = urunAdet * tempUrun.UrunFiyati;

                Session["SepettekiUrunler"] = SepettekiUrunler;
                return RedirectToAction("Basket", "Bayi");
            }
            else
            {
                return RedirectToAction("Basket", "Bayi");
            }
        }

        public ActionResult SepettenCikarUrunu(int urunId)
        {
            List<SepetUrun> SepettekiUrunler = new List<SepetUrun>();

            if (Session["SepettekiUrunler"] != null)
            {
                SepettekiUrunler = (List<SepetUrun>)Session["SepettekiUrunler"];
            }

            if (SepettekiUrunler.Count > 0)
            {
                SepetUrun tempUrun = SepettekiUrunler.Find(x => x.UrunId == urunId);
                SepettekiUrunler.Remove(tempUrun);

                Session["SepettekiUrunler"] = SepettekiUrunler;
                return RedirectToAction("Basket", "Bayi");
            }
            else
            {
                return RedirectToAction("Basket", "Bayi");
            }
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public bool SiparisGeldiEmailGonder()
        {
            MailMessage ePosta = new MailMessage();
            ePosta.From = new MailAddress("ayyelift.iletisim@gmail.com");
            //
            ePosta.To.Add("yunus4emre@gmail.com");
            //
            //ePosta.Attachments.Add(new Attachment(@"C:\deneme-upload.jpg"));
            //
            ePosta.Subject = "Yeni Sipariş Geldi";
            //
            ePosta.Body = "Yeni sipariş geldi. Sisteme giriş yaparak siparişi inceleyebilirsiniz.";
            //
            SmtpClient smtp = new SmtpClient();
            //
            string smtpUserName = "ayyelift.iletisim@gmail.com";
            string smtpPassword = "yunuss";
            smtp.Credentials = new System.Net.NetworkCredential(smtpUserName, smtpPassword);
            smtp.Port = 587;
            smtp.Host = "smtp.gmail.com";
            smtp.EnableSsl = true;
            object userState = ePosta;
            bool kontrol = true;
            try
            {
                smtp.Send(ePosta);
            }
            catch (SmtpException ex)
            {
                kontrol = false;
            }
            return kontrol;
        }
        public string Sec(int id)
        {
            Urun urun = new Urun();
            List<Urun> urunler = new List<Urun>();
            if (Session["Karsilastir"] != null)
            {
                urunler = (List<Urun>)Session["Karsilastir"];
            }
            urun = urunRepo.Query<Urun>().Where(k => k.Id == id).FirstOrDefault();
            if (!urunler.Contains(urun))
            {
                urunler.Add(urun);
                Session["Karsilastir"] = urunler;

            }
            return "Karşılaştırmak için ürün seçtiniz. Geri dönerek karşılaştırmak istediğiniz başka ürünler de seçebilirsiniz. Ardından 'Karşılaştır' butonuna tıklamalısınız.";
        }
        public ActionResult Karsilastir()
        {
            List<Urun> urunler = new List<Urun>();

            if (Session["Karsilastir"] != null)
            {
                urunler = (List<Urun>)Session["Karsilastir"];
                return View(urunler);
            }
            else
                return View(db.Urun.Where(d => d.IsDeleted == false).ToList());
        }

    }
}
