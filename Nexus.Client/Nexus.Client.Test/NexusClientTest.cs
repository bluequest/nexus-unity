using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;

namespace Nexus.Client.Test
{
    [TestClass]
    public class NexusClientTest
    {
        private NexusTestSettings settings;
        private NexusClient client;
        private Mock<HttpMessageHandler> mockHttpHandler;
        private string ExpectedSecret = "foo";
        private string UnexpectedSecret = "bar";
        private const string NexusSharedSecretHeader = "x-shared-secret";
        
        private NexusCreators NonEmptyCreators = new NexusCreators(new NexusCreator[]
        {
            new NexusCreator("lorem", "1", "lorem1"), 
            new NexusCreator("ipsum", "2", "ipsum2"), 
            new NexusCreator("dolor", "3", "dolor3"), 
        });

        [TestInitialize]
        public void Setup()
        {
            this.settings = new NexusTestSettings();
            this.mockHttpHandler = new Mock<HttpMessageHandler>();
            HttpClient httpClient = new HttpClient(this.mockHttpHandler.Object);
            this.client = new NexusClient(this.settings, httpClient);
        }
        
        [TestMethod]
        public async Task GetCreatorsSettingsNullReturnsNotNull()
        {
            this.SetupGetCreators(HttpStatusCode.OK, NonEmptyCreators);

            HttpClient httpClient = new HttpClient(this.mockHttpHandler.Object);
            this.client = new NexusClient(null, httpClient);
            
            NexusCreators creators = await this.client.GetCreators();
            Assert.IsNotNull(creators);
            Assert.IsNotNull(creators.Creators);
            Assert.AreEqual(0,creators.Creators.Length);
        }
        
        [TestMethod]
        public async Task GetCreatorsNoSecretReturnsNotNull()
        {
            this.SetupGetCreators(HttpStatusCode.OK, NonEmptyCreators);
            
            NexusCreators creators = await this.client.GetCreators();
            Assert.IsNotNull(creators);
            Assert.IsNotNull(creators.Creators);
            Assert.AreEqual(0,creators.Creators.Length);
        }

        [TestMethod]
        public async Task GetCreatorsInvalidSecretReturnsNotNull()
        {
            this.SetupGetCreators(HttpStatusCode.OK, NonEmptyCreators);
            this.settings.SharedSecret = UnexpectedSecret;
            
            NexusCreators creators = await this.client.GetCreators();
            Assert.IsNotNull(creators);
            Assert.IsNotNull(creators.Creators);
            Assert.AreEqual(0,creators.Creators.Length);
        }

        [TestMethod]
        public async Task GetCreatorsValidSecretInvalidJsonResponseReturnsNotNull()
        {
            this.SetupGetCreators(HttpStatusCode.OK, "monkey wrench");
            this.settings.SharedSecret = ExpectedSecret;
            
            NexusCreators creators = await this.client.GetCreators();
            Assert.IsNotNull(creators);
            Assert.IsNotNull(creators.Creators);
            Assert.AreEqual(0,creators.Creators.Length);
        }

        [TestMethod]
        public async Task GetCreatorsValidSecretReturnsNotNullNotEmpty()
        {
            this.SetupGetCreators(HttpStatusCode.OK, NonEmptyCreators);
            this.settings.SharedSecret = ExpectedSecret;
            
            NexusCreators creators = await this.client.GetCreators();
            Assert.IsNotNull(creators);
            Assert.IsNotNull(creators.Creators);
            Assert.AreEqual(3,creators.Creators.Length);
        }
        
        [TestMethod]
        public async Task AttributeCreatorSettingsNullReturnsFalse()
        {
            this.SetupAttributeCreator(HttpStatusCode.OK, true);

            HttpClient httpClient = new HttpClient(this.mockHttpHandler.Object);
            this.client = new NexusClient(null, httpClient);
            
            bool result = await this.client.AttributeCreator();
            Assert.IsFalse(result);
        }
        
        [TestMethod]
        public async Task AttributeCreatorNoSecretReturnsFalse()
        {
            this.SetupAttributeCreator(HttpStatusCode.OK, true);
            
            bool result = await this.client.AttributeCreator();
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AttributeInvalidSecretReturnsFalse()
        {
            this.SetupAttributeCreator(HttpStatusCode.OK, true);
            this.settings.SharedSecret = UnexpectedSecret;
            
            bool result = await this.client.AttributeCreator();
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AttributeValidSecretInvalidStatusCodeReturnsFalse()
        {
            this.SetupAttributeCreator(HttpStatusCode.NotFound, "monkey wrench");
            this.settings.SharedSecret = ExpectedSecret;
            
            bool result = await this.client.AttributeCreator();
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AttributeValidSecretInvalidJsonResponseReturnsTrue()
        {
            // this test demonstrates that the server's response doesn't matter, at present
            this.SetupAttributeCreator(HttpStatusCode.OK, "monkey wrench");
            this.settings.SharedSecret = ExpectedSecret;
            
            bool result = await this.client.AttributeCreator();
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task AttributeValidSecretReturnsTrue()
        {
            this.SetupAttributeCreator(HttpStatusCode.OK, true);
            this.settings.SharedSecret = ExpectedSecret;
            
            bool result = await this.client.AttributeCreator();
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Mock HTTP-Get call:
        ///   * Return unauthorized if bad secret
        ///   * Return status, object if good secret
        /// </summary>
        private void SetupGetCreators(HttpStatusCode statusCode, object o)
        {
            // normally return Unauthorized response for all get requests
            HttpResponseMessage failResponse = new HttpResponseMessage();
            failResponse.StatusCode = HttpStatusCode.Unauthorized;
            failResponse.Content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
            
            this.mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(failResponse);
         
            // return given status code, serialized object for get requests with secret equal to `ExpectedSecret` 
            string json = JsonConvert.SerializeObject(o);   
            HttpResponseMessage successResponse = new HttpResponseMessage();
            successResponse.StatusCode = statusCode;
            successResponse.Content = new StringContent(json, Encoding.UTF8, "application/json"); 
            
            this.mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get && r.Headers.GetValues(NexusSharedSecretHeader).Contains(ExpectedSecret)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(successResponse);
        }

        /// <summary>
        /// Mock HTTP-Post call:
        ///   * Return unauthorized if bad secret
        ///   * Return status, object if good secret
        /// </summary>
        private void SetupAttributeCreator(HttpStatusCode statusCode, object o)
        {
            // normally return Unauthorized response for all get requests
            HttpResponseMessage failResponse = new HttpResponseMessage();
            failResponse.StatusCode = HttpStatusCode.Unauthorized;
            failResponse.Content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
            
            this.mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Post),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(failResponse);
         
            // return given status code, serialized object for get requests with secret equal to `ExpectedSecret` 
            string json = JsonConvert.SerializeObject(o);   
            HttpResponseMessage successResponse = new HttpResponseMessage();
            successResponse.StatusCode = statusCode;
            successResponse.Content = new StringContent(json, Encoding.UTF8, "application/json"); 
            
            this.mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Post && r.Headers.GetValues(NexusSharedSecretHeader).Contains(ExpectedSecret)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(successResponse);
        }
    }
}