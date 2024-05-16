using ForsakenGraves.Test;
using MessagePipe;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Scope.CrossScene
{
    public class CrossSceneScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            RegisterMessagePipe(builder);


            builder.RegisterComponentInHierarchy<CrossSceneView>();
            builder.Register<CrossSceneService>(Lifetime.Singleton);
            
            builder.UseEntryPoints(Lifetime.Singleton, entryPoints =>
                                                       {
                                                           entryPoints.Add<CrossScenePresenter>();
                                                       });
        }

        private static void RegisterMessagePipe(IContainerBuilder builder)
        {
            MessagePipeOptions options = builder.RegisterMessagePipe();
            builder.RegisterBuildCallback(c => GlobalMessagePipe.SetProvider(c.AsServiceProvider()));
        }
    }
}