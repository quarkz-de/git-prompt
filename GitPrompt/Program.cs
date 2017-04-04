using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp;

// ReSharper disable PossibleNullReferenceException

namespace GitPrompt
{
    class Program
    {
        static void Main(string[] args)
        {
            bool isPrompt = args?.Contains("--prompt") ?? false;

            string prompt = "";
            if (isPrompt)
                prompt = " $";

            try
            {
                string path = Environment.CurrentDirectory;
                bool isRoot = Path.GetFullPath(path).ToUpperInvariant() == Path.GetFullPath(Path.Combine(path, "..")).ToUpperInvariant();
                if (!isRoot)
                    path = Repository.Discover(Environment.CurrentDirectory);
                if (path == null)
                {
                    Console.WriteLine(prompt.Trim());
                    return;
                }

                var repo = new Repository(path);
                if (repo.Info.IsBare)
                {
                    Console.WriteLine("BARE" + prompt);
                    return;
                }

                string branchName = repo.Head.FriendlyName;
                if (branchName == "(no branch)")
                    branchName = "HEAD";

                string result = branchName;

                var ahead = repo.Head.TrackingDetails.AheadBy ?? 0;
                var behind = repo.Head.TrackingDetails.BehindBy ?? 0;

                bool isAhead = ahead != 0;
                bool isBehind = behind != 0;

                if (isAhead || isBehind)
                {
                    if (isPrompt)
                        result += " ";
                    else
                        result += " [";

                    if (isAhead)
                    {
                        if (isPrompt)
                            result += "+" + ahead;
                        else
                            result += ahead + " ahead";
                    }
                    if (isAhead && isBehind)
                    {
                        if (isPrompt)
                            result += "/";
                        else
                            result += ", ";
                    }
                    if (isBehind)
                    {
                        if (isPrompt)
                        {
                            result += "-" + behind;
                        }
                        else
                        {
                            result += behind + " behind";
                        }
                    }
                    if (!isPrompt)
                        result += "]";
                }

                var states = new List<string>();
                switch (repo.Info.CurrentOperation)
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
                        states.Add("CHERRY-PICK");
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
                if (!repo.Index.IsFullyMerged)
                    states.Add("CONFLICT");

                if (states.Count > 0)
                {
                    result += isPrompt ? " " : " ** ";
                    result += string.Join(" ", states);
                    result += isPrompt ? string.Empty : " **";
                }

                Console.WriteLine((result + prompt).Trim());
            }
            catch (RepositoryNotFoundException)
            {
                Console.WriteLine(prompt.Trim());
            }
        }
    }
}
