
using SchoolPilot.Export.Enums;

namespace SchoolPilot.Export.Attributes
{
    public class ExportHeaderAttribute : Attribute
    {
        public string Header { get; }
        public ExportDataFormat DataFormat { get; }
        public HorizontalAlignment? Alignment { get; set; }
        public int Index { get; set; }

        //We only have 2 types of parameters, AggregateFilterParameters - ones that filter (the IF part of SumIfByGroup and SumIf) and AggregateGroupParameters - ones that group (the group part of SumIfByGroup).
        // If we get more types, these should likely be merged into a dictionary with the type as the key and the list as the value.

        //These should be in pairs of 2. The first is the column to check and the second is the value it should be.
        //e.g. ["1", "", "3", "Yes"] means that column 1 should have an empty value and column 3 has a Yes value.
        public string[] AggregateFilterParameters { get; set; }

        //These are which columns should be part of the group key.
        // e.g. ["1", "3"] means that the group is the permutations of values in columns 1 and 3. If they were True/False fields, the groups could be TT, TF, FF, FT
        public string[] AggregateGroupParameters { get; set; }

        public AggregateType Aggregate { get; set; }

        public bool WrapText { get; set; }

        public ExportHeaderAttribute(string header, int index = -1)
        {
            Header = header;
            Index = index;
        }

        public ExportHeaderAttribute(string header, ExportDataFormat dataFormat, int index = -1)
            : this(header, index)
        {
            DataFormat = dataFormat;
        }

        public ExportHeaderAttribute(string header, HorizontalAlignment alignment, int index = -1, bool wrapText = false)
            : this(header, index)
        {
            Alignment = alignment;
            WrapText = wrapText;
        }

        /// <summary>
        /// Needs to be used along side <see cref="VisibilityFilterPropertyName"/> in order to filter out this column
        /// </summary>
        public Type FilterType { get; set; }

        /// <summary>
        /// Needs to be used along side <see cref="FilterType"/> in order to filter out this column
        /// </summary>
        public string VisibilityFilterPropertyName { get; set; }

        /// <summary>
        /// A non bold title for a header, which will be placed above the title.
        /// Will only work in excel files and not CSV.
        /// </summary>
        public string SubTitle { get; set; }
    }

    public class NestedExportHeaderAttribute : Attribute
    {
        public Type Type { get; }

        public NestedExportHeaderAttribute(Type type)
        {
            Type = type;
        }
    }
}
