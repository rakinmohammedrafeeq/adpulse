using AdPulse.Core.Domain.Common;
using AdPulse.Core.Shared.Enums;

namespace AdPulse.Core.Domain.ValueObjects;

/// <summary>
/// �����Ϣֵ����
/// </summary>
public class AuditInfo : ValueObject
{
    /// <summary>
    /// ���״̬
    /// </summary>
    public AuditStatus Status { get; private set; }

    /// <summary>
    /// ��˷�����Ϣ
    /// </summary>
    public string? Feedback { get; private set; }

    /// <summary>
    /// ������ʱ��
    /// </summary>
    public DateTime LastAuditTime { get; private set; }

    /// <summary>
    /// ���ԱID
    /// </summary>
    public string? AuditorId { get; private set; }

    /// <summary>
    /// ���Ա����
    /// </summary>
    public string? AuditorName { get; private set; }

    /// <summary>
    /// ��������
    /// </summary>
    public string? CorrectionSuggestion { get; private set; }

    /// <summary>
    /// ������κ�
    /// </summary>
    public string? AuditBatchId { get; private set; }

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private AuditInfo() { }

    /// <summary>
    /// ���캯��
    /// </summary>
    public AuditInfo(
        AuditStatus status,
        string? feedback = null,
        string? auditorId = null,
        string? auditorName = null,
        string? correctionSuggestion = null,
        string? auditBatchId = null)
    {
        Status = status;
        Feedback = feedback;
        LastAuditTime = DateTime.UtcNow;
        AuditorId = auditorId;
        AuditorName = auditorName;
        CorrectionSuggestion = correctionSuggestion;
        AuditBatchId = auditBatchId;
    }

    /// <summary>
    /// ���������״̬
    /// </summary>
    public static AuditInfo CreatePending()
    {
        return new AuditInfo(AuditStatus.Pending);
    }

    /// <summary>
    /// ���������״̬
    /// </summary>
    public static AuditInfo CreateInProgress(string? auditorId = null, string? auditorName = null)
    {
        return new AuditInfo(AuditStatus.InProgress, auditorId: auditorId, auditorName: auditorName);
    }

    /// <summary>
    /// �������ͨ��״̬
    /// </summary>
    public static AuditInfo CreateApproved(string? auditorId = null, string? auditorName = null, string? feedback = null)
    {
        return new AuditInfo(AuditStatus.Approved, feedback, auditorId, auditorName);
    }

    /// <summary>
    /// ������˾ܾ�״̬
    /// </summary>
    public static AuditInfo CreateRejected(string feedback, string? auditorId = null, string? auditorName = null)
    {
        return new AuditInfo(AuditStatus.Rejected, feedback, auditorId, auditorName);
    }

    /// <summary>
    /// ������Ҫ�޸�״̬
    /// </summary>
    public static AuditInfo CreateRequiresChanges(string correctionSuggestion, string? auditorId = null, string? auditorName = null)
    {
        return new AuditInfo(AuditStatus.RequiresChanges, correctionSuggestion: correctionSuggestion, auditorId: auditorId, auditorName: auditorName);
    }

    /// <summary>
    /// �Ƿ������ͨ��
    /// </summary>
    public bool IsApproved => Status == AuditStatus.Approved;

    /// <summary>
    /// �Ƿ񱻾ܾ�
    /// </summary>
    public bool IsRejected => Status == AuditStatus.Rejected;

    /// <summary>
    /// �Ƿ���Ҫ�޸�
    /// </summary>
    public bool RequiresChanges => Status == AuditStatus.RequiresChanges;

    /// <summary>
    /// �Ƿ��ѹ���
    /// </summary>
    public bool IsExpired => Status == AuditStatus.Expired;

    /// <summary>
    /// �Ƿ����Ͷ��
    /// </summary>
    public bool CanDeliver => Status == AuditStatus.Approved;

    /// <summary>
    /// ��ȡ����ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Status;
        yield return Feedback ?? string.Empty;
        yield return LastAuditTime;
        yield return AuditorId ?? string.Empty;
        yield return AuditorName ?? string.Empty;
        yield return CorrectionSuggestion ?? string.Empty;
        yield return AuditBatchId ?? string.Empty;
    }
}