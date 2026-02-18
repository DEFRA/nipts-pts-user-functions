# Pipeline Changes Required for Isolated Worker Model Migration

## Overview

The migration from **in-process** to **isolated worker** model requires **critical changes** to the Azure Pipeline configuration. Failing to make these changes will result in deployment failures.

---

## ?? CRITICAL CHANGE REQUIRED

### **Remove `FUNCTIONS_INPROC_NET8_ENABLED` Setting**

**Location:** `pipeline/pipeline-function.yaml`

**Current Configuration (INCORRECT for Isolated Worker):**
```yaml
appSettingsEnv:
  dev: >-
    -FUNCTIONS_INPROC_NET8_ENABLED "1"
  snd: >-
    -FUNCTIONS_INPROC_NET8_ENABLED "1"
  tst: >-
    -FUNCTIONS_INPROC_NET8_ENABLED "1"
  pre: >-
    -FUNCTIONS_INPROC_NET8_ENABLED "1"
  prd: >-
    -FUNCTIONS_INPROC_NET8_ENABLED "1"
```

**Required Configuration (for Isolated Worker):**
```yaml
appSettingsEnv:
  dev: >-
    -FUNCTIONS_WORKER_RUNTIME "dotnet-isolated"
  snd: >-
    -FUNCTIONS_WORKER_RUNTIME "dotnet-isolated"
  tst: >-
    -FUNCTIONS_WORKER_RUNTIME "dotnet-isolated"
  pre: >-
    -FUNCTIONS_WORKER_RUNTIME "dotnet-isolated"
  prd: >-
    -FUNCTIONS_WORKER_RUNTIME "dotnet-isolated"
```

### Why This Change Is Critical

| Setting | Purpose | When to Use |
|---------|---------|-------------|
| `FUNCTIONS_INPROC_NET8_ENABLED` | Enables in-process .NET 8 support | **In-Process Model ONLY** |
| `FUNCTIONS_WORKER_RUNTIME` | Specifies the worker runtime | **Isolated Worker Model** |

Using `FUNCTIONS_INPROC_NET8_ENABLED` with isolated worker binaries will cause the function app to fail to start.

---

## Additional Pipeline Considerations

### 1. ? Build Configuration
**No changes needed** - The current pipeline build configuration is compatible:
```yaml
buildProjects: |
  **/*Functions.csproj
  **/*Tests.csproj
publishProject: '**/*Functions.csproj'
```

The build will automatically use the new `Microsoft.NET.Sdk.Worker` SDK from the project file.

### 2. ? Connection Strings
**No changes needed** - Connection strings configuration remains the same:
```yaml
connectionStrings: '[{"name": "sql_db", "value": "Server=tcp:$(sqlServerName),1433;Database=pet-travel;Authentication=Active Directory Managed Identity;", "type": "SQLAzure", "slotSetting": false}]'
```

### 3. ? Test Execution
**No changes needed** - Test projects have been updated and will work with existing pipeline test steps.

### 4. ? App Type
**No changes needed** - The app type remains as `functionApp`:
```yaml
appType: 'functionApp'
```

---

## Recommended Additional Settings (Optional)

While not strictly required, consider adding these application settings for better isolated worker performance:

```yaml
appSettingsEnv:
  dev: >-
    -FUNCTIONS_WORKER_RUNTIME "dotnet-isolated"
    -FUNCTIONS_WORKER_PROCESS_COUNT "10"
  snd: >-
-FUNCTIONS_WORKER_RUNTIME "dotnet-isolated"
    -FUNCTIONS_WORKER_PROCESS_COUNT "10"
  tst: >-
    -FUNCTIONS_WORKER_RUNTIME "dotnet-isolated"
    -FUNCTIONS_WORKER_PROCESS_COUNT "10"
  pre: >-
    -FUNCTIONS_WORKER_RUNTIME "dotnet-isolated"
    -FUNCTIONS_WORKER_PROCESS_COUNT "10"
  prd: >-
    -FUNCTIONS_WORKER_RUNTIME "dotnet-isolated"
    -FUNCTIONS_WORKER_PROCESS_COUNT "10"
```

### Additional Optional Settings:

