using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLogAnalysis.Filtering
{
    public class Filter
    {
        private string strValue = string.Empty;

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

        public DateTime? DateValue { get; private set; }
        public List<DateTime> DateValues { get; private set; } = new List<DateTime>();
        public Color HighlightColor { get; set; } = Color.LightYellow;
        public bool IsQuickFilter { get; set; } = false;

        /// <summary>
        /// relation: way of comparing object to value
        /// </summary>
        public FilterRelation Relation { get; set; } = new RelationContains();

        /// <summary>
        /// value: with relation determines if filter is met for input object
        /// </summary>
        public required string Value
        {
            get { return strValue; }
            set
            {
                strValue = value.ToLower();

                if (DateTime.TryParse(value, out var date))
                {
                    DateValue = date;
                }

                // set possible compound values
                Values = value.Split(',').Select(x => x.ToLower()).ToList();

                DateValues.Clear();
                foreach (var str in Values)
                {
                    if (DateTime.TryParse(str, out var date2))
                    {
                        DateValues.Add(date2);
                    }
                }
            }
        }

        public List<string> Values { get; private set; } = new List<string> { string.Empty };

        public bool TestObject(object obj)
        {
            if (Relation.UseComplexFilter)
            {
                return Relation.TestComplexFilter(obj, this);
            }

            if (Column.IsDate && DateValue.HasValue)
            {
                // if the column and filter can both provide date values
                // then call the relation's date function.
                // If the relation doesn't implment one, this just returns false;
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