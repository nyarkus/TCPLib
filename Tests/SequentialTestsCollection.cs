using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    using Xunit;

    [CollectionDefinition("SequentialTests", DisableParallelization = true)]
    public class SequentialTestsCollection
    {
        // Здесь можно инициализировать общие ресурсы, если нужно
    }

}
