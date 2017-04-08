using System;
using JetBrains.Annotations;

namespace GitPrompt.Commander.Events
{
    [PublicAPI]
    public class ProcessExitedEvent : ProcessEvent
    {
        [PublicAPI]
        public ProcessExitedEvent(TimeSpan executionDuration, DateTime timestamp, int exitCode)
            : base(executionDuration, timestamp)
        {
            ExitCode = exitCode;
        }

        [PublicAPI]
        public int ExitCode
        {
            get;
        }

        [PublicAPI]
        public override string ToString()
        {
            return $"{base.ToString()}: <exited: {ExitCode}>";
        }
    }
}