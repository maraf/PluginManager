using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.ViewModels.Commands
{
    public class CallCounter
    {
        public int Count { get; private set; }
        public void Increment() => Count++;

        public static implicit operator bool(CallCounter counter) => (counter?.Count ?? 0) > 0;
        public static implicit operator int(CallCounter counter) => counter?.Count ?? 0;
    }
}
