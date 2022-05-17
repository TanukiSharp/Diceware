using System;
using System.Threading.Tasks;

namespace Diceware.Library;

public interface IStorage
{
    bool IsContentAvailable { get; }
    Task<string> LoadContent();
    Task SaveContent(string content);
}

public class NullStorage : IStorage
{
    public bool IsContentAvailable => false;

    public Task<string> LoadContent()
    {
        throw new NotImplementedException();
    }

    public Task SaveContent(string content)
    {
        return Task.CompletedTask;
    }
}
