using AdPulse.Infrastructure.Common.Extensions;
using AdPulse.Infrastructure.Common.Models;
using System.Reflection;

namespace AdPulse.Infrastructure.DependencyInjection.Discovery;

/// <summary>
/// �������ɨ����
/// ����ɨ������е��������
/// </summary>
public class AssemblyComponentScanner
{
    private readonly List<Assembly> _assemblies = new();
    private readonly List<string> _includePatterns = new();
    private readonly List<string> _excludePatterns = new();

    /// <summary>
    /// ���Ҫɨ��ĳ���
    /// </summary>
    /// <param name="assembly">����</param>
    /// <returns>ɨ����ʵ��</returns>
    public AssemblyComponentScanner AddAssembly(Assembly assembly)
    {
        if (assembly != null && !_assemblies.Contains(assembly))
        {
            _assemblies.Add(assembly);
        }
        return this;
    }

    /// <summary>
    /// ��Ӷ��Ҫɨ��ĳ���
    /// </summary>
    /// <param name="assemblies">�����б�</param>
    /// <returns>ɨ����ʵ��</returns>
    public AssemblyComponentScanner AddAssemblies(IEnumerable<Assembly> assemblies)
    {
        foreach (var assembly in assemblies)
        {
            AddAssembly(assembly);
        }
        return this;
    }

    /// <summary>
    /// ��ӳ�������ģʽ
    /// </summary>
    /// <param name="pattern">����ģʽ</param>
    /// <returns>ɨ����ʵ��</returns>
    public AssemblyComponentScanner AddAssemblyPattern(string pattern)
    {
        if (!string.IsNullOrEmpty(pattern))
        {
            _includePatterns.Add(pattern);
        }
        return this;
    }

    /// <summary>
    /// �ų���������ģʽ
    /// </summary>
    /// <param name="pattern">�ų�ģʽ</param>
    /// <returns>ɨ����ʵ��</returns>
    public AssemblyComponentScanner ExcludeAssemblyPattern(string pattern)
    {
        if (!string.IsNullOrEmpty(pattern))
        {
            _excludePatterns.Add(pattern);
        }
        return this;
    }

    /// <summary>
    /// ɨ����������
    /// </summary>
    /// <returns>�����б�</returns>
    public IEnumerable<Type> ScanTypes()
    {
        var types = new List<Type>();
        var assemblies = GetAllAssemblies();

        foreach (var assembly in assemblies)
        {
            try
            {
                var assemblyTypes = assembly.GetConcreteTypes();
                types.AddRange(assemblyTypes);
            }
            catch (Exception)
            {
                // ���Գ��򼯼��ش���
            }
        }

        return types;
    }

    /// <summary>
    /// ɨ���������
    /// </summary>
    /// <returns>��������б�</returns>
    public IEnumerable<Type> ScanComponentTypes()
    {
        return ScanTypes().Where(IsComponentType);
    }

    /// <summary>
    /// ɨ��ʵ��ָ���ӿڵ�����
    /// </summary>
    /// <param name="interfaceType">�ӿ�����</param>
    /// <returns>ʵ�ֽӿڵ������б�</returns>
    public IEnumerable<Type> ScanTypesImplementing(Type interfaceType)
    {
        return ScanTypes().Where(t => t.ImplementsInterface(interfaceType));
    }

    /// <summary>
    /// ɨ�����ָ�����Ե�����
    /// </summary>
    /// <typeparam name="TAttribute">��������</typeparam>
    /// <returns>�������Ե������б�</returns>
    public IEnumerable<Type> ScanTypesWithAttribute<TAttribute>()
        where TAttribute : Attribute
    {
        return ScanTypes().Where(t => t.GetCustomAttributeSafe<TAttribute>() != null);
    }

    /// <summary>
    /// ��ȡ����Ҫɨ��ĳ���
    /// </summary>
    /// <returns>�����б�</returns>
    private IEnumerable<Assembly> GetAllAssemblies()
    {
        var assemblies = new HashSet<Assembly>(_assemblies);

        // ���û���ֶ���ӳ��򼯣������ģʽɨ��
        if (!assemblies.Any())
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            if (_includePatterns.Any())
            {
                // ���ݰ���ģʽ��ӳ���
                foreach (var assembly in loadedAssemblies)
                {
                    if (ShouldIncludeAssembly(assembly))
                    {
                        assemblies.Add(assembly);
                    }
                }
            }
            else
            {
                // Ĭ�����������س���
                var defaultPatterns = new[] { "AdPulse.*" };
                foreach (var assembly in loadedAssemblies)
                {
                    if (MatchesPatterns(assembly.FullName, defaultPatterns))
                    {
                        assemblies.Add(assembly);
                    }
                }
            }
        }

        return assemblies;
    }

    /// <summary>
    /// ����Ƿ�Ӧ�ð�������
    /// </summary>
    /// <param name="assembly">����</param>
    /// <returns>�Ƿ����</returns>
    private bool ShouldIncludeAssembly(Assembly assembly)
    {
        var assemblyName = assembly.FullName ?? assembly.GetName().Name ?? "";

        // ����ų�ģʽ
        if (_excludePatterns.Any(pattern => MatchesPattern(assemblyName, pattern)))
        {
            return false;
        }

        // ������ģʽ
        return !_includePatterns.Any() || _includePatterns.Any(pattern => MatchesPattern(assemblyName, pattern));
    }

    /// <summary>
    /// ��������Ƿ�ƥ��ģʽ�б�
    /// </summary>
    /// <param name="name">����</param>
    /// <param name="patterns">ģʽ�б�</param>
    /// <returns>�Ƿ�ƥ��</returns>
    private bool MatchesPatterns(string? name, IEnumerable<string> patterns)
    {
        if (string.IsNullOrEmpty(name)) return false;
        
        return patterns.Any(pattern => MatchesPattern(name, pattern));
    }

    /// <summary>
    /// ��������Ƿ�ƥ��ģʽ
    /// </summary>
    /// <param name="name">����</param>
    /// <param name="pattern">ģʽ</param>
    /// <returns>�Ƿ�ƥ��</returns>
    private bool MatchesPattern(string name, string pattern)
    {
        if (string.IsNullOrEmpty(pattern)) return false;
        
        // �򵥵�ͨ���ƥ��
        if (pattern.EndsWith("*"))
        {
            var prefix = pattern.Substring(0, pattern.Length - 1);
            return name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
        }
        
        if (pattern.StartsWith("*"))
        {
            var suffix = pattern.Substring(1);
            return name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);
        }
        
        return name.Equals(pattern, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// ��������Ƿ�Ϊ�������
    /// </summary>
    /// <param name="type">����</param>
    /// <returns>�Ƿ�Ϊ�������</returns>
    private bool IsComponentType(Type type)
    {
        if (type == null || !type.IsConcreteClass())
            return false;

        // ����Ƿ����������
        if (type.GetCustomAttributeSafe<Attributes.ComponentAttribute>() != null)
            return true;

        // ����Ƿ�������Լ��
        return AdPulse.Infrastructure.Common.Conventions.ComponentConventions.IsComponent(type);
    }
}