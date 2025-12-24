using System.Web.Mvc;
using FotografciTakipWeb.Models;

namespace FotografciTakipWeb.Areas.Otomasyon.Controllers
{
    public class HataController : Controller
    {
        FotoTakipContext dbContext = new FotoTakipContext();
        // GET: Otomasyon/Hata
        public ActionResult YetkisizGiris()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Hata";
            ViewBag.AltMenu = "Yetkisiz Giriş";
            return View();
        }

        public ActionResult SayfaBulunamadi()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Hata";
            ViewBag.AltMenu = "Erişmek İstediğiniz Sayfa Bulunamadı";
            return View();
        }

        public ActionResult GenelHata()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            TempData["HataMesajı"] = "Hata";
            ViewBag.AltMenu = "Erişmek İstediğiniz Sayfa Bulunamadı";
            return View();
        }
    }
}