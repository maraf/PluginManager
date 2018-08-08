using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Services
{
    partial class SelfUpdateService
    {
        public interface IApplication
        {
            IArgs Args { get; }
            void Shutdown();
        }
    }
}
