using AspNetCore.Identity.MongoDbCore.Models;
using FinanceFolio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Core.Authentication;
using System.Security.Claims;

namespace FinanceFolio.Controllers
{
    [ApiController]
    [Route("[Controller]")]
 
    public class AccountController : ControllerBase
    {
        private readonly UserManager<MongoIdentityUser> _userManager;
        private readonly SignInManager<MongoIdentityUser> _signInManager;

        public AccountController(UserManager<MongoIdentityUser> userManager, SignInManager<MongoIdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;

        }

        [HttpPost("Register")]

        public async Task<IActionResult> Register(Register model)
        {


            var user = new MongoIdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok("User Registration Successful");
            }
            else
            {

                return BadRequest(result.Errors.Select(e => e.Description).ToArray());
            }

        }


        [HttpGet("UserInfo")]
        [Authorize]
        public async Task<IActionResult> GetUserInfo()
        {
           var user=await _userManager.GetUserAsync(User);

            return Ok(new {
                Username=user?.UserName,
                Email=user?.Email,
                PhoneNumber=user?.PhoneNumber
            });
        }




        [HttpPut("updateUserProfile")]
        [Authorize]
        public async Task<IActionResult> UpdateUserInfo(Register model)
        {

            var user=await _userManager.FindByEmailAsync(model.Email);
            user.UserName = model.UserName;
            user.Email=model.Email;
            user.PhoneNumber=model.PhoneNumber;
            await _userManager.UpdateSecurityStampAsync(user);

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok("User update Successful");
            }
            else
            {

                return BadRequest(result.Errors.Select(e => e.Description).ToArray());
            }

        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] Register resetViewModel)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 403;
                return new JsonResult(new SerializableError(ModelState).ToList());
            }
            var user = await _userManager.FindByEmailAsync(resetViewModel.Email);
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, resetViewModel.Password);
            return Ok(result.Succeeded);
        }


        [HttpPost("login")]
        public async Task<IActionResult> LogIn(LoginModel model)
        {

            //var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

            var user=await _userManager.FindByEmailAsync(model.Email);

            IEnumerable<Claim> claims=new List<Claim>{ new Claim(ClaimTypes.Email, model.Email) };

            if(user != null)
            {
                await _signInManager.SignInWithClaimsAsync(user, model.RememberMe, claims);
                return Ok("Login Successful.");
            }
           
            else
            {
                return BadRequest("Login Failed!Invalid Username and Password.");
            }
        }


        

        [HttpPost("logout")]

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return Ok("User Logged out.");
        }

    }
}
