# User secrets
This project uses local User Secrets for connection strings.
To add a secrets file to the project:
* Right click on the project file.
* Click "Manage User Secrets".
* Paste the following structure and insert your connection strings instead.

```json
{
  "ConnectionProperties": {
    "POSGRESQL": {
      "Server": "localhost",
      "Port": 5432,
      "Username": "postgres",
      "Password": "password"
    },
    "MYSQL": {
      "Server": "localhost",
      "Port": 3306,
      "Username": "root",
      "Password": "password"
    }
  }
}
```

# `experiments.json`
This file describes what tests to run from the `Tests/` folder, as well as what order.
The general structure of this file is:
```json
{
  "Experiments": [
    {
      "ExperimentName": "Experiment One",
      "RunParallel": false,
      "RunExperiment": true,
      "PreRunData": [
        {
          "ConnectorName": "POSGRESQL",
          "ConnectorID": "EquiDepth",
          "TestFiles": [
            ...
          ]
        }
      ],
      "RunData": [
        {
          "ConnectorName": "MYSQL",
          "ConnectorID": "EquiDepth",
          "TestFiles": [
            ...
          ]
        }
      ],
      "OptionalTestSettings": {
      }
    }
  ]
}
```

Where the "TestFiles" is an ordered array of what tests to run.
The "OptionalTestSettings" is there for tests that have some additional setting (such as bucket size).