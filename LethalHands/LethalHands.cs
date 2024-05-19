using EmotesAPI;
using GameNetcodeStuff;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LethalHands
{
    public class LethalHands
    {
        public static LethalHands Instance { get; private set; } 
        public bool isSquaredUp = false;
        public float punchCooldown = 0f;
        static int shovelMask = 11012424;
        public PlayerControllerB playerControllerInstance;
        static readonly string[] controlTips = { "Punch : [LMB]", "Punch but right : [RMB]" };
        AudioClip[] hitSounds = new AudioClip[2];

        const float PUNCH_DELAY = 4f;
        const float PUNCH_RANGE = 0.9f; // Shovel : 1.5, Knife : 0.75
        const float CHANCE_TO_DEAL_DMG_TO_MONSTER = 0.5f;


        public void Awake()
        {
            Instance = this;
            Input.SquareUpInput.Instance.SquareUpKey.performed += SquareUpPerformed;
            hitSounds[0] = Assets.Load<AudioClip>("hit1.ogg");
            hitSounds[1] = Assets.Load<AudioClip>("hit2.ogg");
        }

        public void SquareUpPerformed(InputAction.CallbackContext context)
        {
            if (playerControllerInstance == null)
            {
                if (GameNetworkManager.Instance.localPlayerController != null) {
                    playerControllerInstance = GameNetworkManager.Instance.localPlayerController;
                }
                else return;
            }
            if (context.performed && !playerControllerInstance.quickMenuManager.isMenuOpen &&
                ((playerControllerInstance.IsOwner && playerControllerInstance.isPlayerControlled &&
                (!playerControllerInstance.IsServer || playerControllerInstance.isHostPlayerObject)) || playerControllerInstance.isTestingPlayer) &&
                !playerControllerInstance.inSpecialInteractAnimation && !playerControllerInstance.isTypingChat)
            {
                if(isSquaredUp)
                {
                    SquareDown(true);
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
                CustomEmotesAPI.PlayAnimation("SlapitNow.LethalHands__squareup");
                IngamePlayerSettings.Instance.playerInput.actions.FindAction("ActivateItem").performed += PunchPerformed;
                IngamePlayerSettings.Instance.playerInput.actions.FindAction("PingScan").performed += PunchButRightPerformed;
                HUDManager.Instance.ClearControlTips();
                HUDManager.Instance.ChangeControlTipMultiple(controlTips);
                LethalHandsPlugin.Instance.manualLogSource.LogInfo("Squaring up " + isSquaredUp);
            }
        }

        public void SquareDown(bool animate)
        {
            if (isSquaredUp)
            {
                IngamePlayerSettings.Instance.playerInput.actions.FindAction("ActivateItem").performed -= PunchPerformed;
                IngamePlayerSettings.Instance.playerInput.actions.FindAction("PingScan").performed -= PunchButRightPerformed;
                if (animate) CustomEmotesAPI.PlayAnimation("SlapitNow.LethalHands__squaredown");
                else CustomEmotesAPI.PlayAnimation("none");
                HUDManager.Instance.ClearControlTips();
                isSquaredUp = false;
                LethalHandsPlugin.Instance.manualLogSource.LogInfo("Squaring down " + isSquaredUp);
            }
        }

        public void Punch()
        {
            LethalHandsPlugin.Instance.manualLogSource.LogInfo("Punching");
            CustomEmotesAPI.PlayAnimation("SlapitNow.LethalHands__lpunch");
            PunchThrow();
        }

        public void PunchButRight()
        {
            LethalHandsPlugin.Instance.manualLogSource.LogInfo("Punching but right");
            CustomEmotesAPI.PlayAnimation("SlapitNow.LethalHands__rpunch");
            PunchThrow();
        }

        // our shovel code
        public void PunchThrow()
        {
            punchCooldown = PUNCH_DELAY;
            playerControllerInstance.sprintMeter = Mathf.Max(0f, playerControllerInstance.sprintMeter - 0.1f);

            RaycastHit[] objectsHitByPunch = Physics.SphereCastAll(playerControllerInstance.gameplayCamera.transform.position, 0.8f, playerControllerInstance.gameplayCamera.transform.forward, PUNCH_RANGE, shovelMask, QueryTriggerInteraction.Collide);
            List<RaycastHit> objectsHitByPunchList = objectsHitByPunch.OrderBy((RaycastHit raycast) => raycast.distance).ToList();

            bool hitSomething = false;
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
                        int damage = UnityEngine.Random.Range(0f, 1f) < CHANCE_TO_DEAL_DMG_TO_MONSTER ? 1 : 0;
                        hittable.Hit(damage, forward, playerWhoHit: playerControllerInstance, playHitSFX: true);
                    } catch { }
                    break;
                }
            }
            if (hitSomething)
            {
                int randomIndex = UnityEngine.Random.Range(0, hitSounds.Length);
                LethalHandsNetworker.Instance.PunchHitSoundServerRpc((int)playerControllerInstance.playerClientId, randomIndex);
                RoundManager.PlayRandomClip(playerControllerInstance.movementAudio, hitSounds);
                UnityEngine.Object.FindObjectOfType<RoundManager>().PlayAudibleNoise(playerControllerInstance.transform.position, 10f, 0.5f);
            }
        }

        public void PunchHitSound(int playerID, int soundIndex)
        {
            try
            {
                AudioSource sauce = StartOfRound.Instance.allPlayerScripts[playerID].movementAudio;
                try
                {
                    sauce.PlayOneShot(hitSounds[soundIndex]);
                    try
                    {
                        WalkieTalkie.TransmitOneShotAudio(sauce, hitSounds[soundIndex]);
                    }
                    catch (System.Exception e) { LethalHandsPlugin.Instance.manualLogSource.LogInfo($"Error on line 3 {e}"); }
                }
                catch (System.Exception e) { LethalHandsPlugin.Instance.manualLogSource.LogInfo($"Error on line 2 {e}"); }
            }
            catch (System.Exception e) { LethalHandsPlugin.Instance.manualLogSource.LogInfo($"Error on line 1 {e}"); }
            LethalHandsPlugin.Instance.manualLogSource.LogInfo($"Punch hit sound {playerID} {soundIndex}");
        }
    }
}
