using System.Text;
using Microsoft.Extensions.Configuration;

namespace ShortenUrl.Application.Utils;

public static class EncodeBase62
{
    private static IConfiguration _configuration;

    public static void Initialize(IConfiguration configuration)
        => _configuration = configuration;
    
    public static string Encode(long id)
    {
        string charBase62 = _configuration.GetValue<string>("Base62:CharacterSet") ?? throw new NullReferenceException("Base62:CharacterSet");

        var sb = new StringBuilder();
        while (id > 0)
        {
            var remainder = (int)(id % 62);
            id /= 62;    
            sb.Insert(0, charBase62[remainder]);
        }
        
        return sb.ToString();
    }
}