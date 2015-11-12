using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using RexReplace.GUI.UserControls;

namespace RexReplace.GUI.Logic
{
    [Serializable]
    public class RuleSet
    {
        readonly List<Rule> Rules = new List<Rule>();

        public void AddRule(Rule rule)
        {
            Rules.Add(rule);
        }

        public string Run(string source)
        {
            StringBuilder sb = new StringBuilder(source.Length);
            if (_interpreter == null) _interpreter = new ReplaceExpression();

            foreach (Rule rule in Rules)
            {
                int index = 0;
                var mc = rule.Match.Matches(source);
                IEnumerator<Match> sorted = SortMatches(rule.SortingOptions, rule.SortingGroup, mc);

                int count = mc.Count;
                for (int j = 0; j < count; j++)
                {
                    sorted.MoveNext();
                    Match m = mc[j];
                    if (!rule.DiscardUnmatchedText)
                    {
                        int prevcount = m.Index - index;
                        if (prevcount != 0) sb.Append(source.Substring(index, prevcount));
                    }
                    try
                    {
                        string replacement = _interpreter.Execute(rule.Replacement, sorted.Current.Groups, j, count);
                        sb.Append(replacement);
                    }
                    catch (RuleException rex)
                    {
                        rex.RuleId = Rules.IndexOf(rule) + 1;
                        throw;
                    }

                    index = m.Index + m.Value.Length;
                }
                if (!rule.DiscardUnmatchedText && index < source.Length) sb.Append(source.Substring(index));
                source = sb.ToString();
                sb.Clear();
            }

            return source;
        }

        private static IEnumerator<Match> SortMatches(SortingOptions sortBy, int sortGroup, MatchCollection mc)
        {
            IEnumerator<Match> sorted = null;
            switch (sortBy)
            {
                case SortingOptions.Asc:
                    sorted = mc.Cast<Match>().OrderBy(x => x.Groups[sortGroup].Value).GetEnumerator();
                    break;
                case SortingOptions.Desc:
                    sorted = mc.Cast<Match>().OrderBy(x => x.Groups[sortGroup].Value).Reverse().GetEnumerator();
                    break;
                case SortingOptions.NumberAsc:
                case SortingOptions.NumberDesc:
                    IEnumerable<Match> query = mc.Cast<Match>().OrderBy(x =>
                    {
                        string str = x.Groups[sortGroup].Value;
                        int i = 0;
                        while (i < str.Length && Char.IsDigit(str[i]))
                        {
                            i++;
                        }
                        str = str.Substring(0, i);
                        if (str.Length > 0)
                            return int.Parse(str);
                        else
                            return int.MaxValue;

                    }).ThenBy(x => x.Groups[sortGroup].Value);
                    if (sortBy == SortingOptions.NumberDesc)
                    {
                        query = query.Reverse();
                    }
                    sorted = query.GetEnumerator();
                    break;
                case SortingOptions.DatetimeAsc:
                case SortingOptions.DatetimeDesc:
                    var qry = mc.Cast<Match>().OrderBy<Match, DateTime>(x =>
                    {
                        DateTime retval;
                        if (!DateTime.TryParse(x.Groups[sortGroup].Value, out retval))
                        {
                            retval = DateTime.MaxValue;
                        }
                        return retval;

                    });
                    if (sortBy == SortingOptions.DatetimeDesc)
                    {
                        sorted = qry.Reverse().GetEnumerator();
                    }
                    else
                    {
                        sorted = qry.GetEnumerator();
                    }

                    break;
                case SortingOptions.Reverse:
                    sorted = (mc.Cast<Match>()).Reverse().GetEnumerator();
                    break;
                default:
                    sorted = (mc.Cast<Match>()).GetEnumerator();
                    break;
            }
            return sorted;
        }

        public Rule At(int index)
        {
            return Rules[index];
        }

        public int Count
        {
            get
            {
                return Rules.Count;
            }
        }

        [NonSerialized]
        private ReplaceExpression _interpreter;
    }

}
