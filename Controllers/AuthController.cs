//AuthController
//using Asp_FinalProject.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MVCAPI.DTOs;
using MVCAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MVCAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;

        public AuthController(
     UserManager<AppUser> userManager,
     SignInManager<AppUser> signInManager,
     RoleManager<IdentityRole> roleManager,
     IConfiguration config) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _config = config;
        }


        [HttpPost("register")]

        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            try
            {

                if (await _userManager.FindByEmailAsync(dto.Email!) is not null)
                    return BadRequest(new { message = "Email is already registered." });

                var user = new AppUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    FullName = dto.FullName
                };

                var result = await _userManager.CreateAsync(user, dto.Password!);
                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                if (!await _roleManager.RoleExistsAsync(dto.Role))
                    await _roleManager.CreateAsync(new IdentityRole(dto.Role));

                await _userManager.AddToRoleAsync(user, dto.Role);

                return Ok(new { message = $"User {user.Email}  registered successfully with role: {dto.Role}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

      
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginDTO loginuser)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == loginuser.Email.ToLower());
            if (user == null)
                return Unauthorized(new { message = "Invalid email." });

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, loginuser.Password, false);
            if (!signInResult.Succeeded)
                return Unauthorized(new { message = "Invalid password." });

            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateToken(user, roles);

            return Ok(new
            {
                token,
                user = new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    roles
                }
            });
        }

        private string GenerateToken(AppUser user, IList<string> roles)
        {
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.GivenName, user.FullName ?? ""),
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.UserName ?? "")
    };

            // إضافة الأدوار
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:skey"]));
            var signCred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = signCred,
                Issuer = _config["JWT:iss"],
                Audience = _config["JWT:aud"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized("User not found.");
            var email = user.Email;

            var roles = await _userManager.GetRolesAsync(user);

            await _signInManager.SignOutAsync();

            return Ok(new
            {
                message = "User logged out successfully.",
                email = email,
                username = user.UserName,
                role = roles.FirstOrDefault()
            });
        }

    }

    }
