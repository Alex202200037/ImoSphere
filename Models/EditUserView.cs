public class EditUserViewModel
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public string Role { get; set; }

    // Propriedades para a nova senha e confirmação de senha (opcionais)
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}
