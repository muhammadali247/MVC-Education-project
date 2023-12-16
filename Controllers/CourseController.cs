using AutoMapper;
using EduHome.Models;
using EduHome.ViewModels.CourseViewModel;
using EduHome.ViewModels.EventViewModel;
using Microsoft.EntityFrameworkCore;

namespace EduHome.Controllers;

public class CourseController : Controller
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CourseController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index(string category)
    {
        //var courses = await _context.Courses.ToListAsync();

        //List<CourseViewModel> courseViewModels = _mapper.Map<List<CourseViewModel>>(courses);

        //ViewBag.Categories = await _context.Categories.Select(c => c.Name).ToListAsync();

        //return View(courseViewModels);

        IQueryable<Course> coursesQuery = _context.Courses.Include(c => c.CourseCategories).ThenInclude(cs => cs.Category);

        if (!string.IsNullOrEmpty(category))
        {
            coursesQuery = coursesQuery.Where(c => c.CourseCategories.Any(cc => cc.Category.Name == category));
        }

        var courses = await coursesQuery.ToListAsync();

        List<CourseViewModel> courseViewModels = _mapper.Map<List<CourseViewModel>>(courses);

        ViewBag.Categories = await _context.Categories.Select(c => c.Name).ToListAsync();

        return View(courseViewModels);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            //Course? course = await _context.Courses.Include(c => c.CourseCategories).ThenInclude(cs => cs.Category).FirstOrDefaultAsync(b => b.Id >= 1);
            //if (course is null)
                return NotFound();
            //var courseDetailViewModel = _mapper.Map<CourseDetailViewModel>(course);
            //return View(courseDetailViewModel);
        }
        else
        { 
            Course? course = await _context.Courses.Include(c => c.CourseCategories).ThenInclude(cs => cs.Category).FirstOrDefaultAsync(b => b.Id == id);
            if (course is null)
                return NotFound();
            ViewBag.Categories = _context.Categories.Include(c => c.CourseCategories).Where(c => c.CourseCategories.Any(cc => cc.CourseId == course.Id));
            var courseDetailViewModel = _mapper.Map<CourseDetailViewModel>(course);
            return View(courseDetailViewModel);
        }
    }
}
