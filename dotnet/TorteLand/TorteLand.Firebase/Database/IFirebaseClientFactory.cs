using Firebase.Database;

namespace TorteLand.Firebase.Database;

internal interface IFirebaseClientFactory
{
    ValueTask<FirebaseClient> Create();
}