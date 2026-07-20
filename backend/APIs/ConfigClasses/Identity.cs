using APIs.Controllers;
using Business;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharedDto_Enum;
using Stripe.Identity;

namespace APIs.ConfigClasses
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method)]
    public class RequiersdClaimAttribute : Attribute,IAuthorizationFilter
    {
       
        private readonly string _ClaimValue;
        private readonly enPlaformRoles _DirectAutherizedRoles;

     public  RequiersdClaimAttribute(string ClaimValue,enPlaformRoles role){
        
         
        _ClaimValue=ClaimValue;
            _DirectAutherizedRoles=role;



       }

       public void OnAuthorization(AuthorizationFilterContext FilterContext)
        {
            string? authorization;
            long auth;
            long.TryParse(_ClaimValue, out long ClaimValueInt);

           var httpContext = FilterContext.HttpContext;
            var  user= httpContext.User;

            if(user.HasClaim("IsTheOwner", "true"))
            {
             
                FilterContext.Result= new OkResult();
             

            }


            switch (_DirectAutherizedRoles) {

                    case enPlaformRoles.User:

                    if(user.HasClaim("IsATenant", "true"))
                    {
                        

                        FilterContext.Result = new OkResult();

                    
                    }
                    authorization = user.Claims.SingleOrDefault(e => e.Type == "Authorization")?.Value;
                  
                    long.TryParse(authorization, out  auth);

                    if ((auth & ClaimValueInt) == ClaimValueInt)
                    {
                        
                        FilterContext.Result = new OkResult();


                    }


                    break;

                    case enPlaformRoles.Tenant:

                    if (user.HasClaim("IsATenant", "true"))
                    {

                        if (user.HasClaim("Actvie", "true"))
                        {

                            var Tenantauth = user.Claims.SingleOrDefault(e => e.Type == "TenantAuthorizations").Value;
                            long.TryParse(Tenantauth, out long TenantAuthLong);

                            if ((TenantAuthLong & ClaimValueInt) == ClaimValueInt)
                            {
                                FilterContext.Result = new OkResult();
                                return;
                            }
                            else
                            {
                                FilterContext.Result = new ForbidResult();
                            }

                        }
                        else
                        {
                            FilterContext.Result = new ForbidResult();
                            return;
                        }
                        



                    }
                    else
                    {
                        FilterContext.Result = new ForbidResult();
                        return;
                    }

                        break;

                    case enPlaformRoles.Employee:
                  
                    if (user.HasClaim("IsATenant", "true"))
                    {
                        FilterContext.Result = new ForbidResult();


                    }
                    else if (user.HasClaim("IsAnEmployee", "true"))
                    {
                       authorization = user.Claims.SingleOrDefault(e => e.Type == "Authorization")?.Value;

                        if (long.TryParse(authorization, out  auth))
                        {

                            if ((auth&ClaimValueInt)==ClaimValueInt)
                            {
                                FilterContext.Result = new OkResult();

                            }
                            else
                            {
                                FilterContext.Result = new ForbidResult();
                            }


                        }


                    }
                        break;
            
            }




       
        }

     

    }
}
