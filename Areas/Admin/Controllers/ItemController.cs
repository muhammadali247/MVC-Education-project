using System;
using System.Collections.Generic;
using System.Linq;
using EduHome.Models;
using Microsoft.AspNetCore.Mvc;

[Area("Admin")]
public class ItemController : Controller
{
    private readonly List<Item> _items; // Replace with your data source

    public ItemController()
    {
        // Initialize the list of items (replace with your data retrieval logic)
        _items = new List<Item>();
        for (int i = 1; i <= 100; i++)
        {
            _items.Add(new Item { Id = i, Name = $"Item {i}" });
        }
    }

    public IActionResult Index(int page = 1, int pageSize = 10)
    {
        var paginatedItems = _items.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)_items.Count / pageSize);
        ViewBag.PageSize = pageSize;

        return View(paginatedItems);
    }
}
