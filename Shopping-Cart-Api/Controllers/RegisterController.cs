using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shopping_Cart_Api.Data;
using Shopping_Cart_Api.Model;
using Shopping_Cart_Api.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping_Cart_Api.Controllers
{
    [Route("api/[controller]")]
    public class RegisterController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public RegisterController(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RegistrationViewModel registrationViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = registrationViewModel.UserName,
                    Email = registrationViewModel.Email
                };
                try
                {
                    var result = await _userManager.CreateAsync(user, registrationViewModel.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, Constants.SimpleUser);
                        return Ok();
                    }
                    else return Ok(result);
                }
                catch
                {
                    return BadRequest("Can Not Create User");
                }
            }
            else return BadRequest(ModelState);
        }
    }
}