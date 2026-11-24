using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using VynilServer.Application.Auth;
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
    //Выход (чистит данный токен)
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] LogoutCommand request)
    {
        var userIdString = User.FindFirstValue(JwtRegisteredClaimNames.Sub); 
    
        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized(new { message = "Некорректный токен" });

        var result = await mediator.Send(request with { UserId = userId });

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return Ok(new { message = "Вы успешно вышли из системы" });
    }
    
    //Полный выход(удаляет все токены)
    [HttpPost("total-logout")]
    [Authorize]
    public async Task<IActionResult> TotalLogout([FromBody] TotalLogoutCommand request)
    {
        var userIdString = User.FindFirstValue(JwtRegisteredClaimNames.Sub); 
    
        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized(new { message = "Некорректный токен" });
        
        var result = await mediator.Send(request with { UserId = userId });

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return Ok(new { message = "Вы успешно вышли из системы" });
    }
    
    //Смена пароля
    [HttpPut("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand request)
    {
        var userIdString = User.FindFirstValue(JwtRegisteredClaimNames.Sub); 
    
        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized(new { message = "Некорректный токен" });
        
        var result = await mediator.Send(request with { UserId = userId });

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return Ok(new { message = "Пароль успешно сменён" });
    }
    
    //Самоудаление (Анонимизация)
    [HttpDelete("self-delete")]
    [Authorize]
    public async Task<IActionResult> SelfDelete([FromBody] SelfDeleteCommand request)
    {
        var userIdString = User.FindFirstValue(JwtRegisteredClaimNames.Sub); 
    
        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized(new { message = "Некорректный токен" });
        
        var result = await mediator.Send(request with { UserId = userId });

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return Ok(new { message = "bye" });
    }
    
    //Auth(admin-only)
    //Бан
    [HttpDelete("user-delete")]
    [Authorize(Roles = nameof(Role.Admin))]
    public async Task<IActionResult> UserDelete([FromBody] UserDeleteCommand request)
    {
        var result = await mediator.Send(request);
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });
        return Ok(new { message = "bye" });
    }
    //Создание админа
    [HttpPost("admin-create")]
    [Authorize(Roles = nameof(Role.Admin))]
    public async Task<IActionResult> AdminCreate([FromBody] AdminCreateCommand request)
    {
        var result = await mediator.Send(request);
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });
        return Ok(new { message = "alright!" });
    }
    
}