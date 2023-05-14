using System.Collections.Generic;
using SoftwareCraft.Functional;
using TorteLand.Core.Contracts.Storage;

namespace TorteLand.Core.Contracts.Notebooks.Models;

public sealed record AddNotesIteration(
    IQuestionableNotebook Notebook,
    Either<IReadOnlyCollection<int>, Question> Result);