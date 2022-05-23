using Diceware.Library;

namespace Diceware.Web;

public class EntropyService
{
    private readonly WordListService wordListService;

    private EntropyCalculator? entropyCalculator;

    public EntropyService(WordListService wordListService)
    {
        this.wordListService = wordListService;
    }

    public async ValueTask<double> Compute(string passphrase, bool extraSecurity)
    {
        if (entropyCalculator == null)
        {
            Dictionary<int, string> words = await wordListService.GetWordList();
            entropyCalculator = new EntropyCalculator(words);
        }

        return entropyCalculator.Compute(passphrase, extraSecurity);
    }
}
