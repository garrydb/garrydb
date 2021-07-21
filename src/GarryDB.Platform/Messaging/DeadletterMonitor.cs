using Akka.Actor;
using Akka.Event;

using Debug = System.Diagnostics.Debug;

namespace GarryDB.Platform.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    public class DeadletterMonitor : ReceiveActor
    {
        /// <summary>
        /// 
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