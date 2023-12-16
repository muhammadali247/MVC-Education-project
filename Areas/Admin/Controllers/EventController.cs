using AutoMapper;
using EduHome.Areas.Admin.ViewModels.BlogViewModel;
using EduHome.Areas.Admin.ViewModels.EventViewModel;
using EduHome.Exceptions;
using EduHome.Models;
using EduHome.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using System.Reflection.Metadata;

namespace EduHome.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles ="Admin,Moderator")]
public class EventController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IFileService _fileService;
    private readonly IEmailSender _emailSender;
    private readonly IMapper _mapper;

    public EventController(AppDbContext context, IWebHostEnvironment webHostEnvironment, IFileService fileService, IMapper mapper, IEmailSender emailSender)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        _fileService = fileService;
        _mapper = mapper;
        _emailSender = emailSender;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 3)
    {
        var events = await _context.Events.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        List<EventViewModel> eventViewModels = _mapper.Map<List<EventViewModel>>(events);
        var count = await _context.Events.CountAsync();
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)count / pageSize);
        ViewBag.PageSize = pageSize;
        return View(eventViewModels);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Speakers = new SelectList(await _context.speakers.ToListAsync(), "Id", "Name");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateEventViewModel createEventViewModel)
    {
        ViewBag.Speakers = new SelectList(await _context.speakers.ToListAsync(), "Id", "Name");

        if (!ModelState.IsValid)
            return View();

        var eventCard = _mapper.Map<Event>(createEventViewModel);

        try
        {
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "event");
            eventCard.Image = await _fileService.CreateFileAsync(createEventViewModel.Image, path, 10000, "image/");
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

        List<EventSpeaker> evSpeakers = new List<EventSpeaker>();
        for (int i = 0; i < createEventViewModel.SpeakerId.Count; i++)
        {
            EventSpeaker eventSpeaker = new EventSpeaker()
            {
                SpeakerId = createEventViewModel.SpeakerId[i],
                EventId = eventCard.Id,
            };

            evSpeakers.Add(eventSpeaker);
        }
        eventCard.EventSpeakers = evSpeakers;


        await _context.Events.AddAsync(eventCard);
        await _context.SaveChangesAsync();

        var activeSubs  = await _context.Subscribers.Where(subscriber => subscriber.IsActive).ToListAsync();

        foreach (var sub in activeSubs)
        {
            _emailSender.SendEmail(sub.Email, "New Event Notification", "A new event has been created!");
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Update(int id)
    {
        var EventCard = await _context.Events.FirstOrDefaultAsync(EventCard => EventCard.Id == id);
        if (EventCard is null)
            return NotFound();

        var updateEventViewModel = _mapper.Map<UpdateEventViewModel>(EventCard);

        return View(updateEventViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id,UpdateEventViewModel updateEventViewModel)
    {
        if (!ModelState.IsValid)
            return View();

        var EventCard = await _context.Events.FirstOrDefaultAsync(ec => ec.Id == id);
        if (EventCard is null)
            return NotFound();

        string fileName = EventCard.Image;
        if (updateEventViewModel.Image is not null)
        {
            try
            {
                string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "event");
                _fileService.DeleteFile(Path.Combine(path, EventCard.Image));

                fileName = await _fileService.CreateFileAsync(updateEventViewModel.Image, path, 100, "image/");
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
        _mapper.Map(updateEventViewModel, EventCard);
        EventCard.Image = fileName;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var EventCard = await _context.Events.FirstOrDefaultAsync(ec => ec.Id == id);

        if (EventCard is null)
            return NotFound();

        var deleteEventViewModel = _mapper.Map<DeleteEventViewModel>(EventCard);
        return View(deleteEventViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var EventCard = await _context.Events.FirstOrDefaultAsync(ec => ec.Id == id);

        if (EventCard is null)
            return NotFound();

        string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "event", EventCard.Image);
        _fileService.DeleteFile(path);

        EventCard.IsDeleted = true;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Detail(int id)
    {
        Event? EventCard = await _context.Events.FirstOrDefaultAsync(ec => ec.Id == id);
        if (EventCard is null)
            return NotFound();

        return View(EventCard);
    }
}
