using System.Reflection;

namespace AdPulse.Infrastructure.Common.Extensions;

/// <summary>
/// ������չ����
/// </summary>
public static class ReflectionExtensions
{
    /// <summary>
    /// ��ȡ���������еľ���������
    /// </summary>
    /// <param name="assembly">����</param>
    /// <returns>�����������б�</returns>
    public static IEnumerable<Type> GetConcreteTypes(this Assembly assembly)
    {
        try
        {
            return assembly.GetTypes().Where(t => t.IsConcreteClass());
        }
        catch (ReflectionTypeLoadException ex)
        {
            // �������ͼ����쳣�����سɹ����ص�����
            return ex.Types.Where(t => t != null && t.IsConcreteClass())!;
        }
    }

    /// <summary>
    /// ��ȡ������ʵ��ָ���ӿڵ���������
    /// </summary>
    /// <param name="assembly">����</param>
    /// <param name="interfaceType">�ӿ�����</param>
    /// <returns>ʵ�ֽӿڵ������б�</returns>
    public static IEnumerable<Type> GetTypesImplementing(this Assembly assembly, Type interfaceType)
    {
        return assembly.GetConcreteTypes().Where(t => t.ImplementsInterface(interfaceType));
    }

    /// <summary>
    /// ��ȡ������ʵ��ָ���ӿ����Ƶ���������
    /// </summary>
    /// <param name="assembly">����</param>
    /// <param name="interfaceName">�ӿ�����</param>
    /// <returns>ʵ�ֽӿڵ������б�</returns>
    public static IEnumerable<Type> GetTypesImplementing(this Assembly assembly, string interfaceName)
    {
        return assembly.GetConcreteTypes().Where(t => t.ImplementsInterface(interfaceName));
    }

    /// <summary>
    /// ��ȡ�����м̳�ָ���������������
    /// </summary>
    /// <param name="assembly">����</param>
    /// <param name="baseType">��������</param>
    /// <returns>�̳л���������б�</returns>
    public static IEnumerable<Type> GetTypesInheriting(this Assembly assembly, Type baseType)
    {
        return assembly.GetConcreteTypes().Where(t => baseType.IsAssignableFrom(t) && t != baseType);
    }

    /// <summary>
    /// ��ȡ�����д���ָ�����Ե���������
    /// </summary>
    /// <param name="assembly">����</param>
    /// <param name="attributeType">��������</param>
    /// <returns>�������Ե������б�</returns>
    public static IEnumerable<Type> GetTypesWithAttribute(this Assembly assembly, Type attributeType)
    {
        // ʹ�� IsDefined �Ա��⵱ͬһ����������Ӧ��ʱ���� AmbiguousMatchException
        return assembly.GetConcreteTypes().Where(t => t.IsDefined(attributeType, inherit: false));
    }

    /// <summary>
    /// ��ȡ�����д���ָ�����Ե���������
    /// </summary>
    /// <typeparam name="TAttribute">��������</typeparam>
    /// <param name="assembly">����</param>
    /// <returns>�������Ե������б�</returns>
    public static IEnumerable<Type> GetTypesWithAttribute<TAttribute>(this Assembly assembly)
        where TAttribute : Attribute
    {
        return assembly.GetTypesWithAttribute(typeof(TAttribute));
    }

    /// <summary>
    /// ��ȫ�ػ�ȡ���͵��Զ�������
    /// </summary>
    /// <typeparam name="TAttribute">��������</typeparam>
    /// <param name="type">����</param>
    /// <returns>����ʵ��������������򷵻�null</returns>
    public static TAttribute? GetCustomAttributeSafe<TAttribute>(this Type type)
        where TAttribute : Attribute
    {
        try
        {
            // �����ڶ����ͬ����ʱ��GetCustomAttribute �����׳������쳣�������Ϊ��ȡ��һ��
            return type.GetCustomAttributes(typeof(TAttribute), inherit: false).FirstOrDefault() as TAttribute;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// ��ȫ�ػ�ȡ���͵������Զ�������
    /// </summary>
    /// <typeparam name="TAttribute">��������</typeparam>
    /// <param name="type">����</param>
    /// <returns>����ʵ���б�</returns>
    public static IEnumerable<TAttribute> GetCustomAttributesSafe<TAttribute>(this Type type)
        where TAttribute : Attribute
    {
        try
        {
            // ʹ�÷Ƿ������أ������������ʱʵ���ϵ��쳣���
            return type.GetCustomAttributes(typeof(TAttribute), inherit: false).Cast<TAttribute>();
        }
        catch
        {
            return Enumerable.Empty<TAttribute>();
        }
    }

    /// <summary>
    /// �������͵�ʵ��
    /// </summary>
    /// <param name="type">����</param>
    /// <param name="args">���캯������</param>
    /// <returns>����ʵ��</returns>
    public static object? CreateInstance(this Type type, params object[] args)
    {
        try
        {
            return Activator.CreateInstance(type, args);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// ��ȫ�ش������͵�ʵ��
    /// </summary>
    /// <typeparam name="T">Ŀ������</typeparam>
    /// <param name="type">����</param>
    /// <param name="args">���캯������</param>
    /// <returns>����ʵ��</returns>
    public static T? CreateInstance<T>(this Type type, params object[] args)
        where T : class
    {
        return CreateInstance(type, args) as T;
    }
}