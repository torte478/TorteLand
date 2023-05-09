using Firebase.Database;

namespace TorteLand.Firebase.Integration;

internal interface IFirebaseClientFactory
{
    FirebaseClient Create();
}