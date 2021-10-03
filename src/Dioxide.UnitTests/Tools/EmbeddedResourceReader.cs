using System;
using System.IO;
using System.Linq;

internal static class EmbeddedResourceReader
{
    /// <summary>
    /// Extract a string of text from an embedded resource from an assembly.
    /// </summary>
    /// <typeparam name="TAssembyType">The type that belongs to the assembly from which the resource is extracted.</typeparam>
    /// <param name="resourceName">The name of resource file.</param>
    /// <returns>The text from the resource.</returns>
    public static string ReadString<TAssembyType>(string resourceName)
    {
        using (Stream stream = ReadStream<TAssembyType>(resourceName))
        using (var reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }

    /// <summary>
    /// Extract stream to an embedded resource from the assembly.
    /// </summary>
    /// <typeparam name="TAssembyType">The type that belongs to the assembly from which the resource is extracted.</typeparam>
    /// <param name="resourceName">The name of resource file.</param>
    /// <returns>The stream of resource.</returns>
    public static Stream ReadStream<TAssembyType>(string resourceName)
    {
        var assembly = typeof(TAssembyType).Assembly;
        if (assembly != null && !string.IsNullOrWhiteSpace(resourceName))
        {
            var assemblyResources = assembly
                .GetManifestResourceNames()
                .Where(resource => resource.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (assemblyResources.Length > 0)
            {
                var resourcePath = assemblyResources.Length > 1
                    ? assemblyResources.OrderBy(resource => resource.Length).First()
                    : assemblyResources[0];

                return assembly.GetManifestResourceStream(resourcePath);
            }
        }

        throw new InvalidOperationException("Resource not found: " + resourceName);
    }
}