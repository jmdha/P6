# Result Sentinels
To verify that results dont change between test runs and connectors, a set of "sentinels" have been made.
These consists of a list that saves results. This is saved as a combination of `Experiment Name`, `Case Name` and `Connector Name`.
Next time those 3 parameters are the same for a result, its `GetHashCode()` is compared with the `GetHashCode()` from the saved one.
If the two are not the same, it is put in a log with some printing of the differences.

All in all there are 2 sentinels.
* **Estimator Result Sentinel**
  * Checks if the estimator gave the same results.
* **Query Plan Parser Result Sentinel**
  * Checks if the parsed query plan is the same as before.