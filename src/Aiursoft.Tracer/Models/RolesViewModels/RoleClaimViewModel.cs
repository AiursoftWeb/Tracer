namespace Aiursoft.Tracer.Models.RolesViewModels;

public class RoleClaimViewModel
{
    public required string Key { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public bool IsSelected { get; set; }
}
