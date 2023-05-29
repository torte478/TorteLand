using System;

namespace TorteLand.Contracts;

public interface IClock
{
    DateTimeOffset ToNow();
}