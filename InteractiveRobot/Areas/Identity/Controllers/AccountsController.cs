using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InteractiveRobot.Areas.Identity.Controllers
{
    [Route("api/[area]/[controller]")]
    [Area("Identity")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            var user = new ApplicationUser
            {
                Email = registerRequest.Email,
                UserName = registerRequest.UserName,
                Name = registerRequest.Name,
                UserType = registerRequest.UserType == UserType.Doctor
                    ? UserType.Doctor
                    : UserType.Parent
            };

            var result = await _userManager.CreateAsync(user, registerRequest.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action("ConfirmEmail", "Accounts", new { userId = user.Id, token }, Request.Scheme);
            await _emailSender.SendEmailAsync(user.Email!, "Confirm Your Email", $"Click <a href='{confirmationLink}'>here</a> to confirm your email");

            await _userManager.AddToRoleAsync(user, registerRequest.UserType.ToString());

            if (user.UserType == UserType.Doctor && registerRequest.SpecialtyIds is not null && registerRequest.SpecialtyIds.Any())
            {
                foreach (var specialtyId in registerRequest.SpecialtyIds)
                {
                    _context.DoctorSpecialties.Add(new DoctorSpecialty
                    {
                        DoctorId = user.Id,
                        SpecialtyId = specialtyId
                    });
                }

                await _context.SaveChangesAsync();
            }

            return Created();
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.EmailOrUserName) ??
                       await _userManager.FindByNameAsync(loginRequest.EmailOrUserName);

            if (user is null)
                return Unauthorized(new { Message = "Invalid credentials" });

            if (!await _userManager.CheckPasswordAsync(user, loginRequest.Password))
                return Unauthorized(new { Message = "Invalid credentials" });

            if (!user.EmailConfirmed)
                return Unauthorized(new { Message = "Email not confirmed" });

            if (user.IsBanned)
                return Unauthorized(new { Message = "You Banned" });

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
                        {
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(ClaimTypes.NameIdentifier, user.Id),
                            new Claim(ClaimTypes.Name, user.UserName!),
                            new Claim(ClaimTypes.Email, user.Email!),
                            new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? "")
                        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("EraaSoft##EraaSoft##EraaSoft##EraaSoft##EraaSoft##"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: creds
            );

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo,
                UserId = user.Id,
                Role = roles.FirstOrDefault() ?? ""
            });
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return NoContent();
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            var applicationUser = await _userManager.FindByIdAsync(userId);

            if (applicationUser is not null)
            {
                var result = await _userManager.ConfirmEmailAsync(applicationUser, token);

                if (result.Succeeded)
                    return NoContent();
                else
                    return BadRequest(result.Errors.Select(e => e.Description));

            }

            return NotFound();
        }

        [HttpPost("ResendEmail")]
        public async Task<IActionResult> ResendEmail([FromBody] ResendEmailRequest resendEmailRequest)
        {
            var user = await _userManager.FindByEmailAsync(resendEmailRequest.EmailOrUserName) ?? 
                await _userManager.FindByNameAsync(resendEmailRequest.EmailOrUserName);

            if (user is null)
                return Unauthorized(new { Message = "Invalid credentials" });

            if (!user.EmailConfirmed)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var confirmationLink = Url.Action("ConfirmEmail", "Accounts", new { userId = user.Id, token }, Request.Scheme);

                await _emailSender.SendEmailAsync(user.Email!, "Confirm Your Email Again!", $"Click <a href='{confirmationLink}'>here</a> to confirm your email");

                return NoContent();
            }

            return BadRequest(new { Message = "Already confirmed!" });
        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest forgetPasswordRequest)
        {
            var user = await _userManager.FindByEmailAsync(forgetPasswordRequest.EmailOrUserName) ??
               await _userManager.FindByNameAsync(forgetPasswordRequest.EmailOrUserName);

            if (user is null)
                return Unauthorized(new { Message = "Invalid credentials" });

            var code = new Random().Next(1000, 9999).ToString();

            _context.PasswordResetCodes.Add(new()
            {
                ApplicationUserId = user.Id,
                Code = code,
                ExpirationCode = DateTime.UtcNow.AddHours(24)
            });
            _context.SaveChanges();

            await _emailSender.SendEmailAsync(user.Email!, "Reset The Password", $"<h1>Please Reset Your Password Using This Code {code}");

            return NoContent();
        }

        [HttpPost("ConfirmResetPassword")]
        public async Task<IActionResult> ConfirmResetPassword([FromBody] ResetPasswordRequest resetPasswordRequest)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordRequest.EmailOrUserName) ??
               await _userManager.FindByNameAsync(resetPasswordRequest.EmailOrUserName);

            if (user is null)
                return Unauthorized(new { Message = "Invalid credentials" });

            var resetCode = _context.PasswordResetCodes.Where(e => e.ApplicationUserId == user.Id).OrderByDescending(e => e.ExpirationCode).FirstOrDefault();

            if (resetCode is not null && resetCode.Code == resetPasswordRequest.Code && resetCode.ExpirationCode > DateTime.UtcNow)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordRequest.Password);

                if (!result.Succeeded)
                    return BadRequest(result.Errors.Select(e => e.Description));

                await _emailSender.SendEmailAsync(user.Email!, "Reset Password Successfully", $"Reset Password Successfully.");

                return NoContent();
            }

            return BadRequest(new { Message = "Error In Code!" });
        }

        [HttpPost("ExternalLogin")]
        public async Task<IActionResult> ExternalLogin([FromBody] ExternalAuthRequest externalAuth)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(externalAuth.IdToken);

            if (payload == null)
                return BadRequest("Invalid External Authentication");

            var info = new UserLoginInfo(externalAuth!.Provider ?? "Google", payload.Subject, externalAuth.Provider);

            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(payload.Email);

                if (user == null)
                {
                    user = new ApplicationUser { Email = payload.Email, UserName = payload.Email };
                    await _userManager.CreateAsync(user);

                    // Prepare and send an email for the email confirmation

                    await _userManager.AddToRoleAsync(user, "Customer");
                    await _userManager.AddLoginAsync(user, info); // Replace => Generate JWT
                }
                else
                {
                    await _userManager.AddLoginAsync(user, info); // Replace => Generate JWT
                }
            }

            // Check for the Locked out account
            return Ok();
        }
    }
}
