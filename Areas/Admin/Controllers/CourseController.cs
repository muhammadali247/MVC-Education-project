using AutoMapper;
using EduHome.Areas.Admin.ViewModels.CourseViewModel;
using EduHome.Areas.Admin.ViewModels.EventViewModel;
using EduHome.Exceptions;
using EduHome.Models;
using EduHome.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EduHome.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,Moderator")]
public class CourseController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;

    public CourseController(AppDbContext context, IWebHostEnvironment webHostEnvironment, IFileService fileService, IMapper mapper)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        _fileService = fileService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 3)
    {
        var courses = await _context.Courses.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        var count = await _context.Courses.CountAsync();
        List<CourseAdminViewModel> courseAdminViewModels = _mapper.Map<List<CourseAdminViewModel>>(courses);
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)count / pageSize);
        ViewBag.PageSize = pageSize;
        return View(courseAdminViewModels);
    }

    public IActionResult Create()
    {
        ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCourseViewModel createCourseViewModel)
    {
        ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");

        if (!ModelState.IsValid)
            return View();

        if (createCourseViewModel is null)
            return NotFound();

        var course = _mapper.Map<Course>(createCourseViewModel);

        try
        {
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "course");
            course.Image = await _fileService.CreateFileAsync(createCourseViewModel.Image, path, 100, "image/");
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


        List<CourseCategory> courseCategories = new List<CourseCategory>();
        for (int i = 0; i < createCourseViewModel.CategoryId.Count; i++)
        {
            CourseCategory courseCategory = new CourseCategory()
            {
                CategoryId = createCourseViewModel.CategoryId[i],
                CourseId = course.Id,
            };

            courseCategories.Add(courseCategory);
        }
        course.CourseCategories = courseCategories;


        await _context.Courses.AddAsync(course);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Update(int id)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
        if (course is null) 
            return NotFound();

        var updateCourseViewModel = _mapper.Map<UpdateCourseViewModel>(course);
        return View(updateCourseViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id,UpdateCourseViewModel updateCourseViewModel)
    {
        if (!ModelState.IsValid)
            return View();

        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
        if (course is null)
            return NotFound();

        string fileName = course.Image;
        if (updateCourseViewModel.Image is not null)
        {
            try
            {
                string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "course");
                _fileService.DeleteFile(Path.Combine(path, course.Image));

                fileName = await _fileService.CreateFileAsync(updateCourseViewModel.Image, path, 100, "image/");
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
        }
        _mapper.Map(updateCourseViewModel, course);
        course.Image = fileName;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
        if (course is null) 
            return NotFound();

        var deleteCourseViewModel = _mapper.Map<DeleteCourseViewModel>(course);
        return View(deleteCourseViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);

        if (course is null)
            return NotFound();

        string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "course", course.Image);
        _fileService.DeleteFile(path);

        course.IsDeleted = true;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Detail(int id)
    {
        Course? course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
        if (course is null)
            return NotFound();

        var courseDetailViewModel = _mapper.Map<AdminCourseDetailViewModel>(course);
        return View(courseDetailViewModel);
    }
}
