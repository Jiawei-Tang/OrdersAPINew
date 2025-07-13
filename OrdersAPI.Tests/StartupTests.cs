
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace OrdersAPI.Tests
{
    [TestClass]
    public class StartupTests
    {
        [TestMethod]
        public async Task Application_StartsWithoutExceptionAndApiExists()
        {
            var factory = new WebApplicationFactory<Program>();

            var client = factory.CreateClient();

            var response = await client.PostAsync("/api/orders", new StringContent("{}", System.Text.Encoding.UTF8, "application/json"));

            Assert.AreNotEqual(HttpStatusCode.NotFound, response.StatusCode, "Endpoint /api/orders not found.");
        }
    }
}