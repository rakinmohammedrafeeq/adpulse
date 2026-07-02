namespace AdPulse.Core.Shared.Entities
{

    /// <summary>
    /// ��������
    /// </summary>
    public record FeatureVector
    {
        /// <summary>
        /// ��������
        /// </summary>
        public required IReadOnlyDictionary<string, object> Features { get; init; }

        /// <summary>
        /// �����汾
        /// </summary>
        public string? Version { get; init; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime GeneratedAt { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// ��Ч��
        /// </summary>
        public DateTime? ExpiresAt { get; init; }

        /// <summary>
        /// ���Ŷ�
        /// </summary>
        public decimal Confidence { get; init; } = 1.0m;
    }
}








