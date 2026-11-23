using MediatR;
using VynilServer.Application.Common;

namespace VynilServer.Application.Auth.ResetPassword;

public record ResetPasswordCommand(Guid UserId,string Code, string NewPassword):IRequest<Result<bool>>;