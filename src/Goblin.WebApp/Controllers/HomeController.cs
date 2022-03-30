﻿using System.Diagnostics;
using Goblin.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Goblin.WebApp.Controllers;

public class HomeController : Controller
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}