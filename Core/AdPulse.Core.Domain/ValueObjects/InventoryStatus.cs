using AdPulse.Core.Shared.Enums;
using AdPulse.Core.Domain.Common;

namespace AdPulse.Core.Domain.ValueObjects;

/// <summary>
/// ���״ֵ̬����
/// </summary>
public class InventoryStatus : ValueObject
{
    /// <summary>
    /// ���λID
    /// </summary>
    public Guid PlacementId { get; private set; }

    /// <summary>
    /// ���ÿ������
    /// </summary>
    public int AvailableInventory { get; private set; }

    /// <summary>
    /// ��Ԥ���������
    /// </summary>
    public int ReservedInventory { get; private set; }

    /// <summary>
    /// �ܿ������
    /// </summary>
    public int TotalInventory { get; private set; }

    /// <summary>
    /// ���������
    /// </summary>
    public decimal UtilizationRate => TotalInventory > 0 ?
        (decimal)ReservedInventory / TotalInventory : 0m;

    /// <summary>
    /// Ԥ�ڿ��
    /// </summary>
    public int? ForecastedInventory { get; private set; }

    /// <summary>
    /// ������ʱ��
    /// </summary>
    public DateTime LastUpdated { get; private set; }

    /// <summary>
    /// ���״̬
    /// </summary>
    public InventoryStatusType Status { get; private set; }

    /// <summary>
    /// �Ƿ��п�����
    /// </summary>
    public bool HasAvailableInventory => AvailableInventory > 0 && Status == InventoryStatusType.Available;

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private InventoryStatus()
    {
        PlacementId = Guid.Empty;
        AvailableInventory = 0;
        ReservedInventory = 0;
        TotalInventory = 0;
        LastUpdated = DateTime.UtcNow;
        Status = InventoryStatusType.Available;
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public InventoryStatus(
        Guid placementId,
        int availableInventory,
        int reservedInventory,
        int totalInventory,
        InventoryStatusType status = InventoryStatusType.Available,
        int? forecastedInventory = null,
        DateTime? lastUpdated = null)
    {
        ValidateInput(placementId, availableInventory, reservedInventory, totalInventory);

        PlacementId = placementId;
        AvailableInventory = availableInventory;
        ReservedInventory = reservedInventory;
        TotalInventory = totalInventory;
        Status = status;
        ForecastedInventory = forecastedInventory;
        LastUpdated = lastUpdated ?? DateTime.UtcNow;
    }

    /// <summary>
    /// �������״̬
    /// </summary>
    public static InventoryStatus Create(
        Guid placementId,
        int totalInventory,
        int reservedInventory = 0)
    {
        var availableInventory = totalInventory - reservedInventory;
        return new InventoryStatus(placementId, availableInventory, reservedInventory, totalInventory);
    }

    /// <summary>
    /// Ԥ�����
    /// </summary>
    public InventoryStatus ReserveInventory(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Ԥ�������������0", nameof(quantity));

        if (quantity > AvailableInventory)
            throw new InvalidOperationException("���ÿ�治��");

        return new InventoryStatus(
            PlacementId,
            AvailableInventory - quantity,
            ReservedInventory + quantity,
            TotalInventory,
            Status,
            ForecastedInventory,
            DateTime.UtcNow);
    }

    /// <summary>
    /// �ͷſ��
    /// </summary>
    public InventoryStatus ReleaseInventory(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("�ͷ������������0", nameof(quantity));

        if (quantity > ReservedInventory)
            throw new InvalidOperationException("�ͷ�����������Ԥ�����");

        return new InventoryStatus(
            PlacementId,
            AvailableInventory + quantity,
            ReservedInventory - quantity,
            TotalInventory,
            Status,
            ForecastedInventory,
            DateTime.UtcNow);
    }

    /// <summary>
    /// ���ӿ��
    /// </summary>
    public InventoryStatus AddInventory(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("���������������0", nameof(quantity));

        return new InventoryStatus(
            PlacementId,
            AvailableInventory + quantity,
            ReservedInventory,
            TotalInventory + quantity,
            Status,
            ForecastedInventory,
            DateTime.UtcNow);
    }

    /// <summary>
    /// ����״̬
    /// </summary>
    public InventoryStatus UpdateStatus(InventoryStatusType newStatus)
    {
        return new InventoryStatus(
            PlacementId,
            AvailableInventory,
            ReservedInventory,
            TotalInventory,
            newStatus,
            ForecastedInventory,
            DateTime.UtcNow);
    }

    /// <summary>
    /// ����Ԥ�ڿ��
    /// </summary>
    public InventoryStatus WithForecast(int forecastedInventory)
    {
        return new InventoryStatus(
            PlacementId,
            AvailableInventory,
            ReservedInventory,
            TotalInventory,
            Status,
            forecastedInventory,
            LastUpdated);
    }

    /// <summary>
    /// �Ƿ��治��
    /// </summary>
    public bool IsLowInventory(decimal threshold = 0.1m)
    {
        return UtilizationRate > (1m - threshold);
    }

    /// <summary>
    /// ��ȡ�ȼ��ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PlacementId;
        yield return AvailableInventory;
        yield return ReservedInventory;
        yield return TotalInventory;
        yield return Status;
        yield return ForecastedInventory ?? 0;
        yield return LastUpdated;
    }

    /// <summary>
    /// ��֤�������
    /// </summary>
    private static void ValidateInput(
        Guid placementId,
        int availableInventory,
        int reservedInventory,
        int totalInventory)
    {
        if (placementId == Guid.Empty)
            throw new ArgumentException("���λID����Ϊ��", nameof(placementId));

        if (availableInventory < 0)
            throw new ArgumentException("���ÿ����������Ϊ����", nameof(availableInventory));

        if (reservedInventory < 0)
            throw new ArgumentException("��Ԥ�������������Ϊ����", nameof(reservedInventory));

        if (totalInventory < 0)
            throw new ArgumentException("�ܿ����������Ϊ����", nameof(totalInventory));

        if (availableInventory + reservedInventory != totalInventory)
            throw new ArgumentException("���ÿ�����Ԥ�����֮�ͱ�������ܿ��");
    }
}