﻿using JiraTools.Engine;
using JiraToolsTest.Container;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace JiraToolsTest
{
    [TestClass]
    public class UserTest
    {

        [TestMethod]
        public void GetUser()
        {
            var container = ContainerForTest.DefaultInstance.Value;

            var engine = container.Resolve<UserGetter>();


            //var users = engine.Execute("username = pierluigi.nanni@prometeia.com");
            //var users = engine.Execute("accountId = \"70121:67b933a3-5693-47d2-82c0-3f997f279387\"");
            //var users = engine.Execute("70121:67b933a3-5693-47d2-82c0-3f997f279387");
            var users = engine.Execute("67b933a3-5693-47d2-82c0-3f997f279387");


            //    "70121:67b933a3-5693-47d2-82c0-3f997f279387");

            Assert.IsNotNull(users);
        }

        [TestMethod]
        public void GetUsers()
        {
            var container = ContainerForTest.DefaultInstance.Value;
            
            var engine = container.Resolve<UserListGetter>();

            var users = engine.Execute("Administrators");

            foreach (var user in users)
                Assert.IsNotNull(user);

        }
    }
}
