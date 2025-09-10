using System.ComponentModel.DataAnnotations;

public class LoginDTO
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Senha { get; set; } = string.Empty;
}