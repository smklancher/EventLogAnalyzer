using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLogAnalysis.Filtering
{
    public class Filter
    {
        /// <summary>
        /// action: what to do if met (include/exclude/maybe highlight/etc)
        /// </summary>
        public FilterAction Action { get; set; } = FilterAction.Include;

        /// <summary>
        /// applicable object: class or interface the filter can apply to
        /// </summary>
        public Type AppliesTo => Column.Type;

        /// <summary>
        /// knows the type and how to get the relevant value from the type instance
        /// </summary>
        public required FilterColumn Column { get; set; }

        public DateTime? DateValue { get; set; }

        /// <summary>
        /// relation: way of comparing object to value
        /// </summary>
        public FilterRelation Relation { get; set; } = new RelationContains();

        /// <summary>
        /// value: with relation determines if filter is met for input object
        /// </summary>
        public required string Value { get; set; }

        public bool TestObject(object obj)
        {
            if (Column.IsDate && DateValue.HasValue)
            {
                var objValue = Column.DateValue(obj);

                return Relation.TestDates(objValue, DateValue);
            }
            else
            {
                var objValue = Column.ObjectValue(obj).ToLower();

                return Relation.TestValues(objValue, Value.ToLower());
            }
        }

        public override string ToString()
        {
            return $"{Action} {Column.DisplayName} {Relation.DisplayName} {Value}";
        }
    }
}