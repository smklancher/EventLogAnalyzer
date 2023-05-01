using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLogAnalysis.Filtering
{
    public static class KnownFilter
    {
        public static Dictionary<string, FilterAction> Actions { get; } = new Dictionary<string, FilterAction>();
        public static Dictionary<string, FilterColumn> Columns { get; } = new Dictionary<string, FilterColumn>();
        public static Dictionary<string, FilterRelation> Relations { get; } = new Dictionary<string, FilterRelation>();

        public static void Init()
        {
            var types = typeof(KnownFilter).Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(FilterRelation)));
            foreach (var type in types)
            {
                var inst = Activator.CreateInstance(type);
                if (inst != null)
                {
                    var rel = (FilterRelation)inst;
                    Relations.Add(rel.DisplayName, rel);
                }
            }

            foreach (var a in Enum.GetValues(typeof(FilterAction)))
            {
                if (a != null)
                {
                    Actions.Add(a.ToString(), (FilterAction)a);
                }
            }
        }
    }
}