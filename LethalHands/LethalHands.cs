using EmotesAPI;
using GameNetcodeStuff;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace LethalHands
{
    public class LethalHands
    {
        public bool isSquaredUp = false;
        public float punchCooldown = 0f;
        static int shovelMask = 11012424;
        public PlayerControllerB playerControllerInstance;
        static readonly string[] controlTips = { "Punch : [LMB]", "Punch but right : [RMB]" };

        public void Awake()
        {
            Input.SquareUpInput.Instance.SquareUpKey.performed += SquareUpPerformed;
        }

        public void SquareUpPerformed(InputAction.CallbackContext context)
        {
            if (playerControllerInstance == null)
            {
                playerControllerInstance = GameNetworkManager.Instance.localPlayerController;
            }
            if (context.performed && !playerControllerInstance.quickMenuManager.isMenuOpen &&
                ((playerControllerInstance.IsOwner && playerControllerInstance.isPlayerControlled &&
                (!playerControllerInstance.IsServer || playerControllerInstance.isHostPlayerObject)) || playerControllerInstance.isTestingPlayer) &&
                !playerControllerInstance.inSpecialInteractAnimation && !playerControllerInstance.isTypingChat)
            {
                if(isSquaredUp)
                {
                    SquareDown();
                }
                else
                {
                    SquareUp();
                }
            }
        }

        public void PunchPerformed(InputAction.CallbackContext context)
        {
            if (context.performed && isSquaredUp && punchCooldown <= 0f)
            {
                Punch();
            }
        }

        public void PunchButRightPerformed(InputAction.CallbackContext context)
        {
            if (context.performed && isSquaredUp && punchCooldown <= 0f)
            {
                PunchButRight();
            }
        }

        public void SquareUp()
        {
            if (!isSquaredUp && !playerControllerInstance.inSpecialInteractAnimation)
            {
                playerControllerInstance.DropAllHeldItemsAndSync();
                playerControllerInstance.performingEmote = false;
                playerControllerInstance.StopPerformingEmoteServerRpc();
                playerControllerInstance.timeSinceStartingEmote = 0f;
                isSquaredUp = true;
                CustomEmotesAPI.PlayAnimation("SlapitNow.LethalHands__squaredupidle");
                IngamePlayerSettings.Instance.playerInput.actions.FindAction("ActivateItem").performed += PunchPerformed;
                IngamePlayerSettings.Instance.playerInput.actions.FindAction("PingScan").performed += PunchButRightPerformed;
                HUDManager.Instance.ClearControlTips();
                HUDManager.Instance.ChangeControlTipMultiple(controlTips);
                LethalHandsPlugin.Instance.manualLogSource.LogInfo("Squaring up " + isSquaredUp);
            }
        }

        public void SquareDown()
        {
            if (isSquaredUp)
            {
                IngamePlayerSettings.Instance.playerInput.actions.FindAction("ActivateItem").performed -= PunchPerformed;
                IngamePlayerSettings.Instance.playerInput.actions.FindAction("PingScan").performed -= PunchButRightPerformed;
                CustomEmotesAPI.PlayAnimation("none");
                HUDManager.Instance.ClearControlTips();
                isSquaredUp = false;
                LethalHandsPlugin.Instance.manualLogSource.LogInfo("Squaring down " + isSquaredUp);
            }
        }

        public void Punch()
        {
            LethalHandsPlugin.Instance.manualLogSource.LogInfo("Punching");
            // Play left punch animation
            PunchThrow();
        }

        public void PunchButRight()
        {
            LethalHandsPlugin.Instance.manualLogSource.LogInfo("Punching but right");
            // Play right punch 
            PunchThrow();
        }

        // our shovel code
        public void PunchThrow()
        {
            punchCooldown = 4f;
            playerControllerInstance.sprintMeter = Mathf.Max(0f, playerControllerInstance.sprintMeter - 0.1f);

            RaycastHit[] objectsHitByPunch = Physics.SphereCastAll(playerControllerInstance.gameplayCamera.transform.position, 0.8f, playerControllerInstance.gameplayCamera.transform.forward, 1.2f, shovelMask, QueryTriggerInteraction.Collide);
            List<RaycastHit> objectsHitByPunchList = objectsHitByPunch.OrderBy((RaycastHit raycast) => raycast.distance).ToList();

            bool hitSomething = false;
            bool hitHittable = false;
            int hitTerrainIndex = -1;
            IHittable hittable;
            RaycastHit hitInfo;
            foreach (RaycastHit hit in objectsHitByPunchList)
            {
                if (hit.transform.gameObject.layer == 8 || hit.transform.gameObject.layer == 11) // terrain
                {
                    hitSomething = true;
                    string hitObjectTag = hit.collider.gameObject.tag;
                    for (int j = 0; j < StartOfRound.Instance.footstepSurfaces.Length; j++)
                    {
                        if (StartOfRound.Instance.footstepSurfaces[j].surfaceTag == hitObjectTag)
                        {
                            LethalHandsPlugin.Instance.manualLogSource.LogInfo($"Hit {hitObjectTag}");
                            hitTerrainIndex = j;
                            break;
                        }
                    }
                }
                else if (hit.transform.TryGetComponent<IHittable>(out hittable) &&
                    (hit.point == Vector3.zero || !Physics.Linecast(playerControllerInstance.gameplayCamera.transform.position, hit.point, out hitInfo, StartOfRound.Instance.collidersAndRoomMaskAndDefault)))
                {
                    if (hit.transform == playerControllerInstance.transform) continue; // Stop hitting yourself, (unless TZP ???)
                    hitSomething = true;
                    Vector3 forward = playerControllerInstance.gameplayCamera.transform.forward;
                    if (hit.collider)
                    {
                        EnemyAICollisionDetect enemyCollision = hit.collider.GetComponent<EnemyAICollisionDetect>();
                        if (enemyCollision != null)
                        {
                            enemyCollision.onlyCollideWhenGrounded = false; // magic flag that makes enemies not get hit otherwise
                        }
                    }
                    try
                    {
                        hittable.Hit(1, forward, playerControllerInstance, playHitSFX: true);
                        hitHittable = true;
                    }
                    catch (Exception e)
                    {
                        LethalHandsPlugin.Instance.manualLogSource.LogError($"Exception when punching {e}");
                    }
                    break;
                }
            }
            if (hitSomething)
            {
                if (!hitHittable && hitTerrainIndex != -1)
                {

                    // Play audio
                    // RPC to play sounds
                }

            }
        }
    }
}
