namespace api_access.DTOs;

public class UserUpdatedDetailsDto
{
   public string? NewEmail { get; set; }
   public string? NewPassword { get; set; }
   public string ConfirmPassword { get; set; }
}