using Firebase.Database;

namespace TorteLand.Firebase.Database;

internal interface IFirebaseClientFactory
{
    Task<FirebaseClient> Create();
}