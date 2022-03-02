using QueryTestSuite.Connectors;
using QueryTestSuite.Models;
using System.Data;


// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");


var pgConn = new PostGres("Host=localhost;Username=postgres;Password=Aber1234;Database=JanBase");
var myConn = new MySql("Host=localhost;Username=Jantimizer;Password=Aber1234");

var connectorSet = new List<DbConnector>() { pgConn };


//var query_selectJanTable = new Query("SELECT * FROM jan_scheme.jan_table");


//var test = await pgConn.CallQuery("SELECT * FROM jan_scheme.jan_table");


string testBaseDirPath = Path.GetFullPath("../../../Tests");

foreach(DirectoryInfo testDir in new DirectoryInfo(testBaseDirPath).GetDirectories())
{
    TestSuite suite = new TestSuite(connectorSet);

    Console.WriteLine($"Running Collection: {testDir}");
    await suite.RunTests(testDir);
}





//foreach(DataRow row in test.Tables[0].Rows)
//{
//    Console.WriteLine($"{row["Id"]}, {row["Name"]}");

//}

//var count = await pgConn.GetCardinalityActual("SELECT * FROM jan_scheme.jan_table");

//Console.WriteLine($"Row count: {count}");


//var explain = await pgConn.CallQuery("EXPLAIN SELECT * FROM jan_scheme.jan_table");


//var query_janJoinJano = new Query(new Dictionary<Type, string>
//{
//    { typeof(MySql),    "SELECT * FROM jan_scheme.jan_table JOIN jan_scheme.jano_table ON jan_table.`Id` = jano_table.`Id`" },
//    { typeof(PostGres), "SELECT * FROM jan_scheme.jan_table JOIN jan_scheme.jano_table ON jan_table.\"Id\" = jano_table.\"Id\"" }
//});


////string joinQuery = $"SELECT * FROM jan_scheme.jan_table JOIN jan_scheme.jano_table ON jan_table.\"Id\" = jano_table.\"Id\"";

//var analysis = await pgConn.GetAnalysis(query_janJoinJano);

//Console.WriteLine(analysis.ToString());

//Console.WriteLine(explain);
