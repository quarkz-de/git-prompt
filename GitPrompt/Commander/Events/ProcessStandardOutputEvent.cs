using System;
using JetBrains.Annotations;

namespace GitPrompt.Commander.Events
{
    [PublicAPI]
    public class ProcessStandardOutputEvent : ProcessOutputEvent
    {
        [PublicAPI]
        public ProcessStandardOutputEvent(TimeSpan relativeTimestamp, DateTime timestamp, [NotNull] string line)
            : base(relativeTimestamp, timestamp, line)
        {
        }
    }
}