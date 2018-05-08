using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;
using System.Threading.Tasks;
using DPO.Common;
using DPO.Domain;
using DPO.Data;

namespace DPO.WebAPI
{
    public class DPOAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {

        //UserServices userservices = new UserServices(new DPOContext());
        //BaseServices baseServices = new BaseServices();
        //Repository repo = new Repository();

        //string efConnection = "metadata=res://*/Context.DPOContext.csdl|res://*/Context.DPOContext.ssdl|res://*/Context.DPOContext.msl;provider=System.Data.SqlClient;provider connection string='Data Source = 10.10.90.202; Initial Catalog = dbDaikinProjectOfficeStaging; Integrated Security = False; Persist Security Info=False;User ID = DaikinAdmin; Password=Da1k1n20L4;MultipleActiveResultSets=True;Connect Timeout = 120; Application Name = EntityFramework'";


        //DPOContext context = new DPOContext();

        UserServices userservices = new UserServices();
        


        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId = string.Empty;
            string clientSecret = string.Empty;


            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            if (context.ClientId == null)
            {
                context.SetError("Invalid Client", "Client credential could not be retrieved through the Authorization header.");
                context.Rejected();
                return;
            }

            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
           
            //Get User by Email
            var user = userservices.GetUserByEmail(context.UserName);

            if (user == null)
            {
                context.SetError("invalid_grant", "Email does not exist in system.");
                return;
            }

            //Check password
            var pwHash = Common.Crypto.Hash(context.Password, (int)user.Salt);

            if (string.Compare(pwHash, user.Password) != 0)
            {
                context.SetError("invalid_grant", "Provided username and password is incorrect.");
                return;
            }
            else if (!user.Approved)
            {
                context.SetError("invalid_grant", "User has not been approved.");
                return;
            }
            else if (!user.Enabled)
            {
                context.SetError("invalid_grant", "User is not enabled.");
                return;
            }
            #region check if business is enabled?
            //else if (!user.Business.Enabled) //Do we need this?
            //{
            //    context.SetError("invalid_grant", "Business is not enabled.");
            //    return;
            //}
            #endregion
            else
            {
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString(), ClaimValueTypes.Double));
                identity.AddClaim(new Claim(ClaimTypes.Name, user.Email));
                identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
                identity.AddClaim(new Claim(ClaimTypes.Role, user.UserTypeId.ToString()));
                identity.AddClaim(new Claim("UserType", user.UserTypeId.ToString()));
                context.Validated(identity);
            }

            user.LastLoginOn = DateTime.UtcNow;

            userservices.Update(user);
            
        }
    }
}