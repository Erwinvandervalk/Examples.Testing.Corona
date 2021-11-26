using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CoronaTest.MockLess.Web.Commands;
using CoronaTest.MockLess.Web.Controllers;
using CoronaTest.MockLess.Web.Queries;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Shouldly;

namespace CoronaTest.MockLess.Tests.testinfra
{
    public class CoronaTestClient
    {
        private readonly HttpClient _client;

        public CoronaTestClient(HttpClient client)
        {
            _client = client;
        }


        public async Task<HttpResponseMessage> ScheduleTest(ScheduleTestRequest request,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK)
        {
            var queryString = GetDefaultQueryString();
            var result = await _client.PostAsync("/CoronaTest" + queryString, request.ToStringContent());

            result.StatusCode.ShouldBe(expectedStatusCode);

            return result;
        }

        public async Task<(HttpResponseMessage response, GetCoronaTestQuery.Response result)> GetTest(Guid id,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK)
        {
            var queryString = GetDefaultQueryString();
            var result = await _client.GetAsync("/CoronaTest/" + id + queryString);

            result.StatusCode.ShouldBe(expectedStatusCode);

            var readAsStringAsync = await result.Content.ReadAsStringAsync();
            var resultObj = JsonConvert.DeserializeObject<GetCoronaTestQuery.Response>(readAsStringAsync);
            return (result, resultObj);
        }

        private static QueryString GetDefaultQueryString()
        {
            var queryString = new QueryString();
            //return queryString.Add("ApiVersion", "2.0");
            return queryString;
        }
    }
}