using MediatR;
using VynilServer.Application.Common;

namespace VynilServer.Application.Auth;

public record AdminCreateCommand(string Email):IRequest<Result<bool>>;