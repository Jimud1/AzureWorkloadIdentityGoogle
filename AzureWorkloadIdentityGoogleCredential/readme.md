Step 1 register WorkloadIdentityConfig in your appsettings.json
"WorkloadIdentityConfig": {
  "AzureScope": "api://app-id-uri",
  "GcpStsAudience": "//iam.googleapis.com/projects/{PROJECT_NUMBER}}/locations/global/workloadIdentityPools/{POOL}/providers/{PROVIDER}",
  "GcpScopes": [ "https://www.googleapis.com/auth/androidpublisher" ],
  "GcpServiceAccountEmail": "email-services-account-to-impersonate.iam.gserviceaccount.com"
}

step 2 to use

var workloadConfig = configuration.GetSection(nameof(WorkloadIdentityConfig)).Get<WorkloadIdentityConfig>() ??
    throw new InvalidOperationException("WorkloadIdentityConfig has not been set");

var googleCredential = new AzureWorkloadIdentityGoogleCredential(new DefaultAzureCredential(), new CloudSecurityTokenService(), workloadConfig);