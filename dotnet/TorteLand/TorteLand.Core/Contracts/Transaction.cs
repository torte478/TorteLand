using System;

namespace TorteLand.Core.Contracts;

public record Transaction(
    Guid Id,
    string Text);