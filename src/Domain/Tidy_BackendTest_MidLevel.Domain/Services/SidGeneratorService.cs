namespace Tidy_BackendTest_MidLevel.Domain.Services;

/// <summary>
/// 產生 20 字元唯一識別碼，邏輯與 SQL Server NEWSID 預存程序相同：
/// [2字元年份前綴(Base-36)][3字元年第幾天][5字元當天秒數][10字元隨機英數字]
/// </summary>
public class SidGeneratorService
{
    private const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private static readonly Random Rng = new();

    public string Generate()
    {
        var now = DateTime.Now;

        // 2-char year prefix (base-36, years since 2000)
        int yearsSince2000 = Math.Min(now.Year - 2000, 1295);
        char firstDigit = Alphabet[(yearsSince2000 / 36) % 36];
        char secondDigit = Alphabet[yearsSince2000 % 36];
        string yearPart = $"{firstDigit}{secondDigit}";

        // 3-char day-of-year (001–366)
        string dayPart = now.DayOfYear.ToString("D3");

        // 5-char seconds since midnight (00000–86399)
        int secondOfDay = now.Hour * 3600 + now.Minute * 60 + now.Second;
        string secondPart = secondOfDay.ToString("D5");

        // 10-char random alphanumeric
        string randomPart = GenerateRandomPart(10);

        return yearPart + dayPart + secondPart + randomPart;
    }

    public bool IsValidFormat(string? sid)
    {
        if (string.IsNullOrEmpty(sid)) return false;
        if (sid.Length != 20) return false;
        return sid.All(c => Alphabet.Contains(c));
    }

    private static string GenerateRandomPart(int length)
    {
        var chars = new char[length];
        for (int i = 0; i < length; i++)
            chars[i] = Alphabet[Rng.Next(Alphabet.Length)];
        return new string(chars);
    }
}
