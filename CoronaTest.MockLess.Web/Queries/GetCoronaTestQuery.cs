using System;
using CoronaTest.MockLess.Web.Persistence;
using MediatR;

namespace CoronaTest.MockLess.Web.Queries
{
    public class GetCoronaTestQuery : IRequest<GetCoronaTestQuery.Response>
    {
        public Guid Id { get; set; }

        public class Response
        {
            public Guid Id { get; set; }
            public string Location { get; set; }
            public DateTimeOffset CreatedOn { get; set; }
            public DateTimeOffset LastChanged { get; set; }
            public DateTimeOffset ScheduledOn { get; set; }
            public string TestSubjectIdentificatieNummer { get; set; }
            public string TestSubjectName { get; set; }
            public DateTimeOffset? AdministeredOn { get; set; }

            public TestStatus Status { get; set; }

            public DateTimeOffset? ResultsKnown { get; set; }
            public TestResult TestResult { get; set; }
            public long Version { get; set; }
        }
    }
}