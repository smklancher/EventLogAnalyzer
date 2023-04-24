using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLogAnalysis.Filtering
{
    //applicable object: class or interface the filter can apply to

    //relation: way of comparing object to value
    //value: with relation determines if filter is met for input object
    //action: what to do if met (include/exclude/maybe highlight/etc)

    public class Filter
    {
        public FilterAction Action { get; set; } = FilterAction.Include;
        public Type AppliesTo => Column.Type;

        //object value function: Func<object type,string>
        // the column list of the filter window is actually whatever this is
        // so needs to be it's own type and more fleshed out
        // probably "AppliesTo" becomes part of this type instead of distinct
        public required FilterColumn Column { get; set; }

        public FilterRelation Relation { get; set; } = new RelationContains();

        public required string Value { get; set; }

        public void test()
        {
            //var x = List<IList>();
            var obj = new object();
            var filters = new List<Filter>();
            foreach (var filter in filters)
            {
                var meetsFilter = filter.TestObject(obj);
            }
        }

        public bool TestObject(object obj)
        {
            var objValue = Column.ObjectValue(obj).ToLower();

            return Relation.TestValues(objValue, Value.ToLower());
        }
    }
}