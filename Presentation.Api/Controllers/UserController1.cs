using Microsoft.AspNetCore.Mvc;

namespace Presentation.Api.Controllers;

public class UserController1 : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}