using AutoFixture;
using AutoFixture.AutoMoq;
using NUnit.Framework;
namespace System.Webapi.BasicToolRepo.UnitTests.TestUtility
{
    public class BaseTest
    {
        public IFixture _fixture;
        [SetUp]
        public void BaseSetUp()
        {
            _fixture = new Fixture() { RepeatCount = 1 }.Customize(new AutoMoqCustomization());
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }
    }
}
