using MediatR;
using VynilServer.Application.Common;

namespace VynilServer.Application.Auth.RegisterUser;

public record RegisterUserCommand(string email, string login,string password):IRequest<Result<Guid>>;