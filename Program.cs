using EduHome.Areas.Admin.ViewModels.EventViewModel;
using EduHome.Controllers;
using EduHome.Models.Identity;
using EduHome.Services.Implementations;
using EduHome.Services.Interfaces;
using EduHome.ViewModels.AccountViewModel;
using EduHome.ViewModels.AuthViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EventController = EduHome.Areas.Admin.Controllers.EventController;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:Default"]);
});

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    options.User.RequireUniqueEmail = true;
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);

    options.Lockout.AllowedForNewUsers = false;
}).AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<IFileService, FileService>();

//builder.Services.AddScoped<AppDbContext>();

builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddScoped<AppDbContextInitializer>();

builder.Services.ConfigureApplicationCookie(c =>
{
    c.LoginPath = "/Auth/Login";
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseStaticFiles();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<AppDbContextInitializer>();
    await initializer.InitializeAsync();
    await initializer.UserSeedAsync();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "Default",
    pattern: "{Controller=Home}/{Action=Index}/{Id?}"
);

app.MapPost("/subscribe", async (HttpContext context) =>
{
    var homeController = context.RequestServices.GetRequiredService<HomeController>();

    var email = context.Request.Form["email"];

    if (!string.IsNullOrWhiteSpace(email))
    {
        await homeController.Subscribe(email);
    }

    return Results.Redirect("/");
});

app.MapPost("/create", async (HttpContext context) =>
{
    var eventController = context.RequestServices.GetRequiredService<EventController>();

    var formFile = context.Request.Form.Files["Image"];
    var eventName = context.Request.Form["Name"];
    var eventDescription = context.Request.Form["Description"];
    var eventTime = context.Request.Form["Time"];
    var eventVenue = context.Request.Form["Venue"];

    var createEventViewModel = new CreateEventViewModel
    {
        Image = formFile,
        Name = eventName,
        Time = eventTime,
        Venue = eventVenue,
        Description = eventDescription
    };

    await eventController.Create(createEventViewModel);

    return Results.Redirect("/");
});

app.MapPost("/ForgotPassword", async (HttpContext context) =>
{
    var authController = context.RequestServices.GetRequiredService<AuthController>();

    var email = context.Request.Form["email"];

    var forgotPasswordViewModel = new ForgotPasswordViewModel
    {
        Email = email,
    };

    await authController.ForgotPassword(forgotPasswordViewModel);

    return Results.Redirect("/");
});

//app.MapPost("/Login", async (HttpContext context) =>
//{
//    var authController = context.RequestServices.GetRequiredService<AuthController>();

//    var UsernameOrEmail = context.Request.Form["UsernameOrEmail"];
//    var Password = context.Request.Form["Password"];
//    bool.TryParse(context.Request.Form["RememberMe"], out bool RememberMe);

//    var loginViewModel = new LoginViewModel
//    {
//        UsernameOrEmail = UsernameOrEmail,
//        Password = Password,
//        RememberMe = RememberMe,
//    };

//    await authController.Login(loginViewModel, returnUrl: null);

//    return Results.Redirect("/");
//});

app.MapPost("/Register", async (HttpContext context) =>
{
    var accountController = context.RequestServices.GetRequiredService<AccountController>();

    var email = context.Request.Form["Email"];
    var fullName = context.Request.Form["Fullname"];
    var Username = context.Request.Form["Username"];
    var Password = context.Request.Form["Password"];
    var PasswordConfirm = context.Request.Form["PasswordConfirm"];

    var registerViewModel = new RegisterViewModel
    {
        Email = email,
        Fullname = fullName,
        Username = Username,
        Password = Password,
        PasswordConfirm = PasswordConfirm,
    };

    await accountController.Register(registerViewModel);

    return Results.Redirect("/");
});


app.Run();