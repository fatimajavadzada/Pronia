using Microsoft.AspNetCore.Identity;

namespace Pronia.Models;

public class AppUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public ICollection<BasketItem> BasketItems { get; set; } = [];

}
