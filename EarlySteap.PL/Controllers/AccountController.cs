using EarlyStep.BLL.Interfaces;
using EarlyStep.Contracts.DTOS.Requests.AccountRequests;
using EarlyStep.DAL.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using System.Text;
namespace EarlySteap.PL.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
    {
    private readonly IAuthRepository _authRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;

    public AuthController(
        IAuthRepository authRepository,
        UserManager<ApplicationUser> userManager,
        IEmailSender emailSender)
        {
        _authRepository = authRepository;
        _userManager = userManager;
        _emailSender = emailSender;
        }
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromForm] RegisterRequests request)
        {
        var result = await _authRepository.RegisterAsync(request);

        if (result == null)
            return BadRequest("Registration failed.");

        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return BadRequest("Registration failed.");

        var token =
            await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var encodedToken = WebEncoders.Base64UrlEncode(
            Encoding.UTF8.GetBytes(token));

        var confirmUrl = Url.Action(
            nameof(ConfirmEmail),
            "Auth",
            new
                {
                userId = user.Id,
                token = encodedToken
                },
            protocol: Request.Scheme,
            host: Request.Host.Value);

        await _emailSender.SendEmailAsync(
            request.Email,
            "Welcome To EarlyStep - Confirm Your Email",
            $@"
        <h1>Welcome {user.FullName} 👋</h1>

        <p>We're excited to have you on <b>EarlyStep</b>.</p>

        <p>Please confirm your email to activate your account.</p>

        <a href='{confirmUrl}'
           style='display:inline-block;
                  padding:10px 20px;
                  background-color:#4CAF50;
                  color:white;
                  text-decoration:none;
                  border-radius:5px'>
            Confirm Email
        </a>");

        return Ok(result);
        }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] LoginRequests request)
        {
        var result = await _authRepository.LoginAsync(request);

        if (result == null)
            return Unauthorized();

        return Ok(result);
        }
    [HttpGet("confirmEmail")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
        var result = await _authRepository.ConfirmEmailAsync(userId, token);
        return result.Succeeded ? Ok(new { message = "Email confirmed" }) : BadRequest(result.Errors.Select(e => e.Description));

        }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _authRepository.ChangePasswordAsync(userId!, request);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        return Ok("تم تغيير كلمة المرور بنجاح.");
        }


    [HttpPost("ForgotPassword")]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
    [FromBody] ForgotPasswordRequest request,
    CancellationToken ct)
        {
        var code = await _authRepository.ForgotPassword(request, ct);

        if (!string.IsNullOrEmpty(code))
            {
            var html = $@"
        <div style='font-family:Segoe UI,Arial,sans-serif'>
            <h2>إعادة تعيين كلمة المرور</h2>
            <p>Hello,</p>
            <p>استخدم الرمز أدناه لإعادة تعيين كلمة مرورك. تنتهي صلاحيته خلال ١٥ دقيقة.</p>
            <div style='font-size:28px;font-weight:700; letter-spacing:6px;margin:16px 0'>
                {code}
            </div>
            <hr />
            <p>EarlyStep</p>
        </div>";

            await _emailSender.SendEmailAsync(
                request.Email,
                "إعادة تعيين كلمة المرور - EarlyStep",
                html);
            }

        return Ok(new
            {
            message = "إذا كان البريد الإلكتروني مسجلاً، سيتم إرسال تعليمات استعادة كلمة المرور."
            });
        }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken ct)
        {
        var result = await _authRepository.ResetPasswordAsync(request, ct);

        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        return Ok(new { message = "تم تير كلمة السر بنجاح." });
        }


    }