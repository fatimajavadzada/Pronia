using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Abstraction;
using Pronia.Contexts;
using System.Security.Claims;

namespace Pronia.Controllers;
[Authorize]

public class BasketController(AppDbContext _context, IBasketService _basketService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var basketItems = await _basketService.GetBasketItemsAsync();
        return View(basketItems);
    }

    public async Task<IActionResult> AddToBasket(int productId)
    {
        var isExistProduct = await _context.Products.AnyAsync(p => p.Id == productId);

        if (!isExistProduct)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var IsExistUser = await _context.Users.AnyAsync(u => u.Id == userId);

        if (!IsExistUser)
        {
            return BadRequest();
        }

        var existBasketItem = await _context.BasketItems.FirstOrDefaultAsync(b => b.ProductId == productId && b.AppUserId == userId);

        if (existBasketItem is { })
        {
            existBasketItem.Count++;
            _context.Update(existBasketItem);
        }
        else
        {
            BasketItem basketItem = new()
            {
                ProductId = productId,
                Count = 1,
                AppUserId = userId!
            };

            await _context.BasketItems.AddAsync(basketItem);
        }


        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Product added to basket successfully.";
        return RedirectToAction("Index", "Shop");

    }

    public async Task<IActionResult> RemoveFromBasket(int productId)
    {
        var isExistProduct = await _context.Products.AnyAsync(p => p.Id == productId);

        if (!isExistProduct)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var IsExistUser = await _context.Users.AnyAsync(u => u.Id == userId);

        if (!IsExistUser)
        {
            return BadRequest();
        }

        var existBasketItem = await _context.BasketItems.FirstOrDefaultAsync(b => b.ProductId == productId && b.AppUserId == userId);

        if (existBasketItem is null)
        {
            return NotFound();
        }

        _context.BasketItems.Remove(existBasketItem);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Product removed from basket successfully.";

        var returnUrl = Request.Headers["Referer"];

        if (!string.IsNullOrWhiteSpace(returnUrl))
        {
            return Redirect(returnUrl!);
        }

        return RedirectToAction("Index", "Shop");
    }

    public async Task<IActionResult> DecreaseBasketItemCount(int productId)
    {
        var isExistProduct = await _context.Products.AnyAsync(p => p.Id == productId);

        if (!isExistProduct)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var IsExistUser = await _context.Users.AnyAsync(u => u.Id == userId);

        if (!IsExistUser)
        {
            return BadRequest();
        }

        var existBasketItem = await _context.BasketItems.FirstOrDefaultAsync(b => b.ProductId == productId && b.AppUserId == userId);

        if (existBasketItem is null)
        {
            return NotFound();
        }

        if (existBasketItem.Count > 1)
        {
            existBasketItem.Count--;
        }

        _context.BasketItems.Update(existBasketItem);
        await _context.SaveChangesAsync();

        //var returnUrl = Request.Headers["Referer"];

        //if (!string.IsNullOrWhiteSpace(returnUrl))
        //{
        //    return Redirect(returnUrl!);
        //}

        //return RedirectToAction("Index");
        var basketItems = await _basketService.GetBasketItemsAsync();

        return PartialView("_BasketPartialView", basketItems);
    }

    public async Task<IActionResult> IncreaseBasketItemCount(int productId)
    {
        var isExistProduct = await _context.Products.AnyAsync(p => p.Id == productId);

        if (!isExistProduct)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var IsExistUser = await _context.Users.AnyAsync(u => u.Id == userId);

        if (!IsExistUser)
        {
            return BadRequest();
        }

        var existBasketItem = await _context.BasketItems.FirstOrDefaultAsync(b => b.ProductId == productId && b.AppUserId == userId);

        if (existBasketItem is null)
        {
            return NotFound();
        }

        existBasketItem.Count++;

        _context.BasketItems.Update(existBasketItem);
        await _context.SaveChangesAsync();

        var basketItems = await _basketService.GetBasketItemsAsync();

        return PartialView("_BasketPartialView", basketItems);
    }
}
