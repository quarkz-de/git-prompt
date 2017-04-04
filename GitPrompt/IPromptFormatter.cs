using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace GitPrompt
{
    public interface IPromptFormatter
    {
        string Format([NotNull] string branchIdentifier, [NotNull] string remoteBranchIdentifier, int ahead, int behind, [NotNull] List<string> states, [NotNull] List<string> tags);

        string FormatNoRepository();
    }
}
