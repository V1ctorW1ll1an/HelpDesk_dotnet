using Dapper;
using HelpDesk.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace HelpDesk.Controllers;

public class ChamadoController : Controller
{

  public readonly IConfiguration _configuration;

  public ChamadoController(IConfiguration configuration)
  {
    this._configuration = configuration;
  }

  public async Task<ActionResult<List<ChamadoViewModel>>> Index()
  {
    var connectionString = _configuration.GetConnectionString("DefaultConnection");

    var sql = @"SELECT * FROM tb_chamado JOIN tb_categoria ON tb_chamado.id_categoria = tb_categoria.id_categoria JOIN tb_usuario ON tb_chamado.id_usuario = tb_usuario.id_usuario";

    using var connection = new NpgsqlConnection(connectionString);
    connection.Open();

    var chamados = await connection.QueryAsync<ChamadoViewModel, CategoriaViewModel, UsuarioViewModel, ChamadoViewModel>(
      sql, (chamado, categoria, usuario) =>
      {
        chamado.categoria = categoria;
        chamado.usuario = usuario;

        return chamado;
      },
      splitOn: "id_usuario"
    );

    return View(chamados.ToList());
  }

}