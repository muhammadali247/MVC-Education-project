using AutoMapper;
using EduHome.Areas.Admin.ViewModels.BlogViewModel;
using EduHome.Areas.Admin.ViewModels.CategoryViewModel;
using EduHome.Exceptions;
using EduHome.Models;
using EduHome.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduHome.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoryController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;

    public CategoryController(AppDbContext context, IWebHostEnvironment webHostEnvironment, IFileService fileService, IMapper mapper)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        _fileService = fileService;
        _mapper = mapper;
    }

    public  async Task<IActionResult> Index(int page = 1, int pageSize = 3)
    {
        var categories = await _context.Categories.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        var count = await _context.Categories.CountAsync();
        List<CategoryAdminViewModel> categoryAdminViewModels = _mapper.Map<List<CategoryAdminViewModel>>(categories);
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)count / pageSize);
        ViewBag.PageSize = pageSize;
        return View(categoryAdminViewModels);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCategoryViewModel createCategoryViewModel)
    {
        if (!ModelState.IsValid)
            return View();

        var category = _mapper.Map<Category>(createCategoryViewModel);
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Update(int id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(category => category.Id == id);
        if (category is null)
            return NotFound();

        var updateCategoryViewModel = _mapper.Map<UpdateCategoryViewModel>(category);

        return View(updateCategoryViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, UpdateCategoryViewModel updateCategoryViewModel)
    {
        if (!ModelState.IsValid)
            return View();

        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category is null)
            return NotFound();

        _mapper.Map(updateCategoryViewModel, category);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

        if (category is null)
            return NotFound();

        var deleteCategoryViewModel = _mapper.Map<DeleteCategoryViewModel>(category);
        return View(deleteCategoryViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

        if (category is null)
            return NotFound();

        category.IsDeleted = true;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Detail(int id)
    {
        Category? category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category is null)
            return NotFound();

        var categoryDetailViewModel = _mapper.Map<CategoryDetailViewModel>(category);
        return View(categoryDetailViewModel);
    }
}
