using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using VynilServer.Application.Auth.ForgotPassword;
using VynilServer.Application.Auth.Login;
using VynilServer.Application.Auth.Refresh;
using VynilServer.Application.Auth.RegisterUser;
using VynilServer.Application.Auth.ResendCode;
using VynilServer.Application.Auth.ResetPassword;
using VynilServer.Application.Auth.VerifyLogin;
using VynilServer.Application.Auth.VerifyUser;
using VynilServer.Domain.Entities;

namespace VynilServer.Presentation.Controllers;


public class AuthController(IMediator mediator) : ApiController
{
    //anon Users
    //Рега усера
    [HttpPost("registration")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserCommand request)
    {
        var result = await mediator.Send(request);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return Ok(new {userId = result.Value, message = "Регистрация успешна. Проверьте почту для подтверждения." });
    }
    
    //Подтв почты при реге
    [HttpPost("verify-email")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyMail([FromBody] VerifyUserCommand request)
    {
        var result = await mediator.Send(request);
        
        if (!result.IsSuccess) 
            return BadRequest(new { message = result.Error });
        
        return Ok(new {isSuccess = result.Value ,message = "Верификация " + (result.Value ? "успешна" : "провалена")});
    }
    
    //Повторная отправка кода подтверждения
    [HttpPost("resend-code")]
    [AllowAnonymous]
    public async Task<IActionResult> ResendCode([FromBody] ResendCodeCommand request)
    {
        var result = await mediator.Send(request);
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });
        return Ok(new {isSuccess = result.Value, message = result.Value ? "Код выслан на почту" : "Тебе это не нужно"});
    }
    
    //Логин
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand request)
    {
        var result = await mediator.Send(request);
        if (!result.IsSuccess)
        {
            if (result.Error == null) 
                return BadRequest(
                    new { status = "Unverified", message = "Аккаунт не верифицирован. Код отправлен на почту." });
            return BadRequest(new { status = "InvalidCredentials", message = result.Error });
        }
        if (result.Value == null) 
            return Ok(new { status = "Requires2FA", message = "Код подтверждения входа отправлен на почту" });
        return Ok(new { status = "Success", refreshToken = result.Value });
    }
    
    //2FA для админа
    [HttpPost("verify-login")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyLogin([FromBody] VerifyLoginCommand request)
    {
        var result = await mediator.Send(request);
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });
        return Ok(new {refreshToken = result.Value.RefreshToken, accessToken = result.Value.AccessToken});
    }
    
    //Обновление токена
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshCommand request)
    {
        var result = await mediator.Send(request);
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });
        return Ok(new {refreshToken = result.Value.RefreshToken, accessToken = result.Value.AccessToken});
    }
    
    //Запрос сброса пароля
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand request)
    {
        var result = await mediator.Send(request);
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });
        return Ok(
            new
            {
                isSuccess = result.Value, 
                message = result.Value ? "Успех!" : "Сначала следует подтвердить почту"
            });
    }
    //Установка нового пароля
    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand request)
    {
        var result = await mediator.Send(request);
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });
        return Ok(
            new
            {
                isSuccess = result.Value,
                message = result.Value ? "Успех!" : "Проверьте правильность введения кода"
            });
    }
    
    //Auth
    //Выход (просто чистит токены в бд)
    [HttpPost("[action]")]
    [Authorize]
    public async Task<IActionResult> Logout() {}
    //Смена пароля
    [HttpPut("[action]")]
    [Authorize]
    public async Task<IActionResult> ChangePassword() {}
    //Самоудаление
    [HttpDelete("[action]")]
    [Authorize]
    public async Task<IActionResult> SelfDelete() {}
    
    //Auth(admin-only)
    //Бан
    [HttpDelete("[action]")]
    [Authorize(Roles = nameof(Role.Admin))]
    public async Task<IActionResult> UserDelete() {}
    //Создание админа
    [HttpPost("[action]")]
    [Authorize(Roles = nameof(Role.Admin))]
    public async Task<IActionResult> AdminCreate() {}
}