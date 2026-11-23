using MediatR;
using VynilServer.Application.Common;

namespace VynilServer.Application.Auth;

public record RefreshCommand(string RefreshToken,string Fingerprint )
    :IRequest<Result<(string RefreshToken, string AccessToken)>>;