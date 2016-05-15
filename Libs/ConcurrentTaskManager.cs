using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPL
{
    public class ConcurrentTaskManager<T>
    {

        Dictionary<string, InstanceQM<T>> RedundantTaskThrottler = new Dictionary<string, InstanceQM<T>>();

        public async Task<T> SendOperationKeyAndWaitForAccessAsync(string operationKey)
        {
            if (!RedundantTaskThrottler.ContainsKey(operationKey))
            {
                RedundantTaskThrottler.Add(operationKey, null);
            }
            else
            {
                var throttler = RedundantTaskThrottler[operationKey];
                if (throttler == null)
                {
                    throttler = RedundantTaskThrottler[operationKey] = new InstanceQM<T>(1, true);
                    await throttler.GetFreeInstanceAsync();
                }
                var obj = await throttler.GetFreeInstanceAsync();
                throttler.ReleaseInstance(obj);
                return obj;
            }
            return default(T);
        }

        public void ReleaseOperation(string operationKey, T operationResult)
        {
            if (RedundantTaskThrottler.ContainsKey(operationKey))
            {
                var throttler = RedundantTaskThrottler[operationKey];
                if (throttler != null)
                {
                    throttler.ReleaseInstance(operationResult);
                }
                RedundantTaskThrottler.Remove(operationKey);
            }
        }
    }
}
