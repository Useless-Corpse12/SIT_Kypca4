using MediatR;
using VynilServer.Application.Common;

namespace VynilServer.Application.Auth.ForgotPassword;

public record ForgotPasswordCommand(string Email) : IRequest<Result<bool>>;