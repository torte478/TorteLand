using System.Collections.Generic;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Core.Contracts.Notebooks.Models;

namespace TorteLand.Core.Contracts.Factories;

public interface IQuestionableNotebookFactory
{
    IQuestionableNotebook Create(IReadOnlyCollection<Note> notes);
}