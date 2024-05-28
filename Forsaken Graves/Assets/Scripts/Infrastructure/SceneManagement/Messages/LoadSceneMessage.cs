using ForsakenGraves.Identifiers;

namespace ForsakenGraves.Infrastructure.SceneManagement.Messages
{
    public readonly struct LoadSceneMessage
    {
        private readonly SceneIdentifier _sceneID;
        private readonly bool _useNetworkManager;

        public LoadSceneMessage(SceneIdentifier sceneID, bool useNetworkManager)
        {
            _sceneID = sceneID;
            _useNetworkManager = useNetworkManager;
        }

        public SceneIdentifier SceneID => _sceneID;
        public bool UseNetworkManager => _useNetworkManager;
    }
}