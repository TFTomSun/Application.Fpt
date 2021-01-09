using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Thomas.Apis.Core;

/// <summary>
/// Provides extensions for DirectoryInfo
/// </summary>
public static class DirectoryInfoExtensions
{

    public static FileInfo GetFile(this DirectoryInfo directory, string filePattern, SearchOption searchOption  = SearchOption.TopDirectoryOnly, bool throwException = true)
    {
        var file = directory.GetFiles(filePattern, searchOption).SingleOrDefault(
            $"The file pattern '{filePattern}' is not unique within the directory '{directory.FullName}");

        if (throwException && file == null)
        {
            throw Api.Create.Exception(
                $"A file with the pattern '{filePattern}' could not be found in the directory '{directory.FullName}'.");
        }
        return file;
    }

    ///// <summary>
    ///// Get directories by wildcard
    ///// </summary>
    ///// <param name="directory">Root directory.</param>
    ///// <param name="wildCardString">Directory wildcard pattern string.</param>
    ///// <returns>Array of directories which matched the pattern.</returns>
    //public static DirectoryInfo[] GetDirectories(this DirectoryInfo directory, string wildCardString = null)
    //{
    //    if (wildCardString == null || wildCardString == "*")
    //    {
    //        return directory.GetDirectories();
    //    }
    //    return directory.GetDirectories().Where(d => d.Name.CompareByWildcard(wildCardString)).ToArray();
    //}
    
    
  
    /// <summary>
    /// Get all directories and subdirectories
    /// </summary>
    /// <param name="directory">Root directory</param>
    /// <param name="maxDepth">Determines the maximum depth of the recursion. Default is full recursion, if '0' is specified, only the root sequence will be returned.</param>
    /// <returns>IEnumerable of all directories and subdirectories</returns>
    public static IEnumerable<DirectoryInfo> GetAllDirectories(this DirectoryInfo directory, int? maxDepth = null)
    {
        return Directory.EnumerateDirectories(directory.FullName).Flatten(
            Directory.EnumerateDirectories, false, maxDepth).Select(x => new DirectoryInfo(x));
    }


    /// <summary>
    /// Determines whether the directory is empty.
    /// </summary>
    /// <param name="directory">The directory on which the extension is invoked.</param>
    /// <returns>true, if the directory is empty, otherwise false.</returns>
    public static bool IsEmpty(this DirectoryInfo directory)
    {
        return !Directory.EnumerateFileSystemEntries(directory.FullName).Any();
    }

    public static string MakeAbsolutePath(this DirectoryInfo baseDirectory, string relativeOrAbsoluteFilePath)
    {
        if (Path.IsPathRooted(relativeOrAbsoluteFilePath))
        {
            return relativeOrAbsoluteFilePath;
        }

        var absolutePath = Path.Combine(baseDirectory.FullName, relativeOrAbsoluteFilePath);
        var result = Path.GetFullPath(absolutePath);
        return result;
    }

    /// <summary>
    /// Get relative path of file or directory.
    /// </summary>
    /// <param name="directory">Relative parent directory.</param>
    /// <param name="fileOrDirectory">File or directory to get relative path from.</param>
    /// <returns>Relative path as string.</returns>
    public static string MakeRelativePath(this DirectoryInfo directory, FileSystemInfo fileOrDirectory)
    {
        var targetUri = new Uri(fileOrDirectory.FullName.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar);
        var baseUri = new Uri(directory.FullName.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar);

        var relativeUri = baseUri.MakeRelativeUri(targetUri);
        var relativePath = Uri.UnescapeDataString(relativeUri.ToString()).Replace('/', Path.DirectorySeparatorChar);
        return relativePath.TrimEnd(Path.DirectorySeparatorChar);

    }

