﻿using TorteLand.Core.Contracts.Notebooks;

namespace TorteLand.Firebase.Database;

internal interface INotebookFactory
{
    Task<IQuestionableNotebook> Create(string key);
}