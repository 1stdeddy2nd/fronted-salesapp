# SalesApp

A secure .NET 8 Web API for managing sales data, featuring:

- 🔐 **Azure Entra ID (Azure AD) Authentication & RBAC**
- 🔑 **Secrets managed with Azure Key Vault**
- 📘 **Swagger UI secured with OAuth 2.0 Authorization Code (PKCE)**
- 📊 **SQL Server via Entity Framework Core**
- ☁️ **Deployed to Azure App Service**

---

## 🔧 Technologies

- ASP.NET Core 8 Web API
- Azure Active Directory (Entra ID)
- Azure Key Vault
- Swagger (Swashbuckle)
- Entity Framework Core (SQL Server)
- Azure App Service

---

## 🚀 Getting Started

### 1. Clone the Repo

```bash
git clone https://github.com/1stdeddy2nd/fronted-salesapp.git
cd fronted-salesapp
```

### 2. Setup Local Configuration

Copy the config template:

```bash
cp appsettings-example.json appsettings.json
```

Update `appsettings.json` with your **local or test** values:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=...;User Id=...;Password=...;"
  },
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "<your-tenant-id>",
    "ClientId": "<your-api-client-id>",
    "SwaggerClientId": "<your-swagger-client-id>",
    "Scopes": {
      "Read": [ "App.Read" ]
    }
  }
}
```

> 🔐 In production, these values are pulled from **Azure Key Vault** automatically using **Managed Identity**.

---

## 🧪 Running the App

```bash
dotnet build
dotnet run
```

Visit: `https://localhost:5001/swagger`

Click **Authorize** to log in via Azure AD and test authenticated endpoints.

---

## 🔐 Authentication & Roles

- Uses **Azure AD JWT Bearer tokens**
- App Roles defined in Azure:
  - `Sales.Admin`
  - `Sales.User`

Use `[Authorize(Roles = "Sales.Admin")]` in controllers to secure endpoints.

---

## 🔑 Azure Key Vault Integration

Secrets like connection strings and client IDs are securely stored in Azure Key Vault and loaded using:

```csharp
builder.Configuration.AddAzureKeyVault(
    new Uri("https://<your-keyvault>.vault.azure.net/"),
    new DefaultAzureCredential());
```

Ensure your App Service has **Managed Identity** enabled and **Key Vault access** (role: `Key Vault Secrets User`).

---

## 🧼 .gitignore Best Practices

```gitignore
# Binaries
bin/
obj/

# User secrets and sensitive config
appsettings.json
appsettings.Development.json
appsettings.Production.json
!appsettings-example.json
```

---

## 📦 Deployment

1. Push to Azure Repo / GitHub
2. Deploy to **Azure App Service**
3. Assign Managed Identity to the app
4. Grant access to Azure Key Vault via IAM
5. Configure custom domain or TLS (optional)

---

## 🧭 Useful Azure Resources

- [Azure Key Vault RBAC](https://learn.microsoft.com/en-us/azure/key-vault/general/rbac-guide)
- [App Roles in Azure AD](https://learn.microsoft.com/en-us/azure/active-directory/develop/howto-add-app-roles-in-azure-ad-apps)
- [Swagger OAuth2 PKCE Setup](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/)

---

## 📫 Contact

Maintained by [Deddy Setiawan](mailto:deddysetiawan17@gmail.com)

---

## 📝 License

MIT
