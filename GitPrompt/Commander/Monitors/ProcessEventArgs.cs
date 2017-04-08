using System;
using JetBrains.Annotations;

namespace GitPrompt.Commander.Monitors
{
    [PublicAPI]
    public abstract class ProcessEventArgs : EventArgs
    {
        [PublicAPI]
        protected ProcessEventArgs(DateTime timestamp, TimeSpan relativeTimestamp, [NotNull] IProcess process)
        {
            Timestamp = timestamp;
            RelativeTimestamp = relativeTimestamp;
            Process = process ?? throw new ArgumentNullException(nameof(process));
        }

        [PublicAPI]
        public DateTime Timestamp
        {
            get;
        }

        [PublicAPI]
        public TimeSpan RelativeTimestamp
        {
            get;
        }

        [PublicAPI, NotNull]
        public IProcess Process
        {
            get;
        }
    }
}