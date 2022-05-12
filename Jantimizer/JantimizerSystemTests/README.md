# User secrets
For the system tests to be able to run, Postgres must be installed. 
It should just use the same secrets for Postgres that was set in the `ExperimentSuite` project.

```json
{
  "ConnectionProperties": {
    "POSGRESQL": {
      "Server": "localhost",
      "Port": 5432,
      "Username": "postgres",
      "Password": "password"
    }
  }
}
```
