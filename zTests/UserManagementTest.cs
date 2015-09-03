using System.Linq;
using Hosts_Install;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace zTests
{
    [TestClass]
    public class UserManagementTest
    {
        [TestMethod]
        public void TestUserManagement()
        {
            UserManager manager = new UserManager(@"c:\temp");
            
            Assert.IsNotNull(manager, "UserManager is null");
        }

        [TestMethod]
        public void TestUser()
        {
            UserManager manager = new UserManager(@"c:\temp");
            var user = manager.Users.First(z => z.Name == "Alunos");

            var res = user.DownloadHosts();
            Assert.IsTrue(res);
        }
    }
}
