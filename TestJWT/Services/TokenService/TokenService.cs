
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TestJWT.Data.Models;
using TestJWT.DTOs;
using TestJWT.Helpers;

namespace TestJWT.Services.TokenService
{
	public class TokenService : ITokenService
	{
		private readonly JWT _jwt;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<ApplicationUser> _userManager;

		public TokenService(RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt, UserManager<ApplicationUser> userManager)
		{
			this._jwt = jwt.Value;
			this._roleManager = roleManager;
			this._userManager = userManager;
		}
		public async Task<UserAuthDto> LoginService(LoginDto model)
		{
			var user = await _userManager.FindByEmailAsync(model.Email);
			var UserAuth = new UserAuthDto();
			if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
			{
				UserAuth.Message = "Email Or Password Is Not Correct";
				return UserAuth;
			}
			UserAuth.IsAuthenticated = true;
			UserAuth.Email = user.Email;
			UserAuth.UserName = user.UserName;
			UserAuth.Token = await CreateToken(user, _userManager);
			UserAuth.Roles = (await _userManager.GetRolesAsync(user)).ToList();
			return UserAuth;
		}

		public async Task<string> CreateToken(ApplicationUser user, UserManager<ApplicationUser> userManager)
		{
			var UserClaims = await userManager.GetClaimsAsync(user);
			var AuthClaims = new List<Claim>
			{
				new Claim(ClaimTypes.GivenName,user.UserName),
			new Claim(ClaimTypes.Email,user.Email)
			};
			var Roles = await userManager.GetRolesAsync(user);
			foreach (var role in Roles)
			{
				AuthClaims.Add(new Claim(ClaimTypes.Role, role));
			}
			var AllClaims = AuthClaims.Union(UserClaims);
			var AuthKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecurityKey));
			var Token = new JwtSecurityToken(
				issuer: _jwt.Issuer,
				audience: _jwt.Audience,
				expires: DateTime.Now.AddDays(_jwt.DurationInDays),
				claims: AuthClaims
				, signingCredentials: new SigningCredentials(AuthKey, SecurityAlgorithms.HmacSha256Signature)
				);
			return new JwtSecurityTokenHandler().WriteToken(Token);
		}

		public async Task<string> AddRoleAsync(AddRoleDto model)
		{
			var user = await _userManager.FindByIdAsync(model.UserId);
			if (user is null || !await _roleManager.RoleExistsAsync(model.RoleName))
				return "Invalid User id OR Role ";
			if (await _userManager.IsInRoleAsync(user, model.RoleName))
				return "This role is already assigned to this user";
			var Result =await _userManager.AddToRoleAsync(user, model.RoleName);
			return Result.Succeeded?string.Empty:"Something Went Error";
		}
	}
}
