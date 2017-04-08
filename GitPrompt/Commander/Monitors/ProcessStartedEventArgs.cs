using System;
using JetBrains.Annotations;

namespace GitPrompt.Commander.Monitors
{
    [PublicAPI]
    public class ProcessStartedEventArgs : ProcessEventArgs
    {
        [PublicAPI]
        public ProcessStartedEventArgs(DateTime timestamp, TimeSpan relativeTimestamp, [NotNull] IProcess process)
            : base(timestamp, relativeTimestamp, process)
        {
        }
    }
}