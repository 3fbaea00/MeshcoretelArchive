using System.Text.Json;

Directory.CreateDirectory(RootDirectory);

var lastArchiveMessageDatesFilePath = Path.Combine(RootDirectory, "_");
if (!File.Exists(lastArchiveMessageDatesFilePath))
{
    using var file = File.Create(lastArchiveMessageDatesFilePath);
    file.Write("{}"u8);
}

var lastArchiveMessageDates = JsonSerializer.Deserialize<Dictionary<string, DateTime>>(File.ReadAllText(lastArchiveMessageDatesFilePath))!;

while (true)
{
    var catalog = await Client.GetCatalog();
    var regions = new List<(CatalogRegionJson region, string path)>();
    foreach (var country in catalog.Countries)
    {
        var countryDirectory = Path.Combine(RootDirectory, country.Country.NameEn);
        Directory.CreateDirectory(countryDirectory);

        var members = country.Meshes;
        foreach (var region in country.Regions)
        {
            var directory = countryDirectory;

            var mesh = members.FirstOrDefault(member => member.MemberRegionCodes.Any(code => code == region.Code));
            if (mesh is not null)
            {
                directory = Path.Combine(directory, mesh.Name);
                Directory.CreateDirectory(directory);
            }

            var regionDirectory = Path.Combine(directory, region.Name);
            Directory.CreateDirectory(regionDirectory);
            regions.Add((region, regionDirectory));
        }
    }

    foreach (var (region, regionPath) in regions)
    {
        var channelOffset = 0;
        while (true)
        {
            var channelRespond = await Client.GetChannels(region.Code, offset: channelOffset);
            channelOffset += channelRespond.Returned;

            foreach (var channel in channelRespond.Items)
            {
                if (channel.LastMessageAt is not { } newLastMessageTime)
                    continue;

                var internalId = region.Name + "-" + channel.Name;
                if (!lastArchiveMessageDates.TryGetValue(internalId, out DateTime lastMessageTime))
                    lastMessageTime = DateTime.MinValue;

                if (newLastMessageTime <= lastMessageTime)
                    continue;

                using var channelArchive = new ChannelArchive(regionPath, region, channel);
                var newMessages = new List<MessageItemJson>();

                var messages = await Client.GetLastMessages(region.Code, channel.Id);
                while (true)
                {
                    if (messages.Messages.Length == 0)
                        break;

                    var firstMessage = messages.Messages.First();
                    if (firstMessage.When <= lastMessageTime)
                        break;

                    foreach (var message in messages.Messages)
                    {
                        if (message.When is not { } messageDate)
                            continue;

                        if (messageDate <= lastMessageTime)
                            continue;

                        newMessages.Add(message);
                    }

                    var lastMessage = messages.Messages.Last();
                    messages = await Client.GetMessagesBefore(region.Code, channel.Id, lastMessage.Id);
                }

                newMessages.Reverse();
                foreach (var message in newMessages)
                    channelArchive.AddMessage(message);

                lastArchiveMessageDates[internalId] = newLastMessageTime;
                File.WriteAllText(lastArchiveMessageDatesFilePath, JsonSerializer.Serialize(lastArchiveMessageDates)!);
            }

            if (channelOffset >= channelRespond.Total)
                break;
        }
    }

    Thread.Sleep(600000);
}

Console.ReadLine();