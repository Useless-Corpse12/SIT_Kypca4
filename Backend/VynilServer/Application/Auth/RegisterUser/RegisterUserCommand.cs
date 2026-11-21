using MediatR;
using VynilServer.Application.Common;

namespace VynilServer.Application.Auth.RegisterUser;

public record RegisterUserCommand(string Email, string Login,string Password):IRequest<Result<Guid>>;