using Aiursoft.Tracer.Entities;

namespace Aiursoft.Tracer.Models.UsersViewModels;

public class UserWithRolesViewModel
{
    public required User User { get; set; }
    public required IList<string> Roles { get; set; }
}