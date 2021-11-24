using System;
using MediatR;

namespace CoronaTest.Web.Commands
{
    public class RecordTestResultCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public TestResult TestResult { get; set; }
    }
}