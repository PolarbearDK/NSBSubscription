using System;
using System.Collections.Generic;
using System.Linq;
using Miracle.Arguments;
using Raven.Abstractions.Extensions;
using Raven.Client;

namespace Miracle.NSBSubscription.Arguments
{
    public abstract class SubscriptionCommandBase
    {
        public virtual void Start()
        {
            if(Filters != null)
                Filters.ForEach(x=>x.Initialize());
        }

        public virtual void StartSession(IDocumentSession session)
        {
        }

        public abstract void Do(IDocumentSession session, string database, Subscription subscription);

        [ArgumentCommand(typeof(WhereCommand), "Where", "And")]
        public WhereCommand[] Filters { get; set; }

        public virtual bool Match(SingleSubscription context)
        {
            return Filters == null || Filters.All(x => x.Match(context));
        }

        public virtual void EndSession(IDocumentSession session)
        {
        }

        public virtual void End()
        {
            if (Filters != null)
                Filters.ForEach(x => x.Cleanup());
        }
    }

    public abstract class SubscriptionListCommandBase: SubscriptionCommandBase
    {
        protected readonly List<SingleSubscription> SubscriptionList = new List<SingleSubscription>();

        public override void Start()
        {
            base.Start();

            SubscriptionList.Clear();
        }

        public override void Do(IDocumentSession session, string database, Subscription subscription)
        {
            SubscriptionList.AddRange(
                subscription.Clients
                    .Select(cli => new SingleSubscription(database, subscription.MessageType, cli.Machine, cli.Queue))
                    .Where(Match)
                );
        }
    }

    [ArgumentDescription("Generate NSB subscription statistics.")]
    public class Statistics : SubscriptionListCommandBase
    {
        [ArgumentName("on")]
        [ArgumentRequired]
        public Part StatisticsType { get; set; }

