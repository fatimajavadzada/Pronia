using Microsoft.AspNetCore.Mvc;
using Pronia.Contexts;
using Pronia.ViewModels;

namespace Pronia.Controllers
{
    public class HomeController : Controller
    {
        private AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var sliders = _context.Sliders.ToList();
            var services = _context.Services.ToList();

            HomeViewModel vm = new HomeViewModel()
            {
                Sliders = sliders,
                Services = services
            };

            return View(vm);
        }
    }
}
