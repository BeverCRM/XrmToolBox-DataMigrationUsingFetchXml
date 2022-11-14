# Data Migration using FetchXml

This utility allows the user to migrate data of the entity from source to target. \
The user needs to define FetchXml queries for each entity to which data should be migrated.   

![Untitled](https://user-images.githubusercontent.com/60586462/201646610-1670de23-7695-411a-87f5-74db0c99185a.png)

The utility has the following input parameters: 

- **Source Instance** – Source instance is the default instance that is connected via XRMToolBox. 
- **Target Instance** – Target instance can be selected by clicking on the `Select Target Instance` button. 
- **Logs Path** – Directory where the system will save the `Log.txt` file. 
- **FetchXml Table** – Table to add FetchXml either by typing manually or selecting a file. 

For the lookup fields, the user can specify the search by name instead of searching by id. For that, it’s needed to add the `SearchByPrimaryField="true"` attribute in the FetchXml `attribute` tag. 

For Example
```xml
<attribute SearchByPrimaryField="true" name="transactioncurrencyid" />
```

> ***Note*** **Link entities can be used only for filtering or sorting purposes. For field values migration each FetchXml grid line should represent only one entity.** 
