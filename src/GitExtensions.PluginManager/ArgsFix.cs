using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo
{
    public interface ICloneable<T>
    {
        T Clone();
    }
}
