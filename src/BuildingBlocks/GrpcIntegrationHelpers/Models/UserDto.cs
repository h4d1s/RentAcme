namespace GrpcIntegrationHelpers.Models;

public class UserDto
{
    public Guid Id { get; set; } = Guid.Empty;
    public string ExternalId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
