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
    public class YoneticiController : Controller
    {
        private DatabaseContext db = new DatabaseContext();
        BaseRepository<Urun> urunRepo = new BaseRepository<Urun>();
        BaseRepository<Rct> rctRepo = new BaseRepository<Rct>();

        BaseRepository<Hammadde> hmmdRepo = new BaseRepository<Hammadde>();
        BaseRepository<Siparisler> siparisRepo = new BaseRepository<Siparisler>();
        BaseRepository<OdemeYontemleri> odemeYontemleriRepo = new BaseRepository<OdemeYontemleri>();

        public void StokKontrol()
        {
            string stokAlarm = "";
            int i = 0;
            List<Hammadde> hammadde = hmmdRepo.Query<Hammadde>().Where(k => k.IsDeleted == false).ToList();
            foreach (var item in hammadde)
            {
                if (item.StokSayisi <= item.EsikStokSayisi)
                {
                    if (i == 0)
                    {
                        stokAlarm += item.HammaddeAdi;
                    }
                    else
                    {
                        stokAlarm += ", "+item.HammaddeAdi;
                    }
                }
                i++;
            }
            StokAlarmEpostaGonder(stokAlarm);

        }
        public bool StokAlarmEpostaGonder( string stokAlarmListesi)
        {
            MailMessage ePosta = new MailMessage();
            ePosta.From = new MailAddress("liftmarket.iletisim@gmail.com");
            //
            ePosta.To.Add("esraozcan911@gmail.com");
            //
            //ePosta.Attachments.Add(new Attachment(@"C:\deneme-upload.jpg"));
            //
            ePosta.Subject = "Stok miktarı azalan ürün bildirimi";
            //
            ePosta.Body = "("+stokAlarmListesi + ") belirtilen hammadde/hammaddeler eşik miktarının altına düşmüştür. Tedarik süreci için bilginize sunulur. ";
            //
            SmtpClient smtp = new SmtpClient();
            //
            string smtpUserName = "liftmarket.iletisim@gmail.com";
            string smtpPassword = "esra2021";
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

        // GET: Yonetici
        public ActionResult Index()
        {
            StokKontrol();

            List<Urun> urun = urunRepo.Query<Urun>().Where(k => k.IsDeleted == false).ToList();

            return View(urun);
        }

        // GET: Yonetici/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Urun urun = db.Urun.Find(id);
            if (urun == null)
            {
                return HttpNotFound();
            }
            return View(urun);
        }

        // GET: Yonetici/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Yonetici/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UrunAdi,BayiFiyati,MusteriFiyati,BakimFiyati,UretimSure,BakimSure,KarDurumu,CreateDate,IsDeleted")] Urun urun)
        {
            if (ModelState.IsValid)
            {
                urunRepo.Add(urun);

                return RedirectToAction("Index");
            }

            return View(urun);
        }

        // GET: Yonetici/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Urun urun = urunRepo.Query<Urun>().FirstOrDefault(k => k.Id == id);
            if (urun == null)
            {
                return HttpNotFound();
            }
            return View(urun);
        }

        // POST: Yonetici/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UrunAdi,BayiFiyati,MusteriFiyati,BakimFiyati,UretimSure,BakimSure,KarDurumu")] Urun urun)
        {
            if (ModelState.IsValid)
            {
                Urun urunDB = urunRepo.Query<Urun>().FirstOrDefault(k => k.Id == urun.Id);

                urunDB.UrunAdi = urun.UrunAdi;
                urunDB.BayiFiyati = urun.BayiFiyati;
                urunDB.MusteriFiyati = urun.MusteriFiyati;
                urunDB.BakimFiyati = urun.BakimFiyati;
                urunDB.UretimSure = urun.UretimSure;
                urunDB.BakimSure = urun.BakimSure;
                urunDB.KarDurumu = urun.KarDurumu;

                urunRepo.saveChanges();
                return RedirectToAction("Index");
            }
            return View(urun);
        }

        // GET: Yonetici/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Urun urun = urunRepo.Query<Urun>().FirstOrDefault(k => k.Id == id);

            if (urun == null)
            {
                return HttpNotFound();
            }
            return View(urun);
        }

        // POST: Yonetici/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Urun urun = urunRepo.Query<Urun>().FirstOrDefault(k => k.Id == id);
            //urun.IsDeleted = true;
            //db.Urun.Remove(urun);
            urunRepo.Delete(urun);
            return RedirectToAction("Index");
        }

        public ActionResult Siparisler()
        {
            var mymodel = (from s in siparisRepo.Query<Siparisler>()
                           join
                           u in siparisRepo.Query<Urun>() on s.UrunId equals u.Id
                           //join
                           //b in siparisRepo.Query<ServisBakim>() on u.Id equals b.UrunId
                           where s.IsDeleted == false
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
                               OnaylandiMi = s.OnaylandiMi,
                               OdendiMi = s.OdendiMi,
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
        OnaylandiMi = mymodel.Where(s => s.SiparisNo == r.Key).FirstOrDefault().OnaylandiMi,
        OdendiMi = mymodel.Where(s => s.SiparisNo == r.Key).FirstOrDefault().OdendiMi
    };
            //mymodel.Reverse();
            return View(orders);
        }

        public ActionResult OrderDetail(string SiparisNo)
        {
            var mymodel = (from s in siparisRepo.Query<Siparisler>()
                           join
                           u in siparisRepo.Query<Urun>() on s.UrunId equals u.Id
                           join
                           b in siparisRepo.Query<ServisBakim>() on u.Id equals b.UrunId
                           where (s.IsDeleted == false && s.SiparisNo == SiparisNo && b.SiparisNo == SiparisNo)
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
                               OnayDurumu = s.OnaylandiMi == true ? "Siparişiniz onaylandı" : "Siparişiniz onay bekliyor",
                               OnaylandiMi = s.OnaylandiMi,
                               OdendiMi = s.OdendiMi
                           }).Distinct().ToList();

            //mymodel.Reverse();
            return View(mymodel);
        }
        public ActionResult Onayla(string SiparisNo)
        {
            List<Siparisler> siparisler = db.Siparisler.ToList();
            foreach (var item in siparisler)
            {
                if (item.SiparisNo == SiparisNo)
                    item.OnaylandiMi = true;
            }
            db.SaveChanges();
            return RedirectToAction("Siparisler", "yonetici");

        }
        public ActionResult OdemeBilgisiEkle(string SiparisNo)
        {
            Siparisler siparis = urunRepo.Query<Siparisler>().FirstOrDefault(k => k.SiparisNo == SiparisNo);
            List<OdemeYontemleri> odemeYontemleri = odemeYontemleriRepo.Query<OdemeYontemleri>().ToList();
            ViewBag.odemeYontemleri = new SelectList(odemeYontemleri, "Id", "OdemeSekli");
            return View(siparis);
        }

        [HttpPost, ActionName("OdemeBilgisiEkle")]
        [ValidateAntiForgeryToken]
        public ActionResult OdemeBilgisiEkle([Bind(Include = "Id,SiparisNo,OdendiMi")] Siparisler siparis)
        {
            List<Siparisler> siparisDB = siparisRepo.Query<Siparisler>().Where(k => k.SiparisNo == siparis.SiparisNo).ToList();
            List<OdemeYontemleri> odemeYontemleriList = odemeYontemleriRepo.Query<OdemeYontemleri>().ToList();

            if (siparis.OdendiMi)
            {
                foreach (var item in siparisDB)
                {
                    item.OdendiMi = true;
                }
            }
            else
            {
                foreach (var item in siparisDB)
                {
                    item.OdendiMi = false;
                }
            }
            siparisRepo.saveChanges();
            return RedirectToAction("Siparisler");
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
            ePosta.From = new MailAddress("liftmarket.iletisim@gmail.com");
            //
            ePosta.To.Add("esraozcan911@gmail.com");
            //
            //ePosta.Attachments.Add(new Attachment(@"C:\deneme-upload.jpg"));
            //
            ePosta.Subject = "Yeni Sipariş Geldi";
            //
            ePosta.Body = "Yeni sipariş geldi. Sisteme giriş yaparak siparişi inceleyebilirsiniz.";
            //
            SmtpClient smtp = new SmtpClient();
            //
            smtp.Credentials = new System.Net.NetworkCredential("liftmarket.iletisim@gmail.com", "esra2021");
            smtp.Port = 587;
            smtp.Host = "smtp.gmail.com";
            smtp.EnableSsl = true;
            object userState = ePosta;
            bool kontrol = true;
            try
            {
                smtp.SendAsync(ePosta, (object)ePosta);
            }
            catch (SmtpException ex)
            {
                kontrol = false;
            }
            return kontrol;
        }

        public ActionResult ReceteEkle(int id)
        {
            Urun urun = urunRepo.Query<Urun>().FirstOrDefault(k => k.Id == id);
            List<Hammadde> Hammaddes = hmmdRepo.Query<Hammadde>().ToList(); ;

            if (urun == null)
            {
                return HttpNotFound();
            }
            List<RctTemp> rctTempList = new List<RctTemp>();

            foreach (var hammadde in Hammaddes)
            {
                RctTemp rctTemp = new RctTemp();

                rctTemp.Urun = urun;
                rctTemp.UrunId = id;
                rctTemp.HammaddeAdi = hammadde.HammaddeAdi;
                rctTemp.HammaddeId = hammadde.Id;

                rctTempList.Add(rctTemp);
            }
            return View(rctTempList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReceteEkle([Bind(Include = "UrunId,HammaddeAdi,HammaddeId,NeKadar")] RctTemp rctTemp)
        {
            Rct rct = new Rct();

            rct.UrunId = rctTemp.UrunId;
            rct.HammaddeId = rctTemp.HammaddeId;
            rct.NeKadar = rctTemp.NeKadar;


            return RedirectToAction("Index");
        }


    }
}
