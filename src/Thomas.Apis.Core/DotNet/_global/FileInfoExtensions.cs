using System;
using System.Linq
    ;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Thomas.Apis.Core;

/// <summary>
/// Provides extension methods for the <see cref="FileInfo"/> class.
/// </summary>
public static class FileInfoExtensions
{
    static readonly char[] invalidFileNameChars = Path.GetInvalidFileNameChars();

    /// <summary>
    /// Assembly name of file.
    /// </summary>
    /// <param name="file">File to get assembly name.</param>
    /// <returns>Assembly name of file.</returns>
    public static AssemblyName AssemblyName(this FileInfo file)
    {
        try
        {
            return System.Reflection.AssemblyName.GetAssemblyName(file.FullName);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Returns the file version of the given file.
    /// </summary>
    /// <param name="file">The file on which the extension is invoked.</param>
    public static FileVersionInfo FileVersion(this FileInfo file)
    {
        return FileVersionInfo.GetVersionInfo(file.FullName);
    }

    /// <summary>
    /// Gets all parent directories as a deferred loaded sequence.
    /// </summary>
    /// <param name="file">The file on which the extension is invoked.</param>
    public static IEnumerable<DirectoryInfo> Parents(this FileInfo file)
    {
        var directory = file.Directory;
        return directory.Parents(true);
    }

    /// <summary>
    /// Reads the file content as a string in an async way.
    /// </summary>
    /// <param name="file">The file on which the extension is invoked.</param>
    public static async Task<string> ReadTextAsync(this FileInfo file)
    {
        using (FileStream sourceStream = new FileStream(file.FullName,
            FileMode.Open, FileAccess.Read, FileShare.Read,
            bufferSize: 4096, useAsync: true))
        {
            StringBuilder sb = new StringBuilder();

            byte[] buffer = new byte[0x1000];
            int numRead;
            while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                string text = Encoding.Unicode.GetString(buffer, 0, numRead);
                sb.Append(text);
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// Writes the file content in an async way.
    /// </summary>
    /// <param name="file">The file on which the extension is invoked.</param>
    /// <param name="text">The text to write.</param>
    /// <returns></returns>
    public static async Task WriteTextAsync(this FileInfo file, string text)
    {
        byte[] encodedText = Encoding.Unicode.GetBytes(text);

        using (FileStream sourceStream = new FileStream(file.FullName,
            FileMode.Append, FileAccess.Write, FileShare.None,
            bufferSize: 4096, useAsync: true))
        {
            await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
        };
    }

    /// <summary>
    /// Gets all files of the directory and its subdirectories (recursive).
    /// </summary>
    /// <param name="directoryInfo">The directory on which the extension is invoked.</param>
    public static FileInfo[] GetAllFiles(this DirectoryInfo directoryInfo)
    {
        return directoryInfo.GetFiles(SearchOption.AllDirectories, "*");
    }

    /// <summary>
    /// Gets the directory of the give file system info artifact. If its a directory it returns itself, otherwise it returns the directory of the file.
    /// </summary>
    /// <param name="fileSystemInfo">The file system artifact on which the extension is invoked.</param>
    public static DirectoryInfo Directory(this FileSystemInfo fileSystemInfo)
    {
        return fileSystemInfo as DirectoryInfo ?? ((FileInfo) fileSystemInfo).Directory;
    }

    public static void DeleteRecursive(this FileSystemInfo info)
    {
        if (info is DirectoryInfo dir)
        {
            dir.Delete(true);
        }
        else info.Delete();
    }

    /// <summary>
    /// Creates a file info with invalid characters replaced to make it system compatible.
    /// </summary>
    /// <param name="fileSystemInfo">The file system artifact on which the extension is invoked.</param>
    /// <returns></returns>
    public static T MakeFileSystemCompatible<T>(this T fileSystemInfo)
    where T:FileSystemInfo
    {
        var path = fileSystemInfo.FullName;
        var validPath = path.Split(Path.DirectorySeparatorChar)
            .Select((part,i) => part.Select(ch => i !=0 && invalidFileNameChars.Contains(ch) ? '_' : ch).ToStringValue())
            .Aggregate(Path.DirectorySeparatorChar.ToString());

        if (fileSystemInfo is FileInfo)
        {
            return (T) (object)new FileInfo(validPath);
        } 
        return (T) (object)new DirectoryInfo(validPath);
    }
    /// <summary>
    /// Creates a new file info with the same path and name but different extension.
    /// </summary>
    /// <param name="file">The file on which the extension is invoked.</param>
    /// <param name="extension">The new extension (e.g. ".txt").</param>
    /// <returns>The new file info with the specified extension.</returns>
    public static FileInfo WithExtension(this FileInfo file, string extension)
    {
        return file.Directory.CombineFile(file.NameWithoutExtension() + extension);
    }

    /// <summary>
    /// Gets the file name without extension.
    /// </summary>
    /// <param name="file">The file info instance on which this extension method is invoked.</param>
    /// <returns>The file name without extension.</returns>
    public static String NameWithoutExtension(this FileInfo file)
    {
        return Path.GetFileNameWithoutExtension(file.FullName);
    }



    /// <summary>
    /// Copies a file into the target directory
    /// </summary>
    /// <param name="file">The file info instance on which this extension is invoked.</param>
    /// <param name="targetDirectory">The directory to copy to.</param>
    /// <param name="overwrite">Overwrite flag.</param>
    public static FileInfo CopyTo(this FileInfo file, DirectoryInfo targetDirectory, bool overwrite = false)
    {
        return file.CopyTo(targetDirectory.Combine(file.Name), overwrite);
    }

    /// <summary>
    /// Changes the file attributes to normal.
    /// </summary>
    /// <param name="file">The file info instance on which this extension is invoked.</param>
    public static void MakeWritable(this FileInfo file)
    {
        file.Attributes = FileAttributes.Normal;
    }

    ///// <summary>
    ///// Creates an empty file, as well as the entire directory structure above this file.
    ///// </summary>
    ///// <param name="file">The file info on which this extension is invoked.</param>
    //public static void CreateEmptyFile(this FileInfo file)
    //{
    //    if (!File.Exists(file.FullName))
    //    {
    //        string directoryPath = Path.GetDirectoryName(file.FullName);

    //        if (directoryPath != null)
    //        {
    //            if (!System.IO.Directory.Exists(directoryPath))
    //            {
    //                Directory.CreateDirectory(directoryPath);
    //            }
    //        }
    //        using (file.Create())
    //        {

    //        }
    //    }
    //}

    /// <summary>
    /// Renames/Moves a file.
    /// </summary>
    /// <param name="file">The file info instance on which this extension is invoked. </param>
    /// <param name="newName">The new name of the file.</param>
    /// <param name="overwrite">Determines whether an existing target file should be overwritten. If false, the .NET exception will be thrown.</param>
    public static FileInfo Rename(this FileInfo file, string newName, bool overwrite = false)
    {
        if (file.DirectoryName == null)
        {
            throw Api.Create.Exception("The directory name was null.");
        }

        var newFile = new FileInfo(Path.Combine(file.DirectoryName, newName));
        if (overwrite)
        {
            newFile.DeleteIfExits();
        }

        file.MoveTo(newFile.FullName);
        newFile.Refresh();
        return newFile;
    }

    /// <summary>
    /// Edits a file and performs the operation on its content.
    /// </summary>
    /// <param name="file">The file info instance on which this method is invoked.</param>
    /// <param name="editOperation">The edit operation to be performed on the file content.</param>
    public static void Edit(this FileInfo file, Func<String, String> editOperation)
    {
        String fileContent;
        using (var reader = new StreamReader(file.FullName))
        {
            fileContent = reader.ReadToEnd();
        }
        var newFileContent = editOperation(fileContent);
        using (var writer = new StreamWriter(file.FullName))
        {
            writer.Write(newFileContent);
        }
    }
    /// <summary>
    /// Edits a file and performs the operation on its content.
    /// </summary>
    /// <param name="file">The file info instance on which this method is invoked.</param>
    /// <param name="editOperation">The edit operation to be performed on the file content.</param>
    public static void EditLines(this FileInfo file, Func<String[], IEnumerable<String>> editOperation)
    {
        var newFileContent = editOperation(File.ReadAllLines(file.FullName));
        File.WriteAllLines(file.FullName, newFileContent);
    }


    /// <summary>
    /// Replaces the content of the file.
    /// </summary>
    /// <param name="file">The File info on which this extension is invoked.</param>
    /// <param name="replacements">The map used for the replacements.</param>
    public static void ReplaceContent(this FileInfo file, params (String Original, String Replacement)[] replacements)
    {
        var fileContent = file.ReadContent();
        replacements.ForEach(e => fileContent = fileContent.Replace(e.Original, e.Replacement));
        file.WriteContent(fileContent);
    
    }




    /// <summary>
    /// Determines whether the given file is locked / in use.
    /// </summary>
    /// <param name="file">The file info on which the extension is invoked.</param>
    /// <returns>true if the file is in use, otherwise false.</returns>
    public static bool IsInUse(this FileInfo file)
    {
        if (!file.Exists)
        {
            return false;
        }
        FileStream stream = null;

        try
        {
            stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        }
        catch (IOException)
        {
            //the file is unavailable because it is:
            //still being written to
            //or being processed by another thread
            return true;
        }
        finally
        {
            if (stream != null)
                stream.Close();
        }

        //file is not locked
        return false;

    }

    /// <summary>
    /// Writes the content of the file as a string.
    /// </summary>
    /// <param name="file">The file on which the extension is invoked.</param>
    /// <param name="content"></param>
    /// <returns>The content as a string.</returns>
    public static FileInfo WriteContent(this FileInfo file, string content)
    {
         file.Directory.EnsureExists();
         File.WriteAllText(file.FullName,content);
        return file;
    }

    /// <summary>
    /// Gets the content of the file as a string.
    /// </summary>
    /// <param name="file">The file on which the extension is invoked.</param>
    /// <returns>The content as a string.</returns>
    public static String ReadContent(this FileInfo file)
    {
        return File.ReadAllText(file.FullName);
    }

    /// <summary>
    /// Reads the lines (with deferred execution) of the specified file.
    /// </summary>
    /// <param name="file">The file on which the extension is invoked.</param>
    /// <param name="encoding">The encoding that is used for reading. Default: UTF8</param>
    /// <param name="openReadOnly"></param>
    /// <returns>The content as a string.</returns>
    public static IEnumerable<String> ReadLines(this FileInfo file, Encoding encoding = null, bool openReadOnly = false)
    {
        encoding = encoding ?? Encoding.UTF8;
        if (openReadOnly)
        {
            using (var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(fileStream, encoding))
            {
                while (!sr.EndOfStream)
                {
                    yield return sr.ReadLine();
                }
            }
        }
        else
        {
            foreach (var line in File.ReadLines(file.FullName, encoding))
            {
                yield return line;
            }
        }
    }

    /// <summary>
    /// Loads the assembly 
    /// </summary>
    /// <param name="assemblyFile">The file on which the extension is invoked. Must be an CLR assembly.</param>
    /// <param name="avoidFileLock">Determines how the file is loaded. Please make sure that you can use a different load context when setting this parameter to true.
    ///  See http://msdn.microsoft.com/en-us/library/dd153782.aspx for more information.</param>
    /// <returns>The loaded assembly.</returns>
    public static Assembly LoadAssembly(this FileInfo assemblyFile, bool avoidFileLock = false)
    {
        byte[] assemblyBytes = File.ReadAllBytes(assemblyFile.FullName);
        Assembly assembly = Assembly.Load(assemblyBytes);
        return assembly;
    }


    /// <summary>
    /// Ensure exist of a file
    /// </summary>
    /// <param name="file">File witch should exist</param>
    /// <returns>Return true if must create</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static bool EnsureExists(this FileInfo file)
    {
        if (file == null) throw new ArgumentNullException("file");
        file.Refresh();
        var createFile = !file.Exists;
        if (createFile)
        {
            file.Directory.EnsureExists();
            using (file.Create())
            {
            }
            file.Refresh();
        }

        return createFile;
    }

    /// <summary>
    /// Delete a file if it exists
    /// </summary>
    /// <param name="file">File to delete.</param>
    /// <returns>Return true if file was deleted.</returns>
    public static bool DeleteIfExits(this FileInfo file)
    {
        file.Refresh();
        if (file.Exists)
        {
            file.Delete();
            file.Refresh();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Create file and parent folder
    /// </summary>
    /// <param name="file">File to create</param>
    public static void EnsureCreate(this FileInfo file)
    {
        file.Directory.EnsureExists();
        file.Create();
    }
}

