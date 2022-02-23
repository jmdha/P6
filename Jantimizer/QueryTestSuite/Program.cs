using PostgresTestSuite.Connectors;
using System.Data;


// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");


var pgConn = new PostGres("Host=localhost;Username=postgres;Password=Aber1234;Database=JanBase");

var test = await pgConn.CallQuery("SELECT * FROM jan_scheme.jan_table");


foreach(DataRow row in test.Tables[0].Rows)
{
    Console.WriteLine($"{row["Id"]}, {row["Name"]}");

}

var count = await pgConn.GetCardinalityActual("SELECT * FROM jan_scheme.jan_table");

Console.WriteLine($"Row count: {count}");


var explain = await pgConn.CallQuery("EXPLAIN SELECT * FROM jan_scheme.jan_table");

string joinQuery = $"SELECT * FROM jan_scheme.jan_table JOIN jan_scheme.jano_table ON jan_table.\"Id\" = jano_table.\"Id\"";

var analysis = await pgConn.GetAnalysis(joinQuery);

Console.WriteLine(analysis.ToString());

Console.WriteLine(explain);
