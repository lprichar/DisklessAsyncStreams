using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisklessAsyncStreams.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BigFileController : ControllerBase
    {
        [HttpGet]
        public async Task Get()
        {
            var httpResponseBodyFeature = HttpContext.Features.Get<IHttpResponseBodyFeature>();
            httpResponseBodyFeature.DisableBuffering();
            Response.StatusCode = 200;
            Response.ContentType = "text/csv";
            Response.Headers.Add("Content-Disposition", "attachment;filename=bigfile.csv");
            await Response.StartAsync();
            var outputStream = Response.BodyWriter.AsStream(leaveOpen: true);
            try
            {
                await WriteTradesToStream(outputStream);
            }
            finally
            {
                outputStream.Close();
            }
        }

        private async Task WriteTradesToStream(Stream responseStream)
        {
            //const int batchesToGenerate = 100;
            const int batchesToGenerate = 10_000;
            var batches = Enumerable.Range(0, batchesToGenerate);

            var encoding = new UTF8Encoding();
            foreach (var batchNumber in batches)
            {
                var batch = await GetBatchFromDb(batchNumber);
                foreach (var trade in batch)
                {
                    var line = $"{trade.Date:s},{trade.Price}{Environment.NewLine}";
                    var bytes = encoding.GetBytes(line);
                    await responseStream.WriteAsync(bytes);
                }
                await responseStream.FlushAsync();
            }
        }

        private async Task<IEnumerable<Trade>> GetBatchFromDb(int batch)
        {
            // simulate a database call
            await Task.Delay(1);

            const int batchSize = 10;
            const int maxPriceInDollars = 10;
            var random = new Random();
            return Enumerable.Range(0, batchSize)
                .Select(second => new Trade
                (
                    Date: DateTime.Today.AddSeconds((batch * 10) + second),
                    Price: random.Next(0, maxPriceInDollars * 1000) * 0.01M
                ));
        }
    }

    public record Trade
    (
        DateTime Date,
        decimal Price
    );
}
