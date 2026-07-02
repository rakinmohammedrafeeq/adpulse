using AdPulse.Core.Shared.Enums;
using AdPulse.Core.Domain.Common;

namespace AdPulse.Core.Domain.ValueObjects;

/// <summary>
/// ������Ϣֵ����
/// </summary>
public class CreativeInfo : ValueObject
{
    /// <summary>
    /// �������
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// ��������
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// �ز�URL
    /// </summary>
    public string MaterialUrl { get; private set; }

    /// <summary>
    /// �����תURL
    /// </summary>
    public string ClickUrl { get; private set; }

    /// <summary>
    /// ������
    /// </summary>
    public int Width { get; private set; }

    /// <summary>
    /// ����߶�
    /// </summary>
    public int Height { get; private set; }

    /// <summary>
    /// ý������
    /// </summary>
    public string MimeType { get; private set; }

    /// <summary>
    /// �ļ���С���ֽڣ�
    /// </summary>
    public long FileSize { get; private set; }

    /// <summary>
    /// �����ʽ
    /// </summary>
    public CreativeFormat Format { get; private set; }

    /// <summary>
    /// ��չ����
    /// </summary>
    public IReadOnlyList<ContextProperty> Attributes { get; private set; }

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private CreativeInfo(
        string title,
        string materialUrl,
        string clickUrl,
        int width,
        int height,
        string mimeType,
        long fileSize,
        CreativeFormat format,
        string? description = null,
        IReadOnlyList<ContextProperty>? attributes = null)
    {
        Title = title;
        Description = description;
        MaterialUrl = materialUrl;
        ClickUrl = clickUrl;
        Width = width;
        Height = height;
        MimeType = mimeType;
        FileSize = fileSize;
        Format = format;
        Attributes = attributes?.ToList().AsReadOnly() ?? new List<ContextProperty>().AsReadOnly();
    }

    /// <summary>
    /// ��������������������Ϣ
    /// </summary>
    public static CreativeInfo Create(
        string title,
        string materialUrl,
        string clickUrl,
        int width,
        int height,
        string mimeType,
        long fileSize,
        CreativeFormat format = CreativeFormat.Banner,
        string? description = null,
        IReadOnlyList<ContextProperty>? attributes = null)
    {
        ValidateTitle(title);
        ValidateUrl(materialUrl, nameof(materialUrl));
        ValidateUrl(clickUrl, nameof(clickUrl));
        ValidateDimensions(width, height);
        ValidateMimeType(mimeType);
        ValidateFileSize(fileSize);

        return new CreativeInfo(
            title,
            materialUrl,
            clickUrl,
            width,
            height,
            mimeType,
            fileSize,
            format,
            description,
            attributes);
    }

    /// <summary>
    /// ����ͼƬ����
    /// </summary>
    public static CreativeInfo CreateImageBanner(
        string title,
        string imageUrl,
        string clickUrl,
        int width,
        int height,
        string? description = null)
    {
        return Create(
            title,
            imageUrl,
            clickUrl,
            width,
            height,
            "image/jpeg",
            0,
            CreativeFormat.Banner,
            description);
    }

    /// <summary>
    /// ������Ƶ����
    /// </summary>
    public static CreativeInfo CreateVideoAd(
        string title,
        string videoUrl,
        string clickUrl,
        int width,
        int height,
        long fileSize,
        string? description = null)
    {
        return Create(
            title,
            videoUrl,
            clickUrl,
            width,
            height,
            "video/mp4",
            fileSize,
            CreativeFormat.Video,
            description);
    }

    /// <summary>
    /// ����ԭ����洴��
    /// </summary>
    public static CreativeInfo CreateNativeAd(
        string title,
        string materialUrl,
        string clickUrl,
        string? description = null,
        IReadOnlyList<ContextProperty>? nativeAttributes = null)
    {
        return Create(
            title,
            materialUrl,
            clickUrl,
            0,
            0,
            "text/html",
            0,
            CreativeFormat.Native,
            description,
            nativeAttributes);
    }

    /// <summary>
    /// ��ʽ��������������
    /// </summary>
    public CreativeInfo WithDescription(string? description)
    {
        return new CreativeInfo(
            Title,
            MaterialUrl,
            ClickUrl,
            Width,
            Height,
            MimeType,
            FileSize,
            Format,
            description,
            Attributes);
    }

    /// <summary>
    /// ��ʽ������������չ����
    /// </summary>
    public CreativeInfo WithAttributes(IReadOnlyList<ContextProperty> attributes)
    {
        return new CreativeInfo(
            Title,
            MaterialUrl,
            ClickUrl,
            Width,
            Height,
            MimeType,
            FileSize,
            Format,
            Description,
            attributes);
    }

    /// <summary>
    /// ��ʽ��������ӵ�������
    /// </summary>
    public CreativeInfo WithAttribute(string key, object value)
    {
        var newAttributes = new List<ContextProperty>(Attributes);

        // �Ƴ��Ѵ��ڵ���ͬ��������
        newAttributes.RemoveAll(attr => attr.PropertyKey == key);

        // ���������
        newAttributes.Add(new ContextProperty(key, value?.ToString() ?? string.Empty));

        return WithAttributes(newAttributes.AsReadOnly());
    }

    /// <summary>
    /// ҵ�񷽷����Ƿ�Ϊ��Ƶ���
    /// </summary>
    public bool IsVideoAd => Format == CreativeFormat.Video;

    /// <summary>
    /// ҵ�񷽷����Ƿ�ΪͼƬ���
    /// </summary>
    public bool IsImageAd => Format == CreativeFormat.Banner && MimeType.StartsWith("image/");

