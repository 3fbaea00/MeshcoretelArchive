
using System.Text.Json.Serialization;

public sealed class ChannelsPageJson
{
    [JsonPropertyName("items")]
    public ChannelItemJson[] Items { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    [JsonPropertyName("returned")]
    public int Returned { get; set; }

    [JsonPropertyName("next_offset")]
    public int? NextOffset { get; set; }
}

public sealed class ChannelItemJson
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("channel_hash")]
    public int ChannelHash { get; set; }

    [JsonPropertyName("last_message_at")]
    public DateTime? LastMessageAt { get; set; }

    [JsonPropertyName("last_message_preview")]
    public string LastMessagePreview { get; set; }

    [JsonPropertyName("last_message_id")]
    public long? LastMessageId { get; set; }

    [JsonPropertyName("last_sender")]
    public string LastSender { get; set; }

    [JsonPropertyName("last_before_today_id")]
    public long? LastBeforeTodayId { get; set; }
}