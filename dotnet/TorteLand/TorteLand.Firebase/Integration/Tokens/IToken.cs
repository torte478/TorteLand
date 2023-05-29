using System.Threading.Tasks;

namespace TorteLand.Firebase.Integration.Tokens;

internal interface IToken
{
    Task<string> Provide();
}