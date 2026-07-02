using AdPulse.Core.AdEngine.Abstractions.Models;
using AdPulse.Core.Domain.ValueObjects;
using AdPulse.Core.Domain.Requests;
using AdPulse.Core.Domain.Entities;

namespace AdPulse.Core.AdEngine.Abstractions.Interfaces;

/// <summary>
/// �������״̬ͬ���ص��ӿ�
/// ���������Ͷ�Ź����еĸ���״̬��Ϣ������Ԥ�㡢Ƶ�ο��ơ���������״̬���ݵĻ�ȡ��ͬ��
/// </summary>
public interface IAdEngineStateCallback : IAdEngineCallback
{
    #region Ԥ��״̬����

    /// <summary>
    /// ��ȡԤ��״̬
    /// </summary>
    /// <param name="campaignId">�ID</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>Ԥ��״̬</returns>
    Task<BudgetStatus> GetBudgetStatusAsync(
        string campaignId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ������ȡԤ��״̬
    /// </summary>
    /// <param name="campaignIds">�ID�б�</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>Ԥ��״̬�ֵ�</returns>
    Task<IReadOnlyDictionary<string, BudgetStatus>> GetBudgetStatusBatchAsync(
        IReadOnlyList<string> campaignIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ����Ԥ������
    /// </summary>
    /// <param name="campaignId">�ID</param>
    /// <param name="consumedAmount">���Ľ��</param>
    /// <param name="transactionId">����ID</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�����Ƿ�ɹ�</returns>
    Task<bool> UpdateBudgetConsumptionAsync(
        string campaignId,
        decimal consumedAmount,
        string? transactionId = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region Ƶ�ο��ƹ���

    /// <summary>
    /// ��ȡƵ�ο���״̬
    /// </summary>
    /// <param name="userId">�û�ID</param>
    /// <param name="adId">���ID</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>Ƶ��״̬</returns>
    Task<FrequencyStatus> GetFrequencyControlAsync(
        string userId,
        string adId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ������ȡƵ�ο���״̬
    /// </summary>
    /// <param name="requests">Ƶ�ο��������б�</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>Ƶ��״̬�ֵ�</returns>
    Task<IReadOnlyDictionary<string, FrequencyStatus>> GetFrequencyControlBatchAsync(
        IReadOnlyList<FrequencyControlRequest> requests,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ����Ƶ�μ���
    /// </summary>
    /// <param name="userId">�û�ID</param>
    /// <param name="adId">���ID</param>
    /// <param name="impressionType">�ع����ͣ��ַ�����ʽ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�����Ƿ�ɹ�</returns>
    Task<bool> UpdateFrequencyCountAsync(
        string userId,
        string adId,
        string impressionType = "Display",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ����Ƶ�μ���
    /// </summary>
    /// <param name="userId">�û�ID</param>
    /// <param name="adId">���ID</param>
    /// <param name="resetType">�������ͣ��ַ�����ʽ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�����Ƿ�ɹ�</returns>
    Task<bool> ResetFrequencyCountAsync(
        string userId,
        string adId,
        string resetType,
        CancellationToken cancellationToken = default);

    #endregion

    #region ����������

    /// <summary>
    /// ��ȡ������״̬
    /// </summary>
    /// <param name="request">�������������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>������״̬</returns>
    Task<BlacklistStatus> GetBlacklistStatusAsync(
        BlacklistCheckRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ������������״̬
    /// </summary>
    /// <param name="requests">��������������б�</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>������״̬�ֵ�</returns>
    Task<IReadOnlyDictionary<string, BlacklistStatus>> GetBlacklistStatusBatchAsync(
        IReadOnlyList<BlacklistCheckRequest> requests,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��Ӻ�������
    /// </summary>
    /// <param name="entityId">ʵ��ID</param>
    /// <param name="entityType">ʵ������</param>
    /// <param name="blacklistType">����������</param>
    /// <param name="reason">���ԭ��</param>
    /// <param name="expiresAt">����ʱ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>����Ƿ�ɹ�</returns>
    Task<bool> AddBlacklistItemAsync(
        string entityId,
        string entityType,
        string blacklistType,
        string? reason = null,
        DateTime? expiresAt = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// �Ƴ���������
    /// </summary>
    /// <param name="entityId">ʵ��ID</param>
    /// <param name="entityType">ʵ������</param>
    /// <param name="blacklistType">����������</param>
    /// <param name="reason">�Ƴ�ԭ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�Ƴ��Ƿ�ɹ�</returns>
    Task<bool> RemoveBlacklistItemAsync(
        string entityId,
        string entityType,
        string blacklistType,
        string? reason = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region �������ֹ���

    /// <summary>
    /// ��ȡ��������
    /// </summary>
    /// <param name="adId">���ID</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��������</returns>
    Task<QualityScore> GetQualityScoreAsync(
        string adId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ������ȡ��������
    /// </summary>
    /// <param name="adIds">���ID�б�</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>���������ֵ�</returns>
    Task<IReadOnlyDictionary<string, QualityScore>> GetQualityScoreBatchAsync(
        IReadOnlyList<string> adIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ������������
    /// </summary>
    /// <param name="adId">���ID</param>
    /// <param name="qualityScore">�µ���������</param>
    /// <param name="updateReason">����ԭ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�����Ƿ�ɹ�</returns>
    Task<bool> UpdateQualityScoreAsync(
        string adId,
        QualityScore qualityScore,
        string? updateReason = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region ���״̬����

    /// <summary>
    /// ��ȡ���״̬
    /// </summary>
    /// <param name="placementId">���λID</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>���״̬</returns>
    Task<InventoryStatus> GetInventoryStatusAsync(
        string placementId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ������ȡ���״̬
    /// </summary>
    /// <param name="placementIds">���λID�б�</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>���״̬�ֵ�</returns>
    Task<IReadOnlyDictionary<string, InventoryStatus>> GetInventoryStatusBatchAsync(
        IReadOnlyList<string> placementIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Ԥ����
    /// </summary>
    /// <param name="placementId">���λID</param>
    /// <param name="reservationData">Ԥ������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>Ԥ�������ֵ���ʽ��</returns>
    Task<IReadOnlyDictionary<string, object>> ReserveInventoryAsync(
        string placementId,
        IReadOnlyDictionary<string, object> reservationData,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// �ͷſ��Ԥ��
    /// </summary>
    /// <param name="reservationId">Ԥ��ID</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�ͷ��Ƿ�ɹ�</returns>
    Task<bool> ReleaseInventoryReservationAsync(
        string reservationId,
        CancellationToken cancellationToken = default);

    #endregion

    #region Ͷ��״̬����

    /// <summary>
    /// ���Ͷ���ʸ�
    /// </summary>
    /// <param name="adId">���ID</param>
    /// <param name="context">Ͷ��������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>Ͷ���ʸ�״̬</returns>
    Task<DeliveryEligibility> CheckDeliveryEligibilityAsync(
        string adId,
        AdContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// �������Ͷ���ʸ�
    /// </summary>
    /// <param name="eligibilityRequests">Ͷ���ʸ��������б���ֵ���ʽ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>Ͷ���ʸ�״̬�ֵ�</returns>
    Task<IReadOnlyDictionary<string, DeliveryEligibility>> CheckDeliveryEligibilityBatchAsync(
        IReadOnlyList<IReadOnlyDictionary<string, object>> eligibilityRequests,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ����Ͷ��״̬
    /// </summary>
    /// <param name="adId">���ID</param>
    /// <param name="status">�µ�Ͷ��״̬���ַ�����ʽ��</param>
    /// <param name="reason">״̬���ԭ��</param>
    /// <param name="metadata">״̬Ԫ����</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�Ƿ���³ɹ�</returns>
    Task<bool> UpdateDeliveryStatusAsync(
        string adId,
        string status,
        string? reason = null,
        IReadOnlyDictionary<string, object>? metadata = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region ���������Ĺ���

    /// <summary>
    /// ��ȡ����������״̬
    /// </summary>
    /// <param name="requestId">����ID</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>����������</returns>
    Task<BiddingContext> GetBiddingContextAsync(
        string requestId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��������������
    /// </summary>
    /// <param name="biddingRequest">���������ֵ���ʽ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>����������</returns>
    Task<BiddingContext> CreateBiddingContextAsync(
        IReadOnlyDictionary<string, object> biddingRequest,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ���¾���������
    /// </summary>
    /// <param name="requestId">����ID</param>
    /// <param name="contextUpdate">�����ĸ�����Ϣ���ֵ���ʽ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�����Ƿ�ɹ�</returns>
    Task<bool> UpdateBiddingContextAsync(
        string requestId,
        IReadOnlyDictionary<string, object> contextUpdate,
        CancellationToken cancellationToken = default);

    #endregion

    #region ����״̬����

    /// <summary>
    /// ��ȡʵʱ����״̬
    /// </summary>
    /// <param name="placementId">���λID</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>����״̬��Ϣ���ֵ���ʽ��</returns>
    Task<IReadOnlyDictionary<string, object>> GetCompetitionStatusAsync(
        string placementId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ�г���������
    /// </summary>
    /// <param name="placementId">���λID</param>
    /// <param name="timeWindow">ʱ�䴰��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�г������������ֵ���ʽ��</returns>
    Task<IReadOnlyDictionary<string, object>> GetMarketCompetitionAnalysisAsync(
        string placementId,
        TimeSpan timeWindow,
        CancellationToken cancellationToken = default);

    #endregion

    #region ״̬ͬ������

    /// <summary>
    /// ͬ��״̬������ⲿϵͳ
    /// </summary>
    /// <param name="stateChanges">״̬����б���ֵ���ʽ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ͬ��������ֵ���ʽ��</returns>
    Task<IReadOnlyDictionary<string, object>> SyncStateChangesAsync(
        IReadOnlyList<IReadOnlyDictionary<string, object>> stateChanges,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ����ͬ��״̬���
    /// </summary>
    /// <param name="syncRequests">ͬ�������б���ֵ���ʽ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>����ͬ��������ֵ���ʽ��</returns>
    Task<IReadOnlyDictionary<string, object>> SyncStateChangesBatchAsync(
        IReadOnlyList<IReadOnlyDictionary<string, object>> syncRequests,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ״̬ͬ����ʷ
    /// </summary>
    /// <param name="entityId">ʵ��ID</param>
    /// <param name="entityType">ʵ������</param>
    /// <param name="since">��ʼʱ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>״̬ͬ����ʷ���ֵ��б���ʽ��</returns>
    Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> GetStateSyncHistoryAsync(
        string entityId,
        string entityType,
        DateTime? since = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region ϵͳ״̬���

    /// <summary>
    /// ��ȡϵͳ����״̬
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ϵͳ������Ϣ���ֵ���ʽ��</returns>
    Task<IReadOnlyDictionary<string, object>> GetSystemLoadStatusAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ�������״̬
    /// </summary>
    /// <param name="componentName">�������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�������״̬���ֵ���ʽ��</returns>
    Task<IReadOnlyDictionary<string, object>> GetComponentHealthStatusAsync(
        string? componentName = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ����ָ�����
    /// </summary>
    /// <param name="metricTypes">ָ�������б�</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>����ָ����գ��ֵ���ʽ��</returns>
    Task<IReadOnlyDictionary<string, object>> GetPerformanceSnapshotAsync(
        IReadOnlyList<string>? metricTypes = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region ����״̬����

    /// <summary>
    /// ��ȡ����״̬
    /// </summary>
    /// <param name="cacheKey">�����</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>����״̬���ֵ���ʽ��</returns>
    Task<IReadOnlyDictionary<string, object>> GetCacheStatusAsync(
        string cacheKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ˢ�»���
    /// </summary>
    /// <param name="cacheKeys">������б�</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ˢ�½�����ֵ���ʽ��</returns>
    Task<IReadOnlyDictionary<string, object>> RefreshCacheAsync(
        IReadOnlyList<string> cacheKeys,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Ԥ�Ȼ���
    /// </summary>
    /// <param name="warmupRequest">Ԥ�������ֵ���ʽ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>Ԥ�Ƚ�����ֵ���ʽ��</returns>
    Task<IReadOnlyDictionary<string, object>> WarmupCacheAsync(
        IReadOnlyDictionary<string, object> warmupRequest,
        CancellationToken cancellationToken = default);

    #endregion

    #region ״̬��ѯ�;ۺ�

    /// <summary>
    /// ��ȡ�ۺ�״̬��Ϣ
    /// </summary>
    /// <param name="aggregationRequest">�ۺ������ֵ���ʽ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�ۺ�״̬��Ϣ���ֵ���ʽ��</returns>
    Task<IReadOnlyDictionary<string, object>> GetAggregatedStateInfoAsync(
        IReadOnlyDictionary<string, object> aggregationRequest,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ִ��״̬��ѯ
    /// </summary>
    /// <param name="queryRequest">��ѯ�����ֵ���ʽ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��ѯ������ֵ���ʽ��</returns>
    Task<IReadOnlyDictionary<string, object>> ExecuteStateQueryAsync(
        IReadOnlyDictionary<string, object> queryRequest,
        CancellationToken cancellationToken = default);

    #endregion
}