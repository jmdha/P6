using DatabaseConnector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Tools.Models;

namespace DatabaseConnectorTests.Connectors
{
    [TestClass]
    public class PostgreSqlConnectorTests
    {
        #region Constructor

        [TestMethod]
        [DataRow("connnnn","abc",1234,"aa","bb","cc","dd")]
        public void Can_Set_Constructor(string connectionString, string server, int port, string username, string password, string database, string schema)
        {
            // ARRANGE
            var properties = new ConnectionProperties(connectionString, server, port, username, password, database, schema);

            // ACT
            IDbConnector connector = new DatabaseConnector.Connectors.PostgreSqlConnector(properties);

            // ASSERT
            Assert.AreEqual(properties, connector.ConnectionProperties);
        }

        #endregion

        #region CheckConnection

        [TestMethod]
        public void Cant_CheckConnection_IfNotPossible()
        {
            // ARRANGE
            var properties = new ConnectionProperties("SomeIncorrectConnectionString", "", 1, "", "", "", "");
            IDbConnector connector = new DatabaseConnector.Connectors.PostgreSqlConnector(properties);

            // ACT
            var result = connector.CheckConnection();

            // ASSERT
            Assert.IsFalse(result);
        }

        #endregion

        #region CallQuery

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task Cant_CallQuery_IfConnectionStringIncorrect()
        {
            // ARRANGE
            var properties = new ConnectionProperties("SomeIncorrectConnectionString", "", 1, "", "", "", "");
            IDbConnector connector = new DatabaseConnector.Connectors.PostgreSqlConnector(properties);

            // ACT
            var result = await connector.CallQuery("abc");
        }

        [TestMethod]
        [ExpectedException(typeof(SocketException))]
        public async Task Cant_CallQuery_IfServerNotExist()
        {
            // ARRANGE
            var properties = new ConnectionProperties("Host=janhost;Port=5432;Username=postgres;Password=password;Database=postgres;SearchPath=public", "", 1, "", "", "", "");
            IDbConnector connector = new DatabaseConnector.Connectors.PostgreSqlConnector(properties);

            // ACT
            var result = await connector.CallQuery("abc");
        }

        #endregion
    }
}
