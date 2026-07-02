using AdPulse.Core.Domain.Common;

namespace AdPulse.Core.Domain.ValueObjects;



/// <summary>
/// ���������
/// </summary>
public class GeoPoint : ValueObject
{
    /// <summary>
    /// γ��
    /// </summary>
    public decimal Latitude { get; private set; }

    /// <summary>
    /// ����
    /// </summary>
    public decimal Longitude { get; private set; }

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private GeoPoint()
    {
        Latitude = 0;
        Longitude = 0;
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public GeoPoint(decimal latitude, decimal longitude)
    {
        ValidateInput(latitude, longitude);

        Latitude = latitude;
        Longitude = longitude;
    }

    /// <summary>
    /// �������������
    /// </summary>
    public static GeoPoint Create(decimal latitude, decimal longitude)
    {
        return new GeoPoint(latitude, longitude);
    }

    /// <summary>
    /// ���㵽��һ����ľ��루�ף�
    /// </summary>
    public double CalculateDistanceTo(GeoPoint other)
    {
        const double earthRadius = 6371000; // ����뾶���ף�

        var dLat = ToRadians((double)(other.Latitude - Latitude));
        var dLon = ToRadians((double)(other.Longitude - Longitude));

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians((double)Latitude)) * Math.Cos(ToRadians((double)other.Latitude)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadius * c;
    }

    /// <summary>
    /// ��ȡ�ȼ��ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Latitude;
        yield return Longitude;
    }

    /// <summary>
    /// ��֤�������
    /// </summary>
    private static void ValidateInput(decimal latitude, decimal longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentException("γ�ȱ�����-90��90֮��", nameof(latitude));

        if (longitude < -180 || longitude > 180)
            throw new ArgumentException("���ȱ�����-180��180֮��", nameof(longitude));
    }

    /// <summary>
    /// �Ƕ�ת����
    /// </summary>
    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}
