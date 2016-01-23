using System;
using System.Data.Entity;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using WebApiTemplate.ResourceAccess.Context;
using WebApiTemplate.ResourceAccess.Migrations;
using WebApiTemplate.ResourceAccess.Repositories;
using WebApiTemplate.ResourceAccess.Uow;

namespace WebApiTemplate.IntegrationTests
{
    public abstract class ServiceTest
    {
        private const string Authorization = "Authorization";
        private string _token = "";
        readonly string _baseUrl = "url";
        protected TestServer Server { get; private set; }

        protected virtual void PrepareTestData()
        { }

        protected abstract void Given();

        protected abstract void When();

        [OneTimeSetUp]
        public void BeforeTest()
        {
            //_token = GetToken(_baseUrl);
            Server = TestServer.Create(app =>
            {
                var startup = new Startup();
                startup.Configuration(app);
            });
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<EntityContext, Configuration>());
            Server.BaseAddress = new Uri("https://localhost/");
            Factory.RegisterInstance<IRequestState>(new PerThreadRequestState());
            PrepareTestData();
            Given();
            When();
        }

        [OneTimeTearDown]
        public void AfterTest()
        {
            using (var uow = UnitOfWork.Start())
            {
                uow.Context.Database.Connection.Close();
                uow.Context.Database.Delete();
            }
            Server.Dispose();
        }

        protected Task<HttpResponseMessage> CreateGetRequest(string request)
        {

            return Server.CreateRequest(request).AddHeader(Authorization, _token).GetAsync();
        }

        protected Task<HttpResponseMessage> CreatePostRequest<T>(string request, T model)
        {
            var rootFormatter = new JsonMediaTypeFormatter()
            {
                SerializerSettings =
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }
            };
            return Server.CreateRequest(request).AddHeader(Authorization, _token)
                .And(x => x.Content = new ObjectContent(typeof(T), model, rootFormatter))
                .And(x => x.Method = HttpMethod.Post)
                .PostAsync();
        }

        protected Task<HttpResponseMessage> CreatePutRequest<T>(string request, T model)
        {
            var rootFormatter = new JsonMediaTypeFormatter
            {
                SerializerSettings =
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }
            };
            return Server.CreateRequest(request).AddHeader(Authorization, _token)
                .And(x => x.Content = new ObjectContent(typeof(T), model, rootFormatter))
                .And(x => x.Method = HttpMethod.Put)
                .SendAsync("PUT");
        }

        protected Task<HttpResponseMessage> CreatePatchRequest<T>(string request, T model)
        {
            var rootFormatter = new JsonMediaTypeFormatter 
            {
                SerializerSettings =
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }
            };
            return Server.CreateRequest(request).AddHeader(Authorization, _token)
                .And(x => x.Content = new ObjectContent(typeof(T), model, rootFormatter))
                .And(x => x.Method = new HttpMethod("PATCH"))
                .SendAsync("PATCH");
        }

        protected Task<HttpResponseMessage> CreateDeleteRequest(string request, int id)
        {
            return Server.CreateRequest(request).AddHeader(Authorization, _token)
                .And(x => x.Method = HttpMethod.Delete)
                .SendAsync("DELETE");
        }

        protected T DeserializeJsonWithRootObject<T>(string json)
        {
            var jsonWithoutRoot = JObject.Parse(json).First.First.ToString();
            return JsonConvert.DeserializeObject<T>(jsonWithoutRoot);
        }

        protected IRepository Repository { get { return Container.Resolve<IRepository>(); } }

        #region Authentication


        private string GetToken(string resourceUri)
        {
            string baseUrl = "url";
            var data = new
            {
                CredentialsRequest = new CredentialsRequest()
                {
                    ClientId = "WebApi",
                    ResourceUri = resourceUri.ToLower(),
                    UserCredentials = new Credentials()
                    {
                        UserName = "myusername",
                        Password = "mypassword"
                    }
                }
            };

            var postData = JsonConvert.SerializeObject(data);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var result = client.PostAsync("token/credentials", new StringContent(postData, Encoding.UTF8, "application/json")).Result;

                if (result.IsSuccessStatusCode)
                {
                    var token = JsonConvert.DeserializeObject<RootObjectToken>(result.Content.ReadAsStringAsync().Result);
                    return token.tokenResponse.token.value;
                }
                else
                {
                    Assert.Fail(result.StatusCode.ToString());
                }
            }
            return null;
        }

        #endregion

    }
}
