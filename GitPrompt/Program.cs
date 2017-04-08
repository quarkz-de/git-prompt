using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

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

            var repoRoot = FindRepositoryRoot(Environment.CurrentDirectory);
            if (repoRoot == null)
                Console.WriteLine(formatter.FormatNoRepository());
            else
            {
                var repo = new Repository(repoRoot);
                var branchIdentifier = repo.GetCurrentBranch();
                string remoteBranchIdentifier = repo.GetUpstreamBranch();
                var (ahead, behind) = repo.GetAheadBehindInformation(branchIdentifier, remoteBranchIdentifier);
                var states = GetRepositoryStates(repoRoot, repo);

                var promptString = formatter.Format(branchIdentifier, remoteBranchIdentifier, ahead, behind, states);
                Console.WriteLine(promptString);
            }
        }

        [NotNull]
        private static List<string> GetRepositoryStates([NotNull] string repositoryPath, [NotNull] Repository repository)
        {
            var gitObjectPath = Path.Combine(repositoryPath, ".git");
            if (File.Exists(gitObjectPath))
                return GetStatesOfWorkTree(gitObjectPath, repository);
            return GetStatesOfRepository(gitObjectPath, repository);
        }

        [NotNull]
        private static List<string> GetStatesOfWorkTree([NotNull] string gitObjectPath, [NotNull] Repository repository)
        {
            var gitdir = File.ReadAllText(gitObjectPath);
            var ma = Regex.Match(gitdir, @"^gitdir:\s+(?<path>.*)$");
            if (!ma.Success)
                return new List<string>();

            var workTreeDir = ma.Groups["path"]?.Value;
            if (string.IsNullOrWhiteSpace(workTreeDir))
                return new List<string>();

            return GetStatesOfRepository(workTreeDir, repository);
        }

        [NotNull]
        private static List<string> GetStatesOfRepository([NotNull] string repositoryPath, [NotNull] Repository repository)
        {
            var result = new List<string>();
            if (File.Exists(Path.Combine(repositoryPath, "MERGE_HEAD")))
            {
                result.Add("MERGE");
                if (repository.HasUnmergedChanges())
                    result.Add("CONFLICT");
            }
            if (Directory.Exists(Path.Combine(repositoryPath, "rebase-apply")) || Directory.Exists(Path.Combine(repositoryPath, "rebase-merge")))
            {
                result.Add("REBASE");
                if (repository.HasUnmergedChanges())
                    result.Add("CONFLICT");

            }
            return result;
        }

        [CanBeNull]
        private static string FindRepositoryRoot([NotNull] string startingPath)
        {
            var path = Path.GetFullPath(startingPath);
            while (true)
            {
                var gitObjectPath = Path.Combine(path, ".git");
                if (Directory.Exists(gitObjectPath) || File.Exists(gitObjectPath))
                    return path;

                var parentPath = Path.GetFullPath(Path.Combine(path, ".."));
                if (parentPath == path)
                    return null;
                path = parentPath;
            }

        }
    }
}
