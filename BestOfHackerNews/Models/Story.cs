using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BestOfHackerNews.Models;

public class Story
{
    public string Title { get; set; }
    [JsonProperty(PropertyName = "url")]
    public string Uri { get; set; }
    [JsonProperty(PropertyName = "by")]
    public string PostedBy { get; set; }
    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime Time { get; set; }
    public int Score { get; set; }
    [JsonProperty(PropertyName = "descendants")]
    public int CommentCount { get; set; }
}
