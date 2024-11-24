using ABC.Web.Models;
using ABC.Web.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace ABC.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string _wwwrootPath;
        private readonly IMapper _mapper;
        private readonly IContentService _contentService;

        public HomeController(IWebHostEnvironment env, ILogger<HomeController> logger, IMapper mapper, IContentService contentService)
        {
            _logger = logger;
            _wwwrootPath = env.WebRootPath;
            _mapper = mapper;
            _contentService = contentService;
        }

        public IActionResult Index()
        {
            var preface = _mapper.Map<Section>(_contentService.GetPreface());
            if (_contentService.IsLinkingComplete())
            {
                return RedirectToAction("Index", "Section", "preface");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        private void TraverseJson(JToken token)
        {
            if (token is JObject obj)
            {
                Console.WriteLine(obj.Properties().Any(p => string.Equals(p.Name, "PREFACE", StringComparison.OrdinalIgnoreCase)));
                if (obj.TryGetValue("bohemia-chapter-1", out JToken property))
                {
                var p = _mapper.Map<Preface>(property.ToObject<Section>());
                    Console.WriteLine(property);
                }
            }
        }
    }
}