| Setting | Purpose | Recommended Value |
|---------|---------|-------------------|
| `FUNCTIONS_WORKER_PROCESS_COUNT` | Number of worker processes | `10` (adjust based on load) |
| `FUNCTIONS_WORKER_SHARED_MEMORY_DATA_TRANSFER_ENABLED` | Enable shared memory for better performance | `1` |

---

## Azure Function App Configuration

### Manual Configuration Changes (if not using pipeline)

If you need to manually update Azure Function App settings:

1. **Navigate to Azure Portal** ? Your Function App ? **Configuration**
2. **Remove** the following setting:
   - `FUNCTIONS_INPROC_NET8_ENABLED`
3. **Add/Update** the following setting:
   - **Name:** `FUNCTIONS_WORKER_RUNTIME`
   - **Value:** `dotnet-isolated`
4. **Save** and **Restart** the Function App

---

## Deployment Checklist

Before deploying the migrated code:

- [ ] **Update pipeline YAML** - Remove `FUNCTIONS_INPROC_NET8_ENABLED` and add `FUNCTIONS_WORKER_RUNTIME`
- [ ] **Test in DEV environment first** - Use `forceDevDeploy: true` parameter
- [ ] **Verify function app starts correctly** - Check Azure Portal logs
- [ ] **Test all endpoints** - Ensure API functionality works
- [ ] **Monitor Application Insights** - Check for any errors or warnings
- [ ] **Gradually roll out** - DEV ? TST ? PRE ? PRD

---

## Verification Steps

After deployment, verify the isolated worker model is running correctly:

### 1. Check Function App Settings
```bash
# Using Azure CLI
az functionapp config appsettings list --name <function-app-name> --resource-group <resource-group> --query "[?name=='FUNCTIONS_WORKER_RUNTIME'].value"
```

Expected output: `["dotnet-isolated"]`

### 2. Check Function App Logs
Navigate to **Function App ? Log Stream** and verify:
- ? No errors about "in-process" mode
- ? Functions are loading successfully
- ? HTTP triggers are responding

### 3. Test Health Endpoint
```bash
curl https://<your-function-app>.azurewebsites.net/health
```

Expected: HTTP 200 OK

---

## Rollback Plan

If issues occur after deployment:

### Option 1: Quick Rollback (Revert Pipeline)
1. Revert pipeline YAML to use `FUNCTIONS_INPROC_NET8_ENABLED "1"`
2. Deploy the previous in-process code version
3. Restart function app

### Option 2: Slot Swap
If using deployment slots:
1. Test isolated worker in staging slot first
2. If issues, swap back to production slot
3. Investigate issues before retry

---

## Common Deployment Issues

### Issue 1: Function App Won't Start
**Symptom:** Function app shows as "Stopped" or constantly restarting
**Cause:** `FUNCTIONS_INPROC_NET8_ENABLED` still set with isolated worker binaries
**Fix:** Remove the setting and add `FUNCTIONS_WORKER_RUNTIME "dotnet-isolated"`

### Issue 2: Functions Not Loading
**Symptom:** Functions list is empty in Azure Portal
**Cause:** Incorrect worker runtime configuration
**Fix:** Verify `FUNCTIONS_WORKER_RUNTIME` is set to `dotnet-isolated`

### Issue 3: HTTP 500 Errors
**Symptom:** All endpoints return HTTP 500
**Cause:** Dependency injection or configuration issues
**Fix:** Check Application Insights logs for detailed error messages

---

## Summary of Required Pipeline Changes

### ? Remove:
```yaml
-FUNCTIONS_INPROC_NET8_ENABLED "1"
```

### ? Add:
```yaml
-FUNCTIONS_WORKER_RUNTIME "dotnet-isolated"
```

### ?? Location:
`pipeline/pipeline-function.yaml` ? `appSettingsEnv` section (all environments)

---

## Contact & Support

- **For Pipeline Issues:** Contact DevOps team
- **For Code Issues:** Contact Development team
- **For Azure Issues:** Contact Infrastructure team

---

## References

- [Azure Functions Isolated Worker Configuration](https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide#application-settings)
- [Azure Functions App Settings Reference](https://learn.microsoft.com/en-us/azure/azure-functions/functions-app-settings)
- [Migration Guide (Main Document)](./MIGRATION_SUMMARY.md)
