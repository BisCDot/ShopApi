using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shopping_Cart_Api.Data;
using Shopping_Cart_Api.Model;
using Shopping_Cart_Api.ViewModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Shopping_Cart_Api.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPasswordHasher<ApplicationUser> _hasher;
        private readonly IConfiguration _config;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(ApplicationDbContext applicationDbContext,
            UserManager<ApplicationUser> userManager,
            IPasswordHasher<ApplicationUser> hasher,
            IConfiguration config,
            RoleManager<IdentityRole> roleManager

            )
        {
            _context = applicationDbContext;
            _userManager = userManager;
            _hasher = hasher;
            _config = config;
            _roleManager = roleManager;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    if (_hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) == PasswordVerificationResult.Success)
                    {
                        var claims = new List<Claim>(new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                        });
                        //add role to claim
                        var roleNames = await _userManager.GetRolesAsync(user);
                        foreach (var roleName in roleNames)
                        {
                            var role = await _roleManager.FindByNameAsync(roleName);
                            if (role != null)
                            {
                                var roleClaim = new Claim(JwtRegisteredClaimNames.Nonce, role.Name);
                                claims.Add(roleClaim);
                                var roleClaims = await _roleManager.GetClaimsAsync(role);
                                claims.AddRange(roleClaims);
                            }
                        }
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtIssuerOptions:key"]));
                        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            issuer: _config["JwtIssuerOptions:Issuer"],
                            audience: _config["JwtIssuerOptions:audience"],
                            claims: claims,
                            expires: DateTime.UtcNow.AddHours(60),
                            signingCredentials: cred
                            );
                        var userNameId = _userManager.FindByNameAsync(user.UserName);
                        string userId = userNameId.Result.Id;
                        var exist = await _context.ShoppingCarts.AnyAsync(i => i.UserId == userId);

                        if (!exist)
                        {
                            var shoppingCart = new ShoppingCart() { Id = Guid.NewGuid(), UserId = userId };
                            _context.ShoppingCarts.Add(shoppingCart);
                        }
                        _context.SaveChanges();
                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo,
                        });
                    }
                }
                return BadRequest("User Not Found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [Authorize]
        [HttpGet("GetCurrentUser")]
        public async Task<ActionResult<string>> GetCurrentUser()
        {
            var isAuthentication = User.Identity?.IsAuthenticated;
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok(new
            {
                IsAuthentication = isAuthentication,
                UserName = userId,
            });
        }

    }
}