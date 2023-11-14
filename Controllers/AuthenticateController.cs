using System;

using asp_net.Authentication;
using asp_net.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using asp_net.Context;
using Microsoft.AspNetCore.Authentication.Google;
using Google.Apis.Auth;


namespace asp_net.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController :ControllerBase
	{
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly GoogleUserContext _context;

        public AuthenticateController(UserManager<ApplicationUser> userManager, GoogleUserContext context, RoleManager<IdentityRole> roleManager, IConfiguration configuration, SignInManager<ApplicationUser> signInManager, IWebHostEnvironment env)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            _configuration = configuration;
            _env = env;
            _context = context;
        }

        public IConfiguration Get_configuration()
        {
            return _configuration;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await userManager.FindByNameAsync(model.Username);
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(24),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginModel model)
        {
            try
            {
                var validationSettings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new List<string> { _configuration["Authentication:Google:ClientId"] }
                };

                //GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(
                //    model.IdToken, validationSettings);
                var payload = await GoogleJsonWebSignature.ValidateAsync(
                         model.IdToken, validationSettings);

                string googleUserId = payload.Subject;
                string email = payload.Email;

                // Check if the user with this email already exists in your system
                var user = await userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    // Create a new user in a separate table
                    var newUser = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        // Other user properties as needed
                    };

                    var result = await userManager.CreateAsync(newUser);

                    if (!result.Succeeded)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response
                        {
                            Status = "Error",
                            Message = "User creation failed. Please check user details and try again."
                        });
                    }
                }

                // Now, you have a user, whether new or existing.
                // Continue with the JWT token generation and return it.

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                // Add user roles, if needed
                var userRoles = await userManager.GetRolesAsync(user);

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                // Generate the JWT token and return it
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(24),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Google login failed" });
            }
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExists = await userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            var userExists = await userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Admin creation failed! Please check user details and try again." });

            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await roleManager.RoleExistsAsync(UserRoles.User))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            if (await roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await userManager.AddToRoleAsync(user, UserRoles.Admin);
            }

            return Ok(new Response { Status = "Success", Message = "Admin created successfully!" });
        }


    }
}

