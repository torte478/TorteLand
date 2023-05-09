using System.Threading.Tasks;

namespace TorteLand.Firebase.Integration.Tokens;

internal interface IExpiredToken
{
    Task<ExpiredToken> Provide();
}