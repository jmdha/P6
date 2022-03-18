using DatabaseConnector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models;

namespace DatabaseConnectorTests.Connectors
{
    [TestClass]
    public class MySqlConnectorTests
    {
        #region Constructor

        [TestMethod]
        [DataRow("connnnn","abc",1234,"aa","bb","cc","dd")]
        public void Can_Set_Constructor(string connectionString, string server, int port, string username, string password, string database, string schema)
        {
            // ARRANGE
            var properties = new ConnectionProperties(connectionString, server, port, username, password, database, schema);

            // ACT
            IDbConnector connector = new DatabaseConnector.Connectors.MySqlConnector(properties);

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
            IDbConnector connector = new DatabaseConnector.Connectors.MySqlConnector(properties);

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
            IDbConnector connector = new DatabaseConnector.Connectors.MySqlConnector(properties);

            // ACT
            var result = await connector.CallQuery("abc");
        }

        [TestMethod]
        [ExpectedException(typeof(MySqlException))]
        public async Task Cant_CallQuery_IfServerNotExist()
        {
            // ARRANGE
            var properties = new ConnectionProperties("Server=janhost;Port=3306;Uid=root;Pwd=password;Database=public", "", 1, "", "", "", "");
            IDbConnector connector = new DatabaseConnector.Connectors.MySqlConnector(properties);

            // ACT
            var result = await connector.CallQuery("abc");
        }

        #endregion

        #region StartServer

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public async Task Cant_StartServer_NotImplemented()
        {
            // ARRANGE
            IDbConnector connector = new DatabaseConnector.Connectors.MySqlConnector(new ConnectionProperties());

            // ACT
            var result = await connector.StartServer();
        }

        #endregion

        #region StopServer

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void Cant_StopServer_NotImplemented()
        {
            // ARRANGE
            IDbConnector connector = new DatabaseConnector.Connectors.MySqlConnector(new ConnectionProperties());

            // ACT
            var result = connector.StopServer();
        }
        #endregion
    }
}
