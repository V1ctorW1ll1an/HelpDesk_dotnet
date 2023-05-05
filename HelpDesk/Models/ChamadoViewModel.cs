namespace HelpDesk.Models;


public class ChamadoViewModel
{

  public string? id_chamado { get; set; }
  public string? titulo_chamado { get; set; }
  public string? descricao_chamado { get; set; }
  public CategoriaViewModel? categoria { get; set; }
  public UsuarioViewModel? usuario { get; set; }
}