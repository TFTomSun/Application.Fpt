using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Thomas.Apis.Core;

/// <summary>
/// Provides extension methods for the <see cref="Assembly"/> class.
/// </summary>
// ReSharper disable once UnusedMember.Global
public static class AssemblyExtensions
{
    [DllImport("USER32.DLL")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);
    public static void BringToFront(this Process process)
    {
        SetForegroundWindow(process.MainWindowHandle);
    }
    /// <summary>
    /// Creates a human readable string in the format: yyyy-MM-dd HH:mm:ss
    /// </summary>
    /// <param name="dateTime">The date time on which the extension is invoked.</param>
    /// <returns></returns>
    public static string ToReadableString(this DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }

    /// <summary>
    /// Creates a file name compatible string in the format: yyyy_MM_dd HH.mm.ss_fff
    /// </summary>
    /// <param name="dateTime">The date time on which the extension is invoked.</param>
    public static string ToFileTimeString(this DateTime dateTime)
    {
        return dateTime.ToString("yyyy_MM_dd HH.mm.ss_fff");
    }

    /// <summary>
    /// Tries to parse the give string into a date time. Returns null, if it is not a valid date time string.
    /// </summary>
    /// <param name="value">The date time string.</param>
    public static DateTime? ToDateTime(this string value)
    {
        if (DateTime.TryParse(value, out var dateTime))
            return dateTime;
        return null;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static FileVersionInfo FileVersionInfo(this Assembly assembly)
    {
        return assembly.Location.IsNullOrEmpty() ? null : System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
    }   

    /// <summary>
    /// Get assembly build date time.
    /// </summary>
    /// <param name="fileInfo">File info to get build time.</param>
    /// <returns>Date time of assembly build.</returns>
    public static DateTime AssemblyUtcBuildTime(this FileInfo fileInfo)
    {
        var filePath = fileInfo.FullName;
        const int peHeaderOffset = 60;
        const int linkerTimestampOffset = 8;
        var b = new byte[2048];

        using (var assemblyFileStream = System.IO.File.OpenRead(filePath))
        {
            assemblyFileStream.Read(b, 0, 2048);
        }

        var i = BitConverter.ToInt32(b, peHeaderOffset);
        var secondsSince1970 = BitConverter.ToInt32(b, i + linkerTimestampOffset);
        var dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        dt = dt.AddSeconds(secondsSince1970);
        return dt;
    }

    /// <summary>
    /// Get build time of an assembly.
    /// </summary>
    /// <param name="assembly">Assembly to check the build time.</param>
    /// <returns>Build date time of assembly.</returns>
    public static DateTime? GetUtcBuildTime(this Assembly assembly)
    {
        var copyright = assembly.FileVersionInfo()?.LegalCopyright;

        var timeStamp = copyright?.Contains("([") == true
            ? new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)+copyright.SubstringBetween("([", "])").TryToDouble()?.Seconds()
            : assembly.File()?.AssemblyUtcBuildTime();

        return timeStamp;
    }

