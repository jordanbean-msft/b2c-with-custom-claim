using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Notifications;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace DemoWebAppB2CWithCustomClaim
{
    public partial class Startup
    {
        private static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string aadInstance = EnsureTrailingSlash(ConfigurationManager.AppSettings["ida:AADInstance"]);
        private static string tenantId = ConfigurationManager.AppSettings["ida:TenantId"];
        private static string postLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];
        public static string SignUpPolicyId = ConfigurationManager.AppSettings["ida:SignUpSignInPolicyId"];
        public static string SignInPolicyId = ConfigurationManager.AppSettings["ida:SignUpSignInPolicyId"];
        public static string ProfilePolicyId  = ConfigurationManager.AppSettings["ida:EditProfilePolicyId"];
        private static string authority = aadInstance + tenantId;

        public void ConfigureAuth(IAppBuilder app) {
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions {
                
            });

            // Configure OpenID Connect middleware for each policy
            app.UseOpenIdConnectAuthentication(CreateOptionsFromPolicy(SignUpPolicyId));
            app.UseOpenIdConnectAuthentication(CreateOptionsFromPolicy(ProfilePolicyId));
            app.UseOpenIdConnectAuthentication(CreateOptionsFromPolicy(SignInPolicyId));
        }

        private static string EnsureTrailingSlash(string value)
        {
            if (value == null)
            {
                value = string.Empty;
            }

            if (!value.EndsWith("/", StringComparison.Ordinal))
            {
                return value + "/";
            }

            return value;
        }

        // Used for avoiding yellow-screen-of-death
        private Task AuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> notification) {
            notification.HandleResponse();
            if (notification.Exception.Message == "access_denied") {
                notification.Response.Redirect("/");
            }
            else {
                notification.Response.Redirect("/Home/Error?message=" + notification.Exception.Message);
            }

            return Task.FromResult(0);
        }

        private OpenIdConnectAuthenticationOptions CreateOptionsFromPolicy(string policy) {
            return new OpenIdConnectAuthenticationOptions {
                // For each policy, give OWIN the policy-specific metadata address, and
                // set the authentication type to the id of the policy
                //MetadataAddress = String.Format(aadInstance, tenantId, policy),
                MetadataAddress = String.Format(aadInstance, policy),
                AuthenticationType = policy,

                //ProtocolValidator = new OpenIdConnectProtocolValidator {
                //    RequireNonce = false,
                //    RequireState = false
                //},
                // These are standard OpenID Connect parameters, with values pulled from web.config
                ClientId = clientId,
                RedirectUri = postLogoutRedirectUri,
                PostLogoutRedirectUri = postLogoutRedirectUri,
                Notifications = new OpenIdConnectAuthenticationNotifications {
                    AuthenticationFailed = AuthenticationFailed
                },
                Scope = "openid",
                ResponseType = "id_token",

                // This piece is optional - it is used for displaying the user's name in the navigation bar.
                TokenValidationParameters = new TokenValidationParameters {
                    NameClaimType = "name",
                    SaveSigninToken = true //important to save the token in boostrapcontext
                }

                
            };
        }
    }
}
