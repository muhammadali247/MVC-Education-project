using AutoMapper;
using EduHome.Models;
using EduHome.ViewModels.BlogViewModel;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace EduHome.Controllers;

public class BlogController : Controller
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public BlogController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        var blogs = await _context.Blogs.ToListAsync();

        List<BlogAdsViewModel> blogAds = _mapper.Map<List<BlogAdsViewModel>>(blogs);

        return View(blogAds);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            Blog? blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id >= 1);
            return View(blog);
        }
        else
        {
            Blog? blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);
            return View(blog);
        }
    }
}