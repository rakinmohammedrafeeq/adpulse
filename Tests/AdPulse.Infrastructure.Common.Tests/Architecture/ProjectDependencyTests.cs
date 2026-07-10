using FluentAssertions;
using System.Reflection;
using Xunit;

namespace AdPulse.Infrastructure.Common.Tests.Architecture;

/// <summary>
/// é¡¹ç›®ä¾èµ–å…³ç³»çš„éªŒè¯æµ‹è¯•
/// ç¡®ä¿åˆ†å±‚æž¶æž„çš„ä¾èµ–çº¦æŸ
/// </summary>
public class ProjectDependencyTests
{
    /// <summary>
    /// æµ‹è¯•Commoné¡¹ç›®ä¸åº”ä¾èµ–å…¶ä»–åŸºç¡€è®¾æ–½é¡¹ç›®
    /// </summary>
    [Fact]
    public void Common_Project_Should_Not_Depend_On_Other_Infrastructure_Projects()
    {
        // Arrange
        var commonAssembly = Assembly.GetAssembly(typeof(AdPulse.Infrastructure.Common.Abstractions.IComponent));

        // Act
        var referencedAssemblies = commonAssembly!.GetReferencedAssemblies();
        var infrastructureReferences = referencedAssemblies
            .Where(assembly => assembly.Name!.StartsWith("AdPulse.Infrastructure") &&
                              assembly.Name != "AdPulse.Infrastructure.Common")
            .ToList();

        // Assert
        infrastructureReferences.Should().BeEmpty(
            "Commoné¡¹ç›®ä¸åº”ä¾èµ–å…¶ä»–åŸºç¡€è®¾æ–½é¡¹ç›®ï¼Œä»¥ä¿æŒåˆ†å±‚æž¶æž„çš„å®Œæ•´æ€§");
    }

    /// <summary>
    /// æµ‹è¯•Commoné¡¹ç›®åº”è¯¥åªä¾èµ–Core.Sharedé¡¹ç›®
    /// </summary>
    [Fact]
    public void Common_Project_Should_Only_Depend_On_Core_Shared()
    {
        // Arrange
        var commonAssembly = Assembly.GetAssembly(typeof(AdPulse.Infrastructure.Common.Abstractions.IComponent));

        // Act
        var referencedAssemblies = commonAssembly!.GetReferencedAssemblies();
        var lornReferences = referencedAssemblies
            .Where(assembly => assembly.Name!.StartsWith("AdPulse"))
            .ToList();

        // Assert
        lornReferences.Should().HaveCount(1, "Commoné¡¹ç›®åº”è¯¥åªä¾èµ–Core.Sharedé¡¹ç›®");
        lornReferences.Should().Contain(assembly => assembly.Name == "AdPulse.Core.Shared",
            "Commoné¡¹ç›®åº”è¯¥ä¾èµ–Core.Sharedé¡¹ç›®");
    }

    /// <summary>
    /// æµ‹è¯•Commoné¡¹ç›®åº”è¯¥åªä¾èµ–å¿…è¦çš„Microsoftæ‰©å±•åŒ…
    /// </summary>
    [Fact]
    public void Common_Project_Should_Have_Minimal_Microsoft_Dependencies()
    {
        // Arrange
        var commonAssembly = Assembly.GetAssembly(typeof(AdPulse.Infrastructure.Common.Abstractions.IComponent));

        // Act
        var referencedAssemblies = commonAssembly!.GetReferencedAssemblies();
        var microsoftExtensionsReferences = referencedAssemblies
            .Where(assembly => assembly.Name!.StartsWith("Microsoft.Extensions"))
            .ToList();

        // Assert
        microsoftExtensionsReferences.Should().NotBeEmpty("Commoné¡¹ç›®éœ€è¦Microsoft.Extensionsä¾èµ–");
        microsoftExtensionsReferences.Should().Contain(assembly =>
            assembly.Name!.Contains("HealthChecks") ||
            assembly.Name!.Contains("Diagnostics"),
            "Commoné¡¹ç›®åº”è¯¥åŒ…å«å¥åº·æ£€æŸ¥ç›¸å…³çš„ä¾èµ–");
    }

    /// <summary>
    /// æµ‹è¯•Commoné¡¹ç›®çš„ç¨‹åºé›†å±žæ€§
    /// </summary>
    [Fact]
    public void Common_Project_Assembly_Should_Have_Correct_Properties()
    {
        // Arrange
        var commonAssembly = Assembly.GetAssembly(typeof(AdPulse.Infrastructure.Common.Abstractions.IComponent));

        // Act & Assert
        commonAssembly.Should().NotBeNull();
        commonAssembly!.GetName().Name.Should().Be("AdPulse.Infrastructure.Common");
        commonAssembly.GetName().Version.Should().NotBeNull();
    }

