using Microsoft.AspNetCore.Mvc;
using Pronia.Contexts;

namespace Pronia.Areas.Admin.Controllers;
[Area("Admin")]

public class ServiceController(AppDbContext _context) : Controller
{
    public IActionResult Index()
    {
        var services = _context.Services.ToList();
        return View(services);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Service service)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        _context.Services.Add(service);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    public ActionResult Delete(int id)
    {
        var service = _context.Services.Find(id);
        if (service is null)
        {
            return NotFound();
        }

        _context.Services.Remove(service);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }


    public IActionResult Update(int id)
    {
        var service = _context.Services.Find(id);
        if (service is null)
        {
            return NotFound();
        }

        return View(service);
    }

    [HttpPost]
    public IActionResult Update(Service service)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        var existService = _context.Services.Find(service.Id);
        if (existService is null)
        {
            return NotFound();
        }

        existService.Title = service.Title;
        existService.Description = service.Description;
        existService.ImageUrl = service.ImageUrl;

        _context.Services.Update(existService);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }
}
