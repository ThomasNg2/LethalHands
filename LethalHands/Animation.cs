﻿using LethalEmotesAPI.ImportV2;
using UnityEngine;

namespace LethalHands
{
    internal class Animation
    {
        static HumanBodyBones[] ignoredRootBones = [HumanBodyBones.LeftUpperLeg, HumanBodyBones.RightUpperLeg];
        static HumanBodyBones[] ignoredSoloBones = [HumanBodyBones.Head, HumanBodyBones.Neck, HumanBodyBones.Spine, HumanBodyBones.Hips];
        public static void instantiateAnimations()
        {
            CustomEmoteParams squareUpParams = new CustomEmoteParams
            {
                primaryAnimationClips = [Assets.Load<AnimationClip>("squareup.anim")],
                secondaryAnimationClips = [Assets.Load<AnimationClip>("squaredupidle.anim")],
                visible = false,
                audioLevel = 0,
                audioLoops = false,
                allowJoining = false,
                thirdPerson = false,
                forceCameraMode = true,
                allowThirdPerson = false,
                displayName = " ",
                internalName = "squareup",
                rootBonesToIgnore = ignoredRootBones,
                soloBonesToIgnore = ignoredSoloBones,
                useLocalTransforms = true,
                animateHealthbar = false
            };

            CustomEmoteParams squareDownParams = new CustomEmoteParams
            {
                primaryAnimationClips = [Assets.Load<AnimationClip>("squaredown.anim")],
                visible = false,
                audioLevel = 0,
                audioLoops = false,
                allowJoining = false,
                thirdPerson = false,
                forceCameraMode = true,
                allowThirdPerson = false,
                displayName = " ",
                internalName = "squaredown",
                rootBonesToIgnore = ignoredRootBones,
                soloBonesToIgnore = ignoredSoloBones,
                useLocalTransforms = true,
                animateHealthbar = false
            };

            CustomEmoteParams LPunchParams = new CustomEmoteParams
            {
                primaryAnimationClips = [Assets.Load<AnimationClip>("lpunch.anim")],
                secondaryAnimationClips = [Assets.Load<AnimationClip>("squaredupidle.anim")],
                primaryAudioClips = [Assets.Load<AudioClip>("swing1.ogg"), Assets.Load<AudioClip>("swing2.ogg")],
                visible = false,
                audioLevel = 0,
                audioLoops = false,
                allowJoining = false,
                thirdPerson = false,
                forceCameraMode = true,
                allowThirdPerson = false,
                displayName = " ",
                internalName = "lpunch",
                rootBonesToIgnore = ignoredRootBones,
                soloBonesToIgnore = ignoredSoloBones,
                useLocalTransforms = true,
                animateHealthbar = false    
            };

            CustomEmoteParams RPunchParams = new CustomEmoteParams
            {
                primaryAnimationClips = [Assets.Load<AnimationClip>("rpunch.anim")],
                secondaryAnimationClips = [Assets.Load<AnimationClip>("squaredupidle.anim")],
                primaryAudioClips = [Assets.Load<AudioClip>("swing1.ogg"), Assets.Load<AudioClip>("swing2.ogg")],
                visible = false,
                audioLevel = 0,
                audioLoops = false,
                allowJoining = false,
                thirdPerson = false,
                forceCameraMode = true,
                allowThirdPerson = false,
                displayName = " ",
                internalName = "rpunch",
                rootBonesToIgnore = ignoredRootBones,
                soloBonesToIgnore = ignoredSoloBones,
                useLocalTransforms = true,
                animateHealthbar = false
            };

            EmoteImporter.ImportEmote(squareUpParams);
            EmoteImporter.ImportEmote(squareDownParams);
            EmoteImporter.ImportEmote(LPunchParams);
            EmoteImporter.ImportEmote(RPunchParams);
        }
    }
}