    public static IEnumerable<TFileSystemInfo> FindFileSystemInfosAbove<TFileSystemInfo>(this DirectoryInfo directory,
        Func<DirectoryInfo, TFileSystemInfo[]> getInfos, bool includeSelf, string searchPattern)
        where TFileSystemInfo : FileSystemInfo
    {
        if (directory == null) throw new ArgumentNullException(nameof(directory));
        if (getInfos == null) throw new ArgumentNullException(nameof(getInfos));


        IEnumerable<TFileSystemInfo> GetInfosSafe(DirectoryInfo x)
        {
            try
            {
                var result = getInfos(x);
                return result;
            }
            catch (UnauthorizedAccessException)
            {
                return Enumerable.Empty<TFileSystemInfo>();
            }
            catch (Exception ex)
            {
                throw Api.Create.Exception(
                    ex,
                    $"Failed to find first file above with pattern '{searchPattern}' in directory '{directory.FullName}'.");
            }
        }

        foreach (var fileOrDirectory in directory.Parents(includeSelf).Where(
            d => d.Exists).SelectMany(GetInfosSafe))
        {
            yield return fileOrDirectory;
        }
    }


    /// <summary>
    /// Tries to find the file with the given filename in the given directory or any above directory.
    /// 
    /// Cancels if no access rights.
    /// </summary>
    /// <param name="directory">The directory info on which the method is invoked.</param>
    /// <param name="directorySearchPattern">The name of the file to search.</param>
    /// <param name="includeSelf">Determines whether the directory on which the extension is invoked should be checked for the file, too.</param>
    /// <returns>The file info of the searched file or null if it has not been found.</returns>
    /// <exception cref="ArgumentNullException">When the directory is null.</exception>
    /// <exception cref="ArgumentException">When the given filename is invalid (null or whitespace).</exception>
    public static IEnumerable<DirectoryInfo> FindDirectoriesAbove(this DirectoryInfo directory, string directorySearchPattern, bool includeSelf)
    {
        if (directory == null) throw new ArgumentNullException(nameof(directory));
        if (String.IsNullOrWhiteSpace(directorySearchPattern))
            throw new ArgumentException("Argument is null or whitespace", nameof(directorySearchPattern));

        return directory.FindFileSystemInfosAbove(x => x.GetDirectories(directorySearchPattern), includeSelf,
            directorySearchPattern);
    }

    /// <summary>
    /// Tries to find the file with the given filename in the given directory or any above directory.
    /// 
    /// Cancels if no access rights.
    /// </summary>
    /// <param name="directory">The directory info on which the method is invoked.</param>
    /// <param name="fileSearchPattern">The name of the file to search.</param>
    /// <param name="includeSelf">Determines whether the directory on which the extension is invoked should be checked for the file, too.</param>
    /// <returns>The file info of the searched file or null if it has not been found.</returns>
    /// <exception cref="ArgumentNullException">When the directory is null.</exception>
    /// <exception cref="ArgumentException">When the given filename is invalid (null or whitespace).</exception>
    public static FileInfo FindFirstFileAbove(this DirectoryInfo directory, string fileSearchPattern, bool includeSelf)
    {
        if (directory == null) throw new ArgumentNullException(nameof(directory));
        if (String.IsNullOrWhiteSpace(fileSearchPattern))
            throw new ArgumentException("Argument is null or whitespace", nameof(fileSearchPattern));

        return directory.FindFileSystemInfosAbove(x => x.GetFiles(fileSearchPattern), includeSelf,
            fileSearchPattern).FirstOrDefault();
    }


    /// <summary>
    /// Get parents of directory.
    /// </summary>
    /// <param name="directory">Directory to get parents from.</param>
    /// <param name="includeSelf"></param>
    /// <returns>Enumerable of directories.</returns>
    public static IEnumerable<DirectoryInfo> Parents(this DirectoryInfo directory, bool includeSelf = false)
    {
        if (directory == null) throw new ArgumentNullException(nameof(directory));
        if (includeSelf)
        {
            yield return directory;
        }
        var current = directory.Parent;
        while (current != null)
        {
            yield return current;
            current = current.Parent;
        }
    }

