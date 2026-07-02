using AdPulse.Core.Domain.Common;
using AdPulse.Core.Domain.Enums;
using AdPulse.Core.Domain.Targeting;
using AdPulse.Core.Shared.Entities;

namespace AdPulse.Core.Domain.ValueObjects;

/// <summary>
/// �������ģ��ֵ����
/// ��Ϊ�ɸ��õĶ������ģtemplate��֧�ִ���TargetingConfigʵ��
/// �ṩԤ���õĶ����������ϺͰ汾�������
/// </summary>
public class TargetingPolicy : ValueObject
{
    private readonly List<string> _tags;

    /// <summary>
    /// ��������
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// ��������
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// ���԰汾
    /// </summary>
    public int Version { get; private set; }

    /// <summary>
    /// ������
    /// </summary>
    public string CreatedBy { get; private set; }

    /// <summary>
    /// ��������ģ�弯��
    /// </summary>
    public IReadOnlyList<ITargetingCriteria> CriteriaTemplates { get; private set; }

    /// <summary>
    /// ����Ȩ��
    /// </summary>
    public decimal Weight { get; private set; } = 1.0m;

    /// <summary>
    /// �Ƿ����ö���
    /// </summary>
    public bool IsEnabled { get; private set; } = true;

    /// <summary>
    /// ����״̬
    /// </summary>
    public PolicyStatus Status { get; private set; }

    /// <summary>
    /// �������
    /// </summary>
    public string Category { get; private set; }

    /// <summary>
    /// �Ƿ񹫿�����
    /// </summary>
    public bool IsPublic { get; private set; }

    /// <summary>
    /// ���Ա�ǩ��ֻ����
    /// </summary>
    public IReadOnlyList<string> Tags => _tags.AsReadOnly();

    /// <summary>
    /// ���Դ���ʱ��
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// ���Ը���ʱ��
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// ���캯��
    /// </summary>
    /// <param name="name">��������</param>
    /// <param name="createdBy">������</param>
    /// <param name="description">��������</param>
    /// <param name="version">�汾��</param>
    /// <param name="criteriaTemplates">��������ģ�弯��</param>
    /// <param name="category">�������</param>
    /// <param name="isPublic">�Ƿ񹫿�</param>
    /// <param name="tags">��ǩ�б�</param>
    /// <param name="weight">����Ȩ��</param>
    /// <param name="isEnabled">�Ƿ�����</param>
    /// <param name="status">����״̬</param>
    public TargetingPolicy(
        string name,
        string createdBy,
        string? description = null,
        int version = 1,
        IDictionary<string, ITargetingCriteria>? criteriaTemplates = null,
        string category = "General",
        bool isPublic = false,
        IList<string>? tags = null,
        decimal weight = 1.0m,
        bool isEnabled = true,
        PolicyStatus status = PolicyStatus.Draft)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("�������Ʋ���Ϊ��", nameof(name));
        if (string.IsNullOrEmpty(createdBy))
            throw new ArgumentException("�����߲���Ϊ��", nameof(createdBy));

