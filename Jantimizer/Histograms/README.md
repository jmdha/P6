# Example of Histogram

(to be updated later)

```csharp
    await postgresModel.Connector.CallQuery(new FileInfo("../../../Tests/RandomNumberBlocks/cleanup.postgre.sql"));
    await postgresModel.Connector.CallQuery(new FileInfo("../../../Tests/RandomNumberBlocks/setup.postgre.sql"));
    var valuesToIndexQueryResult = await postgresModel.Connector.CallQuery("SELECT int4 FROM c");
    var valuesToIndexRows = valuesToIndexQueryResult.Tables[0].Rows;
    var valuesToIndex = new List<int>();
    for(int i=0; i< valuesToIndexRows.Count; i++)
        valuesToIndex.Add((int)valuesToIndexRows[i]["int4"]);

    int[] histogramValues = valuesToIndex.ToArray();
    var histogram = new HistogramEquiDepth(histogramValues, 20);
```
