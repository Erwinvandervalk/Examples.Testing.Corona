using System;
using System.Net.Http;
using System.Threading.Tasks;
using CoronaTest.MockLess.Web;
using CoronaTest.MockLess.Web.Commands;
using CoronaTest.MockLess.Web.Persistence;
using ITG.Brix.Integration.Logging;
using ITG.Brix.Integration.Testing.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace CoronaTest.MockLess.Tests.testinfra
{
    public class ApiTestFixture
    {
        private const string InMemoryConnectionString = "DataSource=:memory:";
        private readonly IHostBuilder _hostBuilder;
        private readonly ITestOutputHelper _output;
        private readonly StaticTestData _the;
        private IHost _host;

        public ApiTestFixture(ITestOutputHelper output, StaticTestData the)
        {
            _output = output;
            _the = the;

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
                                    s.AddSingleton<IDateTimeProvider>(new ManualDateTimeProvider(the));
                                    s.AddSingleton<IGuidGenerator>(new DeterministicGuidGenerator(the));
                                    s.AddSingleton<IEmailSender, FakeEmailSender>();

                                    s.AddDbContext<CoronaDbContext>(opt =>
                                        {
                                            opt.UseSqlite(_connection);
                                        });
                                    s.AddTransient<IStartupFilter, RequestLoggingStartupFilter>();
                                })
                            .UseStartup<Startup>();
                    });
        }

        public FakeEmailSender FakeEmailSender => _host.Services.GetService<IEmailSender>() as FakeEmailSender;

        public TestServer Server { get; set; }
        public TestLoggerProvider LoggerProvider { get; set; }

        public async Task StartAsync()
        {
            _host = await _hostBuilder.StartAsync();
            Server = _host.GetTestServer();
        }

        public CoronaTestClient BuildCoronaTestClient() => new CoronaTestClient(BuildClient());

        public async ValueTask DisposeAsync()
        {
            _host.Dispose();
        }

        public HttpClient BuildClient() => new HttpClient(Server.CreateHandler())
        {
            BaseAddress = new Uri("http://not-used/")
        };
    }
}