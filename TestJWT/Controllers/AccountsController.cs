using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TestJWT.Data.Models;
using TestJWT.DTOs;
using TestJWT.Helpers;
using TestJWT.Services.TokenService;

namespace TestJWT.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountsController : ControllerBase
	{
		private readonly JWT _jwt;
		private readonly ITokenService _tokenService;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;

		public AccountsController(IOptions<JWT> jwt, ITokenService tokenService, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
		{
			this._jwt = jwt.Value;
			this._tokenService = tokenService;
			this._userManager = userManager;
			this._signInManager = signInManager;
		}
		[HttpPost("Register")]
		public async Task<ActionResult<UserAuthDto>> Register([FromBody] RegisterDto model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			var user = await _userManager.FindByEmailAsync(model.Email);
			var UserAuth = new UserAuthDto();
			if (user is not null)
			{
				UserAuth.Message = "This Email Is In Use";
				return BadRequest(UserAuth.Message);
			}
			var User = new ApplicationUser()
			{
				FirstName = model.FirstName,
				LastName = model.LastName,
				Email = model.Email,
				UserName = model.UserName
			};
			var Result = await _userManager.CreateAsync(User, model.Password);
			if (!Result.Succeeded)
			{
				var Errors = string.Empty;
				foreach (var error in Result.Errors)
				{
					Errors += $"{error.Description}, ";
				}
				UserAuth.Message = Errors;
				return BadRequest(UserAuth.Message);
			}
			await _userManager.AddToRoleAsync(User, "User");
			return Ok(new UserAuthDto
			{
				Email = User.Email,
				IsAuthenticated = true,
				UserName = User.UserName,
				Roles = new List<string>() { "User" },
				ExpiresOn = DateTime.Now.AddDays(_jwt.DurationInDays),
				Token = await _tokenService.CreateToken(User, _userManager)
			});
		}

		[HttpPost("Login")]
		public async Task<IActionResult> Login([FromBody] LoginDto model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			var UserAuth = await _tokenService.LoginService(model);
			if (!UserAuth.IsAuthenticated)
			{
				return BadRequest(UserAuth.Message);
			}
			return Ok(UserAuth);
		}

		[HttpPost("AddRole")]
		public async Task<IActionResult> AddRole([FromBody] AddRoleDto model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState); 
			var Result = await _tokenService.AddRoleAsync(model);
			if (!string.IsNullOrEmpty(Result))
				return BadRequest(Result);
			return Ok(model);
		}
	}
}
