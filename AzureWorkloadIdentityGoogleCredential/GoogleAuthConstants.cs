namespace AzureWorkloadIdentityGoogleCredential;

public class GoogleAuthConstants
{
    public const string StsGrantType = "urn:ietf:params:oauth:grant-type:token-exchange";
    public const string StsSubjectTokenType = "urn:ietf:params:oauth:token-type:jwt";
    public const string StsScope = "https://www.googleapis.com/auth/cloud-platform";
    public const string StsTokenType = "urn:ietf:params:oauth:token-type:access_token";
    public const string ImpersonatedServiceAccountPrefix = "projects/-/serviceAccounts/";

    public const string NoGcpScopesMessage = "No Google Cloud scopes";
}