    /// <summary>
    /// Deletes a directory, including subdirectories and files, regardless of their attributes.
    /// </summary>
    /// <param name="directoryInfo">The directory info on which the method is invoked.</param>
    /// <param name="recursive">Determines whether subdirectories should be deleted, too.</param>
    /// <param name="deleteReadOnly">Deletes read only files, too.</param>
    public static void Delete(this DirectoryInfo directoryInfo, bool recursive, bool deleteReadOnly)
    {
        if (directoryInfo.Exists)
        {

            if (deleteReadOnly)
            {
                var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                directoryInfo.GetFileSystemInfos(
                    "*", searchOption).ForEach(fi => fi.Attributes = FileAttributes.Normal);
            }

            directoryInfo.Delete(recursive);
            directoryInfo.Refresh();
        }
    }

    /// <summary>
    /// Deletes a directory if it exists
    /// </summary>
    /// <param name="directory">directory to delete.</param>
    /// <param name="recursive">Determines whether sub folders should be deleted too.</param>
    /// <param name="deleteReadOnly"></param>
    /// <returns>Return true if file was deleted.</returns>
    public static DirectoryInfo DeleteIfExits(this DirectoryInfo directory, bool recursive = true, bool deleteReadOnly = true)
    {
        directory.Refresh();
        if (directory.Exists)
        {
            directory.Delete(recursive, deleteReadOnly);
            directory.Refresh();
            return directory;
        }
        return directory;
    }

    /// <summary>
    /// Get latest written file that match the specified file pattern.
    /// </summary>
    /// <param name="directory">Search directory.</param>
    /// <param name="filePatterns">File pattern.</param>
    /// <returns>Latest written file.</returns>
    public static FileInfo GetLatestWrittenFile(this DirectoryInfo directory, params string[] filePatterns)
    {
        if (directory != null && directory.Exists)
        {

            var objectNamesFile = directory.GetFiles(SearchOption.AllDirectories).Where(
                fi => filePatterns.Any(fp => fi.Name.CompareByWildcard(fp))).OrderByDescending(
                    f => f.LastWriteTime).FirstOrDefault();
            return objectNamesFile;
        }
        return null;
    }
    /// <summary>
    /// Gets the size of all the files in the directory.
    /// </summary>
    /// <param name="directory">The directory info instance on which this extension is invoked.</param>
    /// <returns>The size of the directory.</returns>
    public static long GetSize(this DirectoryInfo directory)
    {
        if (directory.Exists)
        {
            return directory.GetFiles("*.*", SearchOption.AllDirectories).Sum(f => f.Length);
        }
        return 0;
    }

   

  

