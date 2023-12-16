using AutoMapper;
using EduHome.Areas.Admin.ViewModels.CourseViewModel;
using EduHome.Areas.Admin.ViewModels.SliderViewModel;
using EduHome.Exceptions;
using EduHome.Models;
using EduHome.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace EduHome.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,Moderator")]
public class SliderController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;

    public SliderController(AppDbContext context, IWebHostEnvironment webHostEnvironment, IFileService fileService, IMapper mapper)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        _fileService = fileService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 3)
    {
        var sliders = await _context.Sliders.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        List<AdminSliderViewModel> adminSliderViewModels = _mapper.Map<List<AdminSliderViewModel>>(sliders);
        var count = await _context.Sliders.CountAsync();
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalCount = (int)Math.Ceiling((double)count / pageSize);
        return View(adminSliderViewModels);
    }

    public async Task<IActionResult> Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSliderViewModel createSliderViewModel)
    {
        if (!ModelState.IsValid)
            return View();

        var slider = _mapper.Map<Slider>(createSliderViewModel);

        try
        {
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "slider");
            slider.Image = await _fileService.CreateFileAsync(createSliderViewModel.Image, path, 100, "image/");
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
        await _context.Sliders.AddAsync(slider);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Update(int id)
    {
        var slider = await _context.Sliders.FirstOrDefaultAsync(slider => slider.Id == id);
        if (slider is null)
            return NotFound();

        var updateSliderViewModel = _mapper.Map<UpdateSliderViewModel>(slider);

        return View(updateSliderViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, UpdateSliderViewModel updateSliderViewModel)
    {
        if (!ModelState.IsValid)
            return View();

        var slider = await _context.Sliders.FirstOrDefaultAsync(c => c.Id == id);
        if (slider is null)
            return NotFound();

        string fileName = slider.Image;
        if (updateSliderViewModel.Image is not null)
        {
            try
            {
                string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "slider");
                _fileService.DeleteFile(Path.Combine(path, slider.Image));

                fileName = await _fileService.CreateFileAsync(updateSliderViewModel.Image, path, 100, "image/");
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
        _mapper.Map(updateSliderViewModel, slider);
        slider.Image = fileName;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var slider = await _context.Sliders.FirstOrDefaultAsync(c => c.Id == id);
        if (slider is null)
            return NotFound();

        var deleteSliderViewModel = _mapper.Map<DeleteSliderViewModel>(slider);
        return View(deleteSliderViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteSlider(int id)
    {
        var slider = await _context.Sliders.FirstOrDefaultAsync(s => s.Id == id);

        if (slider is null)
            return NotFound();

        string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "slider", slider.Image);
        _fileService.DeleteFile(path);

        slider.IsDeleted = true;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Detail(int id)
    {
        Slider? slider = await _context.Sliders.FirstOrDefaultAsync(s => s.Id == id);
        if (slider is null)
            return NotFound();

        var sliderDetailViewModel = _mapper.Map<SliderDetailViewModel>(slider);
        return View(sliderDetailViewModel);
    }
}
