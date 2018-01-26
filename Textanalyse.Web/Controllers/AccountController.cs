using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Textanalyse.Data.Data;
using Textanalyse.Web.Models;

namespace Textanalyse.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _log;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AccountController> log)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._log = log;
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(string provider, string returnUrl = null)
        {
            if (this.ModelState.IsValid)
            {
                var info = await this._signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    this._log.LogError($"Could not retreive informations");
                }

                // User must have a user name -> else error!
                string userName = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
                string userEmail = info.Principal.FindFirstValue(ClaimTypes.Email);
                ApplicationUser user = new ApplicationUser
                {
                    UserName = userName,
                    Email = userEmail
                };

                var result = await this._userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await this._userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await this._signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        this._log.LogError("Could not login to your account.");
                    }
                }
            }

            return View(nameof(HomeController.Index), null);
        }

        [HttpPost("/account/logout")]
        public async Task<IActionResult> Logout()
        {
            await this._signInManager.SignOutAsync();
            this._log.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost("/account/externalLogin")]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = this.Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = this._signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet("/account/externalLoginCallback")]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            var info = await this._signInManager.GetExternalLoginInfoAsync();

            // Sign in the user with this external login provider if the user already has a login.
            var result = await this._signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                this._log.LogInformation("User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }

            return await Register(info.ProviderDisplayName, returnUrl);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
        {
            if (this.ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await this._signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    throw new ApplicationException("Error loading external login information during confirmation.");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await this._userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await this._userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await this._signInManager.SignInAsync(user, isPersistent: false);
                        this._log.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            this.ViewData["ReturnUrl"] = returnUrl;
            return View(nameof(ExternalLogin), model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            var user = await this._userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
            var result = await this._userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (this.Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
