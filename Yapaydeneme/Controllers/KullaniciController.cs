using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Yapaydeneme.Models;

namespace Yapaydeneme.Controllers
{
    public class KullaniciController : Controller
    {
        private UserManager<IdentityUser> _userManager;
        public KullaniciController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public ActionResult Index()
        {
            return View(_userManager.Users);
        }
    }
}
