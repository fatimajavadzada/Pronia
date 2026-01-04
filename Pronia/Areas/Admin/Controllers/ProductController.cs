using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Contexts;
using Pronia.Helpers;
using Pronia.Models;
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
            Rating = x.Rating,
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
        SendItemsWithViewBag();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ProductCreateVM vm)
    {
        if (!ModelState.IsValid)
        {
            SendItemsWithViewBag();
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

        foreach (var image in vm.Images)
        {
            if (!image.CheckType("image"))
            {
                ModelState.AddModelError("Images", "Image format is required!");
                return View(vm);
            }

            if (!image.CheckSize(2))
            {
                ModelState.AddModelError("Images", "Image size cannot be greater than 2 MB!");
                return View(vm);
            }
        }

        var isExistCategory = _context.Categories.Any(x => x.Id == vm.CategoryId);

        if (!isExistCategory)
        {
            SendItemsWithViewBag();
            ModelState.AddModelError("", "Category is not found!");
            return View(vm);
        }

        foreach (var tagId in vm.TagIds)
        {
            var isExistTag = _context.Tags.Any(x => x.Id == tagId);

            if (!isExistTag)
            {
                SendItemsWithViewBag();
                ModelState.AddModelError("", "Tag is not found!");
                return View(vm);
            }
        }

        foreach (var brandId in vm.BrandIds)
        {
            var isExistBrand = _context.Brands.Any(x => x.Id == brandId);

            if (!isExistBrand)
            {
                SendItemsWithViewBag();
                ModelState.AddModelError("", "Brand is not found!");
                return View(vm);
            }
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
            Rating = vm.Rating,
            MainImagePath = mainImageUniqueName,
            HoverImagePath = hoverImageUniqueName,
            ProductTags = [],
            ProductBrands = [],
            ProductImages = []
        };

        foreach (var image in vm.Images)
        {
            string imageUniqueName = image.SaveFile(folderPath);
            ProductImage productImage = new ProductImage()
            {
                ImageUrl = imageUniqueName,
                Product = product
            };
            product.ProductImages.Add(productImage);
        }

        foreach (var tagId in vm.TagIds)
        {
            ProductTag productTag = new ProductTag()
            {
                TagId = tagId,
                Product = product
            };
            product.ProductTags.Add(productTag);
        }

        foreach (var brandId in vm.BrandIds)
        {
            ProductBrand productBrand = new ProductBrand()
            {
                BrandId = brandId,
                Product = product
            };
            product.ProductBrands.Add(productBrand);
        }

        _context.Products.Add(product);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Update(int id)
    {
        SendItemsWithViewBag();

        var product = _context.Products.Include(x => x.ProductTags).Include(x => x.ProductBrands).Include(x => x.ProductImages).FirstOrDefault(x => x.Id == id);

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
            Rating = product.Rating,
            SKU = product.SKU,
            TagIds = product.ProductTags.Select(x => x.TagId).ToList(),
            BrandIds = product.ProductBrands.Select(x => x.BrandId).ToList(),
            MainImagePath = product.MainImagePath,
            HoverImagePath = product.HoverImagePath,
            ImageUrls = product.ProductImages.Select(x => x.ImageUrl).ToList(),
            ImageIds = product.ProductImages.Select(x => x.Id).ToList(),
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Update(ProductUpdateVM vm)
    {
        if (!ModelState.IsValid)
        {
            SendItemsWithViewBag();
            return View(vm);
        }

        var existProduct = _context.Products.Include(x => x.ProductTags).Include(x => x.ProductBrands).Include(x => x.ProductImages).FirstOrDefault(x => x.Id == vm.Id);

        if (existProduct is null)
        {
            return NotFound();
        }

        var isExistCategory = _context.Categories.Any(x => x.Id == vm.CategoryId);

        if (!isExistCategory)
        {
            SendItemsWithViewBag();
            ModelState.AddModelError("CategoryId", "Category is not found!");
            return View(vm);
        }

        foreach (var tagId in vm.TagIds)
        {
            var isExistTag = _context.Tags.Any(x => x.Id == tagId);

            if (!isExistTag)
            {
                SendItemsWithViewBag();
                ModelState.AddModelError("", "Tag is not found!");
                return View(vm);
            }
        }

        foreach (var brandId in vm.BrandIds)
        {
            var isExistBrand = _context.Brands.Any(x => x.Id == brandId);

            if (!isExistBrand)
            {
                SendItemsWithViewBag();
                ModelState.AddModelError("", "Brand is not found!");
                return View(vm);
            }
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

        foreach (var image in vm.Images)
        {
            if (!image.CheckType("image"))
            {
                ModelState.AddModelError("Images", "Image format is required!");
                return View(vm);
            }

            if (!image.CheckSize(2))
            {
                ModelState.AddModelError("Images", "Image size cannot be greater than 2 MB!");
                return View(vm);
            }
        }

        existProduct.Name = vm.Name;
        existProduct.Description = vm.Description;
        existProduct.Price = vm.Price;
        existProduct.SKU = vm.SKU;
        existProduct.Rating = vm.Rating;
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

        existProduct.ProductTags = [];
        existProduct.ProductBrands = [];

        foreach (var tagId in vm.TagIds)
        {
            ProductTag productTag = new ProductTag()
            {
                TagId = tagId,
                ProductId = existProduct.Id
            };
            existProduct.ProductTags.Add(productTag);
        }

        foreach (var brandId in vm.BrandIds)
        {
            ProductBrand productBrand = new ProductBrand()
            {
                BrandId = brandId,
                ProductId = existProduct.Id
            };
            existProduct.ProductBrands.Add(productBrand);
        }

        foreach (var productImage in existProduct.ProductImages.ToList())
        {
            var isExistImage = vm.ImageIds.Any(x => x == productImage.Id);

            if (isExistImage == false)
            {
                existProduct.ProductImages.Remove(productImage);

                if (System.IO.File.Exists(Path.Combine(folderPath, productImage.ImageUrl)))
                {
                    System.IO.File.Delete(Path.Combine(folderPath, productImage.ImageUrl));
                }
            }
        }

        foreach (var image in vm.Images)
        {
            string imageUniqueName = image.SaveFile(folderPath);
            ProductImage productImage = new ProductImage()
            {
                ImageUrl = imageUniqueName,
                ProductId = existProduct.Id
            };
            existProduct.ProductImages.Add(productImage);
        }

        _context.Products.Update(existProduct);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        var product = _context.Products.Include(x => x.ProductImages).FirstOrDefault(x => x.Id == id);

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

        foreach (var productImage in product.ProductImages)
        {
            if (System.IO.File.Exists(Path.Combine(folderPath, productImage.ImageUrl)))
            {
                System.IO.File.Delete(Path.Combine(folderPath, productImage.ImageUrl));
            }
        }

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Detail(int id)
    {
        var product = _context.Products.Select(x => new ProductGetVM()
        {
            Id = x.Id,
            CategoryName = x.Category.Name,
            Description = x.Description,
            Name = x.Name,
            HoverImageUrl = x.HoverImagePath,
            MainImageUrl = x.MainImagePath,
            Price = x.Price,
            SKU = x.SKU,
            Rating = x.Rating,
            TagNames = x.ProductTags.Select(x => x.Tag.Name).ToList(),
            BrandNames = x.ProductBrands.Select(x => x.Brand.Name).ToList(),
            ImageUrls = x.ProductImages.Select(x => x.ImageUrl).ToList(),
        }).FirstOrDefault(x => x.Id == id);

        if (product is null)
        {
            return NotFound();
        }

        return View(product);
    }

    private void SendItemsWithViewBag()
    {
        var categories = _context.Categories.ToList();
        ViewBag.Categories = categories;

        var tags = _context.Tags.ToList();
        ViewBag.Tags = tags;

        var brands = _context.Brands.ToList();
        ViewBag.Brands = brands;
    }
}
