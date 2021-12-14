using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeeUtils.Interfaces
{
    public interface IEvent
    {
        string Name { get; set; }
        object Args { get; set; }

    }
}
