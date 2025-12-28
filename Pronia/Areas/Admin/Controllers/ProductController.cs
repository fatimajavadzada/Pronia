using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Contexts;
using Pronia.Helpers;
using Pronia.ViewModels.ProductViewModels;

namespace Pronia.Areas.Admin.Controllers;
[Area("Admin")]
public class ProductController(AppDbContext _context, IWebHostEnvironment _environment) : Controller
{
    public IActionResult Index()
    {
        var vms = _context.Products.Include(x => x.Category).Select(x => new ProductGetVM()
        {
            Id = x.Id,
            Name = x.Name,
            Price = x.Price,
            Description = x.Description,
            SKU = x.SKU,
            CategoryName = x.Category.Name,
            MainImageUrl = x.MainImagePath,
            HoverImageUrl = x.HoverImagePath
        }).ToList();
        return View(vms);
    }

    [HttpGet]
    public IActionResult Create()
    {
        SendCategoriesWithViewBag();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ProductCreateVM vm)
    {
        if (!ModelState.IsValid)
        {
            SendCategoriesWithViewBag();
            return View(vm);
        }

        if (!vm.MainImage.CheckType("image"))
        {
            ModelState.AddModelError("MainImage", "The format must be image!");
            return View(vm);
        }

        if (!vm.MainImage.CheckSize(2))
        {
            ModelState.AddModelError("MainImage", "Image size cannot be greater than 2 MB!");
            return View(vm);
        }

        if (!vm.HoverImage.CheckType("image"))
        {
            ModelState.AddModelError("HoverImage", "Image format is required!");
            return View(vm);
        }

        if (!vm.HoverImage.CheckSize(2))
        {
            ModelState.AddModelError("HoverImage", "Image size cannot be greater than 2 MB!");
            return View(vm);
        }

        var isExistCategory = _context.Categories.Any(x => x.Id == vm.CategoryId);

        if (!isExistCategory)
        {
            SendCategoriesWithViewBag();
            ModelState.AddModelError("", "Category is not found!");
            return View(vm);
        }

        string folderPath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images");

        string mainImageUniqueName = vm.MainImage.SaveFile(folderPath);
        string hoverImageUniqueName = vm.HoverImage.SaveFile(folderPath);

        Product product = new Product()
        {
            Name = vm.Name,
            CategoryId = vm.CategoryId,
            Description = vm.Description,
            Price = vm.Price,
            SKU = vm.SKU,
            MainImagePath = mainImageUniqueName,
            HoverImagePath = hoverImageUniqueName,
        };

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

        ProductUpdateVM vm = new ProductUpdateVM()
        {
            Id = product.Id,
            Name = product.Name,
            CategoryId = product.CategoryId,
            Description = product.Description,
            Price = product.Price,
            SKU = product.SKU,
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Update(ProductUpdateVM vm)
    {
        if (!ModelState.IsValid)
        {
            SendCategoriesWithViewBag();
            return View(vm);
        }

        var existProduct = _context.Products.Find(vm.Id);

        if (existProduct is null)
        {
            return NotFound();
        }

        var isExistCategory = _context.Categories.Any(x => x.Id == vm.CategoryId);

        if (!isExistCategory)
        {
            SendCategoriesWithViewBag();
            ModelState.AddModelError("CategoryId", "Category is not found!");
            return View(vm);
        }

        if (!vm.MainImage?.CheckType("image") ?? false)
        {
            ModelState.AddModelError("MainImage", "The format must be image!");
            return View(vm);
        }

        if (!vm.MainImage?.CheckSize(2) ?? false)
        {
            ModelState.AddModelError("MainImage", "Image size cannot be greater than 2 MB!");
            return View(vm);
        }

        if (!vm.HoverImage?.CheckType("image") ?? false)
        {
            ModelState.AddModelError("HoverImage", "Image format is required!");
            return View(vm);
        }

        if (!vm.HoverImage?.CheckSize(2) ?? false)
        {
            ModelState.AddModelError("HoverImage", "Image size cannot be greater than 2 MB!");
            return View(vm);
        }

        existProduct.Name = vm.Name;
        existProduct.Description = vm.Description;
        existProduct.Price = vm.Price;
        existProduct.SKU = vm.SKU;
        existProduct.CategoryId = vm.CategoryId;

        string folderPath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images");

        if (vm.MainImage is { })
        {
            string newMainImageName = vm.MainImage.SaveFile(folderPath);

            if (System.IO.File.Exists(Path.Combine(folderPath, existProduct.MainImagePath)))
            {
                System.IO.File.Delete(Path.Combine(folderPath, existProduct.MainImagePath));
            }

            existProduct.MainImagePath = newMainImageName;
        }

        if (vm.HoverImage is { })
        {
            string newHoverImageName = vm.HoverImage.SaveFile(folderPath);

            if (System.IO.File.Exists(Path.Combine(folderPath, existProduct.HoverImagePath)))
            {
                System.IO.File.Delete(Path.Combine(folderPath, existProduct.HoverImagePath));
            }

            existProduct.HoverImagePath = newHoverImageName;
        }

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

        string folderPath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images");

        if (System.IO.File.Exists(Path.Combine(folderPath, product.MainImagePath)))
        {
            System.IO.File.Delete(Path.Combine(folderPath, product.MainImagePath));
        }

        if (System.IO.File.Exists(Path.Combine(folderPath, product.HoverImagePath)))
        {
            System.IO.File.Delete(Path.Combine(folderPath, product.HoverImagePath));
        }

        return RedirectToAction(nameof(Index));
    }

    private void SendCategoriesWithViewBag()
    {
        var categories = _context.Categories.ToList();
        ViewBag.Categories = categories;
    }
}
