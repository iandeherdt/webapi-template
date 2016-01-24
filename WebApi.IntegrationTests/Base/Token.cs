namespace WebApiTemplate.IntegrationTests
{
    public class Token
    {
        public string userName { get; set; }
        public string value { get; set; }
        public string validTo { get; set; }
    }

    public class TokenResponse
    {
        public Token token { get; set; }
    }

    public class RootObjectToken
    {
        public TokenResponse tokenResponse { get; set; }
    }
}