        public override void End()
        {
            switch (StatisticsType)
            {
                case Part.Machine:
                    ColumnFormatter.WriteColumns(
                        Console.Out,
                        SubscriptionList
                            .GroupBy(x => x.Machine)
                            .OrderBy(x => x.Key),
                        new[]
                        {
                            new Tuple<string, Func<IGrouping<string, SingleSubscription>, object>>("Machine", x => x.Key),
                            new Tuple<string, Func<IGrouping<string, SingleSubscription>, object>>("Database", x => x.GroupBy(sub => sub.Database).Count()),
                            new Tuple<string, Func<IGrouping<string, SingleSubscription>, object>>("MessageTypes", x => x.GroupBy(sub => sub.MessageType).Count()),
                            new Tuple<string, Func<IGrouping<string, SingleSubscription>, object>>("Queues", x => x.GroupBy(sub => sub.Queue).Count()),
                        });
                    break;

                case Part.Queue:
                    ColumnFormatter.WriteColumns(
                        Console.Out,
                        SubscriptionList
                            .GroupBy(x => x.Queue)
                            .OrderBy(x => x.Key),
                        new[]
                        {
                            new Tuple<string, Func<IGrouping<string, SingleSubscription>, object>>("Queue", x => x.Key),
                            new Tuple<string, Func<IGrouping<string, SingleSubscription>, object>>("Machines", x => x.GroupBy(sub => sub.Machine).Count()),
                            new Tuple<string, Func<IGrouping<string, SingleSubscription>, object>>("Databases", x => x.GroupBy(sub => sub.Database).Count()),
                            new Tuple<string, Func<IGrouping<string, SingleSubscription>, object>>("MessageTypes", x => x.GroupBy(sub => sub.MessageType).Count()),
                            new Tuple<string, Func<IGrouping<string, SingleSubscription>, object>>("Namespaces", x => x.GroupBy(sub => sub.Namespace).Count()),
                        });
                    break;

                case Part.Database:
                    ColumnFormatter.WriteColumns(
                        Console.Out,
                        SubscriptionList
                            .GroupBy(x=>x.Database)
                            .OrderBy(x=>x.Key),
                        new[]
                        {
                            new Tuple<string, Func<IGrouping<string, SingleSubscription>, object>>("Database", x => x.Key),
                            new Tuple<string, Func<IGrouping<string, SingleSubscription>, object>>("Machines", x => x.GroupBy(sub => sub.Machine).Count()),
                            new Tuple<string, Func<IGrouping<string, SingleSubscription>, object>>("Queues", x => x.GroupBy(sub => sub.Queue).Count()),
                            new Tuple<string, Func<IGrouping<string, SingleSubscription>, object>>("MessageTypes", x => x.GroupBy(sub => sub.MessageType).Count()),
                            new Tuple<string, Func<IGrouping<string, SingleSubscription>, object>>("Namespaces", x => x.GroupBy(sub => sub.Namespace).Count()),
                        });
                    break;

                case Part.MessageType:
                    ColumnFormatter.WriteColumns(
                        Console.Out,
                        SubscriptionList
                            .GroupBy(x=>x.MessageType)
                            .OrderBy(x=>x.Key),
                        new[]
                        {
                            new Tuple<string, Func<IGrouping<string, SingleSubscription>, object>>("MessageType", x => x.Key),
                            new Tuple<string, Func<IGrouping<string, SingleSubscription>, object>>("Machines", x => x.GroupBy(sub => sub.Machine).Count()),
                            new Tuple<string, Func<IGrouping<string, SingleSubscription>, object>>("Databases", x => x.GroupBy(sub => sub.Database).Count()),
                            new Tuple<string, Func<IGrouping<string, SingleSubscription>, object>>("Queues", x => x.GroupBy(sub => sub.Queue).Count()),
                        });
                    break;

                case Part.TypeName:
                    ColumnFormatter.WriteColumns(
                        Console.Out,
                        SubscriptionList
                            .GroupBy(x => x.TypeName)
                            .OrderBy(x => x.Key),
                        new[]
                        {
                            new Tuple<string, Func<IGrouping<string, SingleSubscription>, object>>("TypeName", x => x.Key),
                            new Tuple<string, Func<IGrouping<string, SingleSubscription>, object>>("Machines", x => x.GroupBy(sub => sub.Machine).Count()),
                            new Tuple<string, Func<IGrouping<string, SingleSubscription>, object>>("Databases", x => x.GroupBy(sub => sub.Database).Count()),
                            new Tuple<string, Func<IGrouping<string, SingleSubscription>, object>>("Queues", x => x.GroupBy(sub => sub.Queue).Count()),
                            new Tuple<string, Func<IGrouping<string, SingleSubscription>, object>>("Namespaces", x => x.GroupBy(sub => sub.Namespace).Count()),
                        });
                    break;

                case Part.Namespace:
                    ColumnFormatter.WriteColumns(
                        Console.Out,
                        SubscriptionList
                            .GroupBy(x => x.Namespace)
                            .OrderBy(x => x.Key),
                        new[]
                        {
                            new Tuple<string, Func<IGrouping<string, SingleSubscription>, object>>("Namespace", x => x.Key),
                            new Tuple<string, Func<IGrouping<string, SingleSubscription>, object>>("Machines", x => x.GroupBy(sub => sub.Machine).Count()),
                            new Tuple<string, Func<IGrouping<string, SingleSubscription>, object>>("Databases", x => x.GroupBy(sub => sub.Database).Count()),
                            new Tuple<string, Func<IGrouping<string, SingleSubscription>, object>>("Queues", x => x.GroupBy(sub => sub.Queue).Count()),
                            new Tuple<string, Func<IGrouping<string, SingleSubscription>, object>>("MessageTypes", x => x.GroupBy(sub => sub.MessageType).Count()),
                        });
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    [ArgumentDescription("List NSB subscriptions.")]
    public class List : SubscriptionListCommandBase
    {
        [ArgumentName("LongMessageType","Long")]
        public bool LongMessageType { get; set; }

        public override void End()
        {
            if (SubscriptionList.Any())
            {
                ColumnFormatter.WriteColumns(
                    Console.Out,
                    SubscriptionList
                        .OrderBy(x => x.Machine).ThenBy(x => x.Queue).ThenBy(x => x.MessageType),
                    LongMessageType
                        ? new[]
                          {
                              new Tuple<string, Func<SingleSubscription, object>>("Database", x => x.Database),
                              new Tuple<string, Func<SingleSubscription, object>>("Machine", x => x.Machine),
                              new Tuple<string, Func<SingleSubscription, object>>("Queue", x => x.Queue),
                              new Tuple<string, Func<SingleSubscription, object>>("MessageType", x => x.MessageType),
                          }
                        : new[]
                          {
                              new Tuple<string, Func<SingleSubscription, object>>("Database", x => x.Database),
                              new Tuple<string, Func<SingleSubscription, object>>("Machine", x => x.Machine),
                              new Tuple<string, Func<SingleSubscription, object>>("Queue", x => x.Queue),
                              new Tuple<string, Func<SingleSubscription, object>>("TypeName", x => x.TypeName),
                              new Tuple<string, Func<SingleSubscription, object>>("Namespace", x => x.Namespace),
                          });
            }
            else
            {
                Console.WriteLine("No subscriptions matched.");
            }
        }
    }

    [ArgumentDescription("Delete NSB subscriptions matching criteria.")]
    public class Delete : SubscriptionCommandBase
    {
        private bool IsDirty;

        public override void StartSession(IDocumentSession session)
        {
            base.StartSession(session);

            IsDirty = false;
        }

        public override void Do(IDocumentSession session, string database, Subscription subscription)
        {
            bool isDirty = false;

            foreach (var client in subscription.Clients.ToArray())
            {
                var ss = new SingleSubscription(database, subscription.MessageType, client.Machine, client.Queue);
                if (Match(ss))
                {
                    subscription.Clients.Remove(client);
                    if (!isDirty)
                    {
                        Console.WriteLine("Deleting subscription(s) to type {0}", ss.MessageType);
                        isDirty = true;
                    }
                    Console.WriteLine("From {0}@{1}", ss.Queue, ss.Machine);
                }
            }

            if (isDirty)
            {
                if (!subscription.Clients.Any())
                    session.Delete(subscription);

                IsDirty = true;
            }
        }

        public override void EndSession(IDocumentSession session)
        {
            base.EndSession(session);

            if(IsDirty)
                session.SaveChanges();
        }
    }
}