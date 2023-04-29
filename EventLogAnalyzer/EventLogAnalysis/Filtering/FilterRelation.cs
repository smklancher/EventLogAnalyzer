using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLogAnalysis.Filtering
{
    //displayname
    // Func<string,string,bool>

    public abstract class FilterRelation
    {
        public abstract string DisplayName { get; }

        //public abstract string Description { get; }

        public virtual bool TestDates(DateTime? objectDate, DateTime? filterDate) => false;

        public abstract bool TestValues(string objectValue, string filterValue);
    }

    public class RelationContains : FilterRelation
    {
        public override string DisplayName { get => "Contains"; }

        public override bool TestValues(string objectValue, string filterValue) => objectValue.Contains(filterValue);
    }

    public class RelationExcludes : FilterRelation
    {
        public override string DisplayName { get => "Excludes"; }

        public override bool TestValues(string objectValue, string filterValue) => !objectValue.Contains(filterValue);
    }

    public class RelationGreaterThan : FilterRelation
    {
        public override string DisplayName { get => "Greater than"; }

        public override bool TestDates(DateTime? objectDate, DateTime? filterDate)
        {
            return objectDate > filterDate;
        }

        public override bool TestValues(string objectValue, string filterValue) => false;
    }

    public class RelationLessThan : FilterRelation
    {
        public override string DisplayName { get => "Less than"; }

        public override bool TestDates(DateTime? objectDate, DateTime? filterDate)
        {
            return objectDate < filterDate;
        }

        public override bool TestValues(string objectValue, string filterValue) => false;
    }
}