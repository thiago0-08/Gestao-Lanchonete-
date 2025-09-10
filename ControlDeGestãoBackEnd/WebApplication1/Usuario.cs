using System.ComponentModel.DataAnnotations;

public class Usuario
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Senha { get; set; } = string.Empty;

    // Adicione uma propriedade para a função do usuário
    public string Funcao { get; set; } = "Atendente"; // Atendente, Gerente
}