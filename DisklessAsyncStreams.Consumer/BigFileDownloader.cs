using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DisklessAsyncStreams.Consumer
{
    public class BigFileDownloader
    {
        public async Task StreamWriteLines()
        {
            Stopwatch totalElapsed = new Stopwatch();
            totalElapsed.Start();
            var rows = StreamReadLines();
            await foreach (var row in rows)
            {
                if (row.Number % 10 == 0)
                {
                    Console.WriteLine($"{totalElapsed.Elapsed} - {row.Number}: {row.Time:hh:mm:ss} - {row.Price:C}");
                }
            }
            totalElapsed.Stop();
        }

        private async IAsyncEnumerable<Trade> StreamReadLines()
        {
            var timeout = new TimeSpan(days: 0, hours: 4, minutes: 0, seconds: 0);
            const long defaultBufferSizeIsTwoGigs = 2_147_483_647;
            const long tenMegabytes = 10_485_760;
            const long oneMegabyte = 1_048_576;
            using var httpClient = new HttpClient
            {
                Timeout = timeout,
                MaxResponseContentBufferSize = defaultBufferSizeIsTwoGigs
            };
            var uri = new Uri("http://localhost:61938/BigFile");
            // simulate getting query params from db
            await Task.Delay(100);
            using var response = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            await using var stream = await response.Content.ReadAsStreamAsync();
            using var streamReader = new StreamReader(stream, Encoding.UTF8);
            var rowNum = 0;
            while (!streamReader.EndOfStream)
            {
                var line = await streamReader.ReadLineAsync();
                // don't do this, delay in reading causes buffer to overflow
                //await Task.Delay(1000);
                var trade = GetFromLine(rowNum, line);
                rowNum += 1;
                yield return trade;
            }
        }

        public Trade GetFromLine(int number, string line)
        {
            var parts = line.Split(',');
            return new Trade(
                Number: number,
                Time: DateTime.Parse(parts[0]),
                Price: decimal.Parse(parts[1]));
        }
    }
}