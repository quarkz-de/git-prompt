using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Commander;
using Commander.Events;
using Commander.Monitors;
using JetBrains.Annotations;

namespace GitPrompt
{
    public class Repository
    {
        [NotNull]
        private readonly string _Path;

        public Repository([NotNull] string path)
        {
            _Path = path ?? throw new ArgumentNullException(nameof(path));
        }

        [NotNull]
        public string GetCurrentBranch()
        {
            var output = ExecuteGit("rev-parse --abbrev-ref HEAD");
            if (output.Count == 0)
                return string.Empty;

            if (output[0] != "HEAD")
                return output[0];

            output = ExecuteGit("rev-parse HEAD");
            if (output.Count == 0)
                return string.Empty;
            return output[0];
        }

        [NotNull, ItemNotNull]
        private List<string> ExecuteGit(string arguments)
        {
            var psi = new ProcessStartInfo("git.exe", arguments)
            {
                CreateNoWindow = true,
                WorkingDirectory = _Path
            };
            var monitor = new ProcessCollectOutputMonitor();
            var exitCode = ProcessEx.ExecuteAsync(psi, monitor).Result;
            if (exitCode != 0)
                return new List<string>();
            return monitor.Events.OfType<ProcessOutputEvent>().Select(evt => evt.Line).ToList();
        }

        [NotNull]
        public string GetUpstreamBranch()
        {
            var output = ExecuteGit("rev-parse --abbrev-ref HEAD@{u}");
            if (output.Count == 0)
                return string.Empty;
            return output[0];
        }

        public (int ahead, int behind) GetAheadBehindInformation(string branchIdentifier, string remoteBranchIdentifier)
        {
            var output = ExecuteGit($"rev-list --left-right --count \"{branchIdentifier}...{remoteBranchIdentifier}\"");
            if (output.Count == 0)
                return (0, 0);

            var parts = output[0].Split('\t');
            if (parts.Length != 2)
                return (0, 0);

            if (!int.TryParse(parts[0], out int ahead))
                return (0, 0);
            if (!int.TryParse(parts[1], out int behind))
                return (0, 0);

            return (ahead, behind);
        }

        public bool HasUnmergedChanges()
        {
            return ExecuteGit("diff --name-only --diff-filter=U").Count > 0;
        }
    }
}