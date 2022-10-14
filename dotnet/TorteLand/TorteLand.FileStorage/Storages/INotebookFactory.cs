using TorteLand.Core.Contracts.Notebooks;

namespace TorteLand.FileStorage.Storages;

internal interface INotebookFactory
{
    IQuestionableNotebook Create(string path);
}