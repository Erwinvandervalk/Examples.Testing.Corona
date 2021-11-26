using System;

namespace CoronaTest.MockLess.Web.Commands
{
    public class SuccessResponse : CommandResponse
    {
        public Guid Id { get; set; }
        public long Version { get; set; }
    }
}