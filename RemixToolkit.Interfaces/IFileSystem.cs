namespace RemixToolkit.Interfaces;

public interface IFileSystem
{
    void CopyFile(string sourceFile, string destFile);

    void CopyFolder(string sourceDir, string destDir);

    void CreateFolder(string dir);

    void DeleteFile(string file);

    void DeleteFolder(string dir);

    bool FileExists(string file);

    bool FolderExists(string dir);

    string ReadFile(string file);

    void WriteFile(string file, string content);
}
