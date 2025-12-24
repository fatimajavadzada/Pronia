using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Contexts;

namespace Pronia.Areas.Admin.Controllers;
[Area("Admin")]
public class ProductController(AppDbContext _context) : Controller
{
    public IActionResult Index()
    {
        var products = _context.Products.Include(x => x.Category).ToList();
        return View(products);
    }

    [HttpGet]
    public IActionResult Create()
    {
        SendCategoriesWithViewBag();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Product product)
    {
        if (!ModelState.IsValid)
        {
            return View(product);
        }

        var isExistCategory = _context.Categories.Any(x => x.Id == product.CategoryId);

        if (!isExistCategory)
        {
            SendCategoriesWithViewBag();
            ModelState.AddModelError("", "Category is not found!");
            return View(product);
        }

        _context.Products.Add(product);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Update(int id)
    {
        SendCategoriesWithViewBag();

        var product = _context.Products.Find(id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Update(Product product)
    {
        if (!ModelState.IsValid)
        {
            SendCategoriesWithViewBag();
            return View(product);
        }

        var existProduct = _context.Products.Find(product.Id);

        if (existProduct is null)
        {
            return NotFound();
        }

        var isExistCategory = _context.Categories.Any(x => x.Id == product.CategoryId);

        if (!isExistCategory)
        {
            SendCategoriesWithViewBag();
            ModelState.AddModelError("CategoryId", "Category is not found!");
            return View(product);
        }

        existProduct.Name = product.Name;
        existProduct.Description = product.Description;
        existProduct.Price = product.Price;
        existProduct.SKU = product.SKU;
        existProduct.MainImagePath = product.MainImagePath;
        existProduct.HoverImagePath = product.HoverImagePath;
        existProduct.CategoryId = product.CategoryId;

        _context.Products.Update(existProduct);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        var product = _context.Products.Find(id);

        if (product is null)
        {
            return NotFound();
        }

        _context.Products.Remove(product);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    private void SendCategoriesWithViewBag()
    {
        var categories = _context.Categories.ToList();
        ViewBag.Categories = categories;
    }
}
