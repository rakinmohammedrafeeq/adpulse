using AdPulse.Core.AdEngine.Abstractions.Models;
using AdPulse.Infrastructure.Monitoring.Models;

namespace AdPulse.Core.AdEngine.Abstractions.Interfaces;

/// <summary>
/// ����������ϱ��ص��ӿ�
/// </summary>
public interface IAdEngineMetricsCallback : IAdEngineCallback
{
    /// <summary>
    /// �ϱ�����ִ��ָ��
    /// </summary>
    /// <param name="metrics">����ָ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�ϱ����</returns>
    Task ReportStrategyMetricsAsync(
        StrategyMetrics metrics,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��¼������Ϣ
    /// </summary>
    /// <param name="error">������Ϣ</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��¼���</returns>
    Task RecordErrorAsync(
        StrategyError error,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��¼��������
    /// </summary>
    /// <param name="performance">��������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��¼���</returns>
    Task RecordPerformanceAsync(
        PerformanceMetrics performance,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��¼ҵ��ָ��
    /// </summary>
    /// <param name="metrics">ҵ��ָ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��¼���</returns>
    Task RecordBusinessMetricsAsync(
        BusinessMetrics metrics,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��¼ִ��ͳ����Ϣ
    /// </summary>
    /// <param name="statistics">ִ��ͳ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��¼���</returns>
    Task RecordExecutionStatisticsAsync(
        ExecutionStatistics statistics,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// �����ϱ�ָ��
    /// </summary>
    /// <param name="metricsBatch">ָ������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�ϱ����</returns>
    Task ReportMetricsBatchAsync(
        IReadOnlyList<MetricsBatch> metricsBatch,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// �ϱ��澯�¼�
    /// </summary>
    /// <param name="alert">�澯�¼�</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�ϱ����</returns>
    Task ReportAlertAsync(
        AlertEvent alert,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��¼�����־
    /// </summary>
    /// <param name="auditLog">�����־</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��¼���</returns>
    Task RecordAuditLogAsync(
        AuditLog auditLog,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// �����ϱ�ָ������
    /// </summary>
    /// <param name="metricName">ָ������</param>
    /// <param name="value">ָ��ֵ</param>
    /// <param name="tags">��ǩ�ֵ�</param>
    /// <param name="timestamp">ʱ���</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�ϱ����</returns>
    Task IncrementMetricAsync(
        string metricName,
        double value,
        IReadOnlyDictionary<string, string>? tags = null,
        DateTime? timestamp = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ������ָ���ϱ�
    /// </summary>
    /// <param name="counterName">����������</param>
    /// <param name="increment">����ֵ</param>
    /// <param name="tags">��ǩ�ֵ�</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�ϱ����</returns>
    Task IncrementCounterAsync(
        string counterName,
        long increment = 1,
        IReadOnlyDictionary<string, string>? tags = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ֱ��ͼָ���ϱ�
    /// </summary>
    /// <param name="histogramName">ֱ��ͼ����</param>
    /// <param name="value">����ֵ</param>
    /// <param name="tags">��ǩ�ֵ�</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�ϱ����</returns>
    Task RecordHistogramAsync(
        string histogramName,
        double value,
        IReadOnlyDictionary<string, string>? tags = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ʱ��ָ���ϱ�
    /// </summary>
    /// <param name="timerName">��ʱ������</param>
    /// <param name="duration">����ʱ��</param>
    /// <param name="tags">��ǩ�ֵ�</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�ϱ����</returns>
    Task RecordTimerAsync(
        string timerName,
        TimeSpan duration,
        IReadOnlyDictionary<string, string>? tags = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��¼�Զ����¼�
    /// </summary>
    /// <param name="eventName">�¼�����</param>
    /// <param name="properties">�¼�����</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��¼���</returns>
    Task RecordCustomEventAsync(
        string eventName,
        IReadOnlyDictionary<string, object> properties,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ʼ����׷��
    /// </summary>
    /// <param name="operationName">��������</param>
    /// <param name="tags">��ǩ�ֵ�</param>
    /// <returns>׷��������</returns>
    IPerformanceTracker StartPerformanceTracking(
        string operationName,
        IReadOnlyDictionary<string, string>? tags = null);
}

