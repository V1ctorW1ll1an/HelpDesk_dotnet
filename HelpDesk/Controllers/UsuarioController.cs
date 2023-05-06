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

    var query = @"SELECT tb_usuario.*, 
                tb_perfil.* FROM tb_usuario JOIN tb_perfil ON tb_usuario.id_perfil = tb_perfil.id_perfil";

    var users = await connection.QueryAsync<UsuarioViewModel, PerfilViewModel, UsuarioViewModel>(query,
     (usuario, perfil) =>
     {
       usuario.perfil = perfil;
       return usuario;
     }, splitOn: "id_perfil");

    return View(users.ToList());
  }

  public async Task<ActionResult<PerfilViewModel>> CadastrarAsync()
  {

    var connectionString = _configuration.GetConnectionString("DefaultConnection");

    using var connection = new NpgsqlConnection(connectionString);

    connection.Open();

    var query = @"SELECT * FROM tb_perfil";
    var perfis = await connection.QueryAsync<PerfilViewModel>(query);

    return View(perfis);
  }


  [HttpPost]
  public async Task<ActionResult<UsuarioViewModel>> CadastrarAsync([FromForm] UsuarioViewModel usuarioForm, [FromForm] int perfil)
  {

    if (!ModelState.IsValid)
      return View(usuarioForm);

    var connectionString = _configuration.GetConnectionString("DefaultConnection");

    using var connection = new NpgsqlConnection(connectionString);

    connection.Open();

    var query = @"INSERT INTO tb_usuario (
      nome_usuario,email_usuario,senha_usuario,id_perfil) VALUES (
        @Nome, @Email, @Senha, @IdPerfil);
                SELECT LASTVAL();";

    var id = await connection.ExecuteScalarAsync<int>(query, new
    {
      Nome = usuarioForm.nome_usuario,
      Email = usuarioForm.email_usuario,
      Senha = usuarioForm.senha_usuario,
      IdPerfil = perfil
    });

    usuarioForm.id_usuario = id.ToString();

    if (string.IsNullOrEmpty(usuarioForm.id_usuario))
      TempData["errorMsg"] = "Falha ao cadastrar usuario.";
    else
      TempData["successMsg"] = "Usuario cadastrado com sucesso";

    return RedirectToAction("Index");
  }

}