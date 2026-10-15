using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;

namespace Tools;

public class SteamTools
{
    private readonly HttpClient _httpClient;

    public SteamTools(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }



    [Description(
         "Searches the Steam store for games by name. " +
         "Use this when the user asks to find or open a Steam game. " +
         "Returns matching game names and their Steam App IDs."
     )]
    public async Task<string> SearchSteamGames(
         [Description(
            "The name or part of the name of the Steam game. " +
            "Examples: CS2, Counter Strike, GTA, Minecraft."
        )] string gameName)
    {
        if (string.IsNullOrWhiteSpace(gameName))
            return "No game name was provided.";

        try
        {
            var url =
                $"https://store.steampowered.com/api/storesearch/" +
                $"?term={Uri.EscapeDataString(gameName)}" +
                $"&cc=se&l=english";

            var response = await _httpClient.GetFromJsonAsync<JsonElement>(url);

            if (!response.TryGetProperty("items", out var items))
                return $"No Steam games found for '{gameName}'.";

            var results = new List<string>();

            foreach (var item in items.EnumerateArray())
            {
                if (!item.TryGetProperty("id", out var id))
                    continue;

                if (!item.TryGetProperty("name", out var name))
                    continue;

                results.Add($"Name: {name.GetString()}\n" + $"App ID: {id.GetInt32()}"
                );

                if (results.Count >= 10)
                    break;
            }

            if (results.Count == 0)
                return $"No Steam games found for '{gameName}'.";

            return string.Join("\n\n", results);
        }
        catch (HttpRequestException ex)
        {
            return $"Could not connect to Steam: {ex.Message}";
        }
        catch (JsonException ex)
        {
            return $"Could not read Steam search results: {ex.Message}";
        }
    }

    [Description(
        "Launches a Steam game using its Steam App ID. " +
        "Use this after SearchSteamGames has returned the correct App ID."
    )]
    public string OpenSteamGame(
        [Description(
            "The Steam App ID of the game to launch. " +
            "Example: 730 for Counter-Strike 2."
        )] string appId)
    {
        if (string.IsNullOrWhiteSpace(appId))
            return "No Steam App ID was provided.";

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = $"steam://rungameid/{appId}",
                UseShellExecute = true
            });

            return $"Steam launch command sent for App ID {appId}.";
        }
        catch (Exception ex)
        {
            return $"Could not launch Steam game: {ex.Message}";
        }
    }
}