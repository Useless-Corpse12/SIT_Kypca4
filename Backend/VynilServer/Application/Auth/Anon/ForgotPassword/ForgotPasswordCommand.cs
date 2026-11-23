using MediatR;
using VynilServer.Application.Common;

namespace VynilServer.Application.Auth;

public record ForgotPasswordCommand(string Email) : IRequest<Result<bool>>;