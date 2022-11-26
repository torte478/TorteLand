using System.Threading.Tasks;
using Firebase.Database;

namespace TorteLand.Firebase.Integration;

internal interface IFirebaseClientFactory
{
    Task<FirebaseClient> Create();
}