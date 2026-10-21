using System.Data;
using System.Globalization;
using System.Text;

public class ChannelArchive : IDisposable
{
    public ChannelArchive(string directoryPath, CatalogRegionJson region, ChannelItemJson channel)
    {
        this.region = region;
        this.channel = channel;

        var channelFilePath = Path.Combine(directoryPath, channel.Name);
        if (!File.Exists(channelFilePath))
            File.Create(channelFilePath).Dispose();

        channelStream = new ChannelStream(channelFilePath);
    }

    CatalogRegionJson region;
    ChannelItemJson channel;
    ChannelStream channelStream;

    static Dictionary<string, TimeSpan> timezoneOffsets = new()
        {
            { "Europe/Moscow", TimeSpan.FromHours(3) },
            { "Europe/Minsk", TimeSpan.FromHours(3) },
            { "Europe/Madrid", TimeSpan.FromHours(1) },
            { "Europe/Kiev", TimeSpan.FromHours(2) },
            { "Europe/Samara", TimeSpan.FromHours(4) },
            { "Europe/Saratov", TimeSpan.FromHours(4) },
            { "Asia/Oral", TimeSpan.FromHours(5) },
            { "Asia/Qyzylorda", TimeSpan.FromHours(5) },
            { "Asia/Yekaterinburg", TimeSpan.FromHours(5) },
            { "Asia/Omsk", TimeSpan.FromHours(6) },
            { "Asia/Krasnoyarsk", TimeSpan.FromHours(7) },
            { "Asia/Irkutsk", TimeSpan.FromHours(8) },
            { "Asia/Yakutsk", TimeSpan.FromHours(9) },
            { "Asia/Vladivostok", TimeSpan.FromHours(10) }
        };

    static List<string> timezonesWithSummerTime = ["Europe/Madrid", "\"Europe/Kiev\""];

    public void AddMessage(MessageItemJson message)
    {
        const int MetadataLength = 27;
        var time = message.When!.Value;
        if (timezoneOffsets.TryGetValue(region.Timezone, out var timezoneOffset))
        {
            time += timezoneOffset;
            if (timezonesWithSummerTime.Contains(region.Timezone))
                time += TimeSpan.FromHours(1);
        }
        else Console.WriteLine($"Unknown timezone offset for {region.Name}");

        var messageLines = message.Message.Split('\n').Select(line => line.Trim(' ').Trim('\t')).Where(line => !string.IsNullOrWhiteSpace(line));
        var messageBody = string.Join($"\n{new string(' ', MetadataLength)}", messageLines);
        var senderName = ReplaceEmojis(message.Sender, "☺");

        if (senderName.Length > 12)
            senderName = senderName.Substring(0, 12);

        channelStream.WriteLine($"{time.Day:D2}{FormatToMonth(time.Month)}{time.Year % 100:D2} {time.Hour:D2}:{time.Minute:D2} {senderName}".PadRight(MetadataLength) + messageBody);
    }

    string FormatToMonth(int month) =>
        month switch
        {
            1 => "jan",
            2 => "feb",
            3 => "mar",
            4 => "apr",
            5 => "may",
            6 => "jun",
            7 => "jul",
            8 => "aug",
            9 => "sep",
            10 => "oct",
            11 => "nov",
            12 => "dec",
        };


    static string ReplaceEmojis(string input, string replacement)
    {
        var sb = new StringBuilder();
        var enumerator = StringInfo.GetTextElementEnumerator(input);
        while (enumerator.MoveNext())
        {
            var element = enumerator.GetTextElement();
            if (Rune.TryGetRuneAt(element, 0, out Rune firstRune) && Rune.GetUnicodeCategory(firstRune) is UnicodeCategory.OtherSymbol or UnicodeCategory.ModifierSymbol)
                sb.Append(replacement);
            else sb.Append(element);
        }

        return sb.ToString();
    }

    public void Dispose()
    {
        channelStream.Dispose();
    }
}