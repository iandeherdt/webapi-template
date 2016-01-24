namespace WebApiTemplate.IntegrationTests
{
    public class CredentialsRequest
    {
        public Credentials UserCredentials { get; set; }

        public string ResourceUri { get; set; }

        public string ClientId { get; set; }
    }
}