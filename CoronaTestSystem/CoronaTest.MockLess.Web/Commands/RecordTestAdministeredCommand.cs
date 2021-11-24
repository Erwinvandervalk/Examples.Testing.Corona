using System;
using MediatR;

namespace CoronaTest.MockLess.Web.Commands
{
    public class RecordTestAdministeredCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public DateTimeOffset AdministeredOn { get; set; }
    }
}