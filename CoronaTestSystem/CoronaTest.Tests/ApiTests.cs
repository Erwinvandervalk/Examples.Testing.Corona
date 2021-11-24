using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using CoronaTest.Web;
using CoronaTest.Web.Persistence;
using ITG.Brix.Integration.Logging;
using ITG.Brix.Integration.Testing.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace CoronaTest.Tests
{
    public class ApiTests : IAsyncLifetime
    {
        private ApiTestFixture Fixture;
        private HttpClient Client;


        public ApiTests(ITestOutputHelper output)
        {
            Fixture = new ApiTestFixture(output);

            
        }

        [Fact]
        public async Task Can_get_home()
        {
            var result = await Client.GetAsync("/CoronaTest/" + Guid.NewGuid());
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        public async Task InitializeAsync()
        {
            await Fixture.StartAsync();
            Client = Fixture.BuildClient();
        }

        public async Task DisposeAsync()
        {
            await Fixture.DisposeAsync();
        }
    }

    public class ApiTestFixture
    {
        private readonly ITestOutputHelper _output;
        private readonly IHostBuilder _hostBuilder;
        private IHost _host;
        private const string InMemoryConnectionString = "DataSource=:memory:";
        public ApiTestFixture(ITestOutputHelper output)
        {
            _output = output;

            LoggerProvider = new TestLoggerProvider(output);
            var _connection = new SqliteConnection(InMemoryConnectionString);
            _connection.Open();
            var options = new DbContextOptionsBuilder<CoronaDbContext>()
                .UseSqlite(_connection)
                .Options;
            var DbContext = new CoronaDbContext(options);
            DbContext.Database.EnsureCreated();

            _hostBuilder = new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                    {
                        webBuilder.UseTestServer()
                            .ConfigureLogging(x =>
                                {
                                    x.SetMinimumLevel(LogLevel.Debug);
                                    x.AddProvider(LoggerProvider);
                                })
                            .ConfigureServices(s =>
                                {

                                    s.AddDbContext<CoronaDbContext>(opt =>
                                        {
                                            opt.UseSqlite(_connection);
                                        });
                                    s.AddTransient<IStartupFilter, RequestLoggingStartupFilter>();
                                })
                            .UseStartup<Startup>();

                    });

        }
        public async Task StartAsync()
        {
            _host = await _hostBuilder.StartAsync();
            Server = _host.GetTestServer();
        }

        public TestServer Server { get; set; }

        public async ValueTask DisposeAsync()
        {
            _host.Dispose();
        }
        public HttpClient BuildClient() => new HttpClient(Server.CreateHandler())
        {
            BaseAddress = new Uri("http://not-used/"),
        };
        public TestLoggerProvider LoggerProvider { get; set; }
    }
}
