using Azure.Core;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.CloudSecurityToken.v1;
using Google.Apis.CloudSecurityToken.v1.Data;
using Google.Apis.IAMCredentials.v1;
using Google.Apis.IAMCredentials.v1.Data;
using Google.Apis.Services;

namespace AzureWorkloadIdentityGoogleCredential;

public class AzureWorkloadIdentityCredential : ServiceCredential
{
    private readonly TokenCredential _azureCredential;
    private readonly CloudSecurityTokenService _cloudSecurityTokenService;
    private readonly WorkloadIdentityConfig _workloadIdentityConfig;

    public AzureWorkloadIdentityCredential(
        TokenCredential azureCredential,
        CloudSecurityTokenService cloudSecurityTokenService,
        WorkloadIdentityConfig workloadIdentityConfig) : base(new Initializer(GoogleAuthConsts.TokenUrl))
    {
        _azureCredential = azureCredential;
        _cloudSecurityTokenService = cloudSecurityTokenService;
        _workloadIdentityConfig = workloadIdentityConfig;

        CheckWorkloadIdentityConfig(workloadIdentityConfig);
    }

    public override async Task<bool> RequestAccessTokenAsync(CancellationToken taskCancellationToken)
    {
        var azureToken = await GetAzureToken(_workloadIdentityConfig.AzureScope, taskCancellationToken);
        var accessToken = await GetGcpAccessToken(azureToken, taskCancellationToken);

        Token = new TokenResponse()
        {
            AccessToken = accessToken.AccessToken,
            ExpiresInSeconds = TokenExpiresIn(accessToken.ExpireTimeDateTimeOffset),
            IssuedUtc = DateTime.UtcNow
        };

        return true;
    }

    private static long TokenExpiresIn(DateTimeOffset? expireTimeDateTimeOffset)
    {
        var expiresInSeconds = expireTimeDateTimeOffset - DateTimeOffset.UtcNow;
        return (long)expiresInSeconds!.Value.TotalSeconds;
    }

    private async ValueTask<string> GetAzureToken(string scope, CancellationToken cancellationToken) =>
        (await _azureCredential.GetTokenAsync(new TokenRequestContext(scopes: [scope]), cancellationToken)).Token;

    private async Task<GenerateAccessTokenResponse> GetGcpAccessToken(string azureToken, CancellationToken cancellationToken)
    {
        var stsToken = await _cloudSecurityTokenService.V1.Token(new GoogleIdentityStsV1ExchangeTokenRequest
        {
            SubjectToken = azureToken,
            Audience = _workloadIdentityConfig.GcpStsAudience,
            GrantType = GoogleAuthConstants.StsGrantType,
            SubjectTokenType = GoogleAuthConstants.StsSubjectTokenType,
            Scope = GoogleAuthConstants.StsScope,
            RequestedTokenType = GoogleAuthConstants.StsTokenType
        }).ExecuteAsync(cancellationToken);

        using var iamService = new IAMCredentialsService(new BaseClientService.Initializer
        {
            HttpClientInitializer = GoogleCredential.FromAccessToken(stsToken.AccessToken)
        });

        return await iamService.Projects.ServiceAccounts.GenerateAccessToken(new GenerateAccessTokenRequest
        {
            Scope = _workloadIdentityConfig.GcpScopes
        }, $"{GoogleAuthConstants.ImpersonatedServiceAccountPrefix}{_workloadIdentityConfig.GcpServiceAccountEmail}").ExecuteAsync(cancellationToken);
    }

    private static void CheckWorkloadIdentityConfig(WorkloadIdentityConfig workloadIdentityConfig)
    {
        ArgumentException.ThrowIfNullOrEmpty(workloadIdentityConfig.GcpStsAudience);
        ArgumentException.ThrowIfNullOrEmpty(workloadIdentityConfig.AzureScope);
        ArgumentException.ThrowIfNullOrEmpty(workloadIdentityConfig.GcpServiceAccountEmail);

        if (workloadIdentityConfig.GcpScopes.Count == 0)
        {
            throw new ArgumentException(GoogleAuthConstants.NoGcpScopesMessage);
        }
    }
}