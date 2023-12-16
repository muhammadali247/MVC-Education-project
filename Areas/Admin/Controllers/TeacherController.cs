using AutoMapper;
using EduHome.Areas.Admin.ViewModels.TeacherViewModel;
using EduHome.Exceptions;
using EduHome.Models;
using EduHome.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EduHome.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,Moderator")]
public class TeacherController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;

    public TeacherController(AppDbContext context, IWebHostEnvironment webHostEnvironment, IFileService fileService, IMapper mapper)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        _fileService = fileService;
        _mapper = mapper;
    }
    public async Task<IActionResult> Index(int page = 1, int pageSize = 3)
    {
        var teachers = await _context.teachers.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        var count = await _context.teachers.CountAsync();
        List<AdminTeacherViewModel> adminTeacherViewModels = _mapper.Map<List<AdminTeacherViewModel>>(teachers);
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)count / pageSize);
        ViewBag.PageSize = pageSize;
        return View(adminTeacherViewModels);
    }

    public IActionResult Create()
    {
        ViewBag.SocialMedias = new SelectList(_context.socialMedias, "Id", "Icon");
        ViewBag.skills = new SelectList(_context.skills, "Id", "Name");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTeacherViewModel createTeacherViewModel)
    {
        ViewBag.SocialMedias = new SelectList(_context.socialMedias, "Id", "Icon");
        ViewBag.skills = new SelectList(_context.skills, "Id", "Name");

        if (!ModelState.IsValid)
            return View();

        if (createTeacherViewModel is null)
            return NotFound();

        var teacher = _mapper.Map<Teacher>(createTeacherViewModel);

        try
        {
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "teacher");
            teacher.Image = await _fileService.CreateFileAsync(createTeacherViewModel.Image, path, 100, "image/");
        }
        catch (FileSizeException ex)
        {
            ModelState.AddModelError("Image", ex.Message);
            return View();
        }
        catch (FileTypeException ex)
        {
            ModelState.AddModelError("Image", ex.Message);
            return View();
        }


        //List<CourseCategory> courseCategories = new List<CourseCategory>();
        //for (int i = 0; i < createCourseViewModel.CategoryId.Count; i++)
        //{
        //    CourseCategory courseCategory = new CourseCategory()
        //    {
        //        CategoryId = createCourseViewModel.CategoryId[i],
        //        CourseId = course.Id,
        //    };

        //    courseCategories.Add(courseCategory);
        //}
        //course.CourseCategories = courseCategories;


        await _context.teachers.AddAsync(teacher);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
