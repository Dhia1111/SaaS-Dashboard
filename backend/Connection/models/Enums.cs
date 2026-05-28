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
    public enum Roles { 
       
        Tenant__Admine,// full access to the account
        Account__Manager, // alowd to have access to the tenant data with constraints 
        Platform__Admine,
        Platform__Mananger,
     }

    public enum TenantAccountManangerAutherization {
    
        Edit_User__Autherization = 2,
        Delete__User = 4,
        View__User = 8,
        View__UserList = 16,
        Add__Invoice = 32, 
    }
    public enum PlatFromManangerAutherizations { }

}
