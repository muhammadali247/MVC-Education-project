using AutoMapper;
using EduHome.Areas.Admin.ViewModels.BlogViewModel;
using EduHome.Exceptions;
using EduHome.Models;
using EduHome.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduHome.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,Moderator")]
public class BlogController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;

    public BlogController(AppDbContext context, IWebHostEnvironment webHostEnvironment, IFileService fileService, IMapper mapper)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        _fileService = fileService;
        _mapper = mapper;
    }


    public async Task<IActionResult> Index(int page = 1, int pageSize = 3)
    {
        var blogs = await _context.Blogs.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        var count = await _context.Blogs.CountAsync();
        List<BlogViewModel> blogViewModels = _mapper.Map<List<BlogViewModel>>(blogs);
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)count / pageSize);
        ViewBag.PageSize = pageSize;
        return View(blogViewModels);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBlogViewModel createBlogViewModel)
    {
        if (!ModelState.IsValid)
            return View();

        var blog = _mapper.Map<Blog>(createBlogViewModel);

        try
        {
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "blog");
            blog.Image = await _fileService.CreateFileAsync(createBlogViewModel.Image, path, 100, "image/");
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
        await _context.Blogs.AddAsync(blog);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Update(int id)
    {
        var blog = await _context.Blogs.FirstOrDefaultAsync(blog => blog.Id == id);
        if (blog is null)
            return NotFound();

        var updateBlogViewModel = _mapper.Map<UpdateBlogViewModel>(blog);

        return View(updateBlogViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id,UpdateBlogViewModel updateBlogViewModel)
    {
        if (!ModelState.IsValid)
            return View();

        var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);
        if (blog is null) 
            return NotFound();

        string fileName = blog.Image;
        if (updateBlogViewModel.Image is not null)
        {
            try
            {
                string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "blog");
                _fileService.DeleteFile(Path.Combine(path, blog.Image));

                fileName = await _fileService.CreateFileAsync(updateBlogViewModel.Image, path, 100, "image/");
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
        _mapper.Map(updateBlogViewModel, blog);
        blog.Image = fileName;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);

        if (blog is null) 
            return NotFound();

        var deleteBlogViewModel = _mapper.Map<DeleteBlogViewModel>(blog);
        return View(deleteBlogViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteBlog(int id)
    {
        var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);

        if (blog is null) 
            return NotFound();

        string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "blog", blog.Image);
        _fileService.DeleteFile(path);

        blog.IsDeleted = true;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Detail(int id)
    {
        Blog? blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);
        if (blog is null)
            return NotFound();

        return View(blog);
    }
}
