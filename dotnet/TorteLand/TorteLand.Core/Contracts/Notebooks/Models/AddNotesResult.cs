using System.Collections.Generic;

namespace TorteLand.Core.Contracts.Notebooks.Models;

public record AddNotesResult(
    IReadOnlyCollection<int> Indices,
    INotebook Notebook);