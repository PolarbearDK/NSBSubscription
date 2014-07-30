using System;
using System.Text.RegularExpressions;
using Miracle.Arguments;

namespace Miracle.NSBSubscription.Arguments
{
    public abstract class CriteriaBase
    {
        protected CriteriaBase()
        {
            StringComparison = StringComparison.CurrentCultureIgnoreCase;
        }

        public virtual void Initialize()
        {
        }

        public void Cleanup()
        {
        }

        public abstract bool Match(SingleSubscription subscription, Part messagePart);

        [ArgumentName("StringComparison")]
        [ArgumentDescription("StringComparison used for string compares. Default is CurrentCultureIgnoreCase.")]
        public StringComparison StringComparison { get; set; }
    }

    [ArgumentDescription("Check that message part matches a regular expression")]
    public class MatchCriteria : CriteriaBase
    {
        protected Regex Regex;

        [ArgumentPosition(0)]
        [ArgumentRequired]
        public string Pattern { get; set; }

        // In prepare convert pattern into compiled regular expressions
        public override void Initialize()
        {
            Regex = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }

        public override bool Match(SingleSubscription subscription, Part messagePart)
        {
            return Regex.IsMatch(subscription.Get(messagePart));
        }
    }

    [ArgumentDescription("Check that message part is like a simple wildcard expression")]
    public class LikeCriteria : MatchCriteria
    {
        // In prepare convert pattern into compiled regular expressions
        public override void Initialize()
        {
            Regex = new Wildcard(Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }
    }

    [ArgumentDescription("Check that part equals specified criteria")]
    public class EqualsCriteria : CriteriaBase
    {
        [ArgumentPosition(0)]
        [ArgumentRequired]
        public string Text { get; set; }

        public override bool Match(SingleSubscription subscription, Part part)
        {
            return subscription.Get(part).Equals(Text, StringComparison);
        }
    }

    [ArgumentDescription("Check if source contains a string")]
    public class ContainsCriteria : CriteriaBase
    {
        [ArgumentPosition(0)]
        [ArgumentRequired]
        public string Contains { get; set; }

        public override bool Match(SingleSubscription subscription, Part messagePart)
        {
            return subscription.Get(messagePart).IndexOf(Contains, StringComparison) != -1;
        }
    }

    public class WhereCommand
    {
        [ArgumentPosition(0)]
        [ArgumentRequired]
        public Part Part { get; set; }

        [ArgumentName("Not")]
        public bool Not { get; set; }

        [ArgumentCommand(typeof(EqualsCriteria), "Equals", "==", "=", "-eq")]
        [ArgumentCommand(typeof(LikeCriteria), "Like", "-like")]
        [ArgumentCommand(typeof(MatchCriteria), "Match", "Matches")]
        [ArgumentCommand(typeof(ContainsCriteria), "Contain", "Contains")]
        [ArgumentRequired]
        public CriteriaBase Criteria { get; set; }

        public virtual void Initialize()
        {
            Criteria.Initialize();
        }

        public virtual void Cleanup()
        {
            Criteria.Cleanup();
        }

        public bool Match(SingleSubscription subscription)
        {
            return Criteria.Match(subscription, Part) != Not;
        }
    }
}
