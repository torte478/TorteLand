using TorteLand.Core.Contracts.Notebooks;

namespace TorteLand.Firebase.Database;

internal interface INotebookFactory
{
    IQuestionableNotebook Create(string key);
}