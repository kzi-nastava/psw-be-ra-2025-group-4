using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public class ExecutionStats
    {
        public int Starts { get; set; }
        public int Completed { get; set; }
        public int Abandoned { get; set; }
        public int Active { get; set; }
    }
}
