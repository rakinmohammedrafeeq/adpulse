using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AdPulse.Infrastructure.Common.Abstractions;

/// <summary>
/// ����ɽ��н�����������ӿ�
/// ʵ�ִ˽ӿڵ�������Զ�����ӵ��������ϵͳ��
/// </summary>
public interface IHealthCheckable
{
    /// <summary>
    /// ִ�н������
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>����״̬</returns>
    Task<HealthStatus> CheckHealthAsync(CancellationToken cancellationToken = default);
}