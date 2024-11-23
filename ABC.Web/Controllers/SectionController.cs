using ABC.Web.Models;
using ABC.Web.Services.Implementations;
using ABC.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ABC.Web.Controllers
{
    public class SectionController : Controller
    {
        private readonly IContentService _contentService;
        public SectionController(IContentService contentService)
        {
            _contentService = contentService;
        }
        public IActionResult Index(string section)
        {
            section = "preface";
            var sectionObj = _contentService.GetSection(section);
            return View(sectionObj);
        }
        public IActionResult ToSection(string section)
        {
            var sectionObj = _contentService.GetSection(section);
            return View("index", sectionObj);
        }
    }
}
