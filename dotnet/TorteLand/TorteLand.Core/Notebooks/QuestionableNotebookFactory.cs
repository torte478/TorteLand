using System.Collections.Generic;
using TorteLand.Core.Contracts.Factories;
using TorteLand.Core.Contracts.Notebooks;
using TorteLand.Extensions;

namespace TorteLand.Core.Notebooks;

internal sealed class QuestionableNotebookFactory : IQuestionableNotebookFactory
{
    private readonly INotebookFactory _factory;

    public QuestionableNotebookFactory(INotebookFactory factory)
    {
        _factory = factory;
    }

    public IQuestionableNotebook Create(IReadOnlyCollection<Note> notes)
        => notes
           ._(_factory.Create)
           ._<QuestionableNotebook>();
}