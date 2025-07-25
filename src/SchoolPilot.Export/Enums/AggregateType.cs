

namespace SchoolPilot.Export.Enums
{
    // For future implementation, the only builder setup for multiple SumIfByGroup columns is PaymentAndAdjustmentExcelBuilder. It uses the SUMIFS formula, and can be referenced if needed in other builders.
    // ExcelBuilder has SumIfByGroup setup up to also use the filter parameters. It also uses the SUMIFS formula
    [Flags]
    public enum AggregateType
    {
        None = 0,
        Count = 1,
        Sum = 2,
        Average = 4,
        Median = 8,
        SumIf = 16,

        /// <summary>
        /// This is the same as a SumIf but used when there are multiple criteria (groups) to search with.
        /// Group Type   |   Quantity   | SumIfByGroup
        /// --------------------------------------------
        ///       A      |      1       |
        ///       B      |      2       |
        ///       A      |      3       |
        ///       C      |      3       |
        ///       B      |      1       |
        /// --------------------------------------------
        ///              |              |
        ///              |            A | 4
        ///              |            B | 3
        ///              |            C | 3
        /// </summary>
        SumIfByGroup = 32
    }

    public static class AggregateTypeExtensions
    {
        public static bool HasTotalAggregate(this AggregateType aggregateType)
        {
            return (aggregateType & AggregateType.Count) == AggregateType.Count
                   || (aggregateType & AggregateType.Sum) == AggregateType.Sum;
        }

        public static bool HasAverageAggregate(this AggregateType aggregateType)
        {
            return (aggregateType & AggregateType.Average) == AggregateType.Average;
        }

        public static bool HasMedianAggregate(this AggregateType aggregateType)
        {
            return (aggregateType & AggregateType.Median) == AggregateType.Median;
        }

        public static bool HasTotalAggregateNested(this AggregateType aggregateType)
        {
            return (aggregateType & AggregateType.Count) == AggregateType.Count
                   || (aggregateType & AggregateType.SumIf) == AggregateType.SumIf;
        }
    }
}
