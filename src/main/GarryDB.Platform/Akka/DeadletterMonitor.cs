using Akka.Actor;
using Akka.Event;

using Debug = System.Diagnostics.Debug;

namespace GarryDb.Platform.Akka
{
    /// <summary>
    ///     Monitors deadletters in the Akka.
    /// </summary>
    internal sealed class DeadletterMonitor : ReceiveActor
    {
        /// <summary>
        ///     Initializes a new <see cref="DeadletterMonitor" />.
        /// </summary>
        public DeadletterMonitor()
        {
            Receive<DeadLetter>(dl => HandleDeadletter(dl));
        }

        private void HandleDeadletter(DeadLetter dl)
        {
            Debug.WriteLine($"DeadLetter captured: {dl.Message}, sender: {dl.Sender}, recipient: {dl.Recipient}");
        }
    }
}
