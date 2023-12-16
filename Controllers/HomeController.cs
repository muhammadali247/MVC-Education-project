using AutoMapper.Internal;
using EduHome.Models;
using EduHome.Services.Interfaces;
using EduHome.ViewModels.AuthViewModel;
using EduHome.ViewModels.HomeViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace EduHome.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;
    private readonly IEmailSender _emailSender;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public HomeController(AppDbContext context, IEmailSender emailSender, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _emailSender = emailSender;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<IActionResult> Index()
    {
        //ViewBag.SuccessMessage = TempData["SuccessMessage"] as string;

        var sliders = await _context.Sliders.ToListAsync();
        var courses = await _context.Courses.OrderByDescending(c => c.Id).Take(3).ToListAsync();
        var events = await _context.Events.OrderBy(c => c.Id).Take(4).ToListAsync();
        var blogs = await _context.Blogs.OrderByDescending(c => c.Id).Take(3).ToListAsync();

        HomeViewModel homeViewModel = new HomeViewModel
        {
            Sliders = sliders,
            Courses = courses,
            Events = events,
            Blogs = blogs
        };

        return View(homeViewModel);
    }

    //[HttpPost]
    //public async Task<IActionResult> Subscribe(string email)
    //{
    //    if (!string.IsNullOrWhiteSpace(email))
    //    {
    //        var newSubscriber = new Subscriber
    //        {
    //            Email = email,
    //            IsActive = false
    //        };

    //        await _context.Subscribers.AddAsync(newSubscriber);
    //        await _context.SaveChangesAsync();

    //        //TempData["SuccessMessage"] = "Successfully subscribed!";
    //    }

    //    return RedirectToAction("Index");
    //}

    public async Task<IActionResult> Subscribe()
    {
        if (User.Identity.IsAuthenticated)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if (user is null)
            {
                return BadRequest();
            }
            if (await _context.Subscribers.FirstOrDefaultAsync(u => u.Email == user.Email) is not null)
            {

                return RedirectToAction(nameof(Error));
            }



            Subscriber subscriber = new Subscriber()
            {
                Email = user.Email,
            };
            await _context.Subscribers.AddAsync(subscriber);
            await _context.SaveChangesAsync();

        }


        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Subscribe(string email)
    {
        if (!User.Identity.IsAuthenticated)
        {

            if (await _context.Subscribers.FirstOrDefaultAsync(u => u.Email == email) is not null)
            {
                return RedirectToAction(nameof(Error));
            }


            var Token = Guid.NewGuid().ToString();
            string link = Url.Action("Success", "Home", new { token = Token, Email = email }, HttpContext.Request.Scheme);
            string body = await GetEmailTemplateAsync(link);

            //MailRequest mailRequest = new MailRequest()
            //{
            //    Subject = "subscribe confirm",
            //    ToEmail = email,
            //    Body = body

            //};
            //await _mailService.SendEMailAsync(mailRequest);
            _emailSender.SendEmail(email, "Subscription confirmation!", body);

        }


        return RedirectToAction("Index");
    }
    private async Task<string> GetEmailTemplateAsync(string link)
    {
        string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "templates", "confirm-email.html");
        using StreamReader streamReader = new StreamReader(path);
        string result = await streamReader.ReadToEndAsync();
        return result.Replace("[link]", link);


    }
    public async Task<IActionResult> Success(string token, string email)
    {
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(email))
        {
            return BadRequest();
        }
        if (await _context.Subscribers.FirstOrDefaultAsync(s => s.Email == email) is not null)
        {
            return BadRequest();
        }

        Subscriber subscriber = new Subscriber()
        {
            Email = email,
        };
        await _context.Subscribers.AddAsync(subscriber);
        await _context.SaveChangesAsync();

        return Ok("Success");
    }
    public IActionResult Error()
    {
        return View();
    }
}
