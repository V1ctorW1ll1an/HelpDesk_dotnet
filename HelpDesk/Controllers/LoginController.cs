using System.Security.Claims;
using Dapper;
using HelpDesk.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace HelpDesk.Controllers;


public class LoginController : Controller
{
  private readonly IConfiguration _configuration;
  private readonly object _logger;

  public LoginController(IConfiguration configuration, ILogger<LoginController> logger)
  {
    _configuration = configuration;
    _logger = logger;
  }
  public IActionResult Index()
  {
    if (!User.Identity.IsAuthenticated)
      return View();

    return RedirectToAction("Index", "Home");
  }

  [AllowAnonymous]
  [HttpPost]
  public async Task<IActionResult> LoginAsync([Bind("email_usuario", "senha_usuario")] UsuarioViewModel model)
  {
    if (!ModelState.IsValid)
    {
      return View("Index", model);
    }

    var connectionString = _configuration.GetConnectionString("DefaultConnection");

    using var connection = new NpgsqlConnection(connectionString);
    connection.Open();

    var user = await connection.QueryFirstOrDefaultAsync<UsuarioViewModel>("SELECT * FROM tb_usuario WHERE email_usuario = @Email AND senha_usuario = @Password", new { Email = model.email_usuario, Password = model.senha_usuario });

    if (user == null)
    {
      ModelState.AddModelError(string.Empty, "Usuário ou senha inválidos.");
      return View("Index", model);
    }

    var claims = new List<Claim>
    {
      new Claim(ClaimTypes.Name, user.nome_usuario),
      new Claim(ClaimTypes.Email, user.email_usuario),
    };

    var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

    var authProperties = new AuthenticationProperties { };

    await HttpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(claimsIdentity),
        authProperties);

    return RedirectToAction("Index", "Home");
  }


  [Authorize]
  public async Task<IActionResult> LogoutAsync(string? returnUrl = null)
  {
    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
    {
      ModelState.AddModelError(string.Empty, "Usuário ou senha inválidos.");
    }

    // Clear the existing external cookie
    await HttpContext.SignOutAsync(
        CookieAuthenticationDefaults.AuthenticationScheme);

    return RedirectToAction("Index", "Login");
  }
}