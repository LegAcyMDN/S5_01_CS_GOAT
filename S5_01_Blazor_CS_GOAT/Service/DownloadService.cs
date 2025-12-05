using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace S5_01_Blazor_CS_GOAT.Service
{
    public class DownloadService
    {
        private readonly HttpClient _http;

        public DownloadService(HttpClient http)
        {
            _http = http;
        }

        // Téléchargement avec progression %
        public async Task<byte[]> DownloadWithProgressAsync(string url, IProgress<int>? progress = null)
        {
            using var response = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength ?? -1L;
            var totalRead = 0L;
            var buffer = new byte[8192];

            using var stream = await response.Content.ReadAsStreamAsync();
            using var ms = new MemoryStream();

            while (true)
            {
                var read = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length));
                if (read == 0) break;

                await ms.WriteAsync(buffer.AsMemory(0, read));
                totalRead += read;

                if (totalBytes != -1)
                    progress?.Report((int)(totalRead * 100 / totalBytes));
            }

            return ms.ToArray();
        }
    }
}
