using Amazon;

namespace AWS3.Controllers
{
    internal class AmazonSecurityTokenServiceClient
    {
        private string accessKey;
        private string secretKey;
        private RegionEndpoint regionEndpoint;

        public AmazonSecurityTokenServiceClient(string accessKey, string secretKey, RegionEndpoint regionEndpoint)
        {
            this.accessKey = accessKey;
            this.secretKey = secretKey;
            this.regionEndpoint = regionEndpoint;
        }
    }
}