using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Xml.Linq;
using DovizKur.Models;

namespace DovizKur.Controllers
{
    public class HomeController : Controller
    {
        private DovizKurContext db = new DovizKurContext();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(DateTime tarih)
        {
            if (IsWeekendOrHoliday(tarih))
            {
                ViewBag.Message = "Bu tarih hafta sonu veya resmi tatil olduğu için döviz kuru bulunamadı.";
                return View();
            }

            var dovizKuru = db.Kurlar.FirstOrDefault(k => k.Tarih == tarih);

            if (dovizKuru == null)
            {
                string url = $"https://www.tcmb.gov.tr/kurlar/{tarih:yyyyMM}/{tarih:ddMMyyyy}.xml";

                try
                {
                    WebClient client = new WebClient();
                    client.Encoding = System.Text.Encoding.UTF8;
                    string xmlData = client.DownloadString(url);

                    XDocument xml = XDocument.Parse(xmlData);

                    decimal euroKuru = decimal.Parse(xml.Descendants("Currency")
                        .FirstOrDefault(x => x.Attribute("CurrencyCode")?.Value == "EUR")
                        .Element("ForexBuying").Value);

                    decimal dolarKuru = decimal.Parse(xml.Descendants("Currency")
                        .FirstOrDefault(x => x.Attribute("CurrencyCode")?.Value == "USD")
                        .Element("ForexBuying").Value);

                    dovizKuru = new DovizKuru
                    {
                        Tarih = tarih,
                        EuroKuru = euroKuru,
                        DolarKuru = dolarKuru
                    };

                    db.Kurlar.Add(dovizKuru);
                    db.SaveChanges();

                    ViewBag.Message = "Döviz kuru veritabanına kaydedildi.";
                }
                catch
                {
                    ViewBag.Message = "Döviz kuru alınamadı. Lütfen daha sonra tekrar deneyin.";
                }
            }
            else
            {
                ViewBag.Message = "Döviz kuru veritabanında kayıtlı.";
            }

            return View(dovizKuru);
        }

        private bool IsWeekendOrHoliday(DateTime tarih)
        {
            // Hafta sonu kontrolü
            if (tarih.DayOfWeek == DayOfWeek.Saturday || tarih.DayOfWeek == DayOfWeek.Sunday)
            {
                return true;
            }

            // Resmi tatil kontrolü
            if (IsResmiTatil(tarih))
            {
                return true;
            }

            // Dini bayram kontrolü
            if (IsDiniBayram(tarih))
            {
                return true;
            }

            return false;
        }

        private bool IsResmiTatil(DateTime tarih)
        {
            // Resmi tatil kontrolü implementasyonu buraya gelecek
            // ...

            return false;
        }

        private bool IsDiniBayram(DateTime tarih)
        {
            // Dini bayram kontrolü implementasyonu buraya gelecek
            // ...

            return false;
        }
    }
}
