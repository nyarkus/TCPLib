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
            if (task == null) throw new ArgumentNullException(nameof(task));

            var delayTask = Task.Delay(timeout, cancellationToken);

            var completedTask = await Task.WhenAny(task, delayTask);

            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            if (completedTask == delayTask)
            {
                return false;
            }
            return true;
        }
    }
}
