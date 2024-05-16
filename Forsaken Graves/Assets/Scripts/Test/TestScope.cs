using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Test
{
    public class TestScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.UseEntryPoints(Lifetime.Singleton, entryPoints =>
                                                       {
                                                           entryPoints.Add<TestController>();
                                                           entryPoints.Add<TestSubscriber>();
                                                       });
            
            builder.Register<TestService>(Lifetime.Singleton);
            builder.RegisterComponentInHierarchy<TestView>();

        }
    }
}