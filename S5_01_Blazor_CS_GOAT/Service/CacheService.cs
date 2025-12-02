using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using S5_01_Blazor_CS_GOAT.Models;

namespace S5_01_Blazor_CS_GOAT.Service;

using Microsoft.JSInterop;
using System.Net.Http;

public class CacheService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly HttpClient _httpClient;
    private const string CacheName = "model-cache";

    public CacheService(IJSRuntime jsRuntime, HttpClient httpClient)
    {
        _jsRuntime = jsRuntime;
        _httpClient = httpClient;
    }

    // Cache a file from wwwroot
    public async Task<bool> CacheLocalFile(string localPath, string cacheKey)
    {
        try
        {
            var response = await _httpClient.GetAsync(localPath);
            if (!response.IsSuccessStatusCode)
                return false;

            var bytes = await response.Content.ReadAsByteArrayAsync();
            
            return await _jsRuntime.InvokeAsync<bool>(
                "cacheHelper.putInCache",
                CacheName,
                cacheKey,
                bytes
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error caching local file: {ex.Message}");
            return false;
        }
    }

    // NEW: Cache GLTF with modified URIs
    public async Task<bool> CacheGltfWithBlobUrls(string localPath, string cacheKey, string binBlobUrl, string textureBlobUrl)
    {
        try
        {
            var response = await _httpClient.GetAsync(localPath);
            if (!response.IsSuccessStatusCode)
                return false;

            var gltfContent = await response.Content.ReadAsStringAsync();
            
            // Parse and modify the GLTF
            var gltfJson = JsonDocument.Parse(gltfContent);
            var modifiedGltf = ModifyGltfUris(gltfJson, binBlobUrl, textureBlobUrl);
            
            var bytes = Encoding.UTF8.GetBytes(modifiedGltf);
            
            return await _jsRuntime.InvokeAsync<bool>(
                "cacheHelper.putInCache",
                CacheName,
                cacheKey,
                bytes
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error caching GLTF: {ex.Message}");
            return false;
        }
    }

    private string ModifyGltfUris(JsonDocument gltfJson, string binBlobUrl, string textureBlobUrl)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = false }))
        {
            writer.WriteStartObject();
            
            foreach (var property in gltfJson.RootElement.EnumerateObject())
            {
                if (property.Name == "buffers")
                {
                    writer.WritePropertyName("buffers");
                    writer.WriteStartArray();
                    foreach (var buffer in property.Value.EnumerateArray())
                    {
                        writer.WriteStartObject();
                        foreach (var bufferProp in buffer.EnumerateObject())
                        {
                            if (bufferProp.Name == "uri")
                            {
                                writer.WriteString("uri", binBlobUrl);
                            }
                            else
                            {
                                bufferProp.WriteTo(writer);
                            }
                        }
                        writer.WriteEndObject();
                    }
                    writer.WriteEndArray();
                }
                else if (property.Name == "images")
                {
                    writer.WritePropertyName("images");
                    writer.WriteStartArray();
                    foreach (var image in property.Value.EnumerateArray())
                    {
                        writer.WriteStartObject();
                        foreach (var imageProp in image.EnumerateObject())
                        {
                            if (imageProp.Name == "uri")
                            {
                                writer.WriteString("uri", textureBlobUrl);
                            }
                            else
                            {
                                imageProp.WriteTo(writer);
                            }
                        }
                        writer.WriteEndObject();
                    }
                    writer.WriteEndArray();
                }
                else
                {
                    property.WriteTo(writer);
                }
            }
            
            writer.WriteEndObject();
        }
        
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    // Fetch image from API and cache it
    public async Task<bool> FetchAndCacheImage(string apiUrl, string cacheKey)
    {
        try
        {
            var response = await _httpClient.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
                return false;

            ThreeDModel threeDModel = await response.Content.ReadFromJsonAsync<ThreeDModel>();
            
            
                byte[] bytes = threeDModel.Texture[0];
                return await _jsRuntime.InvokeAsync<bool>(
                    "cacheHelper.putInCache",
                    CacheName,
                    cacheKey,
                    bytes
                );
            


        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching and caching image: {ex.Message}");
            return false;
        }
    }
    
    // NEW: Cache GLTF with multiple texture blob URLs
public async Task<bool> CacheGltfWithBlobUrls(string localPath, string cacheKey, string binBlobUrl, Dictionary<string, string> textureUrls)
{
    try
    {
        var response = await _httpClient.GetAsync(localPath);
        if (!response.IsSuccessStatusCode)
            return false;

        var gltfContent = await response.Content.ReadAsStringAsync();
        
        // Parse and modify the GLTF
        var gltfJson = JsonDocument.Parse(gltfContent);
        var modifiedGltf = ModifyGltfUris(gltfJson, binBlobUrl, textureUrls);
        
        var bytes = Encoding.UTF8.GetBytes(modifiedGltf);
        
        return await _jsRuntime.InvokeAsync<bool>(
            "cacheHelper.putInCache",
            CacheName,
            cacheKey,
            bytes
        );
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error caching GLTF: {ex.Message}");
        return false;
    }
}

private string ModifyGltfUris(JsonDocument gltfJson, string binBlobUrl, Dictionary<string, string> textureUrls)
{
    using var stream = new MemoryStream();
    using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = false }))
    {
        writer.WriteStartObject();
        
        foreach (var property in gltfJson.RootElement.EnumerateObject())
        {
            if (property.Name == "buffers")
            {
                writer.WritePropertyName("buffers");
                writer.WriteStartArray();
                foreach (var buffer in property.Value.EnumerateArray())
                {
                    writer.WriteStartObject();
                    foreach (var bufferProp in buffer.EnumerateObject())
                    {
                        if (bufferProp.Name == "uri")
                        {
                            writer.WriteString("uri", binBlobUrl);
                        }
                        else
                        {
                            bufferProp.WriteTo(writer);
                        }
                    }
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
            }
            else if (property.Name == "images")
            {
                writer.WritePropertyName("images");
                writer.WriteStartArray();
                foreach (var image in property.Value.EnumerateArray())
                {
                    writer.WriteStartObject();
                    string? originalUri = null;
                    
                    // First pass: find the original URI
                    foreach (var imageProp in image.EnumerateObject())
                    {
                        if (imageProp.Name == "uri")
                        {
                            originalUri = imageProp.Value.GetString();
                            break;
                        }
                    }
                    
                    // Second pass: write all properties with modified URI
                    foreach (var imageProp in image.EnumerateObject())
                    {
                        if (imageProp.Name == "uri" && originalUri != null && textureUrls.ContainsKey(originalUri))
                        {
                            writer.WriteString("uri", textureUrls[originalUri]);
                        }
                        else
                        {
                            imageProp.WriteTo(writer);
                        }
                    }
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
            }
            else
            {
                property.WriteTo(writer);
            }
        }
        
        writer.WriteEndObject();
    }
    
    return Encoding.UTF8.GetString(stream.ToArray());
}

    // Get a cached URL (blob URL) for use in your 3D viewer
    public async Task<string?> GetCachedUrl(string cacheKey)
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string>(
                "cacheHelper.getCacheUrl",
                CacheName,
                cacheKey
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting cached URL: {ex.Message}");
            return null;
        }
    }
}