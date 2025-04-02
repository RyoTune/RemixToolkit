using RemixToolkit.Interfaces;

namespace RemixToolkit.Reloaded.FileSystem;

internal class FileSystemService : IFileSystem
{
    public static readonly FileSystemService Instance = new();

    public void CreateFolder(string dir) => Directory.CreateDirectory(dir);

    public void WriteFile(string file, string content)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(file)!);
        File.WriteAllText(file, content);
    }

    public string ReadFile(string file) => File.ReadAllText(file);

    public void CopyFile(string sourceFile, string destFile)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(destFile)!);
        File.Copy(sourceFile, destFile, true);
    }

    public void CopyFolder(string sourceDir, string destDir)
    {
        foreach (var file in Directory.EnumerateFiles(sourceDir, "*.*", SearchOption.AllDirectories))
        {
            var relPath = Path.GetRelativePath(sourceDir, file);
            var destFile = Path.Join(destDir, relPath);
            CopyFile(file, destFile);
        }
    }

    public bool FileExists(string file) => File.Exists(file);

    public bool FolderExists(string dir) => Directory.Exists(dir);

    public void DeleteFile(string file)
    {
        if (File.Exists(file)) File.Delete(file);
    }

    public void DeleteFolder(string dir)
    {
        if (Directory.Exists(dir)) Directory.Delete(dir, true);
    }
}
