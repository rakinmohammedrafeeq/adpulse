using AdPulse.Core.Domain.Common;
using AdPulse.Core.Domain.Enums;
using AdPulse.Core.Domain.Events;
using AdPulse.Core.Domain.ValueObjects;
using AdPulse.Core.Shared.Constants;

namespace AdPulse.Core.Domain.Aggregates;

/// <summary>
/// �����ʵ�壨�ۺϸ���
/// </summary>
public class Advertiser : AggregateRoot
{
    /// <summary>
    /// ��˾����
    /// </summary>
    public string CompanyName { get; private set; } = string.Empty;

    /// <summary>
    /// ��ϵ������
    /// </summary>
    public string ContactName { get; private set; } = string.Empty;

    /// <summary>
    /// ����
    /// </summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    /// �绰
    /// </summary>
    public string Phone { get; private set; } = string.Empty;

    /// <summary>
    /// �����״̬
    /// </summary>
    public AdvertiserStatus Status { get; private set; }

    /// <summary>
    /// ע��ʱ��
    /// </summary>
    public DateTime RegisteredAt { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// ������Ϣ
    /// </summary>
    public QualificationInfo Qualification { get; private set; } = null!;

    /// <summary>
    /// �˵���Ϣ
    /// </summary>
    public BillingInfo Billing { get; private set; } = null!;

    /// <summary>
    /// ��漯��
    /// </summary>
    private readonly List<Advertisement> _advertisements = new();
    public IReadOnlyList<Advertisement> Advertisements => _advertisements.AsReadOnly();

    /// <summary>
    /// ˽�й��캯��������ORM
    /// </summary>
    private Advertiser() { }

    /// <summary>
    /// ���캯��
    /// </summary>
    public Advertiser(
        string companyName,
        string contactName,
        string email,
        string phone,
        QualificationInfo qualification,
        BillingInfo billing)
    {
        ValidateInputs(companyName, contactName, email, phone);

        CompanyName = companyName;
        ContactName = contactName;
        Email = email;
        Phone = phone;
        Qualification = qualification;
        Billing = billing;
        Status = AdvertiserStatus.Pending;

        // ���������ע���¼�
        AddDomainEvent(new AdvertiserRegisteredEvent(Id, companyName, email));
    }

    /// <summary>
    /// ע������
    /// </summary>
    public static Advertiser Register(RegistrationInfo info)
    {
        ArgumentNullException.ThrowIfNull(info);

        var advertiser = new Advertiser(
            info.CompanyName,
            info.ContactName,
            info.Email,
            info.Phone,
            info.Qualification,
            info.Billing);

        return advertiser;
    }

    /// <summary>
    /// �ύ������֤
    /// </summary>
    public void SubmitQualification(QualificationInfo qualification)
    {
        ArgumentNullException.ThrowIfNull(qualification);

        if (Status != AdvertiserStatus.Pending && Status != AdvertiserStatus.Rejected)
            throw new InvalidOperationException("ֻ�д���˻򱻾ܾ�״̬�Ĺ���������ύ����");

        Qualification = qualification;
        Status = AdvertiserStatus.UnderReview;

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertiserQualificationSubmittedEvent(Id, qualification.BusinessLicense));
    }

