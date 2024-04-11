using Microsoft.AspNetCore.Mvc;

namespace CurseProject.Controllers
{
    public class Account : Controller
    {
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
