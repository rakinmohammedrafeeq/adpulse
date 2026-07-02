using AdPulse.Core.Shared.Enums;
using AdPulse.Core.Domain.Common;

namespace AdPulse.Core.Domain.ValueObjects;

/// <summary>
/// ������״ֵ̬����
/// </summary>
public class BlacklistStatus : ValueObject
{
    /// <summary>
    /// �Ƿ��ں�������
    /// </summary>
    public bool IsBlacklisted { get; private set; }

    /// <summary>
    /// ����������
    /// </summary>
    public IReadOnlyList<BlacklistType> BlacklistTypes { get; private set; }

    /// <summary>
    /// ������ԭ��
    /// </summary>
    public string? Reason { get; private set; }

    /// <summary>
    /// ���������ʱ��
    /// </summary>
    public DateTime? BlacklistedAt { get; private set; }

    /// <summary>
    /// ��������Ч��
    /// </summary>
    public DateTime? ExpiresAt { get; private set; }

    /// <summary>
    /// �Ƿ����ú�����
    /// </summary>
    public bool IsPermanent => !ExpiresAt.HasValue;

    /// <summary>
    /// �Ƿ��ѹ���
    /// </summary>
    public bool IsExpired => ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value;

    /// <summary>
    /// �Ƿ�ǰ��Ч
    /// </summary>
    public bool IsCurrentlyActive => IsBlacklisted && !IsExpired;

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private BlacklistStatus()
    {
        IsBlacklisted = false;
        BlacklistTypes = Array.Empty<BlacklistType>();
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public BlacklistStatus(
        bool isBlacklisted,
        IReadOnlyList<BlacklistType>? blacklistTypes = null,
        string? reason = null,
        DateTime? blacklistedAt = null,
        DateTime? expiresAt = null)
    {
        ValidateInput(isBlacklisted, blacklistTypes, blacklistedAt, expiresAt);

        IsBlacklisted = isBlacklisted;
        BlacklistTypes = blacklistTypes ?? Array.Empty<BlacklistType>();
        Reason = reason;
        BlacklistedAt = blacklistedAt;
        ExpiresAt = expiresAt;
    }

    /// <summary>
    /// ��������״̬���Ǻ�������
    /// </summary>
    public static BlacklistStatus CreateNormal()
    {
        return new BlacklistStatus(false);
    }

    /// <summary>
    /// ����������״̬
    /// </summary>
    public static BlacklistStatus CreateBlacklisted(
        BlacklistType blacklistType,
        string? reason = null,
        DateTime? expiresAt = null)
    {
        return new BlacklistStatus(
            true,
            new[] { blacklistType },
            reason,
            DateTime.UtcNow,
            expiresAt);
    }

    /// <summary>
    /// ���������ͺ�����״̬
    /// </summary>
    public static BlacklistStatus CreateBlacklisted(
        IReadOnlyList<BlacklistType> blacklistTypes,
        string? reason = null,
        DateTime? expiresAt = null)
    {
        return new BlacklistStatus(
            true,
            blacklistTypes,
            reason,
            DateTime.UtcNow,
            expiresAt);
    }

    /// <summary>
    /// �������ú�����״̬
    /// </summary>
    public static BlacklistStatus CreatePermanentBlacklist(
        BlacklistType blacklistType,
        string? reason = null)
    {
        return new BlacklistStatus(
            true,
            new[] { blacklistType },
            reason,
            DateTime.UtcNow,
            null);
    }

    /// <summary>
    /// ��Ӻ���������
    /// </summary>
    public BlacklistStatus AddBlacklistType(BlacklistType blacklistType)
    {
        if (BlacklistTypes.Contains(blacklistType))
            return this;

        var newTypes = BlacklistTypes.Concat(new[] { blacklistType }).ToArray();
        return new BlacklistStatus(
            true,
            newTypes,
            Reason,
            BlacklistedAt ?? DateTime.UtcNow,
            ExpiresAt);
    }

    /// <summary>
    /// �Ƴ�����������
    /// </summary>
    public BlacklistStatus RemoveBlacklistType(BlacklistType blacklistType)
    {
        if (!BlacklistTypes.Contains(blacklistType))
            return this;

        var newTypes = BlacklistTypes.Where(t => t != blacklistType).ToArray();
        var stillBlacklisted = newTypes.Length > 0;

        return new BlacklistStatus(
            stillBlacklisted,
            newTypes,
            stillBlacklisted ? Reason : null,
            stillBlacklisted ? BlacklistedAt : null,
            stillBlacklisted ? ExpiresAt : null);
    }

    /// <summary>
    /// ���º�����ԭ��
    /// </summary>
    public BlacklistStatus WithReason(string? reason)
    {
        return new BlacklistStatus(
            IsBlacklisted,
            BlacklistTypes,
            reason,
            BlacklistedAt,
            ExpiresAt);
    }

    /// <summary>
    /// �ӳ���������Ч��
    /// </summary>
    public BlacklistStatus ExtendExpiration(DateTime newExpiresAt)
    {
        if (!IsBlacklisted)
            throw new InvalidOperationException("�޷��ӳ��Ǻ�����״̬����Ч��");

        if (newExpiresAt <= DateTime.UtcNow)
            throw new ArgumentException("�µ���Ч�ڱ����ڵ�ǰʱ��֮��", nameof(newExpiresAt));

        return new BlacklistStatus(
            IsBlacklisted,
            BlacklistTypes,
            Reason,
            BlacklistedAt,
            newExpiresAt);
    }

    /// <summary>
    /// ����Ϊ���ú�����
    /// </summary>
    public BlacklistStatus MakePermanent()
    {
        if (!IsBlacklisted)
            throw new InvalidOperationException("�޷����Ǻ�����״̬����Ϊ����");

        return new BlacklistStatus(
            IsBlacklisted,
            BlacklistTypes,
            Reason,
            BlacklistedAt,
            null);
    }

    /// <summary>
    /// ���������״̬
    /// </summary>
    public BlacklistStatus Clear()
    {
        return CreateNormal();
    }

    /// <summary>
    /// ����Ƿ�����ض�����������
    /// </summary>
    public bool ContainsType(BlacklistType blacklistType)
    {
        return BlacklistTypes.Contains(blacklistType);
    }

    /// <summary>
    /// ����Ƿ������һ����������
    /// </summary>
    public bool ContainsAnyType(params BlacklistType[] blacklistTypes)
    {
        return blacklistTypes.Any(type => BlacklistTypes.Contains(type));
    }

    /// <summary>
    /// ��ȡʣ����Чʱ��
    /// </summary>
    public TimeSpan? GetRemainingTime()
    {
        if (!ExpiresAt.HasValue)
            return null;

        var remaining = ExpiresAt.Value - DateTime.UtcNow;
        return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
    }

    /// <summary>
    /// ��ȡ�ȼ��ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return IsBlacklisted;
        yield return Reason ?? string.Empty;
        yield return BlacklistedAt ?? DateTime.MinValue;
        yield return ExpiresAt ?? DateTime.MinValue;

        foreach (var blacklistType in BlacklistTypes.OrderBy(x => x))
        {
            yield return blacklistType;
        }
    }

    /// <summary>
    /// ��֤�������
    /// </summary>
    private static void ValidateInput(
        bool isBlacklisted,
        IReadOnlyList<BlacklistType>? blacklistTypes,
        DateTime? blacklistedAt,
        DateTime? expiresAt)
    {
        if (isBlacklisted && (blacklistTypes == null || blacklistTypes.Count == 0))
            throw new ArgumentException("������״̬�����������һ������������", nameof(blacklistTypes));

        if (!isBlacklisted && blacklistTypes != null && blacklistTypes.Count > 0)
            throw new ArgumentException("�Ǻ�����״̬���ܰ�������������", nameof(blacklistTypes));

        if (isBlacklisted && blacklistedAt.HasValue && blacklistedAt.Value > DateTime.UtcNow)
            throw new ArgumentException("����������ʱ�䲻����δ��ʱ��", nameof(blacklistedAt));

        if (expiresAt.HasValue && blacklistedAt.HasValue && expiresAt.Value <= blacklistedAt.Value)
            throw new ArgumentException("��������Ч�ڱ����ڼ���ʱ��֮��", nameof(expiresAt));
    }
}