    /// <summary>
    /// Renames a directory and moves all its content
    /// </summary>
    /// <param name="directoryInfo">The directory info instance on which this method is invoked.</param>
    /// <param name="name">The new name of the directory info</param>
    /// <returns>The renamed directory info.</returns>
    public static DirectoryInfo RenameTo(this DirectoryInfo directoryInfo, string name)
    {
        if (directoryInfo == null)
        {
            throw new ArgumentNullException("directoryInfo", "Directory info to rename cannot be null");
        }

        if (String.IsNullOrEmpty(name))
        {
            throw new ArgumentException("New name cannot be null or blank", "name");
        }
        Directory.Move(directoryInfo.FullName, directoryInfo.Parent.CombineDirectory(name).FullName);
        return directoryInfo.Parent.CombineDirectory(name);
    }
    /// <summary>
    /// Gets all the files contained in the directory that satisfy the search patterns.
    /// </summary>
    /// <param name="directory">The directory info instance on which this extension is invoked.</param>
    /// <param name="searchOption">The search options</param>
    /// <param name="searchPatterns">The search patterns.</param>
    /// <returns>Array containing the files that satisfy the criteria. </returns>
    public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo directory, SearchOption searchOption,
                                      params string[] searchPatterns)
    {
        if (searchPatterns.Length > 1)
        {
            var files = directory.EnumerateFiles("*", searchOption).Where(
                f =>  searchPatterns.Any(p => f.Name.CompareByWildcard(p)));
            return files;
        }

        return directory.EnumerateFiles(searchPatterns.FirstOrDefault() ?? "*", searchOption);
    }
    /// <summary>
    /// Gets all the files contained in the directory that satisfy the search patterns.
    /// </summary>
    /// <param name="directory">The directory info instance on which this extension is invoked.</param>
    /// <param name="searchOption">The search options</param>
    /// <param name="searchPatterns">The search patterns.</param>
    /// <returns>Array containing the files that satisfy the criteria. </returns>
    public static FileInfo[] GetFiles(this DirectoryInfo directory, SearchOption searchOption,
                                      params string[] searchPatterns)
    {
        if (searchPatterns.Length > 1)
        {
            var files = directory.GetFiles("*", searchOption).Where(
                f =>  searchPatterns.Any(p => f.Name.CompareByWildcard(p))).ToArray();
            return files;
        }

        return directory.GetFiles(searchPatterns.FirstOrDefault() ?? "*", searchOption);
    }

    /// <summary>
    /// Ensure exist of the directory
    /// </summary>
    /// <param name="directory">Directory to ensure exist</param>
    /// <param name="wasCreated">Whether the directory was newly created.</param>
    /// <returns>Return true if directory had to create</returns>
    public static DirectoryInfo EnsureExists(this DirectoryInfo directory, out bool wasCreated)
    {
        if (directory == null) throw new ArgumentNullException(nameof(directory));
        wasCreated = !directory.Exists;
        if (wasCreated)
        {
            directory.Create();
            directory.Refresh();
        }

        return directory;
    }
    /// <summary>
    /// Ensures that the directory exists and returns the directory info.
    /// </summary>
    /// <param name="directory">The directory whose existence should be ensured.</param>
    /// <returns>The given directory.</returns>
    public static DirectoryInfo Clean(this DirectoryInfo directory)
    {
        directory.DeleteIfExits();
        return directory.EnsureExists();
    }

    /// <summary>
    /// Ensures that the directory exists and returns the directory info.
    /// </summary>
    /// <param name="directory">The directory whose existence should be ensured.</param>
    /// <returns>The given directory.</returns>
    public static DirectoryInfo EnsureExists(this DirectoryInfo directory)
    {
        bool unused;
        directory.EnsureExists(out unused);
        return directory;
    }

    /// <summary>
    /// Ensures that directory is newly created and returns it.
    /// </summary>
    /// <param name="directory">The directory whose creation should be ensured.</param>
    /// <returns>The directory which has been (newly) created.</returns>
    public static DirectoryInfo Recreate(this DirectoryInfo directory)
    {
        if (directory == null) throw new ArgumentNullException(nameof(directory));
        if (directory.Exists)
        {
            directory.Delete(true);
            directory.Refresh();
        }

        return directory.EnsureExists();
    }

    /// <summary>
    /// Combine a Directory path
    /// </summary>
    /// <param name="directory">Directory to create path with sub dirs</param>
    /// <param name="subFolders">SubDirectories to combine</param>
    /// <returns>Return last sub directory</returns>
    public static DirectoryInfo CombineDirectory(this DirectoryInfo directory, params string[] subFolders)
    {
        return new DirectoryInfo(directory.Combine(subFolders));
    }

    /// <summary>
    /// Combine file path
    /// </summary>
    /// <param name="directory">Directory to start</param>
    /// <param name="subFoldersAndFile">Sub Directories and file to select</param>
    /// <returns>Return combined file</returns>
    public static FileInfo CombineFile(this DirectoryInfo directory, params string[] subFoldersAndFile)
    {
        return new FileInfo(directory.Combine(subFoldersAndFile));
    }

    /// <summary>
    /// Combine any file or folder path
    /// </summary>
    /// <param name="directory">Directory to start</param>
    /// <param name="subElements">Sub elements to combine</param>
    /// <returns>Return path of elements and directory</returns>
    public static string Combine(this DirectoryInfo directory, params string[] subElements)
    {
        return subElements.Aggregate(directory.FullName, Path.Combine);
    }
}

