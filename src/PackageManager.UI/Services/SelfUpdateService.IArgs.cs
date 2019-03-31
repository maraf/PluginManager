using Neptuo;
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
        public interface IArgs : ICloneable<IArgs>
        {
            string Path { get; }

            bool IsSelfUpdate { get; set; }
            string SelfOriginalPath { get; set; }
        }
    }
}
