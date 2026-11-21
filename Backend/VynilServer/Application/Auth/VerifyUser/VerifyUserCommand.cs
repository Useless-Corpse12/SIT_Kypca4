using MediatR;
using VynilServer.Application.Common;

namespace VynilServer.Application.Auth.VerifyUser;

public record VerifyUserCommand( Guid UserId, string Code) : IRequest<Result<bool>>;