using AdPulse.Infrastructure.Common.Abstractions;

namespace AdPulse.Infrastructure.Common.Base;

/// <summary>
/// �������
/// �ṩ����������ڹ���Ļ���ʵ��
/// </summary>
public abstract class ComponentBase : IComponent, IAsyncDisposable
{
    private bool _isInitialized;
    private bool _isDisposed;

    /// <summary>
    /// �������
    /// </summary>
    public virtual string Name => GetType().Name;

    /// <summary>
    /// ����Ƿ��ѳ�ʼ��
    /// </summary>
    public bool IsInitialized => _isInitialized;

    /// <summary>
    /// ��ʼ�����
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (_isInitialized)
            return;

        if (_isDisposed)
            throw new ObjectDisposedException(Name);

        await OnInitializeAsync(cancellationToken);
        _isInitialized = true;
    }

    /// <summary>
    /// �����ʼ���ľ���ʵ�֣���������д
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    protected virtual Task OnInitializeAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// �ͷ������Դ
    /// </summary>
    public async Task DisposeAsync()
    {
        if (_isDisposed)
            return;

        await OnDisposeAsync();
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// �����Դ�ͷŵľ���ʵ�֣���������д
    /// </summary>
    protected virtual Task OnDisposeAsync()
    {
        return Task.CompletedTask;
    }

    ValueTask IAsyncDisposable.DisposeAsync()
    {
        // �ͷŷ��й���Դ
        if (!_isDisposed)
        {
            // �����������ͷ���Դ���߼�
            _isDisposed = true;
        }
        return ValueTask.CompletedTask;
    }
}