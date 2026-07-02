using AdPulse.Core.Shared.Enums;

namespace AdPulse.Core.Shared.Entities
{
    /// <summary>
    /// 排序条件
    /// </summary>
    public record SortCriteria
    {
        /// <summary>
        /// 排序字段
        /// </summary>
        public required string Field { get; init; }

        /// <summary>
        /// 排序方向
        /// </summary>
        public SortDirection Direction { get; init; } = SortDirection.Ascending;

        /// <summary>
        /// 权重
        /// </summary>
        public decimal Weight { get; init; } = 1.0m;
    }

}
