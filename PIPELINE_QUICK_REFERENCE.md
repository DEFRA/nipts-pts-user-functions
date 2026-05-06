# Quick Reference: Pipeline Changes for Isolated Worker Migration

## ?? CRITICAL CHANGE REQUIRED

### What Changed in `pipeline/pipeline-function.yaml`

#### ? REMOVED (In-Process Model):
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

#### ? ADDED (Isolated Worker Model):
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

---

## Why This Matters

| Without This Change | With This Change |
|-------------------|------------------|
| ? Function App won't start | ? Function App starts correctly |
| ? Deployment will fail | ? Deployment succeeds |
| ? Runtime errors | ? Functions execute properly |

---

## What Stays The Same

? Build process  
? Test execution  
? Connection strings  
? Deployment slots  
? All other pipeline settings  

---

## Quick Deployment Steps

1. **Merge** the code changes (already done)
2. **Merge** the pipeline changes (`pipeline/pipeline-function.yaml`)
3. **Deploy to DEV** first using `forceDevDeploy: true`
4. **Verify** health endpoint and function execution
5. **Promote** through environments: DEV ? TST ? PRE ? PRD

---

## Verification Command

After deployment, verify the setting:

```bash
az functionapp config appsettings list \
  --name <your-function-app> \
  --resource-group <your-rg> \
  --query "[?name=='FUNCTIONS_WORKER_RUNTIME'].value"
```

Expected output: `["dotnet-isolated"]`

---

## ?? If Something Goes Wrong

1. **Check Azure Portal** ? Function App ? Log Stream
2. **Review Application Insights** for error messages
3. **Verify** `FUNCTIONS_WORKER_RUNTIME` is set correctly
4. **Restart** the Function App if needed

---

## Files Changed in This Migration

### Code Changes:
- ? All function files migrated
- ? All test files updated
- ? Project files updated
- ? Program.cs created

### Pipeline Changes:
- ? `pipeline/pipeline-function.yaml` - **UPDATED**

### Documentation:
- ? `MIGRATION_SUMMARY.md` - Complete migration guide
- ? `PIPELINE_CHANGES.md` - Detailed pipeline documentation
- ? `PIPELINE_QUICK_REFERENCE.md` - This file

---

## Need More Details?

See [PIPELINE_CHANGES.md](./PIPELINE_CHANGES.md) for comprehensive documentation.
