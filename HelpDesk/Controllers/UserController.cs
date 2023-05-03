using Dapper;
using HelpDesk.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace HelpDesk.Controllers;
public class UserController : Controller
{
  private readonly IConfiguration _configuration;
  public UserController(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  // GET default controller /user/
  public async Task<ActionResult<List<UserViewModel>>> Index()
  {
    var connectionString = _configuration.GetConnectionString("DefaultConnection");

    using var connection = new NpgsqlConnection(connectionString);
    connection.Open();

    var users = await connection.QueryAsync<UserViewModel>("SELECT * FROM tb_usuario");

    return View(users.ToList());

  }
}