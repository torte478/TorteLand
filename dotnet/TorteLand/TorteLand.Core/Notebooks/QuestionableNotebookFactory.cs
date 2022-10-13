using TorteLand.Core.Contracts.Factories;
using TorteLand.Core.Contracts.Notebooks;

namespace TorteLand.Core.Notebooks;

internal sealed class QuestionableNotebookFactory : IQuestionableNotebookFactory
{
    public IQuestionableNotebook Create(IAsyncNotebook origin)
        => new QuestionableNotebook(origin);
}