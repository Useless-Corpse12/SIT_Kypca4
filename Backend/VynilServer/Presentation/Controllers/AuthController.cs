using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VynilServer.Presentation.Controllers;


public class AuthController : ApiController
{
    //anon Users
    //Рега усера
    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterUser() {}
    //Подтв почты при реге
    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyMail() {}
    //Повторная отправка кода подтверждения
    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<IActionResult> ResendCode() {}
    //Логин
    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<IActionResult> Login() {}
    //2FA для админа
    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyLogin() {}
    //Обновление токена
    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh() {}
    //Запрос сброса пароля
    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword() {}
    //Установка нового пароля
    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword() {}
    
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
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UserDelete() {}
    //Создание админа
    [HttpPost("[action]")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminCreate() {}
}