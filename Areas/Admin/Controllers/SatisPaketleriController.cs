using FotografciTakipWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FotografciTakipWeb.Areas.Admin.Controllers
{
    public class SatisPaketleriController : Controller
    {
        FotoTakipContext dbContext = new FotoTakipContext();
        public ActionResult SatisPaketleri()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "SatisPaketleri";
            ViewBag.AltMenu = "Paketler";

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            //long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);

            List<SatisFiyatlari> paketler = dbContext.SatisFiyatlaris.Select(x => x).ToList();


            return View(paketler);
        }
    }
}