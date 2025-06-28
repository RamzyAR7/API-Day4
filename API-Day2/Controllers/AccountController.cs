using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using API_Day2.DTOs;

namespace API_Day2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        #region Identity
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _config;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserDataDTO model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id)
                };

                // Add admin role claim if user is admin
                if (user.UserName == "admin")
                {
                    userClaims.Add(new Claim(ClaimTypes.Role, "Admin"));
                }

                string keyString = _config["Jwt:Key"];
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: userClaims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: creds);

                string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                return Ok(new { token = tokenString, username = user.UserName, role = user.UserName == "admin" ? "Admin" : "User" });
            }

            return Unauthorized("Invalid username or password");
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserDataDTO model)
        {
            var user = new IdentityUser
            {
                UserName = model.UserName
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                return Ok("User Created Successfully");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return BadRequest(ModelState);
        }

        [HttpPost("CreateAdmin")]
        public async Task<IActionResult> CreateAdmin()
        {
            // Check if admin user already exists
            var adminUser = await _userManager.FindByNameAsync("admin");
            if (adminUser != null)
            {
                return BadRequest("Admin user already exists");
            }

            var user = new IdentityUser
            {
                UserName = "admin"
            };

            var result = await _userManager.CreateAsync(user, "123");
            if (result.Succeeded)
            {
                return Ok("Admin user created successfully with username: admin, password: 123");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return BadRequest(ModelState);
        }
        #endregion

        [HttpPost("StaticLogin")]
        public IActionResult StaticLogin(UserDataDTO user)
        {
            if (user.UserName != "admin" || user.Password != "123")
            {
                return Unauthorized("Invalid admin credentials");
            }

            #region Define Claims
            List<Claim> UserData = new List<Claim>();
            UserData.Add(new Claim("Name", "admin"));
            UserData.Add(new Claim(ClaimTypes.Role, "Admin"));
            UserData.Add(new Claim(ClaimTypes.MobilePhone, "01099011374"));
            #endregion

            #region Secret Key
            string Key = _config["Jwt:Key"] ?? "SecureKeyForAPI_Day2AuthenticationSystem";
            var SecretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
            #endregion

            #region Create Token
            var SignCre = new SigningCredentials(SecretKey, SecurityAlgorithms.HmacSha256);
            var Token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: UserData,
                signingCredentials: SignCre,
                expires: DateTime.Now.AddDays(1)
            );

            var StringToken = new JwtSecurityTokenHandler().WriteToken(Token);
            return Ok(new { token = StringToken, username = "admin", role = "Admin" });
            #endregion
        }

        [HttpGet("Protected")]
        [Authorize]
        public IActionResult Get()
        {
            var username = User.Identity?.Name;
            return Ok(new { message = "You have access to the protected endpoint!", username = username });
        }

        [HttpGet("AdminProtected")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminProtected()
        {
            var username = User.Identity?.Name;
            return Ok(new { 
                message = "Welcome Admin! You have access to the admin-only protected endpoint!", 
                username = username,
                role = "Admin",
                timestamp = DateTime.Now
            });
        }

        [HttpGet("AdminOnly")]
        [Authorize]
        public IActionResult AdminOnly()
        {
            var username = User.Identity?.Name;
            
            if (username != "admin")
            {
                return StatusCode(403, new { message = "Only admin user can access this endpoint" });
            }

            return Ok(new { 
                message = "Admin-only endpoint accessed successfully!", 
                username = username,
                secretData = "This is confidential admin data",
                timestamp = DateTime.Now
            });
        }

        [HttpGet("AdminOnlyBetter")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminOnlyBetter()
        {
            var username = User.Identity?.Name;
            
            // Additional check to ensure it's specifically the admin user
            if (username != "admin")
            {
                return StatusCode(403, new { message = "Only admin user can access this endpoint" });
            }

            return Ok(new { 
                message = "Admin-only endpoint accessed successfully!", 
                username = username,
                secretData = "This is confidential admin data",
                timestamp = DateTime.Now
            });
        }
    }
}