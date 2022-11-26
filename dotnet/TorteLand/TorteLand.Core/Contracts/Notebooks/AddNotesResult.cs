using System.Collections.Generic;

namespace TorteLand.Core.Contracts.Notebooks;

public record AddNotesResult(
    IReadOnlyCollection<int> Indices,
    INotebook Notebook);