using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Squad.Bot.Constants
{
    public class AuthOptions
    {
        public const string ISSUER = "SquadBot"; // Token owner
        public const string AUDIENCE = "MyAuthClient"; // Token audience
        private readonly static string? KEY = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                                                                       .Build()
                                                                       .GetValue<string?>("Key");   // Key for ecnryptions
#pragma warning disable CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
        public static SymmetricSecurityKey? GetSymmetricSecurityKey() =>
            new(Encoding.UTF8.GetBytes(KEY));
#pragma warning restore CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
    }
}
