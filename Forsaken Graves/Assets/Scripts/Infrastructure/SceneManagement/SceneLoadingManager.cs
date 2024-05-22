using Unity.Netcode;

namespace ForsakenGraves.Infrastructure.SceneManagement
{
    public class SceneLoadingManager : NetworkBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
        
        
    }
}