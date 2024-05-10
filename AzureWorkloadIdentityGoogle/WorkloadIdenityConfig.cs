namespace AzureWorkloadIdentityGoogle;

public class WorkloadIdentityConfig
{
    /// <summary>
    /// The application ID Uri e.g. api://app-id-url
    /// </summary>
    public required string AzureScope { get; set; }

    /// <summary>
    /// E.g //iam.googleapis.com/projects/{PROJECT_NUMBER}/locations/global/workloadIdentityPools/{POOL}/providers/{PROVIDER}
    /// </summary>
    public required string GcpStsAudience { get; set; }

    /// <summary>
    /// Google Cloud Scopes, e.g. ["https://www.googleapis.com/auth/androidpublisher", "..."]
    /// </summary>
    public required IList<string> GcpScopes { get; set; }

    /// <summary>
    /// email for impersonated service account
    /// </summary>
    public required string GcpServiceAccountEmail { get; set; }
}
