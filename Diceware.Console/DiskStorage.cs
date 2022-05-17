using Diceware.Library;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Diceware.Console;

public class DiskStorage : IStorage
{
    private readonly string storageFilename;

    public DiskStorage(string localFilename)
    {
        storageFilename = Path.Combine(AppContext.BaseDirectory, localFilename);
    }

    public bool IsContentAvailable => File.Exists(storageFilename);

    public Task<string> LoadContent()
    {
        return File.ReadAllTextAsync(storageFilename);
    }

    public Task SaveContent(string content)
    {
        return File.WriteAllTextAsync(storageFilename, content);
    }
}
