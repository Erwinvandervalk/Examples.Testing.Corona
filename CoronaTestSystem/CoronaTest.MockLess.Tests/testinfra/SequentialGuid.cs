using System;

namespace CoronaTest.MockLess.Tests.testinfra
{
    public struct SequentialGuid
    {
        private static readonly int[] s_orderMap = {15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0};
        private Guid _currentGuid;

        public SequentialGuid(Guid previousGuid)
        {
            _currentGuid = previousGuid;
        }

        public Guid CurrentGuid => _currentGuid;

        public SequentialGuid GetNext()
        {
            var bytes = _currentGuid.ToByteArray();
            for (var mapIndex = 0; mapIndex < 16; mapIndex++)
            {
                var bytesIndex = s_orderMap[mapIndex];
                bytes[bytesIndex]++;
                if (bytes[bytesIndex] != 0)
                {
                    break; // No need to increment more significant bytes
                }
            }

            return new SequentialGuid(new Guid(bytes));
        }

        public static implicit operator Guid(SequentialGuid guid)
        {
            return guid.CurrentGuid;
        }
    }
}