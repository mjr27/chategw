using System.Reflection;
namespace Egw.PubManagement.LegacyImport.BibleCodes;

internal static class AssemblyExtensions
{
    /// <summary>
    /// Uses the assembly name + '.' + suffix to determine whether any resources begin with the concatenation.
    /// If not, the assembly name will be truncated at the '.' beginning from the right side of the string
    /// until a base name is found.
    /// </summary>
    /// <param name="assembly">This <see cref="T:System.Reflection.Assembly" />.</param>
    /// <param name="suffix">A suffix to use on the assembly name to limit the possible resource names to match.
    /// This value can be <c>null</c> to match any resource name in the assembly.</param>
    /// <returns>A base name if found, otherwise <see cref="F:System.String.Empty" />.</returns>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="assembly" /> is <c>null</c>.</exception>
    public static string GetManifestResourceBaseName(this Assembly? assembly, string suffix)
    {
        string[] source = !(assembly == null)
            ? assembly.GetManifestResourceNames()
            : throw new ArgumentNullException(nameof(assembly));
        string str = assembly.GetName().Name!;
        string baseName = string.IsNullOrEmpty(suffix) ? str : str + "." + suffix;
        while (!source.Any(resName => resName.StartsWith(baseName, StringComparison.Ordinal)))
        {
            int length = str.LastIndexOf('.');
            switch (length)
            {
                case > -1 when length < str.Length - 1:
                    str = str[..length];
                    baseName = string.IsNullOrEmpty(suffix) ? str : str + "." + suffix;
                    break;
                case <= -1:
                    return string.Empty;
            }
        }

        return baseName;
    }
}