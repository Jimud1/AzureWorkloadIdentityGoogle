# AzureWorkloadIdentityGoogle
Azure to google workload identity federation
Implement azure to google workload identity federation 
Described in the document below

https://cloud.google.com/iam/docs/workload-identity-federation-with-other-clouds
## Quick overview
- Create Azure enterprise application
- Set application ID URI
- Add App role 
- Create managed identity (if required)
- Give app role to identities

## Setup google workload provider

- Select a provider: OpenID Connect (OIDC).
- Provider name: Name for the provider. This cannot changed.
- Issuer URL: https://sts.windows.net/TENANT_ID. Replace TENANT_ID with the tenant ID (GUID) of your Azure AD tenant.
- Allowed audiences: api://your-application-id-uri

Add your managed identities to Google IAM 'Workload Identity User' role

principal://iam.googleapis.com/projects/{PROJECT-NUMBER}/locations/global/workloadIdentityPools/{PROVIDER-NAME}/subject/{IDENTITY-OBJECT-ID}

Instead of using a json to add to your environment, you can use AzureWorkloadIdentityGoogleCredential
