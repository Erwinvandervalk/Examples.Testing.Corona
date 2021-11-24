using System;
using CoronaTest.Web.Persistence;
using MediatR;

namespace CoronaTest.Web.Commands
{
    public class RecordTestResultCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public TestResult TestResult { get; set; }
    }
}