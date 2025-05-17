using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Yapaydeneme.Models;

namespace Yapaydeneme.Controllers
{
    public class HesapController : Controller
    {
        private UserManager<IdentityUser> _userManager;
        public HesapController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;   
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]

        public async Task<ActionResult> Create(HesapOlusturModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.KullaniciAdi, Email = model.Email };

                var result = await _userManager.CreateAsync(user, model.Sifre);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

    }
}
