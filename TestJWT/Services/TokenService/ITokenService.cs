using Microsoft.AspNetCore.Identity;
using TestJWT.Data.Models;
using TestJWT.DTOs;

namespace TestJWT.Services.TokenService
{
	public interface ITokenService
	{
		Task<UserAuthDto> LoginService(LoginDto model);
		Task<string> AddRoleAsync(AddRoleDto model);
		Task<string> CreateToken(ApplicationUser user,UserManager<ApplicationUser> userManager);
	}
}
