using System.Collections.Generic;
using System.Linq;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts.Factories;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Extensions;

namespace TorteLand.Core.Notebooks;

internal sealed class NotebookFactory : INotebookFactory
{
    public INotebook Create(IReadOnlyCollection<Note> notes)
        => notes
           .OrderByDescending(x => x.Weight)
           .Select(x => x.Text)
           .ToList()
           ._(Maybe.Some<IReadOnlyCollection<string>>)
           ._<Notebook>();
}