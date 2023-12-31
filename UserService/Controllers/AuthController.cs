using Microsoft.AspNetCore.Mvc;
using UserService.Models;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private IAuthRepository _authrepository;
        private IAppRepository _context;
        private IConfiguration Configuration { get; }
        private Logger _logger;
        public AuthController(IAuthRepository authrepository, IAppRepository context,
                              IConfiguration configuration, Logger logger)
        {
            _authrepository = authrepository;
            _context = context;
            Configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var user = await _authrepository.Login(model.Username, model.Password);
                if (user != null)
                {
                    var claims = new[]
                    {
                    new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti,new Guid().ToString())
                    };

                    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("AppSettings:Token").Value));

                    var token = new JwtSecurityToken(
                        issuer: "http://oec.com",
                        audience: "http://oec.com",
                        expires: DateTime.UtcNow.AddDays(7),
                        claims: claims,
                        signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                        );

                    var usertoken = new JwtSecurityTokenHandler().WriteToken(token);
                    user.LoginToken = usertoken;
                    await _authrepository.UpdateUser(user);
                    _logger.LogInInfo(user.Email, "Log in", Request.Path);
                    return Ok(new
                    {
                        token = usertoken,
                        expiration = token.ValidTo
                    });
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                var arguments = this.GetBaseData(_context, _authrepository);
                _logger.LogException(ex, arguments.Email, arguments.Path);
                return BadRequest($"{ex.GetType().Name} was thrown.");
            }
        }

        [HttpPost]
        [Route("forgotpassword")]
        public async Task<IActionResult> ForgotPassword([FromForm] ForgotPasswordModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AppIdentityUser user = await _authrepository.FindUserByEmail(model.Email);
                    if (user != null)
                    {
                        var _code = _authrepository.GeneratePasswordResetToken(user);
                        var callbackUrl = Url.Action(
                                                     "ResetPassword", "Auth",
                                                      new { userId = user.Id, code = _code },
                                                      protocol: HttpContext.Request.Scheme
                                                     );
                        await new EmailService().SendEmailAsync(user.Name, model.Email, $"{user.Name} - Password reset",
                                                               $"To reset click: <a href='{callbackUrl}'>link</a> ");
                        return Ok($"{user.Name}, check your email for reset password");
                    }
                    return NotFound($"User with {model.Email} does not exist");
                }
                return BadRequest("Model is not valid");
            }
            catch (Exception ex)
            {
                var arguments = this.GetBaseData(_context, _authrepository);
                _logger.LogException(ex, arguments.Email, arguments.Path);
                return BadRequest($"{ex.GetType().Name} was thrown.");
            }
        }


        [HttpPost]
        [Route("resetpassword")]
        public async Task<IActionResult> ResetPassword(string code, string userId)
        {
            return Ok();
        }

}
