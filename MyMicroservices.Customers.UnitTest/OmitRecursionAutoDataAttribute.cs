using AutoFixture;
using AutoFixture.Xunit2;

namespace MyMicroservices.Customers.UnitTest
{
    public class OmitRecursionAutoDataAttribute : AutoDataAttribute
    {
        public OmitRecursionAutoDataAttribute() :
            base(() =>
            {
                var fixture = new Fixture();
                fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
                fixture.Behaviors.Add(new OmitOnRecursionBehavior());
                return fixture;
            })
        {
        }
    }
}
