using System;
using System.Threading.Tasks;

namespace DisklessAsyncStreams.Consumer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var bigFileDownloader = new BigFileDownloader();
            await bigFileDownloader.StreamWriteLines();
        }
    }

    public record Trade(int Number, DateTime Time, decimal Price);
}