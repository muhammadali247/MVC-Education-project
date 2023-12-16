using AutoMapper;
using EduHome.Models;
using EduHome.ViewModels.EventViewModel;
using Microsoft.EntityFrameworkCore;

namespace EduHome.Controllers;

public class EventController : Controller
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public EventController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        var events = await _context.Events.ToListAsync();

        List<EventAdsViewModel> eventAds = _mapper.Map<List<EventAdsViewModel>>(events);

        return View(eventAds);
    }


    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            Event? EventCard = await _context.Events.Include(e => e.EventSpeakers).ThenInclude(es => es.Speaker).FirstOrDefaultAsync();
            if (EventCard is null)
                return NotFound();
            var eventDetailViewModel = _mapper.Map<EventDetailViewModel>(EventCard);
            return View(eventDetailViewModel);
        }
        else
        {
            Event? EventCard = await _context.Events.Include(e => e.EventSpeakers).ThenInclude(es => es.Speaker).FirstOrDefaultAsync(b => b.Id == id);
            if (EventCard is null)
                return NotFound();
            var eventDetailViewModel = _mapper.Map<EventDetailViewModel>(EventCard);
            return View(eventDetailViewModel);
        }
    }
}
