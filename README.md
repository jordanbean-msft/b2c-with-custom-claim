# b2c-with-custom-claim

This repo includes a sample application & B2C tenant with custom policy to show how to store a custom claim on the user's profile & generate this unique ID the first time a user logs in.

## Disclaimer

**THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. This is demo code provided only as an example.**

## Architecture

![architecture](./architecture.png)

In this example, the backend ASP<area>.NET Framework MVC application expects a **custom claim** to show up to authorize the incoming user. This is a custom claim that was provided by a different identity provider and is used for all authorization on the backend (it uniquely identifies the incoming user). In this case, the backend applications can be only minimally changed, so this custom claim needs to show up in the OpenIDConnect ID token provided by the identity provider.

We can stand up an [Azure B2C tenant](https://docs.microsoft.com/en-us/azure/active-directory-b2c/) to support this. Azure AD B2C allows us to customize the signin experience for the users, support local & social accounts and work in any Azure environment (such as Government cloud).

We need to create a [custom B2C user policy](https://docs.microsoft.com/en-us/azure/active-directory-b2c/custom-policy-overview) that will guide the user through the sign up/sign in process. They will either create a new account if they don't already have one or sign in with their existing account. If they are signing up for the first time, we need to generate the **custom claim** (in this example, it is called the TalisenID to emulate the previous identity provider). We can use an [Azure Function](https://docs.microsoft.com/en-us/azure/azure-functions/) as an inexpensive & always available HTTP call to generate this ID for new users. Once it has been generated, it will always be stored on the user's profile in Azure AD B2C.

We also want to scan all incoming traffic to the backend applications for potential attacks, so we can direct traffic through an [Azure Application Gateway](https://docs.microsoft.com/en-us/azure/application-gateway/overview) with [Web Application Firewall (WAF)](https://docs.microsoft.com/en-us/azure/web-application-firewall/).

## Structure

The repo is divided into 2 sections.

- **b2c-custom-policy** : The custom B2C policy you will need to upload to an Azure AD B2C tenant
- **DemoWebAppB2CWithCustomClaim** : A dummy ASP.NET Framework MVC app that will redirect the user to sign up/sign in with Azure AD B2C and print the ID token that is received from the identity provider to show the custom claim come through.



## References

- https://docs.microsoft.com/en-us/azure/active-directory-b2c/
- https://docs.microsoft.com/en-us/azure/active-directory-b2c/custom-policy-overview
- https://docs.microsoft.com/en-us/azure/azure-functions/
- https://docs.microsoft.com/en-us/azure/application-gateway/overview
- https://docs.microsoft.com/en-us/azure/web-application-firewall/