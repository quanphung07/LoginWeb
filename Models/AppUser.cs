using System;
using Microsoft.AspNetCore.Identity;

namespace LogInWeb.Models
{
    public class AppUser:IdentityUser
    {
        public string City { get; set; }
        
    }
}