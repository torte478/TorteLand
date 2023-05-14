using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts.Factories;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Contracts.Notebooks.Models;
using TorteLand.Extensions;

namespace TorteLand.Core.Notebooks;

internal sealed class NotebookFactory : INotebookFactory
{
    private readonly byte _pluses;

    public NotebookFactory(IOptions<NotebookSettings> settings)
    {
        _pluses = settings.Value.Pluses;
    }

    public INotebook Create(IReadOnlyCollection<Note> notes)
        => notes
           .OrderByDescending(x => x.Weight)
           .Select(x => (x.Text, x.Pluses.Match(_ => _, () => default)))
           .ToList()
           ._(Maybe.Some<IReadOnlyCollection<(string, byte)>>)
           ._(_ => new Notebook(_, _pluses));
}