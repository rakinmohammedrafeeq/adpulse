using AdPulse.Core.Domain.Common;
using AdPulse.Core.Domain.ValueObjects;

namespace AdPulse.Core.Domain.Targeting
{
    /// <summary>
    /// ���������Ļ���ʵ����
    /// �ṩITargetingContext�ӿڵı�׼ʵ��
    /// </summary>
    public class TargetingContextBase : ValueObject, ITargetingContext
    {
        private readonly List<ContextProperty> _properties;

        /// <summary>
        /// ����������
        /// </summary>
        public virtual string ContextName { get; protected set; }

        /// <summary>
        /// ���������ͱ�ʶ
        /// </summary>
        public string ContextType { get; protected set; }

        /// <summary>
        /// ���������Լ��ϣ�ֻ����
        /// </summary>
        public IReadOnlyList<ContextProperty> Properties => _properties.AsReadOnly();

        /// <summary>
        /// �����Ĵ���ʱ���
        /// </summary>
        public DateTime Timestamp { get; protected set; }

        /// <summary>
        /// �����ĵ�Ψһ��ʶ
        /// </summary>
        public Guid ContextId { get; protected set; }

        /// <summary>
        /// ������������Դ
        /// </summary>
        public string DataSource { get; private set; }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="contextType">����������</param>
        /// <param name="properties">���Լ���</param>
        /// <param name="dataSource">������Դ</param>
        /// <param name="contextId">�����ı�ʶ</param>
        public TargetingContextBase(
            string contextType,
            IEnumerable<ContextProperty>? properties = null,
            string? dataSource = null,
            Guid? contextId = null)
        {
            if (string.IsNullOrEmpty(contextType))
                throw new ArgumentException("���������Ͳ���Ϊ��", nameof(contextType));

            ContextType = contextType;
            ContextName = contextType; // Ĭ��ʹ��������Ϊ����
            _properties = properties?.ToList() ?? new List<ContextProperty>();
            DataSource = dataSource ?? "Unknown";
            ContextId = contextId ?? Guid.CreateVersion7();
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// ���ֵ䴴���Ĺ��캯���������ԣ�
        /// </summary>
        /// <param name="contextType">����������</param>
        /// <param name="properties">�����ֵ�</param>
        /// <param name="dataSource">������Դ</param>
        /// <param name="contextId">�����ı�ʶ</param>
        public TargetingContextBase(
            string contextType,
            IDictionary<string, object>? properties,
            string? dataSource = null,
            Guid? contextId = null)
        {
            if (string.IsNullOrEmpty(contextType))
                throw new ArgumentException("���������Ͳ���Ϊ��", nameof(contextType));

            ContextType = contextType;
            ContextName = contextType; // Ĭ��ʹ��������Ϊ����
            DataSource = dataSource ?? "Unknown";
            ContextId = contextId ?? Guid.CreateVersion7();
            Timestamp = DateTime.UtcNow;

            _properties = new List<ContextProperty>();
            if (properties != null)
            {
                foreach (var kvp in properties)
                {
                    string valueStr;
                    string dataType;

                    if (kvp.Value is null)
                    {
                        valueStr = string.Empty;
                        dataType = "String";
                    }
                    else
                    {
                        var t = kvp.Value.GetType();
                        // Simple primitives we can store as plain strings
                        if (t.IsPrimitive || kvp.Value is decimal || kvp.Value is DateTime || kvp.Value is DateTimeOffset || kvp.Value is Guid || kvp.Value is string)
                        {
                            if (kvp.Value is IFormattable formattable && kvp.Value is not string)
                            {
                                valueStr = formattable.ToString(null, System.Globalization.CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                valueStr = kvp.Value.ToString() ?? string.Empty;
                            }

                            dataType = t == typeof(string) ? "String" : t.Name;
                        }
                        else
                        {
                            // Complex types / collections -> JSON
                            valueStr = System.Text.Json.JsonSerializer.Serialize(kvp.Value);
                            dataType = "Json";
                        }
                    }

                    var property = new ContextProperty(kvp.Key, valueStr, dataType, category: ContextType, isSensitive: false, weight: 1.0m, expiresAt: null, dataSource: DataSource);
                    _properties.Add(property);
                }
            }
        }

        /// <summary>
        /// ��ȡָ����������ʵ��
        /// </summary>
        /// <param name="propertyKey">���Լ�</param>
        /// <returns>����ʵ�壬����������򷵻�null</returns>
        public virtual ContextProperty? GetProperty(string propertyKey)
        {
            if (string.IsNullOrEmpty(propertyKey))
                return null;

            return _properties.FirstOrDefault(p => p.PropertyKey == propertyKey);
        }

        /// <summary>
        /// ��ȡָ�����͵�����ֵ
        /// </summary>
        /// <typeparam name="T">����ֵ����</typeparam>
        /// <param name="propertyKey">���Լ�</param>
        /// <returns>����ֵ������������򷵻�Ĭ��ֵ</returns>
        public virtual T? GetPropertyValue<T>(string propertyKey)
        {
            var property = GetProperty(propertyKey);
            return property == null ? default(T) : property.GetValue<T>();
        }

        /// <summary>
        /// ��ȡָ�����͵�����ֵ������������򷵻�Ĭ��ֵ
        /// </summary>
        /// <typeparam name="T">����ֵ����</typeparam>
        /// <param name="propertyKey">���Լ�</param>
        /// <param name="defaultValue">Ĭ��ֵ</param>
        /// <returns>����ֵ��Ĭ��ֵ</returns>
        public virtual T GetPropertyValue<T>(string propertyKey, T defaultValue)
        {
            var result = GetPropertyValue<T>(propertyKey);
            return result != null ? result : defaultValue;
        }

        /// <summary>
        /// ��ȡ����ֵ���ַ�����ʾ
        /// </summary>
        /// <param name="propertyKey">���Լ�</param>
        /// <returns>����ֵ���ַ�����ʾ������������򷵻ؿ��ַ���</returns>
        public virtual string GetPropertyAsString(string propertyKey)
        {
            var property = GetProperty(propertyKey);
            return property?.PropertyValue ?? string.Empty;
        }

        /// <summary>
        /// ����Ƿ����ָ��������
        /// </summary>
        /// <param name="propertyKey">���Լ�</param>
        /// <returns>�Ƿ����������</returns>
        public virtual bool HasProperty(string propertyKey)
        {
            return GetProperty(propertyKey) != null;
        }

        /// <summary>
        /// ��ȡ�������Լ�
        /// </summary>
        /// <returns>���Լ�����</returns>
        public virtual IReadOnlyCollection<string> GetPropertyKeys()
        {
            return _properties.Select(p => p.PropertyKey).ToList().AsReadOnly();
        }

        /// <summary>
        /// ��ȡָ�����������
        /// </summary>
        /// <param name="category">���Է���</param>
        /// <returns>���Լ���</returns>
        public virtual IReadOnlyList<ContextProperty> GetPropertiesByCategory(string category)
        {
            return _properties.Where(p => p.Category == category).ToList().AsReadOnly();
        }

        /// <summary>
        /// ��ȡδ���ڵ�����
        /// </summary>
        /// <returns>δ���ڵ����Լ���</returns>
        public virtual IReadOnlyList<ContextProperty> GetActiveProperties()
        {
            return _properties.Where(p => !p.IsExpired()).ToList().AsReadOnly();
        }

        /// <summary>
        /// ��������������Ƿ���Ч
        /// </summary>
        /// <returns>�Ƿ���Ч</returns>
        public virtual bool IsValid()
        {
            // ������֤�����������Ͳ���Ϊ�գ�ʱ���������Ĭ��ֵ
            if (string.IsNullOrEmpty(ContextType))
                return false;

            if (Timestamp == default)
                return false;

            return true;
        }

        /// <summary>
        /// ��������������Ƿ��ѹ���
        /// </summary>
        /// <param name="maxAge">�����Ч��</param>
        /// <returns>�Ƿ��ѹ���</returns>
        public virtual bool IsExpired(TimeSpan maxAge)
        {
            return DateTime.UtcNow - Timestamp > maxAge;
        }

        /// <summary>
        /// ��ȡ�����ĵ�Ԫ������Ϣ
        /// </summary>
        /// <returns>Ԫ�������Լ���</returns>
        public virtual IReadOnlyList<ContextProperty> GetMetadata()
        {
            var metadataProperties = new List<ContextProperty>
            {
                new ContextProperty("ContextType", ContextType),
                new ContextProperty("ContextId", ContextId.ToString()),
                new ContextProperty("DataSource", DataSource),
                new ContextProperty("Timestamp", Timestamp.ToString("O")),
                new ContextProperty("PropertyCount", _properties.Count.ToString()),
                new ContextProperty("Age", (DateTime.UtcNow - Timestamp).ToString())
            };

            return metadataProperties.AsReadOnly();
        }

        /// <summary>
        /// ��ȡ�����ĵĵ�����Ϣ
        /// </summary>
        /// <returns>������Ϣ�ַ���</returns>
        public virtual string GetDebugInfo()
        {
            var propertyInfo = string.Join(", ", _properties.Take(5).Select(p => p.PropertyKey));
            if (_properties.Count > 5)
                propertyInfo += $" (and {_properties.Count - 5} more)";

            return $"Context[{ContextType}] Id:{ContextId} Source:{DataSource} " +
                   $"Properties:{_properties.Count} [{propertyInfo}] Age:{DateTime.UtcNow - Timestamp:hh\\:mm\\:ss}";
        }

        /// <summary>
        /// ���������ĵ�����������
        /// </summary>
        /// <param name="includeKeys">Ҫ���������Լ�</param>
        /// <returns>�����������ĸ���</returns>
        public virtual ITargetingContext CreateLightweightCopy(IEnumerable<string> includeKeys)
        {
            var filteredProperties = _properties.Where(p => includeKeys.Contains(p.PropertyKey))
                                               .Select(p => p.Clone())
                                               .ToList();

            return new TargetingContextBase(
                ContextType + "_Lightweight",
                filteredProperties,
                DataSource,
                new Guid(ContextId.ToByteArray()));
        }

        /// <summary>
        /// ���������ĵķ��ั��
        /// </summary>
        /// <param name="categories">Ҫ���������Է���</param>
        /// <returns>���������ĸ���</returns>
        public virtual ITargetingContext CreateCategorizedCopy(IEnumerable<string> categories)
        {
            var filteredProperties = _properties.Where(p => categories.Contains(p.Category))
                                               .Select(p => p.Clone())
                                               .ToList();

            return new TargetingContextBase(
                ContextType + "_Categorized",
                filteredProperties,
                DataSource,
                ContextId);
        }

        /// <summary>
        /// �ϲ���һ�������ĵ�����
        /// </summary>
        /// <param name="other">Ҫ�ϲ���������</param>
        /// <param name="overwriteExisting">�Ƿ񸲸��Ѵ��ڵ�����</param>
        /// <returns>�ϲ������������</returns>
        public virtual ITargetingContext Merge(ITargetingContext other, bool overwriteExisting = false)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            var mergedProperties = new List<ContextProperty>(_properties);
            var mergedContextId = $"{ContextId}_Merged_{other.ContextId}";
            var mergedGuid = Guid.NewGuid();

            foreach (var property in other.Properties)
            {
                var existingProperty = mergedProperties.FirstOrDefault(p => p.PropertyKey == property.PropertyKey);

                if (existingProperty != null && overwriteExisting)
                {
                    mergedProperties.Remove(existingProperty);
                    mergedProperties.Add(property.Clone());
                }
                else if (existingProperty == null)
                {
                    mergedProperties.Add(property.Clone());
                }
            }

            var mergedContextType = $"{ContextType}_Merged_{other.ContextType}";
            var mergedDataSource = $"{DataSource},{other.DataSource}";

            return new TargetingContextBase(
                mergedContextType,
                mergedProperties,
                mergedDataSource,
                new Guid(mergedGuid.ToByteArray()));
        }

        /// <summary>
        /// ��ӻ��������
        /// </summary>
        /// <param name="propertyKey">���Լ�</param>
        /// <param name="propertyValue">����ֵ</param>
        public virtual void SetProperty(string propertyKey, object propertyValue)
        {
            if (string.IsNullOrEmpty(propertyKey))
                throw new ArgumentException("���Լ�����Ϊ��", nameof(propertyKey));

            var existingProperty = GetProperty(propertyKey);
            if (existingProperty != null)
            {
                _properties.Remove(existingProperty);
                var updatedProperty = existingProperty.WithValue(propertyValue ?? string.Empty);
                _properties.Add(updatedProperty);
            }
            else
            {
                var newProperty = new ContextProperty(propertyKey, propertyValue?.ToString() ?? string.Empty);
                _properties.Add(newProperty);
            }
        }

        /// <summary>
        /// �Ƴ�����
        /// </summary>
        /// <param name="propertyKey">���Լ�</param>
        /// <returns>�Ƿ�ɹ��Ƴ�</returns>
        public virtual bool RemoveProperty(string propertyKey)
        {
            if (string.IsNullOrEmpty(propertyKey))
                return false;

            var property = GetProperty(propertyKey);
            if (property != null)
            {
                return _properties.Remove(property);
            }
            return false;
        }

        /// <summary>
        /// ������������
        /// </summary>
        /// <param name="properties">�����ֵ�</param>
        /// <param name="overwriteExisting">�Ƿ񸲸��Ѵ��ڵ�����</param>
        public virtual void SetProperties(IDictionary<string, object> properties, bool overwriteExisting = true)
        {
            if (properties == null)
                return;

            foreach (var kvp in properties)
            {
                if (overwriteExisting || !HasProperty(kvp.Key))
                {
                    SetProperty(kvp.Key, kvp.Value);
                }
            }
        }

        /// <summary>
        /// ��ȡ����ԱȽϵ����
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return ContextType;
            yield return ContextId;
            yield return DataSource;

            // ������������Լ���
            foreach (var property in _properties.OrderBy(p => p.PropertyKey))
            {
                yield return property.PropertyKey;
                yield return property.PropertyValue ?? string.Empty;
            }
        }
    }
}