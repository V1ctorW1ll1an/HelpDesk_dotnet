using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Models
{
  public class UsuarioViewModel
  {

    public string? id_usuario { get; set; }
    public string? nome_usuario { get; set; }
    [EmailAddress(ErrorMessage = "Informe um email válido")]
    public string? email_usuario { get; set; }
    public string? senha_usuario { get; set; }
    public PerfilViewModel? perfil { get; set; }
  }
}