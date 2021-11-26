using System;

namespace CoronaTest.MockLess.Tests.testinfra
{
    public static class ObjectExtensions
    {
        public static T With<T>(this T subject, Action<T> action)
        {
            action(subject);
            return subject;
        }
    }
}