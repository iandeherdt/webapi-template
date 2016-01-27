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
using WebApiTemplate.ResourceAccess.Context;
using WebApiTemplate.ResourceAccess.Migrations;
using WebApiTemplate.ResourceAccess.Repositories;
using WebApiTemplate.ResourceAccess.Uow;
using Xunit;

namespace WebApiTemplate.IntegrationTests
{

    public class ServiceTest : IDisposable
    {
        private const string Authorization = "Authorization";
        private string _token = "";
        protected TestServer Server { get; private set; }

        public ServiceTest()
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
            Given();
        }

        public void Given()
        {

        }

        public void Dispose()
        {
            using (var uow = UnitOfWork.Start())
            {
                uow.Context.Database.Connection.Close();
                uow.Context.Database.Delete();
            }
            Server.Dispose();
        }

        public Task<HttpResponseMessage> CreateGetRequest(string request)
        {

            return Server.CreateRequest(request).AddHeader(Authorization, _token).GetAsync();
        }

        public Task<HttpResponseMessage> CreatePostRequest<T>(string request, T model)
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

        public Task<HttpResponseMessage> CreatePutRequest<T>(string request, T model)
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

        public Task<HttpResponseMessage> CreatePatchRequest<T>(string request, T model)
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

        public Task<HttpResponseMessage> CreateDeleteRequest(string request, int id)
        {
            return Server.CreateRequest(request).AddHeader(Authorization, _token)
                .And(x => x.Method = HttpMethod.Delete)
                .SendAsync("DELETE");
        }

        public T DeserializeJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public IRepository Repository { get { return Container.Resolve<IRepository>(); } }

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
                    Assert.True(false, result.StatusCode.ToString());
                }
            }
            return null;
        }

        #endregion

    }
}
