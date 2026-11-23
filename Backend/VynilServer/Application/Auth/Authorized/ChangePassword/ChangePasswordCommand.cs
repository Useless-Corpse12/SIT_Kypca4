using MediatR;
using VynilServer.Application.Common;

namespace VynilServer.Application.Auth;

public record ChangePasswordCommand(Guid? UserId, string OldPassword, string NewPassword):IRequest<Result<bool>>;