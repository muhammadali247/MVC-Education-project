using EduHome.Services.Interfaces;
using EduHome.ViewModels.AuthViewModel;
using Microsoft.AspNetCore.Identity;
using EduHome.Models.Identity;

namespace EduHome.Controllers;

public class AuthController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IEmailSender _emailSender;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailSender emailSender, IWebHostEnvironment webHostEnvironment)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Login()
    {
        if (User.Identity.IsAuthenticated)
            return BadRequest();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel, string? returnUrl)
    {
        if (User.Identity.IsAuthenticated)
            return BadRequest();

        if (!ModelState.IsValid)
            return View();

        AppUser appUser = await _userManager.FindByEmailAsync(loginViewModel.UsernameOrEmail);
        if (appUser is null)
        {
            appUser = await _userManager.FindByNameAsync(loginViewModel.UsernameOrEmail);
            if (appUser is null)
            {
                ModelState.AddModelError("", "Username/Email or Password is incorrect");
                return View();
            }
        }



        if (!appUser.isActive)
        {
            ModelState.AddModelError("", "Your account is not active, please check out your mail to confirm your registration");
            return View();
        }




        var signInResult = await _signInManager.PasswordSignInAsync(appUser, loginViewModel.Password, loginViewModel.RememberMe, true);
        if (signInResult.IsLockedOut)
        {
            ModelState.AddModelError("", "Get,sonra gelersen!!");
            return View();
        }
        if (!signInResult.Succeeded)
        {
            ModelState.AddModelError("", "Username/Email or Password is incorrect");
            return View();
        }
        //if (!signInResult.Succeeded)
        //{

        //    if (appUser.IsTwoFactorEnabled)
        //    {
        //        var twoFactorCode = _userManager.GenerateTwoFactorTokenAsync(appUser,/* _userManager.Options.Tokens.AuthenticatorTokenProvider*/TokenOptions.DefaultPhoneProvider);
        //        //await _userManager.UpdateAsync(appUser);

        //        var link = Url.Action("VerifyTwoFactor", "Auth", new { userId = appUser.Id ,twoFactorCode = twoFactorCode }, HttpContext.Request.Scheme);
        //        //string body = $"Your 2FA code is: {twoFactorCode}. Use this code to complete your login.<br/><a href='{link}'>Click here to verify</a>";
        //        string body = await GetTFATemplateAsync(link);

        //        _emailSender.SendEmail(appUser.Email, "2FA Verification Code", body);

        //        return RedirectToAction("VerifyTwoFactor", new { userId = appUser.Id });
        //    }
        //    ModelState.AddModelError("", "Username/Email or Password is incorrect");
        //    return View();
        //}

        if (!appUser.LockoutEnabled)
        {
            appUser.LockoutEnabled = true;
            appUser.LockoutEnd = null;
            //appUser.AccessFailedCount = 0;
            await _userManager.UpdateAsync(appUser);
        }

        if (returnUrl is not null)
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }





    //[HttpGet]
    //public async Task<IActionResult> VerifyTwoFactor(string userId)
    //{
    //    var user = await _userManager.FindByIdAsync(userId);
    //    if (user is null)
    //    {
    //        return NotFound();
    //    }

    //    return View(new VerifyTwoFactorViewModel { UserId = userId });
    //}

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> VerifyTwoFactor(VerifyTwoFactorViewModel model)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        return View(model);
    //    }

    //    var user = await _userManager.FindByIdAsync(model.UserId);
    //    if (user is null)
    //    {
    //        return NotFound();
    //    }

    //    if (user.TwoFactorCode == model.Code)
    //    {
    //        // Clear the two-factor code after successful verification
    //        user.TwoFactorCode = null;
    //        await _userManager.UpdateAsync(user);

    //        await _signInManager.SignInAsync(user, isPersistent: false);

    //        return RedirectToAction("Index", "Home");
    //    }

    //    ModelState.AddModelError("", "Invalid verification code");
    //    return View(model);
    //}








    public async Task<IActionResult> Logout()
    {
        if (!User.Identity.IsAuthenticated)
            return BadRequest();

        await _signInManager.SignOutAsync();

        return RedirectToAction(nameof(Login));
    }

    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgotPasswordViewModel)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var user = await _userManager.FindByEmailAsync(forgotPasswordViewModel.Email);

        if (user is null)
        {
            ModelState.AddModelError("Email", "User not found by email");
            return View();
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var link = Url.Action("ResetPassword", "Auth", new { email = forgotPasswordViewModel.Email, token = token}, HttpContext.Request.Scheme);
        //return RedirectToAction(nameof(ResetPassword));
        //return Content(link);
        string body = await GetEmailTemplateAsync(link);
        _emailSender.SendEmail(forgotPasswordViewModel.Email, "Reset your Password", body);

        return RedirectToAction(nameof(Login));
    }

    public async Task<IActionResult> ResetPassword(string email, string token)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            return BadRequest();

        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return NotFound();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel, string email, string token)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest();

        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return NotFound();

        var identityResult = await _userManager.ResetPasswordAsync(user, token, resetPasswordViewModel.NewPassword);

        if (!identityResult.Succeeded)
        {
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View();
        }

        return RedirectToAction(nameof(Login));
    }

    private async Task<string> GetEmailTemplateAsync(string link)
    {
        string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "templates", "reset-password.html");
        using StreamReader streamReader = new StreamReader(path);
        string result = await streamReader.ReadToEndAsync();
        return result.Replace("[link]", link);
    }

    private async Task<string> GetTFATemplateAsync(string link)
    {
        string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "templates", "two-factor.html");
        using StreamReader streamReader = new StreamReader(path);
        string result = await streamReader.ReadToEndAsync();
        return result.Replace("[link]", link);
    }
}
