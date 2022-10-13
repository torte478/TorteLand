using System;

namespace TorteLand.Core.Contracts.Storage;

public record Transaction(
    Guid Id,
    string Text);