using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neptuo;
using PackageManager.Services;

namespace PackageManager
{
    partial class Args : SelfUpdateService.IArgs, ICloneable<SelfUpdateService.IArgs>
    {
        SelfUpdateService.IArgs ICloneable<SelfUpdateService.IArgs>.Clone()
            => Clone();
    }
}
