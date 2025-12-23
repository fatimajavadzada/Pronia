using Microsoft.AspNetCore.Mvc;
using Pronia.Contexts;

namespace Pronia.Areas.Admin.Controllers;
[Area("Admin")]
public class SliderController(AppDbContext _context) : Controller
{
    /* private readonly AppDbContext _context;

     public SliderController(AppDbContext context)
     {
         _context = context;
     }*/
    public IActionResult Index()
    {
        var sliders = _context.Sliders.ToList();
        return View(sliders);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Slider slider)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        /* if (slider.DiscountPercentage < 0 || slider.DiscountPercentage > 100)
         {
             ModelState.AddModelError("DiscountPercentage", "Discount Percentage must be between 0-100");
             return View();
         }*/

        _context.Sliders.Add(slider);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        var slider = _context.Sliders.Find(id);
        if (slider is null)
        {
            return NotFound();
        }

        _context.Sliders.Remove(slider);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Update(int id)
    {
        var slider = _context.Sliders.Find(id);
        if(slider is null)
        {
            return NotFound();
        }
        return View(slider);
    }

    [HttpPost]
    public IActionResult Update(Slider slider)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        var existSlider = _context.Sliders.Find(slider.Id);
        if(existSlider is null)
        {
            return NotFound();
        }

        existSlider.Title = slider.Title;
        existSlider.Description = slider.Description;
        existSlider.ImageUrl = slider.ImageUrl;
        existSlider.DiscountPercentage = slider.DiscountPercentage;

        _context.Sliders.Update(existSlider);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
}
