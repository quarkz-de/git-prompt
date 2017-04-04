using System;
using System.Collections.Generic;
using System.Text;

namespace GitPrompt
{
    public class ConsolePromptFormatter : IPromptFormatter
    {
        public string Format(string branchIdentifier, string remoteBranchIdentifier, int ahead, int behind, List<string> states)
        {
            var result = new StringBuilder();
            //result.Append('<').Append(branchIdentifier).Append('>');
            result.Append(branchIdentifier);

            if (ahead != 0 && behind != 0)
                result.Append($" +{ahead}/-{behind}");
            else if (ahead != 0)
                result.Append($" +{ahead}");
            else if (behind != 0)
                result.Append($" -{behind}");

            if (states.Count > 0)
                result.Append(" ").Append(string.Join(" ", states));

            result.Append(" $");
            return result.ToString().Trim();
        }

        public string FormatNoRepository()
        {
            return "$";
        }
    }
}