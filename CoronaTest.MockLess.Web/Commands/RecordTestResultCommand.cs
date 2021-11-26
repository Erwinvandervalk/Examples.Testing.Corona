using System;
using CoronaTest.MockLess.Web.Persistence;
using MediatR;

namespace CoronaTest.MockLess.Web.Commands
{
    public class RecordTestResultCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public TestResult TestResult { get; set; }
    }
}