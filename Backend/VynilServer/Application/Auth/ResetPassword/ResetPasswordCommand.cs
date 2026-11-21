using MediatR;
using VynilServer.Application.Common;

namespace VynilServer.Application.Auth.ResetPassword;

public record ResetPasswordCommand(string Email,string Code, string NewPassword):IRequest<Result<bool>>;