    /// <summary>
    /// Gets the custom attributes of the specified attribute type.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the attributes to get.</typeparam>
    /// <param name="assembly">The assembly on which the extension in invoked.</param>
    /// <param name="inherit">Determines whether the attribute declarations in base implementations should be considered.</param>
    /// <returns>
    /// The custom attributes.
    /// </returns>
    public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this Assembly assembly, bool inherit = false)
        where TAttribute : Attribute
    {
        return assembly.GetCustomAttributes(typeof(TAttribute), inherit).OfType<TAttribute>();
    }

    /// <summary>
    /// Gets the file info for the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly on which the extension is invoked.</param>
    /// <returns>The file info.</returns>
    public static FileInfo File(this Assembly assembly)
    {
        return assembly.Location.IsNullOrEmpty()? null : new FileInfo(assembly.Location);
    }

    /// <summary>
    /// Get directory of assembly
    /// </summary>
    /// <param name="assembly">The assembly on which the extension is invoked. </param>
    /// <returns>The directory info</returns>
    public static DirectoryInfo Directory(this Assembly assembly)
    {
        return assembly.File()?.Directory;
    }

 
    /// <summary>
    /// Writes all resources of the assembly to the output. The directories will be created according to the resource namespaces.
    /// </summary>
    /// <param name="assembly">The assembly on which the extension method is invoked.</param>
    /// <param name="outputDirectory">The directory where the resource files should be written to.</param>
    /// <param name="fileFilter">A filter that selects the resource files that should be written to the file system.</param>
    /// <param name="resourceNameFilter">A filter that selects the resources that should be written to the file system by their name.</param>
    public static IEnumerable<FileInfo> WriteResourcesToFile(this Assembly assembly, DirectoryInfo outputDirectory, Func<FileInfo, bool> fileFilter = null, Func<string, bool> resourceNameFilter = null)
    {
        var assemblyName = assembly.GetName().Name;

        var files = new List<FileInfo>();

        foreach (var resourceName in assembly.GetManifestResourceNames())
        {
            if (resourceNameFilter == null || resourceNameFilter(resourceName))
            {
                var filePath =
                    outputDirectory.Combine(resourceName.Replace(assemblyName + '.', String.Empty)
                        .Replace('.', Path.DirectorySeparatorChar));
                var lastSeparatorIndex = filePath.LastIndexOf(Path.DirectorySeparatorChar);
                filePath = filePath.ReplaceAt(lastSeparatorIndex, '.');
                var file = new FileInfo(filePath);
                if (fileFilter == null || fileFilter(file))
                {
                    files.Add(file);
                    file.Directory.EnsureExists();

                    using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
                    {
                        using (var fileStream = new FileStream(file.FullName, FileMode.Create))
                        {
                            resourceStream.CopyTo(fileStream);
                        }
                    }
                }
            }
        }
        return files;
    }


    /// <summary>
    /// Writes a embedded resource file to the file system.
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="targetDirectory"></param>
    /// <param name="resourceFileName"></param>
    public static FileInfo WriteResourceToFile(this Assembly assembly, DirectoryInfo targetDirectory, string resourceFileName)
    {
        return assembly.WriteResourceToFile(targetDirectory.CombineFile(resourceFileName));
    }

    /// <summary>
    /// Writes a embedded resource file to the file system.
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="targetFile"></param>
    /// <param name="resourceFileName"></param>
    public static FileInfo WriteResourceToFile(this Assembly assembly, FileInfo targetFile, string resourceFileName = null)
    {
        targetFile.Directory.EnsureExists();
        var fileName = resourceFileName ?? targetFile.Name;
        using (var fileStream = new FileStream(targetFile.FullName,FileMode.Create))
        {
            using (var resourceStream = assembly.GetResourceFileStream(fileName))
            {
                resourceStream.CopyTo(fileStream);
            }
        }

        targetFile.Refresh();
        return targetFile;
    }

    /// <summary>
    /// Tries to get file stream of a embedded resource file in the given assembly
    /// </summary>
    /// <param name="assembly">The assembly on which the extension is invoked.</param>
    /// <param name="resourceOrFileName">Resources name or the name of the file search in assembly</param>
    /// <param name="resourceFileStream"></param>
    /// <returns></returns>
    public static bool TryGetResourceFileStream(this Assembly assembly, String resourceOrFileName, out Stream resourceFileStream)
    {
        var resourceName = assembly.GetManifestResourceNames().SingleOrDefault(
          rn => rn == resourceOrFileName || rn.CompareByWildcard("*."+ resourceOrFileName), "The file name {0} is not unique.", resourceOrFileName);
        if (resourceName == null)
        {
            resourceFileStream = null;
            return false;
        }
        resourceFileStream = assembly.GetManifestResourceStream(resourceName);
        return true;
    }

    /// <summary>
    /// Get file stream of a file in assembly
    /// </summary>
    /// <param name="assembly">The assembly on which the extension is invoked.</param>
    /// <param name="resourceOrFileName">Resources name or the name of the file search in assembly</param>
    /// <returns>The Stream of the file</returns>
    public static Stream GetResourceFileStream(this Assembly assembly, String resourceOrFileName)
    {
        Stream resourceFileStream;
        if (!assembly.TryGetResourceFileStream(resourceOrFileName, out resourceFileStream))
        {
            throw Api.Create.Exception("The file name {0} could not be found.", resourceOrFileName);
        }

        return resourceFileStream;
    }

    /// <summary>
    /// Get content of file stream
    /// </summary>
    /// <param name="assembly">The assembly on which the extension is invoked.</param>
    /// <param name="resourceOrFileName">Resources name or the name of the file search in assembly</param>
    /// <returns>Content of file as string</returns>
    public static String GetResourceFileContent(this Assembly assembly, String resourceOrFileName)
    {
        using (var stream = assembly.GetResourceFileStream(resourceOrFileName))
        {
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }

    /// <summary>
    /// Gets the bytes of a resource file.
    /// </summary>
    /// <param name="assembly">The assembly on which the extension is invoked.</param>
    /// <param name="resourceOrFileName">Resources name or the name of the file search in assembly</param>
    /// <returns>The file bytes.</returns>
    public static byte[] GetResourceFileBytes(this Assembly assembly, String resourceOrFileName)
    {
        using (var stream = assembly.GetResourceFileStream(resourceOrFileName))
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);

                return memoryStream.ToArray();
            }
        }
    }
}
