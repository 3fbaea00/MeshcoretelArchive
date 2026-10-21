using System.Text.Json.Serialization;

public sealed class CatalogRootJson
{
    [JsonPropertyName("world")]
    public CatalogRegionJson World { get; set; }

    [JsonPropertyName("meshes")]
    public CatalogRegionJson[] Meshes { get; set; }

    [JsonPropertyName("countries")]
    public CatalogCountryItemJson[] Countries { get; set; }

    [JsonPropertyName("selectable")]
    public CatalogRegionJson[] Selectable { get; set; }
}

public sealed class CatalogRegionJson
{
    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("timezone")]
    public string Timezone { get; set; }

    [JsonPropertyName("kind")]
    public string Kind { get; set; }

    [JsonPropertyName("country_code")]
    public string CountryCode { get; set; }

    [JsonPropertyName("member_region_codes")]
    public string[] MemberRegionCodes { get; set; }
}

public sealed class CatalogCountryItemJson
{
    [JsonPropertyName("kind")]
    public string Kind { get; set; }

    [JsonPropertyName("country")]
    public CatalogCountryInfoJson Country { get; set; }

    [JsonPropertyName("selection")]
    public CatalogRegionJson Selection { get; set; }

    [JsonPropertyName("meshes")]
    public CatalogRegionJson[] Meshes { get; set; }

    [JsonPropertyName("regions")]
    public CatalogRegionJson[] Regions { get; set; }
}

public sealed class CatalogCountryInfoJson
{
    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("name_en")]
    public string NameEn { get; set; }

    [JsonPropertyName("name_ru")]
    public string NameRu { get; set; }
}