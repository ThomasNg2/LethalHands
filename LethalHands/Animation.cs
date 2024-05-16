using EmotesAPI;
using LethalEmotesAPI.ImportV2;
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
                displayName = " ",
                internalName = "squareup",
                rootBonesToIgnore = ignoredRootBones,
                soloBonesToIgnore = ignoredSoloBones,
                useLocalTransforms = true
            };
            CustomEmoteParams squareDownParams = new CustomEmoteParams
            {
                primaryAnimationClips = [Assets.Load<AnimationClip>("squaredown.anim")],
                visible = false,
                audioLevel = 0,
                audioLoops = false,
                allowJoining = false,
                thirdPerson = false,
                displayName = " ",
                internalName = "squaredown",
                rootBonesToIgnore = ignoredRootBones,
                soloBonesToIgnore = ignoredSoloBones,
                useLocalTransforms = true
            };
            EmoteImporter.ImportEmote(squareUpParams);
            EmoteImporter.ImportEmote(squareDownParams);
        }
    }
}