    /// <summary>
    /// æµ‹è¯•Commoné¡¹ç›®çš„å‘½åç©ºé—´ç»“æž„
    /// </summary>
    [Fact]
    public void Common_Project_Should_Have_Correct_Namespace_Structure()
    {
        // Arrange
        var commonAssembly = Assembly.GetAssembly(typeof(AdPulse.Infrastructure.Common.Abstractions.IComponent));

        // Act
        var types = commonAssembly!.GetTypes();
        var namespaces = types.Select(t => t.Namespace).Distinct().Where(ns => ns != null).ToList();

        // Assert
        namespaces.Should().Contain("AdPulse.Infrastructure.Common.Abstractions");
        namespaces.Should().Contain("AdPulse.Infrastructure.Common.Base");
        namespaces.Should().Contain("AdPulse.Infrastructure.Common.Models");
        namespaces.Should().Contain("AdPulse.Infrastructure.Common.Extensions");
        namespaces.Should().Contain("AdPulse.Infrastructure.Common.Conventions");

        // éªŒè¯æ‰€æœ‰å‘½åç©ºé—´éƒ½ä»¥æ­£ç¡®çš„å‰ç¼€å¼€å§‹
        namespaces.Should().AllSatisfy(ns =>
            ns!.Should().StartWith("AdPulse.Infrastructure.Common"));
    }

    /// <summary>
    /// æµ‹è¯•Commoné¡¹ç›®çš„å…¬å…±ç±»åž‹å¯è®¿é—®æ€§
    /// </summary>
    [Fact]
    public void Common_Project_Should_Expose_Required_Public_Types()
    {
        // Arrange
        var commonAssembly = Assembly.GetAssembly(typeof(AdPulse.Infrastructure.Common.Abstractions.IComponent));

        // Act
        var publicTypes = commonAssembly!.GetExportedTypes();
        var publicTypeNames = publicTypes.Select(t => t.Name).ToList();

        // Assert
        publicTypeNames.Should().Contain("IComponent");
        publicTypeNames.Should().Contain("IConfigurable");
        publicTypeNames.Should().Contain("IHealthCheckable");
        publicTypeNames.Should().Contain("ComponentBase");
        publicTypeNames.Should().Contain("ConfigurableComponentBase");
        publicTypeNames.Should().Contain("HealthCheckableComponentBase");
        publicTypeNames.Should().Contain("ComponentMetadata");
        publicTypeNames.Should().Contain("ComponentDescriptor");
        publicTypeNames.Should().Contain("InfrastructureOptions");
        publicTypeNames.Should().Contain("ServiceLifetime");
    }

    /// <summary>
    /// æµ‹è¯•Commoné¡¹ç›®çš„æ‰©å±•æ–¹æ³•ç±»å¯è®¿é—®æ€§
    /// </summary>
    [Fact]
    public void Common_Project_Should_Expose_Extension_Methods()
    {
        // Arrange
        var commonAssembly = Assembly.GetAssembly(typeof(AdPulse.Infrastructure.Common.Abstractions.IComponent));

        // Act
        var publicTypes = commonAssembly!.GetExportedTypes();
        var extensionClasses = publicTypes.Where(t => t.IsClass && t.IsSealed && t.IsAbstract).ToList();
        var extensionClassNames = extensionClasses.Select(t => t.Name).ToList();

        // Assert
        extensionClassNames.Should().Contain("TypeExtensions");
        extensionClassNames.Should().Contain("ReflectionExtensions");
        extensionClassNames.Should().Contain("ValidationExtensions");

        // éªŒè¯æ‰©å±•æ–¹æ³•ç±»åŒ…å«æ‰©å±•æ–¹æ³•
        foreach (var extensionClass in extensionClasses)
        {
            var methods = extensionClass.GetMethods(BindingFlags.Public | BindingFlags.Static);
            var extensionMethods = methods.Where(m => m.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute))).ToList();

