using MediatR;
using VynilServer.Application.Common;

namespace VynilServer.Application.Auth.ResendCode;

public record ResendCodeCommand(Guid UserId) : IRequest<Result<bool>>;