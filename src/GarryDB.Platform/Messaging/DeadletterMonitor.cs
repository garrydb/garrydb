using Akka.Actor;
using Akka.Event;

using Debug = System.Diagnostics.Debug;

namespace GarryDB.Platform.Messaging
{
    /// <summary>
    ///     Monitors deadletters in the Akka.
    /// </summary>
    public class DeadletterMonitor : ReceiveActor
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