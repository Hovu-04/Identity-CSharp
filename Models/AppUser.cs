namespace IdentityLogin.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

public class AppUser : IdentityUser
{
    [Column(TypeName = "nvarchar(400)")]
    [StringLength(400)]
    public string? HomeAddress { get; set; }
}