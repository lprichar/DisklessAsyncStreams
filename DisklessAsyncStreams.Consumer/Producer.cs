using System.Collections.Generic;
using System.Threading.Tasks;

namespace DisklessAsyncStreams.Consumer
{
    internal class Producer
    {
        public static async IAsyncEnumerable<int> GetNumbersAsync()
        {
            int i = 0;
            while (true)
            {
                await Task.Delay(1);
                yield return i;
                i++;
            }
        }
    }
}