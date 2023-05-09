using System;

namespace TorteLand.Core;

public interface IClock
{
    DateTimeOffset ToNow();
}