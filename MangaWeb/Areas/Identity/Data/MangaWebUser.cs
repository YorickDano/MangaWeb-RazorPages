using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MangaWeb.Areas.Identity.Data;

// Add profile data for application users by adding properties to the MangaWebUser class
public class MangaWebUser : IdentityUser
{
    public byte[]? ProfileImage { get; set; }
    public Role Role { get; set; } = Role.User;

}
public enum Role
{
    User,
    Admin,
    Advanced
}


