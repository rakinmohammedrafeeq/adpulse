using AdPulse.Infrastructure.Common.Abstractions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AdPulse.Infrastructure.Common.Base;

/// <summary>
/// �ɽ��н��������������
/// �ṩ�������Ļ���ʵ��
/// </summary>
public abstract class HealthCheckableComponentBase : ComponentBase, IHealthCheckable
{
    /// <summary>
    /// ִ�н������
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>����״̬</returns>
    public virtual async Task<HealthStatus> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // �������Ƿ��ѳ�ʼ��
            if (!IsInitialized)
                return HealthStatus.Unhealthy;

            // ִ�о���Ľ�������߼�
            return await OnCheckHealthAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // ��ʱ/ȡ��Ӧ���ϴ��������ڵ��÷�����������ȡ������
            throw;
        }
        catch (Exception)
        {
            return HealthStatus.Unhealthy;
        }
    }

    /// <summary>
    /// ����Ľ������ʵ�֣���������д
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>����״̬</returns>
    protected virtual Task<HealthStatus> OnCheckHealthAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthStatus.Healthy);
    }
}