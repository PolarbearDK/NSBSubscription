using System;
using Miracle.Arguments;

namespace Miracle.NSBSubscription.Arguments
{
    [ArgumentSettings(
        ArgumentNameComparison = StringComparison.InvariantCultureIgnoreCase,
        DuplicateArgumentBehaviour = DuplicateArgumentBehaviour.Unknown,
        StartOfArgument = new[] { '-' },
        ValueSeparator = new[] { ' ' },
        ShowHelpOnArgumentErrors = false
        )]
    [ArgumentDescription("NServiceBus subscription tool.")]
    public class Arguments
    {
        public Arguments()
        {
            RavenUrl = "http://localhost:8080";
        }

        [ArgumentName("RavenUrl", "Raven", "Url")]
        [ArgumentDescription("Address of RavenDB on the form http://server:port. Default is http://localhost:8080.")]
        public string RavenUrl { get; set; }

        [ArgumentName("Trace")]
        [ArgumentDescription("Output trace messages.")]
        public bool Trace { get; set; }

        [ArgumentName("Help", "H", "?")]
        [ArgumentHelp]
        [ArgumentDescription("Show help")]
        public bool Help { get; set; }

        [ArgumentCommand(typeof(List), "List", "L")]
        [ArgumentCommand(typeof(Statistics), "Statistics", "Stat")]
        [ArgumentCommand(typeof(Delete), "Delete", "Del")]
        public SubscriptionCommandBase[] SubscriptionCommands { get; set; }

        [ArgumentName("CommandHelp", "CH", "??")]
        [ArgumentCommandHelp]
        [ArgumentDescription(@"Display help for command.")]
        public string Command { get; set; }
    }
}