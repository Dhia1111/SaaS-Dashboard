using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connection.models
{
    public enum GeneralState
    {
        Failed = 0,
        Pending = 1,
        Active = 2,
        Suspended = 3,
        Cancelled = 4,
        Success= 5,
    }
    public enum PlatformBillingInterval
    {
        Monthly = 1,
        Yearly = 2
    }

}
