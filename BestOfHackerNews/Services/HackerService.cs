using BestOfHackerNews.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace BestOfHackerNews.Services;

public class HackerService(HttpClient httpClient, IMemoryCache _cache)
{
    private const string BestStoriesUrl = "https://hacker-news.firebaseio.com/v0/beststories.json";
    private const string StoryDetailUrl = "https://hacker-news.firebaseio.com/v0/item/{0}.json";

    private async Task<List<int>?> GetBestStoriesAsync()
    {
        if (_cache.TryGetValue("bestStoryIds", out List<int> storyIds)) return storyIds!;
        var response = await httpClient.GetStringAsync(BestStoriesUrl);
        if (response.Length < 1)
            throw new Exception("No best stories found");
        storyIds = JsonConvert.DeserializeObject<List<int>>(response)!;
        _cache.Set("bestStoryIds", storyIds, TimeSpan.FromSeconds(10));
        return storyIds;
    }

    private async Task<Story> GetStoryDetailsAsync(int storyId)
    {
        var url = string.Format(StoryDetailUrl, storyId);
        var response = await httpClient.GetStringAsync(url);
        if (response.Length < 1)
            throw new Exception($"Story {storyId} details not found");
        return JsonConvert.DeserializeObject<Story>(response)!;
    }

    public async Task<List<Story>> GetTopStoriesAsync(int n)
    {
        var storyIds = await GetBestStoriesAsync();
        var tasks = new List<Task<Story>>();

        // Fetch the details of the first 'n' stories
        for (int i = 0; i < n && i < storyIds.Count; i++)
        {
            tasks.Add(GetStoryDetailsAsync(storyIds[i]));
        }

        var stories = await Task.WhenAll(tasks);

        var sortedStories = new List<Story>(stories);
        sortedStories.Sort((x, y) => y.Score.CompareTo(x.Score));

        return sortedStories;
    }
}