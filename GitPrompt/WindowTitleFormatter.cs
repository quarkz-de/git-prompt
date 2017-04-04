using System;
using System.Collections.Generic;
using System.Text;

namespace GitPrompt
{
    public class WindowTitleFormatter : IPromptFormatter
    {
        public string Format(string branchIdentifier, string remoteBranchIdentifier, int ahead, int behind, List<string> states, List<string> tags)
        {
            var result = new StringBuilder();
            result.Append(branchIdentifier);
            if (branchIdentifier.StartsWith("#"))
                result.Append(" (detached head)");

            if (ahead != 0 && behind != 0)
                result.Append($" [{ahead} ahead, {behind} behind]");
            else if (ahead != 0)
                result.Append($" [{ahead} ahead]");
            else if (behind != 0)
                result.Append($" [{behind} behind, can fast-forward]");

            if (states.Count > 0)
                result.Append(" ").Append(string.Join(" ", states));

            if (!string.IsNullOrWhiteSpace(remoteBranchIdentifier))
                result.Append(" -> ").Append(remoteBranchIdentifier);

            if (tags.Count > 0)
                result.Append(" = " + string.Join(", ", tags));

            return result.ToString();
        }

        public string FormatNoRepository()
        {
            return string.Empty;
        }
    }
}