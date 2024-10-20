using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPLib.Extentions
{
    public static class TaskExtentions
    {
        public static async Task<bool> TimeoutAsync(this Task task, TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            CheckParameters(task, timeout);
            return await WaitForTaskWithTimeout(task, timeout, cancellationToken);
        }

        private static void CheckParameters(Task task, TimeSpan timeout)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (timeout < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must be non-negative.");
        }

        private static async Task<bool> WaitForTaskWithTimeout(Task task, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var delayTask = Task.Delay(timeout, cancellationToken);
            var completedTask = await Task.WhenAny(task, delayTask);

            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            return completedTask != delayTask;
        }

    }
}
