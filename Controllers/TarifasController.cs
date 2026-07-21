using Microsoft.AspNetCore.Mvc;

namespace ParkG.Controllers;

public class TarifasController : Controller
{
    [HttpGet("tarifas")]
    public IActionResult Index()
    {
        ViewData["ShellMode"] = "app";
        ViewData["NavKey"] = "tarifas";
        return View();
    }
}