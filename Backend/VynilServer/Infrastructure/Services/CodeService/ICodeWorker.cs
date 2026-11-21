namespace VynilServer.Infrastructure.Services.CodeService;

public interface ICodeService
{
    string GenerateAndSaveCode(Guid userId,CodeType type);
    bool ValidateCode(Guid userId, string code,CodeType type);
    void RemoveCode(Guid userId,CodeType type);
    
}
public enum CodeType
{
    VerificationCode = 1,
    LoginCode = 2,
    PasswordCode = 3
}