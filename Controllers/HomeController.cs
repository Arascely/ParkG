using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ParkG.Models;

namespace ParkG.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        ViewData["ShellMode"] = "app";
        ViewData["NavKey"] = "dashboard";
        return View();
    }

    [Route("dashboard")]
    public IActionResult Dashboard()
    {
        ViewData["ShellMode"] = "app";
        ViewData["NavKey"] = "dashboard";
        return View("Index");
    }

    [Route("login")]
    [HttpGet]
    public IActionResult Login()
    {
        ViewData["ShellMode"] = "public";
        ViewData["NavKey"] = "login";
        return View();
    }

    [Route("register")]
    [HttpGet]
    public IActionResult Register()
    {
        ViewData["ShellMode"] = "public";
        ViewData["NavKey"] = "register";
        return View();
    }

    [Route("ingreso")]
    [HttpGet]
    public IActionResult Ingreso()
    {
        ViewData["ShellMode"] = "app";
        ViewData["NavKey"] = "ingreso";
        return View();
    }

    public IActionResult Privacy()
    {
        ViewData["ShellMode"] = "public";
        return View();
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        ViewData["ShellMode"] = "public";
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}