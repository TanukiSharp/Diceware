using Diceware.Library;

namespace Diceware.Web;

public class WordListService
{
    private readonly Task<Dictionary<int, string>> words;

    public WordListService(HttpClient httpClient)
    {
        var wordList = new WordList(httpClient, "./diceware.wordlist.asc", new NullStorage());
        words = wordList.GetWordList(false);
    }

    public Task<Dictionary<int, string>> GetWordList()
    {
        return words;
    }
}
