using Dapper;
using HelpDesk.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace HelpDesk.Controllers;

[Authorize]
public class CategoriaController : Controller
{

  private readonly IConfiguration _configuration;
  public CategoriaController(IConfiguration configuration)
  {
    this._configuration = configuration;
  }

  public async Task<ActionResult<List<CategoriaViewModel>>> Index()
  {
    var connectionString = _configuration.GetConnectionString("DefaultConnection");

    using var connection = new NpgsqlConnection(connectionString);
    connection.Open();

    var categorias = await connection.QueryAsync<CategoriaViewModel>("SELECT * FROM tb_categoria");

    return View(categorias.ToList());
  }
}