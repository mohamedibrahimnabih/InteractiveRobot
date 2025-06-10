using InteractiveRobot.Data;
using InteractiveRobot.DTOs.Request;
using InteractiveRobot.Models;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace InteractiveRobot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;// = new();

        public AccountsController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(ApplicationUserRequest applicationUserRequest)
        {
            //ApplicationUser user = new()
            //{
            //    Email = applicationUserRequest.Email,
            //    UserName = applicationUserRequest.UserName,
            //    Name = applicationUserRequest.Name,
            //    PhoneNumber = applicationUserRequest.PhoneNumber,
            //    ApplicationUserType = applicationUserRequest.ApplicationUserType,
            //};

            var result = await _userManager.CreateAsync(applicationUserRequest.Adapt<ApplicationUser>(), applicationUserRequest.Password);

            if(result.Succeeded)
            {
                return Ok("Add User Successfully");
            }

            return BadRequest(result.Errors);
        }

        //[HttpPost("Login")]
        //public IActionResult Login()
        //{

        //}
    }
}
