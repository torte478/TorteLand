using System.Collections.Generic;
using System.Linq;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts.Factories;
using TorteLand.Core.Contracts.Notebooks;

namespace TorteLand.Core.Notebooks;

internal sealed class NotebookFactory : INotebookFactory
{
    public INotebook Create(IReadOnlyCollection<Note> notes)
        => notes
           .OrderBy(x => x.Weight)
           .Select(x => x.Text)
           .ToList()
           ._(Maybe.Some<IReadOnlyCollection<string>>)
           ._(_ => new Notebook(_));
}