using Microsoft.AspNetCore.Mvc;

namespace AdSale.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
