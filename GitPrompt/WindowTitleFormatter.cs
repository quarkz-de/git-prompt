using System;
using System.Collections.Generic;
using System.Text;

namespace GitPrompt
{
    public class WindowTitleFormatter : IPromptFormatter
    {
        public string Format(string branchIdentifier, string remoteBranchIdentifier, int ahead, int behind, List<string> states)
        {
            var result = new StringBuilder();
            result.Append('<').Append(branchIdentifier);
            if (!string.IsNullOrWhiteSpace(remoteBranchIdentifier))
                result.Append(" -> ").Append(remoteBranchIdentifier);
            result.Append('>');

            if (ahead != 0 && behind != 0)
                result.Append($" [{ahead} ahead, {behind} behind]");
            else if (ahead != 0)
                result.Append($" [{ahead} ahead]");
            else if (behind != 0)
                result.Append($" [{behind} behind]");

            if (states.Count > 0)
                result.Append(" ").Append(string.Join(" ", states));

            return result.ToString();
        }

        public string FormatNoRepository()
        {
            return string.Empty;
        }
    }
}