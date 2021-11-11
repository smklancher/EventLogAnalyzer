namespace EventLogAnalyzer;

public class TraitTypesListView
{
    private const string InternalLogName = "InternalLog";

    public TraitTypesListView(ListView lv, TraitValuesListView tvlv)
    {
        TraitValuesList = tvlv;
        list = lv;
        list.BeginUpdate();
        list.View = View.Details;
        list.FullRowSelect = true;
        list.Columns.Add("#");
        list.Columns.Add("Trait Type");

        list.SelectedIndexChanged += List_SelectedIndexChanged;
        //list.VirtualMode = true;
        //list.VirtualListSize = 0;

        //list.RetrieveVirtualItem += List_RetrieveVirtualItem;

        list.EndUpdate();
    }

    public TraitTypeCollection CurrentTraitTypes { get; private set; } = new();

    public TraitValuesListView TraitValuesList { get; }

    private ListView list { get; }

    public string SelectedTraitType()
    {
        if (list.SelectedIndices.Count < 1) { return string.Empty; }
        return list.SelectedItems[0].SubItems[1].Text;
    }

    public void UpdateTraitTypesSource(TraitTypeCollection traitTypes)
    {
        CurrentTraitTypes = traitTypes;

        list.Items.Clear();

        var typesAndCounts = CurrentTraitTypes.TraitTypesAndCounts();

        foreach (var IdxAndCount in typesAndCounts)
        {
            list.Items.Add(new ListViewItem(new string[] { IdxAndCount.Count.ToString(), IdxAndCount.TypeName }));
        }

        list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
    }

    private void List_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(SelectedTraitType()))
        {
            return;
        }

        //unset search text
        //SearchBox.Text = string.Empty;
        var tvc = CurrentTraitTypes.TraitValues(SelectedTraitType());
        TraitValuesList.UpdateTraitValuesSource(tvc);
    }
}