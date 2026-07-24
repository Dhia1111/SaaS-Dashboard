namespace SharedDto_Enum
{

    public enum enPaymentProviders
    {
        stripe = 1,
    }
    public enum enGeneralState
    {
        Failed = 0,
        Pending = 1,
        Active = 2,
        Suspended = 3,
        Cancelled = 4,
        Success = 5,
        Expired,
        eRefunded
    }
    public enum enMarkettingPlatforms
    {

        TikTok = 1,
        FaceBook,
        LinkedIn,
        Youtoube,
        Instigram,
        Google,
        Aothoers,


    }
   
    public enum enRoles
    {

        Account__Manager=1, // alowd to have access to the tenant data with constraints
                          // 

    }
    public enum enPlaformRoles
    {
        
        Tenant,
        User,
        Employee,
        Owner

    }
    public enum enTenantAccountManangerAutherization
    {

        Edit_User__Autherization = 2,
        Delete__User = 4,
        View__User = 8,
        View__UserList = 16,
        Add__Invoice = 32,
    }
    public enum enAdminstrationAuth
    {

        AddNewEmployee = 1,
        ManagePayment

    }
    public enum enPlatFromManangerAutherizations { }

    public enum enTenantPricingCycle
    {
        hours = 1,
        days = 2,
        weeks = 3,
        months = 4,
        years = 5
    }   

    public enum enSubscriptionType
    {
        Free = 1,
        Paid = 2
    }
    public enum enSubscriptionGradeStatus
    {
        Downgrade = 1,
        Upgrade = 2,
        Renewal = 3,
        MoveToPaid = 4,
        MoveToFree = 5,
        FirstTry 

    }


}
