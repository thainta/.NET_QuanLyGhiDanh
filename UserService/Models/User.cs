using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Models;

[Table("users")]
public class User
{
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("email")]
    public string Email { get; set; }
    
    [Column("Password")]
    public string Password { get; set; }
    
    [Column("phone")]
    public string Phone { get; set; }
    
    [Column("role")]
    public string Role { get; set; }
}