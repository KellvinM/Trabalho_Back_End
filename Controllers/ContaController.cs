using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaConsultasUVV.Data;
using SistemaConsultasUVV.Models;
using SistemaConsultasUVV.ViewModels;

namespace SistemaConsultasUVV.Controllers;

public class ContaController(AppDbContext db, IPasswordHasher<Usuario> hasher) : Controller
{
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Cadastro() => View(new CadastroViewModel());

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Cadastro(CadastroViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var email = model.Email.Trim();
        var emailNormalizado = email.ToUpperInvariant();
        if (await db.Usuarios.AnyAsync(u => u.EmailNormalizado == emailNormalizado))
        {
            ModelState.AddModelError(nameof(model.Email), "Este e-mail já está cadastrado.");
            return View(model);
        }

        var usuario = new Usuario
        {
            Nome = model.Nome.Trim(),
            Email = email,
            EmailNormalizado = emailNormalizado,
            DataCadastroUtc = DateTime.UtcNow
        };
        usuario.SenhaHash = hasher.HashPassword(usuario, model.Senha);
        db.Usuarios.Add(usuario);
        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            if (await db.Usuarios.AnyAsync(u => u.EmailNormalizado == emailNormalizado))
            {
                ModelState.AddModelError(nameof(model.Email), "Este e-mail já está cadastrado.");
                return View(model);
            }
            throw;
        }

        await EntrarAsync(usuario);
        return RedirectToAction("Index", "Consultas");
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Consultas");
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var emailNormalizado = model.Email.Trim().ToUpperInvariant();
        var usuario = await db.Usuarios.SingleOrDefaultAsync(u => u.EmailNormalizado == emailNormalizado);
        if (usuario is null || hasher.VerifyHashedPassword(usuario, usuario.SenhaHash, model.Senha) == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "E-mail ou senha incorretos.");
            return View(model);
        }

        await EntrarAsync(usuario);
        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return LocalRedirect(model.ReturnUrl);
        return RedirectToAction("Index", "Consultas");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Sair()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AcessoNegado() => View();

    private async Task EntrarAsync(Usuario usuario)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email)
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }
}
