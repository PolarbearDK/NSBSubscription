namespace Miracle.NSBSubscription
{
    public class Address
    {
        public string Queue { get; private set; }
        public string Machine { get; private set; }

        public override string ToString()
        {
            return Queue + "@" + Machine;
        }
    }
}