namespace AuthorizationService.BLL.DTOs.Request;

public record UserRequest(string Email, string? Password, string Role);
