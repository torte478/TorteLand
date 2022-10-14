using System;

namespace TorteLand.Core.Contracts.Storage;

public record Question(
    Guid Id,
    string Text);