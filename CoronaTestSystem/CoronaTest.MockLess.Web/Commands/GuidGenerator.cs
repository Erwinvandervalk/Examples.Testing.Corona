using System;

namespace CoronaTest.MockLess.Web.Commands
{
    public class GuidGenerator : IGuidGenerator
    {
        public Guid GetNext()
        {
            return Guid.NewGuid();
        }
    }
}