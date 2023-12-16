using System.ComponentModel.DataAnnotations;

namespace TestJWT.DTOs
{
    public class RegisterDto
    {
        [MaxLength(100)]
        public string FirstName { get; set; } 
        [MaxLength(100)]
        public string LastName { get; set; } 
        [MaxLength(50)]
        public string UserName { get; set; } 
        [MaxLength(128)]
        public string Email { get; set; } 
        [MaxLength(150)]
        public string Password { get; set; } 
    }
}
