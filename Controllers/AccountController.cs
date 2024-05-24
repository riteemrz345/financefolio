using AspNetCore.Identity.MongoDbCore.Models;
using FinanceFolio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        [AllowAnonymous]

        public async Task<IActionResult> Register(Register model)
        {


            var user = new MongoIdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                //    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                //    // Send an email with this link
                //    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                //    //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                //    //    "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                //    await _signInManager.SignInAsync(user, isPersistent: false);
                return Ok("User Registration Successful");
            }
            else
            {

                return BadRequest(result.Errors.Select(e => e.Description).ToArray());
            }

        }

        [AllowAnonymous]
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


        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LogIn(LoginModel model)
        {

            //var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

            var user=await _userManager.FindByEmailAsync(model.Email);

            IEnumerable<Claim> claims=new List<Claim>{ new Claim(ClaimTypes.Email, model.Email) };

            if(user != null)
            {
                await _signInManager.SignInWithClaimsAsync(user, model.RememberMe, claims);
                return Ok(new
                {
                    id=user.Id
                });
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
