using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaConsultasUVV.Data;
using SistemaConsultasUVV.Models;
using SistemaConsultasUVV.ViewModels;

namespace SistemaConsultasUVV.Controllers;

[Authorize]
public class ConsultasController(AppDbContext db) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var consultas = await db.Consultas.AsNoTracking()
            .Where(c => c.UsuarioId == UsuarioId)
            .OrderBy(c => c.DataHora)
            .ToListAsync();
        return View(consultas);
    }

    [HttpGet]
    public IActionResult Criar() => View(new ConsultaViewModel { DataHora = DateTime.Now.AddDays(1) });

    [HttpPost]
    public async Task<IActionResult> Criar(ConsultaViewModel model)
    {
        ValidarData(model.DataHora);
        if (!ModelState.IsValid) return View(model);

        db.Consultas.Add(new Consulta
        {
            Especialidade = model.Especialidade.Trim(),
            DataHora = model.DataHora,
            Descricao = model.Descricao.Trim(),
            UsuarioId = UsuarioId
        });
        await db.SaveChangesAsync();
        TempData["Mensagem"] = "Consulta cadastrada.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var consulta = await BuscarConsultaAsync(id);
        if (consulta is null) return NotFound();
        return View(new ConsultaViewModel
        {
            Id = consulta.Id,
            Especialidade = consulta.Especialidade,
            DataHora = consulta.DataHora,
            Descricao = consulta.Descricao
        });
    }

    [HttpPost]
    public async Task<IActionResult> Editar(int id, ConsultaViewModel model)
    {
        if (id != model.Id) return BadRequest();
        var consulta = await BuscarConsultaAsync(id);
        if (consulta is null) return NotFound();
        ValidarData(model.DataHora);
        if (!ModelState.IsValid) return View(model);

        consulta.Especialidade = model.Especialidade.Trim();
        consulta.DataHora = model.DataHora;
        consulta.Descricao = model.Descricao.Trim();
        await db.SaveChangesAsync();
        TempData["Mensagem"] = "Consulta atualizada.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Excluir(int id)
    {
        var consulta = await BuscarConsultaAsync(id);
        if (consulta is null) return NotFound();
        return View(consulta);
    }

    [HttpPost, ActionName("Excluir")]
    public async Task<IActionResult> ConfirmarExclusao(int id)
    {
        var consulta = await BuscarConsultaAsync(id);
        if (consulta is null) return NotFound();
        db.Consultas.Remove(consulta);
        await db.SaveChangesAsync();
        TempData["Mensagem"] = "Consulta excluída.";
        return RedirectToAction(nameof(Index));
    }

    private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private Task<Consulta?> BuscarConsultaAsync(int id) =>
        db.Consultas.SingleOrDefaultAsync(c => c.Id == id && c.UsuarioId == UsuarioId);

    private void ValidarData(DateTime dataHora)
    {
        if (dataHora <= DateTime.Now)
            ModelState.AddModelError(nameof(ConsultaViewModel.DataHora), "Escolha uma data e hora futuras.");
    }
}
