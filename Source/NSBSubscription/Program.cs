using System;
using System.Configuration;
using System.Linq;
using System.Net;
using Miracle.Arguments;
using Raven.Abstractions.Extensions;
using Raven.Client;
using Raven.Client.Document;

namespace Miracle.NSBSubscription
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var arguments = args.ParseCommandLine<Arguments.Arguments>();
            if (arguments != null)
            {
                try
                {
                    using (var store = new DocumentStore {Url = arguments.RavenUrl})
                    {
                        store.Conventions.FindTypeTagName = type => type.Name;
                        store.Conventions.FindClrTypeName = type => ConfigurationManager.AppSettings["Raven-Clr-Type"];

                        store.Initialize();

                        Scan(store, arguments);
                    }
                }
                catch (WebException ex)
                {
                    Console.Error.WriteLine("Unable to connect to Raven database");
                    Console.Error.WriteLine(ex.ToString());
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.ToString());
                }
            }
        }

        private static void Scan(DocumentStore store, Arguments.Arguments arguments)
        {
            var databases = store.DatabaseCommands.GetDocuments(start: 0, pageSize: 1024, metadataOnly: true)
                .Where(x=>x.Key.StartsWith("Raven/Databases/"))
                .Select(x => x.Key.Split('/').Last())
                .OrderBy(x => x)
                .ToArray();

            arguments.SubscriptionCommands.ForEach(cmd => cmd.Start());

            foreach (var database in databases)
            {
                using (IDocumentSession session = store.OpenSession(database))
                {
                    session.Advanced.UseOptimisticConcurrency = true;
                    session.Advanced.AllowNonAuthoritativeInformation = false;

                    arguments.SubscriptionCommands.ForEach(cmd => cmd.StartSession(session));

                    if (arguments.Trace)
                    {
                        Console.WriteLine("Querying database {0}.", database);
                    }
                    var subscriptions = session.Query<Subscription>().ToList();
                    foreach (var subscription in subscriptions)
                    {
                        arguments.SubscriptionCommands.ForEach(cmd => cmd.Do(session, database, subscription));
                    }

                    arguments.SubscriptionCommands.ForEach(cmd => cmd.EndSession(session));
                }
            }

            arguments.SubscriptionCommands.ForEach(cmd => cmd.End());
        }
    }
}