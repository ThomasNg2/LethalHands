﻿using EmotesAPI;
using GameNetcodeStuff;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LethalHands
{
    public class LethalHands : MonoBehaviour
    {
        public static LethalHands Instance { get; private set; }
        public bool isSquaredUp = false;
        static int shovelMask = 11012424;
        public PlayerControllerB playerControllerInstance = GameNetworkManager.Instance.localPlayerController;
        static readonly string[] controlTips = { "Left Punch : [Left Click]", "Right Punch : [Right Click]" };
        AudioClip[] hitSounds = [Assets.Load<AudioClip>("hit1.ogg"), Assets.Load<AudioClip>("hit2.ogg")];
        public float recordedStamina = 1f;
        public bool freezeStaminaRegen = false;


        Coroutine punchingCoroutine = null;
        Coroutine unfreezeStaminaRegenCoroutine = null;

        float punchRange = NetworkConfig.Default.punchRange; // Shovel : 1.5, Knife : 0.75
        float punchDelay = NetworkConfig.Default.punchCooldown;
        public float enemyPunchDamage = NetworkConfig.Default.enemyPunchDamage;
        public int playerPunchDamage = NetworkConfig.Default.playerPunchDamage;

        int punchOffClingersChance = NetworkConfig.Default.punchOffClingersChance;

        float staminaDrain = NetworkConfig.Default.staminaDrain * 0.01f;
        public bool punchingHaltsStaminaRegen = NetworkConfig.Default.punchingHaltsStaminaRegen;
        float punchStaminaRequirement = NetworkConfig.Default.punchStaminaRequirement * 0.01f;

        ItemMode itemMode = NetworkConfig.Default.itemDropMode;
        public bool allowItems = NetworkConfig.Default.allowItems;

        float punchConnectTime = Mathf.Min(0.1f * NetworkConfig.Default.punchCooldown, 0.1f);
        float punchFinishingTime = Mathf.Min(0.9f * NetworkConfig.Default.punchCooldown, 0.9f);
        
        public void Awake()
        {
            if (Instance != null) return;
            Instance = this;
            Input.SquareUpInput.Instance.SquareUpKey.performed += SquareUpPerformed;
        }

        public void LoadConfigValues()
        {
            punchRange = NetworkConfig.Instance.punchRange;
            punchDelay = NetworkConfig.Instance.punchCooldown * 4;
            enemyPunchDamage = NetworkConfig.Instance.enemyPunchDamage;
            playerPunchDamage = NetworkConfig.Instance.playerPunchDamage;

            punchOffClingersChance = NetworkConfig.Instance.punchOffClingersChance;

            staminaDrain = NetworkConfig.Instance.staminaDrain * 0.01f;
            punchingHaltsStaminaRegen = NetworkConfig.Instance.punchingHaltsStaminaRegen;
            punchStaminaRequirement = NetworkConfig.Instance.punchStaminaRequirement * 0.01f;

            itemMode = NetworkConfig.Instance.itemDropMode;
            allowItems = NetworkConfig.Instance.allowItems;

            punchConnectTime = Mathf.Min(0.2f * NetworkConfig.Instance.punchCooldown, 0.1f);
        }

        public void SquareUpPerformed(InputAction.CallbackContext context)
        {
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

        public void LeftPunchPerformed(InputAction.CallbackContext context)
        {
            if (context.performed && isSquaredUp &&
                playerControllerInstance.sprintMeter >= punchStaminaRequirement &&
                punchingCoroutine == null)
            {
                punchingCoroutine = StartCoroutine(LeftPunch());
            }
        }

        public void RightPunchPerformed(InputAction.CallbackContext context)
        {
            if (context.performed && isSquaredUp &&
                playerControllerInstance.sprintMeter >= punchStaminaRequirement &&
                punchingCoroutine == null)
            {
                punchingCoroutine = StartCoroutine(RightPunch());
            }
        }

        public void SquareUp()
        {
            if (!isSquaredUp && !playerControllerInstance.inSpecialInteractAnimation)
            {
                switch (itemMode)
                {
                    case ItemMode.All:
                        playerControllerInstance.DropAllHeldItemsAndSync();
                        break;
                    case ItemMode.MainSlots:
                        DropMainItems((int)playerControllerInstance.playerClientId);
                        LethalHandsNetworker.Instance.DropMainItemsServerRpc((int)playerControllerInstance.playerClientId);
                        break;
                    case ItemMode.Current:
                        if (playerControllerInstance.isHoldingObject) playerControllerInstance.DiscardHeldObject();
                        break;
                    case ItemMode.None:
                        break;
                }
                playerControllerInstance.performingEmote = false;
                playerControllerInstance.StopPerformingEmoteServerRpc();
                playerControllerInstance.timeSinceStartingEmote = 0f;
                isSquaredUp = true;
                CustomEmotesAPI.PlayAnimation("SlapitNow.LethalHands__squareup");
                IngamePlayerSettings.Instance.playerInput.actions.FindAction("ActivateItem").performed += LeftPunchPerformed;
                IngamePlayerSettings.Instance.playerInput.actions.FindAction("PingScan").performed += RightPunchPerformed;
                HUDManager.Instance.ClearControlTips();
                HUDManager.Instance.ChangeControlTipMultiple(controlTips);
                LethalHandsPlugin.Instance.manualLogSource.LogInfo("Squaring up " + isSquaredUp);
            }
        }

        public void SquareDown(bool animate)
        {
            if (isSquaredUp)
            {
                IngamePlayerSettings.Instance.playerInput.actions.FindAction("ActivateItem").performed -= LeftPunchPerformed;
                IngamePlayerSettings.Instance.playerInput.actions.FindAction("PingScan").performed -= RightPunchPerformed;
                if (animate) CustomEmotesAPI.PlayAnimation("SlapitNow.LethalHands__squaredown");
                else CustomEmotesAPI.PlayAnimation("none");
                HUDManager.Instance.ClearControlTips();
                isSquaredUp = false;
                LethalHandsPlugin.Instance.manualLogSource.LogInfo("Squaring down " + isSquaredUp);
            }
        }

        private IEnumerator LeftPunch()
        {
            LethalHandsPlugin.Instance.manualLogSource.LogInfo("Left Punch");
            playerControllerInstance.sprintMeter = Mathf.Max(playerControllerInstance.sprintMeter - staminaDrain, 0f);
            recordedStamina = playerControllerInstance.sprintMeter;
            freezeStaminaRegen = true;
            CustomEmotesAPI.PlayAnimation("SlapitNow.LethalHands__lpunch");
            yield return new WaitForSeconds(punchConnectTime);
            PunchThrow();
            yield return new WaitForSeconds(punchFinishingTime);
            punchingCoroutine = null;
            if(unfreezeStaminaRegenCoroutine != null) StopCoroutine(unfreezeStaminaRegenCoroutine);
            unfreezeStaminaRegenCoroutine = StartCoroutine(UnfreezeStaminaRegen());
        }

        private IEnumerator RightPunch()
        {
            LethalHandsPlugin.Instance.manualLogSource.LogInfo("Right Punch");
            playerControllerInstance.sprintMeter = Mathf.Max(playerControllerInstance.sprintMeter - staminaDrain, 0f);
            recordedStamina = playerControllerInstance.sprintMeter;
            freezeStaminaRegen = true;
            CustomEmotesAPI.PlayAnimation("SlapitNow.LethalHands__rpunch");
            yield return new WaitForSeconds(punchConnectTime);
            PunchThrow();
            yield return new WaitForSeconds(punchFinishingTime);
            punchingCoroutine = null;
            if(unfreezeStaminaRegenCoroutine != null) StopCoroutine(unfreezeStaminaRegenCoroutine);
            unfreezeStaminaRegenCoroutine = StartCoroutine(UnfreezeStaminaRegen());
        }

        private IEnumerator UnfreezeStaminaRegen()
        {
            yield return new WaitForSeconds(0.5f);
            freezeStaminaRegen = false;
        }

        // our shovel code
        public void PunchThrow()
        {
            RaycastHit[] objectsHitByPunch = Physics.SphereCastAll(playerControllerInstance.gameplayCamera.transform.position, 0.8f, playerControllerInstance.gameplayCamera.transform.forward, punchRange, shovelMask, QueryTriggerInteraction.Collide);
            List<RaycastHit> objectsHitByPunchList = objectsHitByPunch.OrderBy((RaycastHit raycast) => raycast.distance).ToList();

            bool hitSomething = false;  
            int hitTerrainIndex = -1;
            IHittable hittable;
            RaycastHit hitInfo;
            foreach (RaycastHit hit in objectsHitByPunchList)
            {
                if ((hit.transform.gameObject.layer == 8 || hit.transform.gameObject.layer == 11) && !hit.collider.isTrigger) // terrain
                {
                    hitSomething = true;
                    string hitObjectTag = hit.collider.gameObject.tag;
                    for (int j = 0; j < StartOfRound.Instance.footstepSurfaces.Length; j++)
                    {
                        if (StartOfRound.Instance.footstepSurfaces[j].surfaceTag == hitObjectTag)
                        {
                            hitTerrainIndex = j;
                            break;
                        }
                    }
                }
                else if (hit.transform.TryGetComponent<IHittable>(out hittable) &&
                    (hit.point == Vector3.zero || !Physics.Linecast(playerControllerInstance.gameplayCamera.transform.position, hit.point, out hitInfo, StartOfRound.Instance.collidersAndRoomMaskAndDefault, QueryTriggerInteraction.Ignore)))
                {
                    if (hit.transform == playerControllerInstance.transform) continue; // Stop hitting yourself, (unless TZP ???)
                    hitSomething = true;
                    Vector3 forward = playerControllerInstance.gameplayCamera.transform.forward;
                    bool hitClinger = punchOffClingersChance > 0 && Random.Range(0, 100) < punchOffClingersChance;
                    if (!hitClinger && hittable is BushWolfTongueCollider bushWolfTongueCollider)
                    {
                        if (bushWolfTongueCollider.bushWolfScript.draggingPlayer == playerControllerInstance) continue;
                    }
                    if (hittable is EnemyAICollisionDetect enemyAICollider)
                    {
                        EnemyAI enemyAI = enemyAICollider.mainScript;
                        if(enemyAI.isEnemyDead) continue;
                        if(!hitClinger && enemyAI is CentipedeAI snarefleaAI)
                        {
                            if (snarefleaAI.clingingToPlayer == playerControllerInstance) continue;
                        }
                        if(!hitClinger && enemyAI is FlowerSnakeEnemy tulipSnekAI)
                        {
                            if (tulipSnekAI.clingingToPlayer == playerControllerInstance) continue;
                        }
                        enemyAICollider.onlyCollideWhenGrounded = false; // magic flag that makes enemies not get hit otherwise
                    }
                    try
                    {
                        hittable.Hit(-22, forward, playerWhoHit: playerControllerInstance, playHitSFX: true);
                    } catch { }
                    break;
                }
            }
            if (hitSomething)
            {
                int randomIndex = UnityEngine.Random.Range(0, hitSounds.Length);
                LethalHandsNetworker.Instance.PunchHitSoundServerRpc((int)playerControllerInstance.playerClientId, randomIndex, hitTerrainIndex);
                RoundManager.PlayRandomClip(playerControllerInstance.movementAudio, hitSounds);
                RoundManager.Instance.PlayAudibleNoise(playerControllerInstance.transform.position, 10f, 0.5f);
            }
        }

        public void PunchHitSound(int playerID, int soundIndex, int terrainIndex)
        {
            AudioSource sauce = StartOfRound.Instance.allPlayerScripts[playerID].movementAudio;
            sauce.PlayOneShot(hitSounds[soundIndex]);
            WalkieTalkie.TransmitOneShotAudio(sauce, hitSounds[soundIndex]);
            if(terrainIndex != -1)
            {
                sauce.PlayOneShot(StartOfRound.Instance.footstepSurfaces[terrainIndex].hitSurfaceSFX);
                WalkieTalkie.TransmitOneShotAudio(sauce, StartOfRound.Instance.footstepSurfaces[terrainIndex].hitSurfaceSFX);
            }
        }

        public void Sleep()
        {
            Input.SquareUpInput.Instance.SquareUpKey.performed -= SquareUpPerformed;
            if (isSquaredUp)
            {
                IngamePlayerSettings.Instance.playerInput.actions.FindAction("ActivateItem").performed -= LeftPunchPerformed;
                IngamePlayerSettings.Instance.playerInput.actions.FindAction("PingScan").performed -= RightPunchPerformed;
            }
            Instance = null;
        }

        // copy of the original DropAllHeldItems that's limited to only the 4 main slots for hotbar mods compatibility
        public void DropMainItems(int playerID)
        {
            PlayerControllerB p = StartOfRound.Instance.allPlayerScripts[playerID];
            for (int i = 0; i < 4; i++)
            {
                GrabbableObject grabbableObject = p.ItemSlots[i];
                if (!(grabbableObject != null))
                {
                    continue;
                }
                grabbableObject.parentObject = null;
                grabbableObject.heldByPlayerOnServer = false;
                if (p.isInElevator)
                {
                    grabbableObject.transform.SetParent(p.playersManager.elevatorTransform, worldPositionStays: true);
                }
                else
                {
                    grabbableObject.transform.SetParent(p.playersManager.propsContainer, worldPositionStays: true);
                }
                p.SetItemInElevator(p.isInHangarShipRoom, p.isInElevator, grabbableObject);
                grabbableObject.EnablePhysics(enable: true);
                grabbableObject.EnableItemMeshes(enable: true);
                grabbableObject.transform.localScale = grabbableObject.originalScale;
                grabbableObject.isHeld = false;
                grabbableObject.isPocketed = false;
                grabbableObject.startFallingPosition = grabbableObject.transform.parent.InverseTransformPoint(grabbableObject.transform.position);
                grabbableObject.FallToGround(randomizePosition: true);
                grabbableObject.fallTime = Random.Range(-0.3f, 0.05f);
                if (p.IsOwner)
                {
                    grabbableObject.DiscardItemOnClient();
                }
                else if (!grabbableObject.itemProperties.syncDiscardFunction)
                {
                    grabbableObject.playerHeldBy = null;
                }
                if (p.IsOwner)
                {
                    HUDManager.Instance.holdingTwoHandedItem.enabled = false;
                    HUDManager.Instance.itemSlotIcons[i].enabled = false;
                    HUDManager.Instance.ClearControlTips();
                    p.activatingItem = false;
                }
                p.ItemSlots[i] = null;
            }
            if (p.isHoldingObject && p.currentItemSlot < 4)
            {
                p.isHoldingObject = false;
                if (p.currentlyHeldObjectServer != null)
                {
                    p.GetType().GetMethod("SetSpecialGrabAnimationBool", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(p, new object[] { false, p.currentlyHeldObjectServer });
                }
                p.playerBodyAnimator.SetBool("cancelHolding", value: true);
                p.playerBodyAnimator.SetTrigger("Throw");
            }
            p.activatingItem = false;
            p.twoHanded = false;
            p.carryWeight = 1f;
            p.currentlyHeldObjectServer = null;
        }
    }
}
