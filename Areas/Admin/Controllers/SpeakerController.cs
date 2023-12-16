using AutoMapper;
using EduHome.Areas.Admin.ViewModels.CourseViewModel;
using EduHome.Areas.Admin.ViewModels.SpeakerViewModel;
using EduHome.Exceptions;
using EduHome.Models;
using EduHome.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduHome.Areas.Admin.Controllers;

[Area("Admin")]
public class SpeakerController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;

    public SpeakerController(AppDbContext context, IWebHostEnvironment webHostEnvironment, IFileService fileService, IMapper mapper)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        _fileService = fileService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 3)
    {
        var speakers = await _context.speakers.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        var count = await _context.Categories.CountAsync();
        List<AdminSpeakerViewModel> adminSpeakerViewModels = _mapper.Map<List<AdminSpeakerViewModel>>(speakers);
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)count / pageSize);
        ViewBag.PageSize = pageSize;
        return View(adminSpeakerViewModels);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSpeakerViewModel createSpeakerViewModel)
    {
        if (!ModelState.IsValid)
            return View();

        if (createSpeakerViewModel is null)
            return NotFound();

        var speaker = _mapper.Map<Speaker>(createSpeakerViewModel);

        try
        {
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "event");
            speaker.Image = await _fileService.CreateFileAsync(createSpeakerViewModel.Image, path, 100, "image/");
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


        await _context.speakers.AddAsync(speaker);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Update(int id)
    {
        var speaker = await _context.speakers.FirstOrDefaultAsync(s => s.Id == id);
        if (speaker is null)
            return NotFound();

        var updateSpeakerViewModel = _mapper.Map<UpdateSpeakerViewModel>(speaker);
        return View(updateSpeakerViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, UpdateSpeakerViewModel updateSpeakerViewModel)
    {
        if (!ModelState.IsValid)
            return View();

        var speaker = await _context.speakers.FirstOrDefaultAsync(s => s.Id == id);
        if (speaker is null)
            return NotFound();

        string fileName = speaker.Image;
        if (updateSpeakerViewModel.Image is not null)
        {
            try
            {
                string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "event");
                _fileService.DeleteFile(Path.Combine(path, speaker.Image));

                fileName = await _fileService.CreateFileAsync(updateSpeakerViewModel.Image, path, 100, "image/");
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
        _mapper.Map(updateSpeakerViewModel, speaker);
        speaker.Image = fileName;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var speaker = await _context.speakers.FirstOrDefaultAsync(s => s.Id == id);
        if (speaker is null)
            return NotFound();

        var deleteSpeakerViewModel = _mapper.Map<DeleteSpeakerViewModel>(speaker);
        return View(deleteSpeakerViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteSpeaker(int id)
    {
        var speaker = await _context.speakers.FirstOrDefaultAsync(s => s.Id == id);

        if (speaker is null)
            return NotFound();

        string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "event", speaker.Image);
        _fileService.DeleteFile(path);

        speaker.IsDeleted = true;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Detail(int id)
    {
        Speaker? speaker = await _context.speakers.FirstOrDefaultAsync(s => s.Id == id);
        if (speaker is null)
            return NotFound();

        var speakerDetailViewModel = _mapper.Map<SpeakerDetailViewModel>(speaker);
        return View(speakerDetailViewModel);
    }
}
