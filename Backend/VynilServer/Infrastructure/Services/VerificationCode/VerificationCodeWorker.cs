using Microsoft.Extensions.Caching.Memory;

namespace VynilServer.Infrastructure.Services.VerificationCode;

public class VerificationCodeWorker(IMemoryCache cache):IVerificationCodeService
{
    private readonly Random _random = new();
    public string GenerateAndSaveCode(Guid userId)
    {
        var code = _random.Next(100000, 999999).ToString();
        
        var cacheKey = $"vrf_code:{userId}";
        var cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
        cache.Set(cacheKey, code, cacheOptions);
        
        return code;
    }
    
    public bool ValidateCode(Guid userId, string code)
    {
        var cacheKey = $"vrf_code:{userId}";
        
        if (cache.TryGetValue(cacheKey, out string? cachedCode))
        {
            var boo = cachedCode == code;
            if (boo) RemoveCode(userId);
            return boo;
        }
        
        return false;
    }
    
    public void RemoveCode(Guid userId)
    {
        var cacheKey = $"vrf_code:{userId}";
        
        cache.Remove(cacheKey);
    }
}