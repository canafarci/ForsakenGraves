using Unity.Netcode.Components;

namespace ForsakenGraves.Infrastructure.Netcode
{
    public class OwnerNetworkAnimator : NetworkAnimator
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}