        Name = name;
        Description = description;
        Version = version;
        CreatedBy = createdBy;
        CriteriaTemplates = criteriaTemplates?.Values.ToList() ?? new List<ITargetingCriteria>();
        Category = category;
        IsPublic = isPublic;
        _tags = tags?.ToList() ?? new List<string>();
        Weight = weight;
        IsEnabled = isEnabled;
        Status = status;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        ValidateWeight(weight);
    }

    /// <summary>
    /// �����յĶ������
    /// </summary>
    /// <param name="name">��������</param>
    /// <param name="createdBy">������</param>
    /// <param name="category">�������</param>
    /// <returns>�ղ���ʵ��</returns>
    public static TargetingPolicy CreateEmpty(string name, string createdBy, string category = "General")
    {
        return new TargetingPolicy(name, createdBy, category: category);
    }

    /// <summary>
    /// ����ȫ���������Ʋ��ԣ�������
    /// </summary>
    /// <param name="name">��������</param>
    /// <param name="createdBy">������</param>
    /// <returns>�����Ʋ���ʵ��</returns>
    public static TargetingPolicy CreateUnrestricted(string name, string createdBy)
    {
        return new TargetingPolicy(
            name: name,
            createdBy: createdBy,
            description: "�޶������Ʋ���",
            category: "Unrestricted",
            isEnabled: false);
    }

    /// <summary>
    /// �����в��Դ���TargetingConfigʵ��
    /// </summary>
    /// <param name="advertisementId">���ID</param>
    /// <param name="runtimeParameters">����ʱ��������ѡ��</param>
    /// <returns>����ʵ��</returns>
    public TargetingConfig CreateConfig(Guid advertisementId, IDictionary<string, object>? runtimeParameters = null)
    {
        if (advertisementId == Guid.Empty)
            throw new ArgumentException("���ID����Ϊ��", nameof(advertisementId));

        return TargetingConfig.CreateFromPolicy(this, advertisementId, runtimeParameters);
    }

    /// <summary>
    /// ��ӻ���¶�������ģ�壨������ʵ����
    /// </summary>
    /// <param name="criteriaType">��������</param>
    /// <param name="criteria">��������</param>
    /// <returns>�����������Ĳ���ʵ��</returns>
    public TargetingPolicy WithCriteriaTemplate(string criteriaType, ITargetingCriteria criteria)
    {
        if (string.IsNullOrEmpty(criteriaType))
            throw new ArgumentException("�������Ͳ���Ϊ��", nameof(criteriaType));
        if (criteria == null)
            throw new ArgumentNullException(nameof(criteria));
        if (Status == PolicyStatus.Archived)
            throw new InvalidOperationException("�ѹ鵵�Ĳ��Բ����޸�");

        var existingCriteria = CriteriaTemplates.FirstOrDefault(c => c.CriteriaType == criteriaType);
        var criteriaList = CriteriaTemplates.ToList();

        if (existingCriteria != null)
        {
            var index = criteriaList.IndexOf(existingCriteria);
            criteriaList[index] = criteria;
        }
        else
        {
            criteriaList.Add(criteria);
        }

        // ת��Ϊ�ֵ���ʹ�����й��캯��
        var criteriaDict = criteriaList.ToDictionary(c => c.CriteriaType, c => c);

        return new TargetingPolicy(
            Name, CreatedBy, Description, Version,
            criteriaDict, Category, IsPublic, _tags,
            Weight, IsEnabled, Status);
    }

    /// <summary>
    /// �Ƴ���������ģ�壨������ʵ����
    /// </summary>
    /// <param name="criteriaType">��������</param>
    /// <returns>�Ƴ�ָ��������Ĳ���ʵ��</returns>
    public TargetingPolicy WithoutCriteriaTemplate(string criteriaType)
    {
        if (string.IsNullOrEmpty(criteriaType))
            return this;
        if (Status == PolicyStatus.Archived)
            throw new InvalidOperationException("�ѹ鵵�Ĳ��Բ����޸�");

        var criteriaToRemove = CriteriaTemplates.FirstOrDefault(c => c.CriteriaType == criteriaType);
        if (criteriaToRemove == null)
            return this;

        var criteriaList = CriteriaTemplates.Where(c => c.CriteriaType != criteriaType).ToList();
        var criteriaDict = criteriaList.ToDictionary(c => c.CriteriaType, c => c);

        return new TargetingPolicy(
            Name, CreatedBy, Description, Version,
            criteriaDict, Category, IsPublic, _tags,
            Weight, IsEnabled, Status);
    }

    /// <summary>
    /// ��ȡָ�����͵Ķ�������ģ��
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    /// <param name="criteriaType">�������ͱ�ʶ</param>
    /// <returns>��������ģ�壬����������򷵻�null</returns>
    public T? GetCriteriaTemplate<T>(string criteriaType) where T : class, ITargetingCriteria
    {
        if (string.IsNullOrEmpty(criteriaType))
            return null;

        return CriteriaTemplates.FirstOrDefault(c => c.CriteriaType == criteriaType) as T;
    }

    /// <summary>
    /// ����Ƿ����ָ�����͵Ķ�������ģ��
    /// </summary>
    /// <param name="criteriaType">�������ͱ�ʶ</param>
    /// <returns>�Ƿ����</returns>
    public bool HasCriteriaTemplate(string criteriaType)
    {
        return !string.IsNullOrEmpty(criteriaType) && CriteriaTemplates.Any(c => c.CriteriaType == criteriaType);
    }

    /// <summary>
    /// ��ȡ���������õĶ�������ģ��
    /// </summary>
    public IEnumerable<ITargetingCriteria> GetEnabledCriteriaTemplates()
    {
        return CriteriaTemplates.Where(c => c.IsEnabled);
    }

    /// <summary>
    /// ��ȡ���ж�����������
    /// </summary>
    public IReadOnlyCollection<string> GetCriteriaTypes()
    {
        return CriteriaTemplates.Select(c => c.CriteriaType).ToList().AsReadOnly();
    }

    /// <summary>
    /// ��ӱ�ǩ
    /// </summary>
    /// <param name="tag">��ǩ</param>
    public void AddTag(string tag)
    {
        if (string.IsNullOrEmpty(tag))
            return;
        if (Status == PolicyStatus.Archived)
            throw new InvalidOperationException("�ѹ鵵�Ĳ��Բ����޸�");

        if (!_tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
        {
            _tags.Add(tag);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// �Ƴ���ǩ
    /// </summary>
    /// <param name="tag">��ǩ</param>
    /// <returns>�Ƿ�ɹ��Ƴ�</returns>
    public bool RemoveTag(string tag)
    {
        if (string.IsNullOrEmpty(tag))
            return false;
        if (Status == PolicyStatus.Archived)
            throw new InvalidOperationException("�ѹ鵵�Ĳ��Բ����޸�");

        var result = _tags.RemoveAll(t => string.Equals(t, tag, StringComparison.OrdinalIgnoreCase)) > 0;
        if (result)
        {
            UpdatedAt = DateTime.UtcNow;
        }
        return result;
    }

    /// <summary>
    /// ��֤�����Ƿ���Ч
    /// </summary>
    public ValidationResult Validate()
    {
        var validationResult = new ValidationResult(string.Empty);

        // ��֤��������
        if (Id == Guid.Empty)
            validationResult.AddError("����ID����Ϊ��");

        if (string.IsNullOrEmpty(Name))
            validationResult.AddError("�������Ʋ���Ϊ��");

        if (string.IsNullOrEmpty(CreatedBy))
            validationResult.AddError("�����߲���Ϊ��");

        if (Weight <= 0)
            validationResult.AddError("Ȩ�ر������0");

        // ������ã�������Ҫһ����������ģ��
        if (IsEnabled)
        {
            var enabledTemplates = GetEnabledCriteriaTemplates().ToList();
            if (!enabledTemplates.Any())
                validationResult.AddWarning("����״̬�½������ٰ���һ����������ģ��");

            // ��֤�������õ�����ģ�嶼����Ч��
            foreach (var template in enabledTemplates)
            {
                if (!template.IsValid())
                    validationResult.AddError($"��������ģ�� {template.CriteriaType} ��Ч");
            }
        }

        return validationResult;
    }

    /// <summary>
    /// �������Ƿ���Ч
    /// </summary>
    public bool IsValid()
    {
        return Validate().IsValid;
    }

    /// <summary>
    /// ��������
    /// </summary>
    public void Publish()
    {
        if (Status == PolicyStatus.Archived)
            throw new InvalidOperationException("�ѹ鵵�Ĳ��Բ��ܷ���");

        var validationResult = Validate();
        if (!validationResult.IsValid)
            throw new InvalidOperationException($"������֤ʧ�ܣ�{string.Join(", ", validationResult.Errors)}");

        Status = PolicyStatus.Published;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// �鵵����
    /// </summary>
    public void Archive()
    {
        Status = PolicyStatus.Archived;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// ��¡����
    /// </summary>
    /// <param name="newName">�²�������</param>
    /// <param name="createdBy">������</param>
    /// <returns>��¡�Ĳ���</returns>
    public TargetingPolicy Clone(string newName, string createdBy)
    {
        if (string.IsNullOrEmpty(newName))
            throw new ArgumentException("�²������Ʋ���Ϊ��", nameof(newName));
        if (string.IsNullOrEmpty(createdBy))
            throw new ArgumentException("�����߲���Ϊ��", nameof(createdBy));

        var newPolicyId = Guid.NewGuid().ToString();
        var clonedCriteriaTemplates = new Dictionary<string, ITargetingCriteria>();

        foreach (var template in CriteriaTemplates)
        {
            // ����ITargetingCriteriaʵ�����������
            clonedCriteriaTemplates[template.CriteriaType] = template; // ������Ҫʵ�����
        }

        return new TargetingPolicy(
            name: newName,
            createdBy: createdBy,
            description: $"��¡�ԣ�{Name}",
            version: 1,
            criteriaTemplates: clonedCriteriaTemplates,
            category: Category,
            isPublic: false, // ��¡�Ĳ���Ĭ�ϲ�����
            tags: _tags.ToList(),
            weight: Weight,
            isEnabled: IsEnabled,
            status: PolicyStatus.Draft);
    }

    /// <summary>
    /// ��ȡ����ʹ��ͳ��
    /// </summary>
    /// <returns>ʹ��ͳ��</returns>
    public PolicyUsageStats GetUsageStatistics()
    {
        // ����Ӧ��ͨ���ִ�������ȡʵ��ʹ��ͳ��
        // Ϊ��ʾĿ�ģ�����ģ������
        return PolicyUsageStats.Create(
            Id,
            0, // ��Ҫ�����ݿ��ѯ
            0, // ��Ҫ�����ݿ��ѯ
            null, // ��Ҫ�����ݿ��ѯ
            0 // ��Ҫ���������ݼ���
        );
    }

    /// <summary>
    /// ��ȡ��������ժҪ
    /// </summary>
    public string GetConfigurationSummary()
    {
        if (!IsEnabled)
            return $"TargetingPolicy[{Id}]: {Name} - Disabled (Unrestricted)";

        var enabledTemplates = GetEnabledCriteriaTemplates().ToList();
        if (!enabledTemplates.Any())
            return $"TargetingPolicy[{Id}]: {Name} - No criteria templates defined";

        var criteriaTypes = enabledTemplates.Select(c => c.CriteriaType);
        var tagInfo = _tags.Any() ? $", Tags: [{string.Join(", ", _tags)}]" : "";

        return $"TargetingPolicy[{Id}]: {Name} - {string.Join(", ", criteriaTypes)} (Weight: {Weight:F2}, Version: {Version}, Status: {Status}){tagInfo}";
    }

    /// <summary>
    /// ���²���Ȩ��
    /// </summary>
    /// <param name="newWeight">��Ȩ��</param>
    public void UpdateWeight(decimal newWeight)
    {
        if (Status == PolicyStatus.Archived)
            throw new InvalidOperationException("�ѹ鵵�Ĳ��Բ����޸�");

        ValidateWeight(newWeight);
        Weight = newWeight;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// ���²�������״̬
    /// </summary>
    /// <param name="enabled">�Ƿ�����</param>
    public void UpdateEnabled(bool enabled)
    {
        if (Status == PolicyStatus.Archived)
            throw new InvalidOperationException("�ѹ鵵�Ĳ��Բ����޸�");

        IsEnabled = enabled;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// ���²�������
    /// </summary>
    /// <param name="description">������</param>
    public void UpdateDescription(string? description)
    {
        if (Status == PolicyStatus.Archived)
            throw new InvalidOperationException("�ѹ鵵�Ĳ��Բ����޸�");

        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// ���²��Կɼ���
    /// </summary>
    /// <param name="isPublic">�Ƿ񹫿�</param>
    public void UpdateVisibility(bool isPublic)
    {
        if (Status == PolicyStatus.Archived)
            throw new InvalidOperationException("�ѹ鵵�Ĳ��Բ����޸�");

        IsPublic = isPublic;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// ��ȡ����ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Id;
        yield return Name;
        yield return Version;
        yield return Weight;
        yield return IsEnabled;
        yield return Status;
        yield return Category;
        yield return IsPublic;

        // ��������������ȷ��һ�µıȽϽ��
        foreach (var template in CriteriaTemplates.OrderBy(c => c.CriteriaType))
        {
            yield return template.CriteriaType;
            yield return template;
        }

        // ����ǩ����ȷ��һ�µıȽϽ��
        foreach (var tag in _tags.OrderBy(t => t))
        {
            yield return tag;
        }
    }

    /// <summary>
    /// ��֤Ȩ��ֵ
    /// </summary>
    /// <param name="weight">Ȩ��ֵ</param>
    private static void ValidateWeight(decimal weight)
    {
        if (weight <= 0)
            throw new ArgumentException("Ȩ�ر������0", nameof(weight));
    }

}













