using AutoMapper;
using EduHome.Models.Identity;
using EduHome.Services.Interfaces;
using EduHome.Utils.Enums;
using EduHome.ViewModels.AccountViewModel;
using Microsoft.AspNetCore.Identity;

namespace EduHome.Controllers;

public class AccountController : Controller
{
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IEmailSender _emailSender;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly AppDbContext _context;

    public AccountController(IMapper mapper, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IEmailSender emailSender, IWebHostEnvironment webHostEnvironment, AppDbContext appDbContext)
    {
        _mapper = mapper;
        _userManager = userManager;
        _roleManager = roleManager;
        _emailSender = emailSender;
        _webHostEnvironment = webHostEnvironment;
        _context = appDbContext;
    }

    public IActionResult Register()
    {
        if (User.Identity.IsAuthenticated)
            return BadRequest();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
    {
        if (User.Identity.IsAuthenticated)
            return BadRequest();

        if (!ModelState.IsValid)
            return View();

        AppUser newUser = _mapper.Map<AppUser>(registerViewModel);
        //newUser.isActive = true;
        IdentityResult identityResult = await _userManager.CreateAsync(newUser, registerViewModel.Password);
        if (!identityResult.Succeeded)
        {
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View();
        }


        var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

        var link = Url.Action("ConfirmEmail", "Account", new { email = registerViewModel.Email, token = token }, HttpContext.Request.Scheme/*, Request.Host.ToString()*/);
        string body = await GetEmailTemplateAsync(link);
        _emailSender.SendEmail(registerViewModel.Email, "Confirm your email", body);


        await _userManager.AddToRoleAsync(newUser, Roles.Member.ToString());

        return RedirectToAction("Login", "Auth");
    }

    public async Task<IActionResult> ConfirmEmail(string email, string token)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest();

        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return View("Error");
        }

        var identityResult = await _userManager.ConfirmEmailAsync(user, token);

        if (!identityResult.Succeeded)
        {
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View();
        }
        else
        {
            user.isActive = true;
            await _context.SaveChangesAsync();
            return RedirectToAction("Login","Auth");
        }
    }

    public async Task<IActionResult> CreateRoles()
    {
        foreach (var role in Enum.GetValues(typeof(Roles)))
        {
            await _roleManager.CreateAsync(new IdentityRole { Name = role.ToString() });/*Enum.GetName()*/
        }
        return Content("OK");
    }

    private async Task<string> GetEmailTemplateAsync(string link)
    {
        string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "templates", "confirm-email.html");
        using StreamReader streamReader = new StreamReader(path);
        string result = await streamReader.ReadToEndAsync();
        return result.Replace("[link]", link);
    }
}
