using Microsoft.AspNetCore.Mvc;

namespace SistemaConsultasUVV.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
    public IActionResult Erro() => View();
}