            if (extensionClass.Name.EndsWith("Extensions"))
            {
                extensionMethods.Should().NotBeEmpty($"{extensionClass.Name} åº”è¯¥åŒ…å«æ‰©å±•æ–¹æ³•");
            }
        }
    }

    /// <summary>
    /// æµ‹è¯•Commoné¡¹ç›®çš„çº¦å®šç±»ç»“æž„
    /// </summary>
    [Fact]
    public void Common_Project_Should_Have_Convention_Classes()
    {
        // Arrange
        var commonAssembly = Assembly.GetAssembly(typeof(AdPulse.Infrastructure.Common.Abstractions.IComponent));

        // Act
        var publicTypes = commonAssembly!.GetExportedTypes();
        var conventionTypes = publicTypes.Where(t => t.Namespace?.Contains("Conventions") == true).ToList();
        var conventionTypeNames = conventionTypes.Select(t => t.Name).ToList();

        // Assert
        conventionTypeNames.Should().Contain("ComponentConventions");
        conventionTypeNames.Should().Contain("ConfigurationConventions");
        conventionTypeNames.Should().Contain("NamingConventions");
    }

    /// <summary>
    /// æµ‹è¯•Commoné¡¹ç›®ä¸åº”åŒ…å«å…·ä½“çš„ä¸šåŠ¡é€»è¾‘å®žçŽ°
    /// </summary>
    [Fact]
    public void Common_Project_Should_Not_Contain_Business_Logic_Implementations()
    {
        // Arrange
        var commonAssembly = Assembly.GetAssembly(typeof(AdPulse.Infrastructure.Common.Abstractions.IComponent));

        // Act
        var types = commonAssembly!.GetTypes();
        var businessLogicTypes = types.Where(t =>
            t.Name.Contains("Service") && !t.Name.Contains("ServiceLifetime") ||
            t.Name.Contains("Manager") ||
            t.Name.Contains("Provider") && !t.Name.Contains("Provider") ||
            t.Name.Contains("Strategy") ||
            t.Name.Contains("Engine") ||
            t.Name.Contains("Processor")).ToList();

        // Assert
        businessLogicTypes.Should().BeEmpty(
            "Commoné¡¹ç›®ä¸åº”åŒ…å«å…·ä½“çš„ä¸šåŠ¡é€»è¾‘å®žçŽ°ï¼Œåªåº”åŒ…å«åŸºç¡€æŠ½è±¡å’Œå·¥å…·ç±»");
    }

    /// <summary>
    /// æµ‹è¯•Commoné¡¹ç›®çš„æ€§èƒ½ç‰¹å¾
    /// </summary>
    [Fact]
    public void Common_Project_Assembly_Should_Load_Quickly()
    {
        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        for (int i = 0; i < 100; i++)
        {
            var assembly = Assembly.GetAssembly(typeof(AdPulse.Infrastructure.Common.Abstractions.IComponent));
            var types = assembly!.GetTypes();
            var publicTypes = assembly.GetExportedTypes();
        }

        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000,
            "Commoné¡¹ç›®ç¨‹åºé›†åº”è¯¥èƒ½å¤Ÿå¿«é€ŸåŠ è½½å’Œåå°„");
    }

    /// <summary>
    /// æµ‹è¯•Commoné¡¹ç›®çš„çº¿ç¨‹å®‰å…¨æ€§
    /// </summary>
    [Fact]
    public void Common_Project_Assembly_Should_Be_Thread_Safe()
    {
        // Act
        var tasks = Enumerable.Range(0, 10)
                             .Select(_ => Task.Run(() =>
                             {
                                 var assembly = Assembly.GetAssembly(typeof(AdPulse.Infrastructure.Common.Abstractions.IComponent));
                                 var types = assembly!.GetTypes();
                                 var publicTypes = assembly.GetExportedTypes();
                                 var referencedAssemblies = assembly.GetReferencedAssemblies();

                                 return new
                                 {
                                     TypeCount = types.Length,
                                     PublicTypeCount = publicTypes.Length,
                                     ReferencedAssemblyCount = referencedAssemblies.Length
                                 };
                             }))
                             .ToArray();

        var results = Task.WhenAll(tasks).Result;

        // Assert
        results.Should().AllSatisfy(result =>
        {
            result.TypeCount.Should().BeGreaterThan(0);
            result.PublicTypeCount.Should().BeGreaterThan(0);
            result.ReferencedAssemblyCount.Should().BeGreaterThan(0);
        });

        // éªŒè¯æ‰€æœ‰ç»“æžœä¸€è‡´
        var firstResult = results[0];
        results.Should().AllSatisfy(result =>
        {
            result.TypeCount.Should().Be(firstResult.TypeCount);
            result.PublicTypeCount.Should().Be(firstResult.PublicTypeCount);
            result.ReferencedAssemblyCount.Should().Be(firstResult.ReferencedAssemblyCount);
        });
    }

    /// <summary>
    /// æµ‹è¯•Commoné¡¹ç›®çš„ç‰ˆæœ¬å…¼å®¹æ€§
    /// </summary>
    [Fact]
    public void Common_Project_Should_Target_Correct_Framework_Version()
    {
        // Arrange
        var commonAssembly = Assembly.GetAssembly(typeof(AdPulse.Infrastructure.Common.Abstractions.IComponent));

        // Act
        var targetFrameworkAttribute = commonAssembly!.GetCustomAttribute<System.Runtime.Versioning.TargetFrameworkAttribute>();

        // Assert
        targetFrameworkAttribute.Should().NotBeNull("ç¨‹åºé›†åº”è¯¥æœ‰ç›®æ ‡æ¡†æž¶å±žæ€§");
        targetFrameworkAttribute!.FrameworkName.Should().StartWith(".NETCoreApp,Version=v9.0",
            "Commoné¡¹ç›®åº”è¯¥ç›®æ ‡.NET 9.0æ¡†æž¶");
    }
}