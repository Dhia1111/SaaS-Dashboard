using APIs.Controllers;
using Business;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharedDto_Enum;
using Stripe.Identity;
using System.Security;

namespace APIs.ConfigClasses
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method)]
    public class RequiersdClaimAttribute : Attribute,IAuthorizationFilter
    {
       
        private readonly string _ClaimVKey;
        private readonly enPlaformRoles _DirectAutherizedRoles;
        private readonly long _permissionValue;

     public  RequiersdClaimAttribute(string ClaimValue,enPlaformRoles role){
        
            _ClaimVKey=ClaimValue;
            _DirectAutherizedRoles=role;




        }





        public void OnAuthorization(AuthorizationFilterContext FilterContext )
        {

    

            string? authorization;
            long auth;
            var httpContext = FilterContext.HttpContext;
            var permissionLoader = httpContext.RequestServices.GetRequiredService<IPermissionLoader>();
            permissionLoader.TryGetPermission(_ClaimVKey, out var permissionValue);

            var user = httpContext.User;

            if(user.HasClaim("IsTheOwner", "true"))
            {
             
                 return;

            }
           
            if (!user.HasClaim("IsActive", "true"))
            {
                throw new UnauthorizedAccessException("Account is Not Active");

            }



            switch (_DirectAutherizedRoles) {

                    case enPlaformRoles.User:

                    if(user.HasClaim("IsATenant", "true"))
                    {
                        var Tenantauth = user.Claims.SingleOrDefault(e => e.Type == "TenantAuthorizations").Value;
                        long.TryParse(Tenantauth, out long TenantAuthLong);

                        if ((TenantAuthLong & permissionValue) == permissionValue)
                        {
                            return;
                        }

                        else
                        {
                            throw new UnauthorizedAccessException("Tenant does not have permission for this service ");
                        }
                    
                    }
   
                    authorization = user.Claims.SingleOrDefault(e => e.Type == "Authorization")?.Value;
                  
                    long.TryParse(authorization, out  auth);

                    if ((auth & permissionValue) == permissionValue)
                    {
                        
                         return;


                    }
                    else
                    {
                        throw new UnauthorizedAccessException();

                    }



                    case enPlaformRoles.Tenant:

                    if (user.HasClaim("IsATenant", "true"))
                    {

                        if (user.HasClaim("Actvie", "true"))
                        {

                            var Tenantauth = user.Claims.SingleOrDefault(e => e.Type == "TenantAuthorizations").Value;
                            long.TryParse(Tenantauth, out long TenantAuthLong);

                            if ((TenantAuthLong & permissionValue) == permissionValue)
                            {
                                return;
                            }
                            else
                            {
                                throw new UnauthorizedAccessException();
                            }

                        }
                        else
                        {
                            throw new UnauthorizedAccessException();
                        }




                    }
                    else
                    {
                        throw new UnauthorizedAccessException();
                    }


                    case enPlaformRoles.Employee:
                  
                    if (user.HasClaim("IsATenant", "true"))
                    {
                        return;

                    }
                    else if (user.HasClaim("IsAnEmployee", "true"))
                    {
                       authorization = user.Claims.SingleOrDefault(e => e.Type == "Authorization")?.Value;

                        if (long.TryParse(authorization, out  auth))
                        {

                            if ((auth&permissionValue)==permissionValue)
                            {
                                return;
                            }
                            else
                            {
                                throw new UnauthorizedAccessException();
                            }


                        }


                    }
                    throw new UnauthorizedAccessException();

            }





        }

     

    }
}