    /// <summary>
    /// ��������
    /// </summary>
    public void Activate()
    {
        if (Status != AdvertiserStatus.UnderReview)
            throw new InvalidOperationException("ֻ������еĹ�������Լ���");

        Status = AdvertiserStatus.Active;
        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertiserActivatedEvent(Id));
    }

    /// <summary>
    /// ��ͣ�����
    /// </summary>
    public void Suspend(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("��ͣ�����ṩԭ��", nameof(reason));

        if (Status != AdvertiserStatus.Active)
            throw new InvalidOperationException("ֻ�м���״̬�Ĺ����������ͣ");

        Status = AdvertiserStatus.Suspended;
        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertiserSuspendedEvent(Id, reason));
    }

    /// <summary>
    /// �ܾ������
    /// </summary>
    public void Reject(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("�ܾ������ṩԭ��", nameof(reason));

        if (Status != AdvertiserStatus.UnderReview)
            throw new InvalidOperationException("ֻ������еĹ�������Ծܾ�");

        Status = AdvertiserStatus.Rejected;
        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertiserRejectedEvent(Id, reason));
    }

    /// <summary>
    /// �ָ������
    /// </summary>
    public void Resume()
    {
        if (Status != AdvertiserStatus.Suspended)
            throw new InvalidOperationException("ֻ����ͣ״̬�Ĺ�������Իָ�");

        Status = AdvertiserStatus.Active;
        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertiserResumedEvent(Id));
    }

    /// <summary>
    /// �����˵���Ϣ
    /// </summary>
    public void UpdateBilling(BillingInfo billing)
    {
        ArgumentNullException.ThrowIfNull(billing);

        Billing = billing;
        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertiserBillingUpdatedEvent(Id));
    }

    /// <summary>
    /// ������ϵ��Ϣ
    /// </summary>
    public void UpdateContactInfo(string contactName, string email, string phone)
    {
        ValidateContactInfo(contactName, email, phone);

        ContactName = contactName;
        Email = email;
        Phone = phone;

        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertiserContactUpdatedEvent(Id, email));
    }

    /// <summary>
    /// ��ӹ��
    /// </summary>
    public void AddAdvertisement(Advertisement advertisement)
    {
        ArgumentNullException.ThrowIfNull(advertisement);

        if (advertisement.AdvertiserId != Id)
            throw new ArgumentException("�����������ID��ƥ��");

        if (Status != AdvertiserStatus.Active)
            throw new InvalidOperationException("ֻ�м���״̬�Ĺ�������Դ������");

        _advertisements.Add(advertisement);
        UpdateLastModifiedTime();
        AddDomainEvent(new AdvertiserAdvertisementAddedEvent(Id, advertisement.Id));
    }

    /// <summary>
    /// ��ȡ��Ծ���
    /// </summary>
    public IReadOnlyList<Advertisement> GetActiveAdvertisements()
    {
        return _advertisements.Where(a => a.IsActive && !a.IsDeleted).ToList().AsReadOnly();
    }

    /// <summary>
    /// ��ȡ�����ѽ��
    /// </summary>
    public decimal GetTotalSpent()
    {
        return _advertisements.Sum(a => a.TotalSpent);
    }

    /// <summary>
    /// ��ȡ��չʾ����
    /// </summary>
    public long GetTotalImpressions()
    {
        return _advertisements.Sum(a => a.TotalImpressions);
    }

    /// <summary>
    /// ��ȡ�ܵ������
    /// </summary>
    public long GetTotalClicks()
    {
        return _advertisements.Sum(a => a.TotalClicks);
    }

    /// <summary>
    /// ��ȡƽ�������
    /// </summary>
    public decimal GetAverageClickThroughRate()
    {
        var totalImpressions = GetTotalImpressions();
        var totalClicks = GetTotalClicks();
        return totalImpressions > 0 ? (decimal)totalClicks / totalImpressions : 0m;
    }

    /// <summary>
    /// �Ƿ���Դ������
    /// </summary>
    public bool CanCreateAdvertisement => Status == AdvertiserStatus.Active && !IsDeleted;

    /// <summary>
    /// ��֤�������
    /// </summary>
    private static void ValidateInputs(string companyName, string contactName, string email, string phone)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            throw new ArgumentException("��˾���Ʋ���Ϊ��", nameof(companyName));

        if (companyName.Length > ValidationConstants.StringLength.CompanyNameMaxLength)
            throw new ArgumentException($"��˾���Ƴ��Ȳ��ܳ���{ValidationConstants.StringLength.CompanyNameMaxLength}���ַ�", nameof(companyName));

        ValidateContactInfo(contactName, email, phone);
    }

    /// <summary>
    /// ��֤��ϵ��Ϣ
    /// </summary>
    private static void ValidateContactInfo(string contactName, string email, string phone)
    {
        if (string.IsNullOrWhiteSpace(contactName))
            throw new ArgumentException("��ϵ����������Ϊ��", nameof(contactName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("���䲻��Ϊ��", nameof(email));

        if (!IsValidEmail(email))
            throw new ArgumentException("�����ʽ����ȷ", nameof(email));

        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("�绰����Ϊ��", nameof(phone));
    }

    /// <summary>
    /// ��֤�����ʽ
    /// </summary>
    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}

/// <summary>
/// ע����Ϣ
/// </summary>
public record RegistrationInfo(
    string CompanyName,
    string ContactName,
    string Email,
    string Phone,
    QualificationInfo Qualification,
    BillingInfo Billing);

/// <summary>
/// ������Ϣֵ����
/// </summary>
public record QualificationInfo(
    string BusinessLicense,
    string TaxNumber,
    string LegalRepresentative,
    DateTime IssueDate,
    DateTime ExpiryDate,
    IReadOnlyList<string> Certificates);

