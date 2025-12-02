namespace S5_01_Blazor_CS_GOAT.Service;

public class ImageService
{
      private readonly HttpClient _httpClient;

        public ImageService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<byte[]> DownloadImageAsync(string imageUrl)
        {
            try
            {
                return await _httpClient.GetByteArrayAsync(imageUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading image: {ex.Message}");
                return null;
            }
        }

        public async Task SaveImageToFoldersAsync(string imageUrl, string newFileName)
        {
            // Download the image
            var imageBytes = await DownloadImageAsync(imageUrl);
            
            if (imageBytes == null || imageBytes.Length == 0)
            {
                Console.WriteLine("Failed to download image");
                return;
            }

            // Ensure the filename has .png extension
            if (!newFileName.EndsWith(".png"))
            {
                newFileName += ".png";
            }

            // Save to folder 1
            var folder1Path = Path.Combine("wwwroot", "images", "folder1", newFileName);
            await SaveImageAsync(folder1Path, imageBytes);

            // Save to folder 2
            var folder2Path = Path.Combine("wwwroot", "images", "folder2", newFileName);
            await SaveImageAsync(folder2Path, imageBytes);

            Console.WriteLine($"Image saved successfully as {newFileName}");
        }

        private async Task SaveImageAsync(string filePath, byte[] imageBytes)
        {
            try
            {
                // Ensure directory exists
                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Write the file
                await File.WriteAllBytesAsync(filePath, imageBytes);
                Console.WriteLine($"Saved to: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving image: {ex.Message}");
            }
        }
    }
