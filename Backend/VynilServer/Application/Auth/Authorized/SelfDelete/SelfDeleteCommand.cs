using MediatR;
using VynilServer.Application.Common;

namespace VynilServer.Application.Auth;

public record SelfDeleteCommand(Guid? UserId, string Password) : IRequest<Result<bool>>;