using Microsoft.Extensions.Caching.Memory;

namespace VynilServer.Infrastructure.Services.CodeService;

public class CodeWorker(IMemoryCache cache):ICodeService
{
    private readonly Random _random = new();
    public string GenerateAndSaveCode(Guid userId, CodeType type)
    {
        var code = _random.Next(100000, 999999).ToString();
        var cacheKey = $"{type}:{userId}";
        
        if (cache.TryGetValue(cacheKey,out string? value)) { cache.Remove(cacheKey); }
        
        var cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
        cache.Set(cacheKey, code, cacheOptions);
        
        return code;
    }
    
    public bool ValidateCode(Guid userId, string code,CodeType type)
    {
        var cacheKey = $"{type}:{userId}";
        
        if (cache.TryGetValue(cacheKey, out string? cachedCode))
        {
            var boo = cachedCode == code;
            if (boo) cache.Remove(cacheKey);
            return boo;
        }
        
        return false;
    }
    
    public void RemoveCode(Guid userId,CodeType type) => cache.Remove($"{type}:{userId}");
    
}

