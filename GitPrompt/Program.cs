using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using LibGit2Sharp;

using static GitPrompt.ReSharperHelpers;

namespace GitPrompt
{
    class Program
    {
        static void Main(string[] args)
        {
            bool isPrompt = args?.Contains("--prompt") ?? false;
            IPromptFormatter formatter;

            if (isPrompt)
                formatter = new ConsolePromptFormatter();
            else
                formatter = new WindowTitleFormatter();

            using (var repository = GetCurrentRepository())
            {
                if (repository == null)
                    Console.WriteLine(formatter.FormatNoRepository());
                else
                {
                    string branchIdentifier = GetBranchIdentifier(repository);
                    string remoteBranchIdentifier = GetRemoteBranchIdentifier(repository);
                    var (ahead, behind) = GetAheadBehind(repository);
                    var states = GetRepositoryStates(repository);

                    var tags = GetTagsForHead(repository);
                    var promptString = formatter.Format(branchIdentifier, remoteBranchIdentifier, ahead, behind, states, tags);
                    Console.WriteLine(promptString);
                }
            }
        }

        [NotNull]
        private static List<string> GetTagsForHead([NotNull] Repository repository)
        {
            var id = repository.Head?.Tip?.Id;
            if (id == null)
                return new List<string>();

            assume(repository.Tags != null);

            return (from tag in repository.Tags
                    where tag.PeeledTarget?.Id?.Equals(id) ?? false
                    select tag.FriendlyName).ToList();
        }

        [NotNull]
        private static string GetRemoteBranchIdentifier([NotNull] Repository repository)
        {
            if (repository.Info?.IsBare ?? false)
                return string.Empty;
            if (!repository.Head?.IsTracking ?? false)
                return string.Empty;

            return repository.Head?.TrackedBranch?.FriendlyName ?? string.Empty;
        }

        [NotNull, ItemNotNull]
        private static List<string> GetRepositoryStates([NotNull] Repository repository)
        {
            var states = new List<string>();

            if (repository.Info?.IsBare ?? false)
                return states;

            switch (repository.Info?.CurrentOperation ?? CurrentOperation.None)
            {
                case CurrentOperation.None:
                case CurrentOperation.ApplyMailbox:
                case CurrentOperation.ApplyMailboxOrRebase:
                case CurrentOperation.Revert:
                case CurrentOperation.RevertSequence:
                    break;

                case CurrentOperation.Merge:
                    states.Add("MERGE");
                    break;

                case CurrentOperation.CherryPick:
                case CurrentOperation.CherryPickSequence:
                    states.Add("CHERRY");
                    break;

                case CurrentOperation.Bisect:
                    states.Add("BISECT");
                    break;

                case CurrentOperation.Rebase:
                case CurrentOperation.RebaseInteractive:
                case CurrentOperation.RebaseMerge:
                    states.Add("REBASE");
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!repository.Index?.IsFullyMerged ?? false)
                states.Add("CONFLICT");

            return states;
        }

        private static (int ahead, int behind) GetAheadBehind([NotNull] Repository repository)
        {
            if (!repository.Head?.IsTracking ?? false)
                return (0, 0);
            return (repository.Head?.TrackingDetails?.AheadBy ?? 0, repository.Head?.TrackingDetails?.BehindBy ?? 0);
        }

        [CanBeNull]
        private static Repository GetCurrentRepository()
        {
            string path = Environment.CurrentDirectory;
            bool isRoot = Path.GetFullPath(path).ToUpperInvariant() == Path.GetFullPath(Path.Combine(path, "..")).ToUpperInvariant();
            if (!isRoot)
                path = Repository.Discover(Environment.CurrentDirectory);
            if (path == null)
                return null;
            try
            {
                return new Repository(path);
            }
            catch (RepositoryNotFoundException)
            {
                return null;
            }
        }

        [NotNull]
        private static string GetBranchIdentifier([NotNull] Repository repository)
        {
            if (repository.Info?.IsBare ?? false)
                return "BARE";

            string branchName = repository.Head?.FriendlyName;
            if (branchName == null || branchName == "(no branch)")
            {
                var sha = repository.Head?.Tip?.Sha?.Substring(0, 8);
                if (sha != null)
                    return "#" + sha;

                return "HEAD";
            }

            return branchName;
        }
    }
}
