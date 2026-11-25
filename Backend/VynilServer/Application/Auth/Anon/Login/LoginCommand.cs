using MediatR;
using VynilServer.Application.Common;

namespace VynilServer.Application.Auth;

public record LoginCommand(string Email, string Password,string Fingerprint)
    :IRequest<Result<(string RefreshToken,string AccessToken)>>;