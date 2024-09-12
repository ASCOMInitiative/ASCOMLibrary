namespace ASCOM.Alpaca.Clients
{
    // Dummy class to hold comments that appear at the namespace level in the compiled Help file

    /// <summary>
    /// Alpaca Client toolkit providing a standard interface to Alpaca devices. This namespace is delivered in NuGet package: <b>ASCOM.Alpaca.Components</b>.
    /// </summary>
    /// <remarks>
    /// <para>Please note that you will need to add this PropertyGroup to .NET projects that target Android in order for Alpaca discovery to work as expected:</para>
    /// <code title="Add to Project File" language="xml">
    /// &lt;PropertyGroup Condition = "$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android'"&gt;
    ///    &lt;UseNativeHttpHandler&gt;false&lt;/UseNativeHttpHandler&gt;
    /// &lt;/PropertyGroup&gt;
    /// </code>
    /// <para>This may also be the case for projects that target IOS, but has not yet been confirmed</para>
    /// </remarks>
    [System.Runtime.CompilerServices.CompilerGenerated()]
    class NamespaceDoc
    {
    }
}
