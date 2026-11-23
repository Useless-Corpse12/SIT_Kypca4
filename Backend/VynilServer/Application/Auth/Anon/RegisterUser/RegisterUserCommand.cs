using MediatR;
using VynilServer.Application.Common;

namespace VynilServer.Application.Auth;

public record RegisterUserCommand(string Email, string Login,string Password):IRequest<Result<Guid>>;