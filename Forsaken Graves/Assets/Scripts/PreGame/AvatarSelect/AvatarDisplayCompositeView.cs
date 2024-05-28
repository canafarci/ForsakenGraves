using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForsakenGraves.PreGame.AvatarSelect
{
    public class AvatarDisplayCompositeView : MonoBehaviour
    {
        [SerializeField] private List<AvatarDisplayView> _avatarDisplayViews;
        [SerializeField] private AvatarDisplayView _localAvatarDisplayView;

        public List<AvatarDisplayView> AvatarDisplayViews => _avatarDisplayViews;
        public AvatarDisplayView LocalClientAvatarDisplayView => _localAvatarDisplayView;
    }
}