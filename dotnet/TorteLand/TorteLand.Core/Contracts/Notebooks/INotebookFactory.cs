using System.Collections.Generic;

namespace TorteLand.Core.Contracts.Notebooks;

public interface INotebookFactory
{
    INotebook Create(IReadOnlyCollection<Note> notes);
}