using AutoMapper;
using EduHome.Areas.Admin.ViewModels.UserViewModel;
using EduHome.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EduHome.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class Dashboard : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;

    public Dashboard(AppDbContext context, IWebHostEnvironment webHostEnvironment, IFileService fileService, IMapper mapper)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        _fileService = fileService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 4)
    {
        var users = await _context.Users.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        var count = await _context.Users.CountAsync();
        List<AdminUserViewModel> adminUserViewModels = _mapper.Map<List<AdminUserViewModel>>(users);
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)count / pageSize);
        ViewBag.PageSize = pageSize;
        return View(adminUserViewModels);
    }

    public async Task<IActionResult> ChangeRole(string id)
    {
        ViewBag.Roles = new SelectList(await _context.Roles.ToListAsync(), "Id", "Name");

        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        if (user is null) 
         return NotFound();

        ChangeUserRoleViewModel changeUserRoleViewModel = _mapper.Map<ChangeUserRoleViewModel>(user);
        return View(changeUserRoleViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeRole(ChangeUserRoleViewModel changeUserRoleViewModel, string Id)
    {
        ViewBag.Roles = new SelectList(await _context.Roles.ToListAsync(), "Id", "Name");

        if (changeUserRoleViewModel is null)
            return BadRequest();

        var userRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == Id);

        if (userRole is null)
            return NotFound();

        _context.UserRoles.Remove(userRole);
        await _context.UserRoles.AddAsync(new IdentityUserRole<string> { UserId = userRole.UserId, RoleId = changeUserRoleViewModel.RoleId });

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public IActionResult ChangeState()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeState(string id, bool state)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null)
        {
            return NotFound();
        }

        user.isActive = state;
        _context.Update(user);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

}
