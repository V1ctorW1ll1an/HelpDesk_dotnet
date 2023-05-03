using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.Controllers;

// form action=/user

public class UserController : Controller
{

  public UserController() { }

  // GET default controller /user/
  public IActionResult Index(string id)
  {
    return View();
  }
}