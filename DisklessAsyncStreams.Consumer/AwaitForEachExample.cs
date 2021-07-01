using System.Collections.Generic;
using System.Threading.Tasks;

namespace DisklessAsyncStreams.Consumer
{
    class AwaitForEachExample
    {
        public async Task IterateNumbersTo10()
        {
            IAsyncEnumerable<int> numbers = Producer.GetNumbersAsync();
            await foreach (int number in numbers)
            {
                if (number > 10)
                {
                    break;
                }
            }
        }
    }
}
