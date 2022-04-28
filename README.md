# Task 1

- Username **beibit.abdikalykov@beibittrial.onmicrosoft.com**
- Password **UuTVN8uhiTmX9Kn**
- Environment URL https://orgffab0bd8.crm4.dynamics.com
- Link to the table [Time Entries]

# Task 2
Azure function app has been deployed to https://timeentryapp20220428192941.azurewebsites.net/api/time-entry
You can run sample request and see data in [Time Entries] (if the data is not displayed, change the view type)
## Sample request
```
POST /api/time-entry HTTP/1.1
Host: timeentryapp20220428192941.azurewebsites.net
Content-Type: application/json
x-functions-key: xHHTjyuC2a6ZCmMVr0zuUISfhfwiECaEm38ZueapU0X6AzFuFiWbQw==
Content-Length: 82

{
    "StartOn": "2019-08-12T00:00:00",   
    "EndOn": "2019-08-19T00:00:00"
}
```

## Configuration
To run app in Viusal Studio set connection string **DataverseServiceConnectionString** to your dataverse server in `local.settings.json` file

```
{
  "IsEncrypted": false,
  "Values": {
    "DataverseServiceConnectionString": "<your connection string>"
  }
}
```
[//]: # 
   [Time Entries]: https://make.powerapps.com/environments/d292c8a4-f982-e5e3-9af3-ee00ae07554c/entities/d292c8a4-f982-e5e3-9af3-ee00ae07554c/msdyn_timeentry#data
