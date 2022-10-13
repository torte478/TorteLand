using System.Collections.Generic;
using TorteLand.Core.Contracts.Notebooks;

namespace TorteLand.Core.Contracts.Factories;

public interface INotebookFactory
{
    INotebook Create(IReadOnlyCollection<Note> notes);
}