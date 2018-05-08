using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Domain 
{
    public class AuthenticationHelper : HttpClientHandler
    {
        private string token;
        private byte[] authBytes;

        public AuthenticationHelper(string username, string password)
        {
            token = Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + password));
        }

        public AuthenticationHelper(string usernameAndPassword)
        {
            token = Convert.ToBase64String(Encoding.UTF8.GetBytes(usernameAndPassword).ToArray());
        }

        public string Token => token;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            System.Threading.CancellationToken cancellationToken)
        {
            request.Headers.Add("Authorization", "Basic " + token);

            return base.SendAsync(request, cancellationToken);
        }
    }
}