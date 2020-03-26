using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<AccountController> logger;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, 
            ILogger<AccountController> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        // Need to make method async because of CreateAsync method.
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    City = model.City
                };
                // Must use await keyword due to CreateAsync asynchronous method.
                var result = await userManager.CreateAsync(user, model.Password);

                // Log in user if CreateAsync succeeds
                if (result.Succeeded)
                {
                    // Upon successful regestration, generate an email confirmation token.
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

                    // Generate Email Confirmation Link to email to the user.
                    var confirmationLink = Url.Action("ConfirmEmail", "Account",
                        new { userId = user.Id, token = token }, Request.Scheme);

                    logger.Log(LogLevel.Warning, confirmationLink);

                    //If admin user is already signed in, do not login to newly registered user.
                    if(signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers", "Administration");
                    }

                    //// Set isPersistent to false -- we don't want a permanent cookie
                    //await signInManager.SignInAsync(user, isPersistent: false);
                    //return RedirectToAction("index", "home");

                    // TODO: Create Registration Success View instead.
                    ViewBag.ErrorTitle = "Registration Successful";
                    ViewBag.ErrorMessage = "You must confirm your email before logging in. Please click the confirmation link emailed to you.";
                    return View("Error");
                }
                // If CreateAsync fails, add the errors to the modelstate to be displayed on the validation summary tag helper
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("index", "home");
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"The User ID {userId} is invalid";
                return View("NotFound");
            }

            // Changes EmailConfirmed Column in AspNetUsers from FALSE to TRUE
            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                // TODO: Log User in and Redirect to List Users View
                return View();
            }

            ViewBag.ErrorTitle = "Email Cannot Be Confirmed";
            return View("Error");
        }


        // Remote Validation
        [HttpPost][HttpGet]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if(user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {email} is already taken.");
            }
        }


        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                // GetExternalAuthenticationSchemesAsync returns us the list of all configured external login providers
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            return View(model);
        }

        [HttpPost]
        // TODO: Generate token confirmation if user has not confirmed email
        // Model binding will automatically map the query string value with the returnUrl parameter
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                // Show error if user has not confirmed email
                // Make sure that user uses the correct user name and password before showing the email not confirmed error
                if (user != null && !user.EmailConfirmed && (await userManager.CheckPasswordAsync(user, model.Password)))
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                    return View(model);
                }


                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                // Redirect user if login is successful
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("index", "home");
                    }

                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

            }
            return View(model);
        }

        [HttpPost]
        // The "name" property of the Login View is bound to the parameter of this action if the names match,
        // and the "value" property binds that value automatically to the matching parameter
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account",
                new { ReturnUrl = returnUrl });

            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            // Inherits from IActionResult redirects user to the external login providers login page
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            // If returnUrl is null, we initialize it to the application root Url
            returnUrl = returnUrl ?? Url.Content("~/");

            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View("Login", model);
            }

            // Get information from external provider when the user logs in using their service.
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Error loading external login information.");
                return View("Login", model);
            }

            // Get the email claim from external login provider
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            ApplicationUser user = null;

            if (email != null)
            {
                // Find the user
                user = await userManager.FindByEmailAsync(email);

                // If email is not confirmed, display login view with validation error
                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                    return View("Login", model);
                }
            }

            // Log user in using the information gathered from the external provider.
            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                if (email != null)
                {
                    // Check to see if the user has an existing local account
                    user = await userManager.FindByEmailAsync(email);

                    // If we don't find a user, we create a new local user account for that external user.
                    // The code in this block keeps us from duplicating the same user, regardless of what provider they use to authenticate.
                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                        };

                        await userManager.CreateAsync(user);

                        // Upon successful regestration, generate an email confirmation token.
                        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

                        // Generate Email Confirmation Link to email to the user.
                        var confirmationLink = Url.Action("ConfirmEmail", "Account",
                            new { userId = user.Id, token = token }, Request.Scheme);

                        logger.Log(LogLevel.Warning, confirmationLink);

                        ViewBag.ErrorTitle = "Registration successful";
                        ViewBag.ErrorMessage = "You must confirm your email before logging in. Please click the confirmation link emailed to you.";
                        return View("Error");
                    }

                    // If we find the user, then the external user has a local account and we want to add a row in the AspNetUserLogins table.
                    await userManager.AddLoginAsync(user, info);
                    await signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }

                // If the email claim does not exist on the external login, we cannot continue and redirect to the Error page
                ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
                ViewBag.ErrorMessage = $"Please contact support from jordan.naeem@gmail.com";

                return View("Error");
            }
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }


        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if(user != null && await userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);

                    var passwordResetLink = Url.Action("ResetPassword", "Account",
                        new { email = model.Email, token = token }, Request.Scheme);

                    logger.Log(LogLevel.Warning, passwordResetLink);
                    return View("ForgotPasswordConfirmation");
                }
                // We don't want to reveal that the account doesn't exist due to brute force attempts
                return View("ForgotPasswordConfirmation");
            }

            return View(model);
        }
    }
}
