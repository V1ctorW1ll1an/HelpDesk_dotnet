using Dapper;
using HelpDesk.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace HelpDesk.Controllers;
public class UsuarioController : Controller
{
  private readonly IConfiguration _configuration;
  public UsuarioController(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public async Task<ActionResult<List<UsuarioViewModel>>> Index()
  {
    var connectionString = _configuration.GetConnectionString("DefaultConnection");

    using var connection = new NpgsqlConnection(connectionString);
    connection.Open();

    var users = await connection.QueryAsync<UsuarioViewModel>("SELECT * FROM tb_usuario");

    return View(users.ToList());
  }

  public IActionResult Cadastrar()
  {
    return View();
  }
}