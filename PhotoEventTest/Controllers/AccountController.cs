using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using PhotoEventTest.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using PhotoEventTest.Models.ViewModel;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace PhotoEventTest.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private PHOTOGRAPHYEVENTContext _context;

        public AccountController(PHOTOGRAPHYEVENTContext ctxt)
        {
            _context = ctxt;
        }

        [AllowAnonymous]
        public ViewResult Login(string returnUrl) => View(new LoginModel { ReturnUrl = returnUrl });

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var user = await AuthenticateUser(loginModel);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(loginModel);
                }
                
                #region snippet1
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserId),
                    new Claim(ClaimTypes.Role, "Administrator"),
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    //AllowRefresh = <bool>,
                    // Refreshing the authentication session should be allowed.             
                    //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                    // The time at which the authentication ticket expires. A 
                    // value set here overrides the ExpireTimeSpan option of 
                    // CookieAuthenticationOptions set with AddCookie.
                    //IsPersistent = true,
                    // Whether the authentication session is persisted across 
                    // multiple requests. When used with cookies, controls
                    // whether the cookie's lifetime is absolute (matching the
                    // lifetime of the authentication ticket) or session-based.
                    //IssuedUtc = <DateTimeOffset>,
                    // The time at which the authentication ticket was issued.
                    //RedirectUri = <string>
                    // The full path or absolute URI to be used as an http 
                    // redirect response value.
                };
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
                #endregion
                string returnUrl = user.UserId.ToLower() == "admin" ? "/Admin/Index/" : "/Home/Index";
                return Redirect(loginModel?.ReturnUrl ?? returnUrl);
            }
            ModelState.AddModelError("", "Invalid name or password");
            return View(loginModel);
        }

        private async Task<LoginModel> AuthenticateUser(LoginModel loginModel)
        {
            Users loginUser = _context.Users.SingleOrDefault(u => u.UserId == loginModel.UserId);
            if (loginUser != null)
            {
                if (loginUser.Password == loginModel.Password)
                {
                    return new LoginModel()
                    {
                        UserId = loginModel.UserId,
                        Password = loginModel.Password,
                        ReturnUrl = loginModel.ReturnUrl
                    };
                }
            }
            return null;
        }

        public async Task<RedirectResult> Logout(string returnUrl = "/")
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect(returnUrl);
        }

        // GET: Account/Create
        [AllowAnonymous]
        public ActionResult CreateNewUser()
        {
            return View(new Users());
        }

        // POST: Account/Create
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewUser(Users newUser)
        {
            _context.Users.Add(newUser);
            _context.SaveChanges();
            TempData["resultmessage"] = "New account has been created. Please login.";
            return RedirectToAction(nameof(Login));
        }
    }
}