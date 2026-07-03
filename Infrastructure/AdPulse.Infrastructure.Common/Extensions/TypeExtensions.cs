using System.Reflection;

namespace AdPulse.Infrastructure.Common.Extensions;

/// <summary>
/// ������չ����
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// ��ȡ���͵��Ѻ�����
    /// </summary>
    /// <param name="type">����</param>
    /// <returns>�Ѻ�����</returns>
    public static string GetFriendlyName(this Type type)
    {
        if (type.IsGenericType)
        {
            var genericArgs = type.GetGenericArguments();
            var typeName = type.Name.Substring(0, type.Name.IndexOf('`'));
            var genericArgNames = string.Join(",", genericArgs.Select(x => x.GetFriendlyName()));
            return $"{typeName}<{genericArgNames}>";
        }

        return type.Name;
    }

    /// <summary>
    /// ��������Ƿ�ʵ����ָ���Ľӿ�
    /// </summary>
    /// <param name="type">����</param>
    /// <param name="interfaceType">�ӿ�����</param>
    /// <returns>�Ƿ�ʵ�ֽӿ�</returns>
    public static bool ImplementsInterface(this Type type, Type interfaceType)
    {
        if (!interfaceType.IsInterface)
            return false;

        return interfaceType.IsAssignableFrom(type);
    }

    /// <summary>
    /// ��������Ƿ�ʵ����ָ�����ƵĽӿ�
    /// </summary>
    /// <param name="type">����</param>
    /// <param name="interfaceName">�ӿ�����</param>
    /// <returns>�Ƿ�ʵ�ֽӿ�</returns>
    public static bool ImplementsInterface(this Type type, string interfaceName)
    {
        return type.GetInterfaces().Any(i => i.Name == interfaceName);
    }

    /// <summary>
    /// ��������Ƿ�Ϊ�����ࣨ�ǳ��󡢷ǽӿڡ��Ƿ��Ͷ��壩
    /// </summary>
    /// <param name="type">����</param>
    /// <returns>�Ƿ�Ϊ������</returns>
    public static bool IsConcreteClass(this Type type)
    {
        // �������ͣ��ǳ��󡢷ǿ��ŷ��ͣ�ͬʱ��ֵ����(����DateTime, decimal, �Զ���struct)��Ϊ�����������
        if (type.IsValueType)
            return true;
        return type.IsClass && !type.IsAbstract && !type.IsGenericTypeDefinition;
    }

    /// <summary>
    /// ��ȡ����ʵ�ֵ����нӿڣ������̳еĽӿڣ�
    /// </summary>
    /// <param name="type">����</param>
    /// <returns>�ӿ��б�</returns>
    public static IEnumerable<Type> GetAllInterfaces(this Type type)
    {
        var interfaces = new HashSet<Type>();

        foreach (var iface in type.GetInterfaces())
        {
            interfaces.Add(iface);
            foreach (var baseIface in iface.GetInterfaces())
            {
                interfaces.Add(baseIface);
            }
        }

        return interfaces;
    }

    /// <summary>
    /// ��ȡ���͵�Ĭ�Ϲ��캯��
    /// </summary>
    /// <param name="type">����</param>
    /// <returns>Ĭ�Ϲ��캯��</returns>
    public static ConstructorInfo? GetDefaultConstructor(this Type type)
    {
        return type.GetConstructor(Type.EmptyTypes);
    }

    /// <summary>
    /// ��������Ƿ���Ĭ�Ϲ��캯��
    /// </summary>
    /// <param name="type">����</param>
    /// <returns>�Ƿ���Ĭ�Ϲ��캯��</returns>
    public static bool HasDefaultConstructor(this Type type)
    {
        return GetDefaultConstructor(type) != null;
    }

    /// <summary>
    /// ��ȡ���͵����й������캯��
    /// </summary>
    /// <param name="type">����</param>
    /// <returns>���캯���б�</returns>
    public static IEnumerable<ConstructorInfo> GetPublicConstructors(this Type type)
    {
        return type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
    }
}