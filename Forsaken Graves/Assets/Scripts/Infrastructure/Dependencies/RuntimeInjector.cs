using Cysharp.Threading.Tasks;
using VContainer;

namespace ForsakenGraves.Infrastructure.Dependencies
{
    public class RuntimeInjector
    {
        private readonly IObjectResolver _resolver;

        public RuntimeInjector( IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public void Inject(object instance)
        {
            _resolver.Inject(instance);
        }
    }
}