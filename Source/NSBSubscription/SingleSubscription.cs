using System;

namespace Miracle.NSBSubscription
{
    public enum Part
    {
        Machine,
        Queue,
        Database,
        MessageType,
        Namespace,
        TypeName
    }

    public class SingleSubscription
    {
        public string Database { get; private set; }
        public string MessageType { get; private set; }
        public string Machine { get; private set; }
        public string Queue { get; private set; }
        public string TypeName{ get; private set; }
        public string Namespace{ get; private set; }

        public SingleSubscription(string database, string messageType, string machine, string queue)
        {
            Database = database;
            MessageType = messageType;
            Machine = machine;
            Queue = queue;

            var commaPos = messageType.IndexOf(',');
            if(commaPos != -1)
            {
                var dotPos = messageType.LastIndexOf('.', commaPos);
                if (dotPos != -1)
                {
                    Namespace = messageType.Substring(0, dotPos - 1);
                    TypeName = messageType.Substring(dotPos + 1, commaPos - dotPos - 1);
                }
            }
        }

        public string Get(Part part)
        {
            switch (part)
            {
                case Part.Machine:
                    return Machine;
                case Part.Queue:
                    return Queue;
                case Part.Database:
                    return Database;
                case Part.MessageType:
                    return MessageType;
                case Part.TypeName:
                    return TypeName;
                case Part.Namespace:
                    return Namespace;
                default:
                    throw new ArgumentOutOfRangeException("part");
            }
        }
    }
}