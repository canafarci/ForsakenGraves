using ForsakenGraves.Identifiers;
using ForsakenGraves.Infrastructure.Data;
using JetBrains.Annotations;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Serialization;

namespace ForsakenGraves.Gameplay.Character
{
    public class ClientPlayerMove : NetworkBehaviour
    {
        [SerializeField] private AnticipatedNetworkTransform _anticipatedNetworkTransform;
        [SerializeField] private InputPoller _inputPoller;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private CapsuleCollider _capsuleCollider;

        private float _smoothTime = 0.1f;
        private float _smoothDistance = 3f;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) 
                enabled = false;
        }
        
        public void Move(InputFlags inputs, bool replay = false)
        {
            if ((inputs & InputFlags.Up) != 0)
            {
                Vector3 newPosition = transform.position + transform.right * (Time.fixedDeltaTime * 4);
                _anticipatedNetworkTransform.AnticipateMove(newPosition);
            }

            if ((inputs & InputFlags.Down) != 0)
            {
                Vector3 newPosition = transform.position - transform.right * (Time.fixedDeltaTime * 4);
                _anticipatedNetworkTransform.AnticipateMove(newPosition);
            }

            if ((inputs & InputFlags.Left) != 0)
            {
                transform.Rotate(Vector3.up, -180f * Time.fixedDeltaTime);
                _anticipatedNetworkTransform.AnticipateRotate(transform.rotation);
            }

            if ((inputs & InputFlags.Right) != 0)
            {
                transform.Rotate(Vector3.up, 180f * Time.fixedDeltaTime);
                _anticipatedNetworkTransform.AnticipateRotate(transform.rotation);
            }
        }

        public override void OnReanticipate(double lastRoundTripTime)
        {
            // Have to store the transform's previous state because calls to AnticipateMove() and
            // AnticipateRotate() will overwrite it.
            AnticipatedNetworkTransform.TransformState previousState = _anticipatedNetworkTransform.PreviousAnticipatedState;

            double authorityTime = NetworkManager.LocalTime.Time - lastRoundTripTime;
            // Here we re-anticipate the new position of the player based on the updated server position.
            // We do this by taking the current authoritative position and replaying every input we have received
            // since the reported authority time, re-applying all the movement we have applied since then
            // to arrive at a new anticipated player location.

            foreach (var item in _inputPoller.GetHistory())
            {
                if (item.Time <= authorityTime)
                {
                    continue;
                }

                Move(item.Item, true);
            }

            // Clear out all the input history before the given authority time. We don't need anything before that
            // anymore as we won't get any more updates from the server from before this one. We keep the current
            // authority time because theoretically another system may need that.
            _inputPoller.RemoveBefore(authorityTime);
            // It's not always desirable to smooth the transform. In cases of very large discrepencies in state,
            // it can sometimes be desirable to simply teleport to the new position. We use the SmoothDistance
            // value (and use SqrMagnitude instead of Distance for efficiency) as a threshold for teleportation.
            // This could also use other mechanisms of detection: For example, when the Telport input is included
            // in the replay set, we could set a flag to disable smoothing because we know we are teleporting.
            if (_smoothTime != 0.0)
            {
                var sqDist = Vector3.SqrMagnitude(previousState.Position - _anticipatedNetworkTransform.AnticipatedState.Position);
                if (sqDist <= 0.25 * 0.25)
                {
                    // This prevents small amounts of wobble from slight differences.
                    _anticipatedNetworkTransform.AnticipateState(previousState);
                }
                else if (sqDist < _smoothDistance * _smoothDistance)
                {
                    // Server updates are not necessarily smooth, so applying reanticipation can also result in
                    // hitchy, unsmooth animations. To compensate for that, we call this to smooth from the previous
                    // anticipated state (stored in "anticipatedValue") to the new state (which, because we have used
                    // the "Move" method that updates the anticipated state of the transform, is now the current
                    // transform anticipated state)
                    _anticipatedNetworkTransform.Smooth(previousState,
                                                        _anticipatedNetworkTransform.AnticipatedState,
                                                        _smoothTime);
                }
            }
        }

        [Rpc(SendTo.Server)]
        private void ServerMoveRpc(InputFlags inputs)
        {
            var currentPosition = _anticipatedNetworkTransform.AnticipatedState;
            // Calling Anticipate functions on the authority sets the authority value, too, so we can
            // just reuse the same method here with no problem.
            Move(inputs);
            // Server can use Smoothing for interpolation purposes as well.
            _anticipatedNetworkTransform.Smooth(currentPosition, _anticipatedNetworkTransform.AuthoritativeState, _smoothTime);
        }

        // Input processing happens in FixedUpdate rather than Update because the frame rate of server and client
        // may not exactly match, and if that is the case, doing movement in Update based on Time.deltaTime could
        // result in significantly different calculations between the server and client, meaning greater opportunities
        // for desync. Performing updates in FixedUpdate does not guarantee no desync, but it makes the calculations
        // more consistent between the two. It also means that we don't have to worry about delta times when replaying
        // inputs when we get updates - we can assume a fixed amount of time for each input. Otherwise, we would have
        // to store the delta time of each input and replay using those delta times to get consistent results.
        private void FixedUpdate()
        {
            if (!NetworkManager.IsConnectedClient) return;
            
            if (!IsServer)
            {
                InputFlags input =  _inputPoller.GetInput();
                Move(input);
                ServerMoveRpc(input);
            }
        }
    }
}