using ForsakenGraves.Identifiers;

namespace ForsakenGraves.Infrastructure.SceneManagement.Signals
{
    public readonly struct LoadSceneSignal
    {
        private readonly SceneIdentifier _sceneID;
        private readonly bool _useNetworkManager;

        public LoadSceneSignal(SceneIdentifier sceneID, bool useNetworkManager)
        {
            _sceneID = sceneID;
            _useNetworkManager = useNetworkManager;
        }

        public SceneIdentifier SceneID => _sceneID;
        public bool UseNetworkManager => _useNetworkManager;
    }
}