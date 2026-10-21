using System.Text.Json.Serialization;

public sealed class MessagesPageJson
{
    [JsonPropertyName("messages")]
    public MessageItemJson[] Messages { get; set; }

    [JsonPropertyName("next_before")]
    public long? NextBefore { get; set; }

    [JsonPropertyName("next_after")]
    public long? NextAfter { get; set; }
}

public sealed class MessageItemJson
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("hash")]
    public string Hash { get; set; }

    [JsonPropertyName("when")]
    public DateTime? When { get; set; }

    [JsonPropertyName("sender")]
    public string Sender { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("channel_id")]
    public int ChannelId { get; set; }

    [JsonPropertyName("channel_hash")]
    public int ChannelHash { get; set; }

    [JsonPropertyName("region_scope")]
    public string? RegionScope { get; set; }

    [JsonPropertyName("observer_id")]
    public string ObserverId { get; set; }

    [JsonPropertyName("packet_id")]
    public long? PacketId { get; set; }
}