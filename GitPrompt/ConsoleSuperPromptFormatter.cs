using System.Collections.Generic;
using System.Text;

namespace GitPrompt
{
    internal class ConsoleSuperPromptFormatter : IPromptFormatter
    {
        public string Format(string branchIdentifier, string remoteBranchIdentifier, int ahead, int behind, List<string> states)
        {
            var result = new StringBuilder();
            result.Append('(');
            result.Append(branchIdentifier);
            if (!string.IsNullOrWhiteSpace(remoteBranchIdentifier))
                result.Append(" -> ").Append(remoteBranchIdentifier);

            if (ahead != 0 && behind != 0)
                result.Append($" +{ahead}/-{behind}");
            else if (ahead != 0)
                result.Append($" +{ahead}");
            else if (behind != 0)
                result.Append($" -{behind}*");

            if (states.Count > 0)
                result.Append(" ").Append(string.Join(" ", states));
            result.Append(')');

            return result.ToString().Trim();
        }

        public string FormatNoRepository()
        {
            return string.Empty;
        }
    }
}