    /// <summary>
    /// ҵ�񷽷����Ƿ�Ϊԭ�����
    /// </summary>
    public bool IsNativeAd => Format == CreativeFormat.Native;

    /// <summary>
    /// ҵ�񷽷����Ƿ�Ϊ��ý����
    /// </summary>
    public bool IsRichMediaAd => Format == CreativeFormat.Expandable || Format == CreativeFormat.Interstitial;

    /// <summary>
    /// ҵ�񷽷�����ȡ��߱�
    /// </summary>
    public double GetAspectRatio()
    {
        if (Height == 0) return 0;
        return (double)Width / Height;
    }

    /// <summary>
    /// ҵ�񷽷����Ƿ�Ϊ��׼�ߴ�
    /// </summary>
    public bool IsStandardSize()
    {
        return (Width, Height) switch
        {
            (728, 90) => true,   // ���а�
            (300, 250) => true,  // �еȾ���
            (320, 50) => true,   // �ƶ����
            (160, 600) => true,  // ��Ħ���¥
            (300, 600) => true,  // ��ҳ���
            (320, 480) => true,  // �ƶ�����
            _ => false
        };
    }

    /// <summary>
    /// ҵ�񷽷�����֤�����Ƿ�������ָ���豸
    /// </summary>
    public bool IsCompatibleWithDevice(DeviceType deviceType)
    {
        return deviceType switch
        {
            DeviceType.Smartphone => Width <= 375 && Height <= 667,
            DeviceType.Tablet => Width <= 768 && Height <= 1024,
            DeviceType.PersonalComputer => Width <= 1920 && Height <= 1080,
            _ => true
        };
    }

    /// <summary>
    /// ҵ�񷽷�����ȡ����������Ϣ
    /// </summary>
    public string GetDisplayInfo()
    {
        return $"{Title} ({Width}x{Height}, {Format})";
    }

    /// <summary>
    /// ��ȡָ����������ֵ
    /// </summary>
    /// <param name="key">���Լ�</param>
    /// <returns>����ֵ������������򷵻�null</returns>
    public string? GetAttributeValue(string key)
    {
        return Attributes.FirstOrDefault(attr => attr.PropertyKey == key)?.PropertyValue;
    }

    /// <summary>
    /// ��ȡָ������ǿ��������ֵ
    /// </summary>
    /// <typeparam name="T">Ŀ������</typeparam>
    /// <param name="key">���Լ�</param>
    /// <returns>ת���������ֵ</returns>
    public T? GetAttributeValue<T>(string key) where T : class
    {
        var property = Attributes.FirstOrDefault(attr => attr.PropertyKey == key);
        return property?.GetValue<T>();
    }

    /// <summary>
    /// ����Ƿ����ָ����������
    /// </summary>
    /// <param name="key">���Լ�</param>
    /// <returns>�Ƿ����</returns>
    public bool HasAttribute(string key)
    {
        return Attributes.Any(attr => attr.PropertyKey == key);
    }

    /// <summary>
    /// ��ȡ�������Լ�
    /// </summary>
    /// <returns>���Լ��б�</returns>
    public IEnumerable<string> GetAttributeKeys()
    {
        return Attributes.Select(attr => attr.PropertyKey);
    }

    #region ����У��

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("������ⲻ��Ϊ��", nameof(title));

        if (title.Length > 100)
            throw new ArgumentException("������ⳤ�Ȳ��ܳ���100���ַ�", nameof(title));
    }

    private static void ValidateUrl(string url, string paramName)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException($"{paramName}����Ϊ��", paramName);

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            throw new ArgumentException($"{paramName}��ʽ����ȷ", paramName);

        if (uri.Scheme != "http" && uri.Scheme != "https")
            throw new ArgumentException($"{paramName}������HTTP��HTTPSЭ��", paramName);
    }

    private static void ValidateDimensions(int width, int height)
    {
        if (width < 0)
            throw new ArgumentException("��Ȳ���Ϊ����", nameof(width));

        if (height < 0)
            throw new ArgumentException("�߶Ȳ���Ϊ����", nameof(height));

        if (width == 0 && height == 0)
            return; // ԭ��������û�й̶��ߴ�

        if (width > 0 && height == 0)
            throw new ArgumentException("��������˿�ȣ��߶Ȳ���Ϊ0", nameof(height));

        if (height > 0 && width == 0)
            throw new ArgumentException("��������˸߶ȣ���Ȳ���Ϊ0", nameof(width));
    }

    private static void ValidateMimeType(string mimeType)
    {
        if (string.IsNullOrWhiteSpace(mimeType))
            throw new ArgumentException("ý�����Ͳ���Ϊ��", nameof(mimeType));

        if (!mimeType.Contains('/'))
            throw new ArgumentException("ý�����͸�ʽ����ȷ", nameof(mimeType));
    }

    private static void ValidateFileSize(long fileSize)
    {
        if (fileSize < 0)
            throw new ArgumentException("�ļ���С����Ϊ����", nameof(fileSize));

        if (fileSize > 50 * 1024 * 1024) // 50MB
            throw new ArgumentException("�ļ���С���ܳ���50MB", nameof(fileSize));
    }

    #endregion

    #region �ȼ��ԱȽ�

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Id;
        yield return Title;
        yield return Description ?? string.Empty;
        yield return MaterialUrl;
        yield return ClickUrl;
        yield return Width;
        yield return Height;
        yield return MimeType;
        yield return FileSize;
        yield return Format;

        // ע�⣺����ContextProperty���ϣ�����������ȷ��һ�µ�����ԱȽ�
        foreach (var attr in Attributes.OrderBy(x => x.PropertyKey))
        {
            yield return attr.PropertyKey;
            yield return attr.PropertyValue;
        }
    }

    #endregion
}