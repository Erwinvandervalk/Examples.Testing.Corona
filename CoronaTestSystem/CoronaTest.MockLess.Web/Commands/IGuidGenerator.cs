using System;

namespace CoronaTest.MockLess.Web.Commands
{
    public interface IGuidGenerator
    {
        public Guid GetNext();
    }
}