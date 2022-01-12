using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OuterWario
{
    public static class PatchMesh
    {
        public static GameObject warioPrefab;

        public static void ReplacePlayer()
        {
            Util.Log("Replacing player with Wario");

            if (warioPrefab == null)
            {
                var assetBundle = OuterWario.Instance.ModHelper.Assets.LoadBundle("wario");
                warioPrefab = assetBundle.LoadAsset<GameObject>("Assets/Wario/Wario.prefab");
            }

            var player = "player_mesh_noSuit:Traveller_HEA_Player";
            var playerSuit = "Traveller_Mesh_v01:Traveller_Geo";
            var playerPrefix = "Traveller_Rig_v01:Traveller_";
            foreach (var playerMesh in Util.FindObjectsWithName(player))
            {
                SwapSkeleton(playerMesh, playerPrefix, "_Jnt", warioPrefab, 40f, hearthianBoneMap, playerBoneOffsets, hearthianBoneRotation);
                playerMesh.transform.parent.localScale = new Vector3(0.1f, 0.075f, 0.1f);
            }
            foreach (var playerSuitMesh in Util.FindObjectsWithName(playerSuit))
            {
                SwapSkeleton(playerSuitMesh, playerPrefix, "_Jnt", warioPrefab, 40f, hearthianBoneMap, playerBoneOffsets, hearthianBoneRotation);
            }

            // Marshmallow stick
            GameObject.Find("Player_Body/RoastingSystem/Stick_Root/Stick_Pivot/Stick_Tip/Props_HEA_RoastingStick/RoastingStick_Arm").GetComponent<MeshRenderer>().enabled = false;
            GameObject.Find("Player_Body/RoastingSystem/Stick_Root/Stick_Pivot/Stick_Tip/Props_HEA_RoastingStick/RoastingStick_Arm_NoSuit").GetComponent<MeshRenderer>().enabled = false;
        }

        private static void SwapSkeleton(GameObject originalModel, string bonePrefix, string boneSuffix, GameObject prefab, float scale,
            Func<string, string> boneConversion, Func<string, Vector3> boneOffsets, Func<string, Quaternion> boneRotations)
        {
            SwapSkeleton(originalModel, bonePrefix, boneSuffix, prefab, (_) => scale, boneConversion, boneOffsets, boneRotations);
        }

        private static void SwapSkeleton(GameObject originalModel, string bonePrefix, string boneSuffix, GameObject prefab, Func<string, float> boneScale,
            Func<string, string> boneConversion, Func<string, Vector3> boneOffsets, Func<string, Quaternion> boneRotations)
        {
            var newModel = GameObject.Instantiate(prefab, originalModel.transform.parent.transform);
            newModel.transform.localPosition = Vector3.zero;
            newModel.transform.localScale = Vector3.one * 0.03f;
            newModel.SetActive(true);

            // Disappear existing mesh renderers
            foreach (var skinnedMeshRenderer in originalModel.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (!skinnedMeshRenderer.name.Contains("Props_HEA_Jetpack"))
                {
                    skinnedMeshRenderer.sharedMesh = null;

                    var owRenderer = skinnedMeshRenderer.gameObject.GetComponent<OWRenderer>();
                    if (owRenderer != null) owRenderer.enabled = false;

                    var streamingMeshHandle = skinnedMeshRenderer.gameObject.GetComponent<StreamingMeshHandle>();
                    if (streamingMeshHandle != null) GameObject.Destroy(streamingMeshHandle);
                }
            }

            var skinnedMeshRenderers = newModel.transform.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
            {
                var bones = skinnedMeshRenderer.bones;
                for (int i = 0; i < bones.Length; i++)
                {
                    var bone = bones[i];
                    var newBone = boneConversion(bone.name);
                    if (newBone == null) continue;

                    var newParent = Util.SearchInChildren(originalModel.transform.parent, bonePrefix + newBone + boneSuffix);

                    if (newParent == null) Util.LogError($"Couldn't find parent for {bone.name} in {skinnedMeshRenderer.name}");
                    else bone.parent = newParent;

                    bone.localScale = boneScale(bone.name) * Vector3.one;
                    bone.localPosition = boneScale(bone.name) * boneOffsets(bone.name);
                    bone.localRotation = boneRotations(bone.name);
                }

                skinnedMeshRenderer.rootBone = Util.SearchInChildren(originalModel.transform.parent, bonePrefix + boneConversion("rootBone") + boneSuffix);
                if (skinnedMeshRenderer.rootBone == null) Util.LogError($"Couldn't find root bone for {skinnedMeshRenderer.name}");
                skinnedMeshRenderer.quality = SkinQuality.Bone4;
                skinnedMeshRenderer.updateWhenOffscreen = true;

                // Reparent the skinnedMeshRenderer to the original object.
                skinnedMeshRenderer.transform.parent = originalModel.transform;
            }
            GameObject.Destroy(newModel);
        }

        private static string hearthianBoneMap(string name)
        {
            string newBone;
            if (name.EndsWith("rootBone")) newBone = "Trajectory";
            else if (name.Equals("Bip01")) newBone = "Trajectory";
            else if (name.EndsWith("mixamorig:Hips")) newBone = "ROOT";
            else if (name.Equals("mixamorig:Spine")) newBone = "Spine_01";
            else if (name.Equals("mixamorig:Spine2")) newBone = "Spine_02";
            else if (name.EndsWith("Neck")) newBone = "Neck_01";
            else if (name.EndsWith("Head")) newBone = "Neck_Top";
            else if (name.EndsWith("LeftUpLeg")) newBone = "LF_Leg_Hip";
            else if (name.EndsWith("LeftLeg")) newBone = "LF_Leg_Knee";
            else if (name.EndsWith("LeftFoot")) newBone = "LF_Leg_Ball";
            else if (name.EndsWith("RightUpLeg")) newBone = "RT_Leg_Hip";
            else if (name.EndsWith("RightLeg")) newBone = "RT_Leg_Knee";
            else if (name.EndsWith("RightFoot")) newBone = "RT_Leg_Ball";
            else if (name.EndsWith("LeftShoulder")) newBone = "LF_Arm_Clavicle";
            else if (name.EndsWith("LeftArm")) newBone = "LF_Arm_Shoulder";
            else if (name.EndsWith("LeftForeArm")) newBone = "LF_Arm_Elbow";
            else if (name.EndsWith("RightShoulder")) newBone = "RT_Arm_Clavicle";
            else if (name.EndsWith("RightArm")) newBone = "RT_Arm_Shoulder";
            else if (name.EndsWith("RightForeArm")) newBone = "RT_Arm_Elbow";
            else if (name.EndsWith("LeftHand")) newBone = "LF_Arm_Wrist";
            else if (name.EndsWith("RightHand")) newBone = "RT_Arm_Wrist";
            else newBone = null;
            return newBone;
        }

        private static Vector3 playerBoneOffsets(string name)
        {
            Vector3 offset;
            if (name.Contains("Head")) offset = new Vector3(1f, 0, 0);
            else if (name.Contains("LeftFoot")) offset = new Vector3(0, 0, 1.5f);
            else if (name.Contains("RightFoot")) offset = new Vector3(0, 0, -1.5f);
            else if (name.Contains("Spine2")) offset = new Vector3(-2, 0, 0);
            else if (name.EndsWith("Spine")) offset = new Vector3(-1, 0, 0);
            else if (name.Contains("Left") && (name.Contains("Arm") || name.Contains("Hand"))) offset = new Vector3(-4, 2, 0);
            else if (name.Contains("Right") && (name.Contains("Arm") || name.Contains("Hand"))) offset = new Vector3(4, -2, 0);
            else if (name.EndsWith("LeftUpLeg")) offset = new Vector3(-2, 0, 0);
            else if (name.EndsWith("RightUpLeg")) offset = new Vector3(2, 0, 0);
            else if (name.EndsWith("Hips")) offset = new Vector3(2, 0, 0);
            else offset = Vector3.zero;

            return offset / 40f;
        }

        private static Quaternion hearthianBoneRotation(string name)
        {
            Quaternion localRotation;
            if (name.Contains("RightFoot")) localRotation = Quaternion.Euler(345.4542f, 163.9411f, 82.7463f);
            else if (name.Contains("LeftFoot")) localRotation = Quaternion.Euler(20.6148f, 337.0829f, 110.59f);
            else if (name.Contains("Right")) localRotation = Quaternion.Euler(270, 270, 0);
            else localRotation = Quaternion.Euler(90, 270, 0);
            return localRotation;
        }
    }
}
