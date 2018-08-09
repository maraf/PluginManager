using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Services
{
    partial class ProcessService
    {
        public interface IApplication
        {
            object Args { get; }

            void Shutdown();
        }
    }
}
