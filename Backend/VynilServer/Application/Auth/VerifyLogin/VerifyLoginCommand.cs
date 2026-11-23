using MediatR;
using VynilServer.Application.Common;

namespace VynilServer.Application.Auth.VerifyLogin;

public record VerifyLoginCommand(Guid UserId, string Code, string Fingerprint):IRequest<Result<(string RefreshToken, string AccessToken)>>;