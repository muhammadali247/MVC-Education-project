using AutoMapper;
using EduHome.Models;
using EduHome.ViewModels.teacherViewModel;
using Microsoft.EntityFrameworkCore;

namespace EduHome.Controllers;

public class TeacherController : Controller
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public TeacherController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IActionResult> Index()
    {
        var teachers = await _context.teachers.Include(t => t.SocialMedias).ToListAsync();
        //ViewBag.SocialMedias = await _context.socialMedias.ToListAsync();

        List<TeacherViewModel> teacherAds = _mapper.Map<List<TeacherViewModel>>(teachers);

        return View(teacherAds);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            //ViewBag.teacherSkills = await _context.teachersSkills.Where(ts => ts.TeacherId == id).ToListAsync();
            Teacher? teacher = await _context.teachers.Include(t => t.teacherSkills).ThenInclude(ts => ts.Skill).Include(t => t.SocialMedias).FirstOrDefaultAsync(t => t.Id == id);
            var teacherSkill = _context.teachersSkills.Where(t=>t.TeacherId == id).ToList();
            if (teacher is null)
                return NotFound();
            var teacherDetailViewModel = _mapper.Map<TeacherDetailViewModel>(teacher);
            for (int i = 0; i < teacherSkill.Count(); i++)
            {
                teacherDetailViewModel.Percentage[i] = teacherSkill[i].Percentage;
            }
            return View(teacherDetailViewModel);
        }
        else
        {
            //ViewBag.teacherSkills = await _context.teachersSkills.Where(ts => ts.TeacherId == id).ToListAsync();
            Teacher? teacher = await _context.teachers.Include(t => t.teacherSkills).ThenInclude(ts => ts.Skill).Include(t => t.SocialMedias).FirstOrDefaultAsync(t => t.Id == id);
            var teacherSkill = _context.teachersSkills.Where(t=>t.TeacherId == id).ToList();
            if (teacher is null)
                return NotFound();
            var teacherDetailViewModel = _mapper.Map<TeacherDetailViewModel>(teacher);
            for (int i = 0; i < teacherSkill.Count(); i++)
            {
                teacherDetailViewModel.Percentage[i] = teacherSkill[i].Percentage;
            }
            return View(teacherDetailViewModel);
        }
    }
}
