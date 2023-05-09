using System.Collections.Generic;
using TorteLand.Core.Contracts.Notebooks;

namespace TorteLand.Core.Contracts.Factories;

public interface IQuestionableNotebookFactory
{
    IQuestionableNotebook Create(IReadOnlyCollection<Note> notes);
}