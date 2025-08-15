using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace AIDataGen.Models.Loaders
{
    public class FilesLoader
    {
        private readonly HttpClient _httpClient;

        public FilesLoader(string authToken = null)
        {
            _httpClient = new HttpClient();
            if (authToken != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            }
        }

        public async Task DownloadFileAsync(
            string url,
            string filePath,
            IProgress<DetailedProgress> progress = null,
            CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength ?? -1L;
            var totalBytesRead = 0L;
            var buffer = new byte[16384];
            var stopwatch = Stopwatch.StartNew();

            using var contentStream = await response.Content.ReadAsStreamAsync();
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 16384, useAsync: true);

            int bytesRead;
            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                totalBytesRead += bytesRead;

                var elapsedTime = stopwatch.Elapsed;
                var speedBytesPerSecond = elapsedTime.TotalSeconds > 0 ?
                    totalBytesRead / elapsedTime.TotalSeconds : 0;

                progress?.Report(new DetailedProgress(
                    totalBytesRead,
                    totalBytes,
                    speedBytesPerSecond,
                    elapsedTime));
            }
        }    
    }

    public class DetailedProgress
    {
        public long BytesDownloaded { get; }
        public long TotalBytes { get; }
        public double SpeedBytesPerSecond { get; }
        public TimeSpan ElapsedTime { get; }
        public double? ProgressPercentage => TotalBytes > 0 ? (double)BytesDownloaded / TotalBytes * 100 : null;
        public string SpeedFormatted => SpeedBytesPerSecond switch
        {
            >= 1_073_741_824 => $"{SpeedBytesPerSecond / 1_073_741_824:F1} GB/s",
            >= 1_048_576 => $"{SpeedBytesPerSecond / 1_048_576:F1} MB/s",
            >= 1024 => $"{SpeedBytesPerSecond / 1024:F1} KB/s",
            _ => $"{SpeedBytesPerSecond:F0} B/s"
        };

        public DetailedProgress(long bytesDownloaded, long totalBytes, double speedBytesPerSecond, TimeSpan elapsedTime)
        {
            BytesDownloaded = bytesDownloaded;
            TotalBytes = totalBytes;
            SpeedBytesPerSecond = speedBytesPerSecond;
            ElapsedTime = elapsedTime;
        }
    }

    public class ConsoleProgressBar
    {
        private double? _progress;

        private int _progressBarWidth;

        private int _currentBarLength;

        private int _cursorTop;

        private string _message;

        private int _sectionsCount;

        private int _currentSection;

        private readonly Lock _lock = new();

        private bool _firstDraw = true;

        public ConsoleProgressBar(string message = "Loading...", int sectionsCount = 1, int progressBarWidth = 50)
        {
            _progressBarWidth = 50;
            _message = message ?? string.Empty;
            _sectionsCount = sectionsCount;
        }

        public IProgress<DetailedProgress> GetProgress()
        {
            _currentSection++;
            _progress = 0;
            return new Progress<DetailedProgress>(ProgressToConsole);
        }


        private string GetBarString(DetailedProgress progress)
        {
            int filledWidth = (int)(progress.ProgressPercentage / 100 * _progressBarWidth);
            var bar = "[" +
                new string('█', filledWidth) +
                new string('░', _progressBarWidth - filledWidth) +
                $"] {progress.ProgressPercentage:f2}% {progress.SpeedFormatted}";
            var diff = _currentBarLength - bar.Length;
            var clearString = diff <= 0 ? "" : new string(' ', diff);
            _currentBarLength = bar.Length;
            return bar + clearString;
        }

        private string GetSectionString()
        {
            if (_sectionsCount == 1)
            {
                return "";
            }

            return $"{_currentSection}/{_sectionsCount} ";
        }

        private void SetCursorPosition()
        {
            if (_firstDraw)
            {
                Console.WriteLine(_message);
                _cursorTop = Console.CursorTop;
                _firstDraw = false;
                return;
            }

            Console.SetCursorPosition(0, _cursorTop);
        }

        private void ProgressToConsole(DetailedProgress progress)
        {
            lock (_lock)
            {
                if (progress.ProgressPercentage < _progress)
                {
                    return;
                }

                _progress = progress.ProgressPercentage;
                SetCursorPosition();
                var bar = GetBarString(progress);
                var section = GetSectionString();
                Console.Write(section + bar);
            }
        }
    }
}
