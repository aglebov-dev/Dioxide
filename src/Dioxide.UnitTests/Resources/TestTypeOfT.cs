namespace Dioxide.UnitTests.Resources
{
    public interface ITestTypeOf1 { }
    public interface ITestTypeOf2 { }
    public interface ITestTypeOf3 { }

    public class TestTypeOfT<T> : ITestTypeOf1, ITestTypeOf2, ITestTypeOf3 { }
}
