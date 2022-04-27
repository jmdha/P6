# Histograms

There will be an implementation for each of the histogram types and connector type. Histograms can then be made based on the setup SQL file given.

# Example Usage

```csharp
var histogramManager = new PostgresEquiHistogramManager("SomeConnectionString", 10);
await histogramManager.AddHistogram(SetupFile);
histogramManager.PrintAllHistograms();
```
