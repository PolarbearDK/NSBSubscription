using System.Collections.Generic;

namespace Miracle.NSBSubscription
{
    public class Subscription
    {
        public string Id { get; set; }
        public string MessageType { get; set; }
        public List<Address> Clients { get; set; }
    }
}