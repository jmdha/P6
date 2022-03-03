# User secrets
This project uses local User Secrets for connection strings.
To add a secrets file to the project:
* Right click on the project file
* Click "Manage User Secrets"
* Paste the following structure and insert your connection strings instead

```
{
  "ConnectionStrings": {
    "POSGRESQL": "connection string here",
    "MYSQL": "connection string here"
  }
}
```