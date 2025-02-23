/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, March 2022
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using BoneId = OVRSkeleton.BoneId;

namespace ItSeez3D.AvatarSdk.Oculus.HandTracking
{
	[CustomEditor(typeof(AvatarSDKHandTracking))]
	public class AvatarSDKHandTrackingEditor : Editor
	{
		private Dictionary<BoneId, string> expectedOculusBonesNames = new Dictionary<BoneId, string>()
		{
			{ BoneId.Hand_Thumb0, "thumb0"},
			{ BoneId.Hand_Thumb1, "thumb1" },
			{ BoneId.Hand_Thumb2, "thumb2" },
			{ BoneId.Hand_Thumb3, "thumb3" },
			{ BoneId.Hand_Index1, "index1" },
			{ BoneId.Hand_Index2, "index2" },
			{ BoneId.Hand_Index3, "index3" },
			{ BoneId.Hand_Middle1, "middle1" },
			{ BoneId.Hand_Middle2, "middle2" },
			{ BoneId.Hand_Middle3, "middle3" },
			{ BoneId.Hand_Ring1, "ring1" },
			{ BoneId.Hand_Ring2, "ring2" },
			{ BoneId.Hand_Ring3, "ring3" },
			{ BoneId.Hand_Pinky1, "pinky1" },
			{ BoneId.Hand_Pinky2, "pinky2" },
			{ BoneId.Hand_Pinky3, "pinky3" }
		};

		private Dictionary<BoneId, string> expectedMetaPersonBonesNames = new Dictionary<BoneId, string>()
		{
			{ BoneId.Hand_Thumb0, "Thumb1"},
			{ BoneId.Hand_Thumb1, "Thumb1" },
			{ BoneId.Hand_Thumb2, "Thumb2" },
			{ BoneId.Hand_Thumb3, "Thumb3" },
			{ BoneId.Hand_Index1, "Index1" },
			{ BoneId.Hand_Index2, "Index2" },
			{ BoneId.Hand_Index3, "Index3" },
			{ BoneId.Hand_Middle1, "Middle1" },
			{ BoneId.Hand_Middle2, "Middle2" },
			{ BoneId.Hand_Middle3, "Middle3" },
			{ BoneId.Hand_Ring1, "Ring1" },
			{ BoneId.Hand_Ring2, "Ring2" },
			{ BoneId.Hand_Ring3, "Ring3" },
			{ BoneId.Hand_Pinky1, "Pinky1" },
			{ BoneId.Hand_Pinky2, "Pinky2" },
			{ BoneId.Hand_Pinky3, "Pinky3" }
		};

		private Dictionary<BoneId, string> expectedFitPersonBonesNames = new Dictionary<BoneId, string>()
		{
			{ BoneId.Hand_Thumb0, "thumb0"},
			{ BoneId.Hand_Thumb1, "thumb0" },
			{ BoneId.Hand_Thumb2, "thumb1" },
			{ BoneId.Hand_Thumb3, "thumb2" },
			{ BoneId.Hand_Index1, "index0" },
			{ BoneId.Hand_Index2, "index1" },
			{ BoneId.Hand_Index3, "index2" },
			{ BoneId.Hand_Middle1, "middle0" },
			{ BoneId.Hand_Middle2, "middle1" },
			{ BoneId.Hand_Middle3, "middle2" },
			{ BoneId.Hand_Ring1, "ring0" },
			{ BoneId.Hand_Ring2, "ring1" },
			{ BoneId.Hand_Ring3, "ring2" },
			{ BoneId.Hand_Pinky1, "pinky0" },
			{ BoneId.Hand_Pinky2, "pinky1" },
			{ BoneId.Hand_Pinky3, "pinky2" }
		};

		public override void OnInspectorGUI()
		{
			DrawPropertiesExcluding(serializedObject, new string[] { "oculusHandsBones", "avatarSdkModelBones" });
			serializedObject.ApplyModifiedProperties();

			AvatarSDKHandTracking handTracking = (AvatarSDKHandTracking)target;

			GUILayout.Space(10);

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Auto Map Bones", GUILayout.Width(230), GUILayout.Height(25)))
			{
				AutoMapBones(handTracking);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.Space(5);

			GUILayout.BeginHorizontal();
			GUILayout.Label("Bone Name", EditorStyles.boldLabel, GUILayout.Width(150));
			GUILayout.Label("Oculus", EditorStyles.boldLabel, GUILayout.Width(200));
			GUILayout.Label("Avatar SDK", EditorStyles.boldLabel, GUILayout.Width(200));
			GUILayout.EndHorizontal();

			for (int i = 0; i < handTracking.availableBones.Length; i++)
			{
				BoneId boneId = (BoneId)handTracking.availableBones[i];

				GUILayout.BeginHorizontal();
				string boneName = OVRSkeleton.BoneLabelFromBoneId(OVRSkeleton.SkeletonType.HandLeft, boneId);
				GUILayout.Label(boneName, GUILayout.Width(150));
				handTracking.oculusHandsBones[i] = (Transform)EditorGUILayout.ObjectField(handTracking.oculusHandsBones[i], typeof(Transform), true, GUILayout.Width(200));
				handTracking.avatarSdkModelBones[i] = (Transform)EditorGUILayout.ObjectField(handTracking.avatarSdkModelBones[i], typeof(Transform), true, GUILayout.Width(200));
				GUILayout.EndHorizontal();
			}

			GUILayout.Space(10);

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Align Bones", GUILayout.Width(230), GUILayout.Height(25)))
			{
				AlignBones(handTracking);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void AlignBones(AvatarSDKHandTracking handTracking)
		{
			if (handTracking.oculusHandRoot == null || handTracking.avatarSdkHandRoot == null)
				return;

			// align hands roots position
			handTracking.oculusHandRoot.position = handTracking.avatarSdkHandRoot.position;

			// Rotate Oculus hand Root to align with avatar sdk "Middle1 - Root" line
			Transform oculusMiddle1 = handTracking.GetOculusHandBone(BoneId.Hand_Middle1);
			Transform avatarSDKMiddle1 = handTracking.GetAvatarSDKHandBone(BoneId.Hand_Middle1);
			if (oculusMiddle1 != null && avatarSDKMiddle1 != null)
				handTracking.oculusHandRoot.RotateTransform(oculusMiddle1.position - handTracking.oculusHandRoot.position, avatarSDKMiddle1.position - handTracking.avatarSdkHandRoot.position);

			// Rotate Oculus Root around "Root - Middle1" line to align Pinky1
			Transform oculusPinky1 = handTracking.GetOculusHandBone(BoneId.Hand_Pinky1);
			Transform avatarSDKPinky1 = handTracking.GetAvatarSDKHandBone(BoneId.Hand_Pinky1);
			if (oculusPinky1 != null && avatarSDKPinky1 != null)
				handTracking.oculusHandRoot.RotateTransform(oculusPinky1.position, avatarSDKPinky1.position, handTracking.avatarSdkHandRoot.position, avatarSDKMiddle1.position);

			// Rotate Oculus Root around "Root - middle1" line to align Thumb1
			Transform oculusThumb1 = handTracking.GetOculusHandBone(BoneId.Hand_Thumb1);
			Transform avatarSDKThumb1 = handTracking.GetAvatarSDKHandBone(BoneId.Hand_Thumb1);
			if (oculusThumb1 != null && avatarSDKThumb1 != null)
				handTracking.oculusHandRoot.RotateTransform(oculusThumb1.position, avatarSDKThumb1.position, handTracking.avatarSdkHandRoot.position, avatarSDKMiddle1.position);

			// Align Avatar Pinky2 with Oculus "Pinky2 - Pinky1" line
			Transform oculusPinky2 = handTracking.GetOculusHandBone(BoneId.Hand_Pinky2);
			Transform avatarSDKPinky2 = handTracking.GetAvatarSDKHandBone(BoneId.Hand_Pinky2);
			if (oculusPinky2 != null && avatarSDKPinky2 != null && oculusPinky1 != null && avatarSDKPinky1 != null)
				avatarSDKPinky1.RotateTransform(avatarSDKPinky2.position - avatarSDKPinky1.position, oculusPinky2.position - oculusPinky1.position);

			// Align Avatar Pinky3 with Oculus "Pinky3 - Pinky2" line
			Transform oculusPinky3 = handTracking.GetOculusHandBone(BoneId.Hand_Pinky3);
			Transform avatarSDKPinky3 = handTracking.GetAvatarSDKHandBone(BoneId.Hand_Pinky3);
			if (oculusPinky2 != null && avatarSDKPinky2 != null && oculusPinky3 != null && avatarSDKPinky3 != null)
				avatarSDKPinky2.RotateTransform(avatarSDKPinky3.position - avatarSDKPinky2.position, oculusPinky3.position - oculusPinky2.position);

			// Align Avatar Ring1 with Oculus "Ring2 - Ring1" line 
			Transform oculusRing1 = handTracking.GetOculusHandBone(BoneId.Hand_Ring1);
			Transform avatarSDKRing1 = handTracking.GetAvatarSDKHandBone(BoneId.Hand_Ring1);
			Transform oculusRing2 = handTracking.GetOculusHandBone(BoneId.Hand_Ring2);
			Transform avatarSDKRing2 = handTracking.GetAvatarSDKHandBone(BoneId.Hand_Ring2);
			if (oculusRing1 != null && oculusRing2 != null && avatarSDKRing1 != null && avatarSDKRing2 != null)
				avatarSDKRing1.RotateTransform(avatarSDKRing2.position - avatarSDKRing1.position, oculusRing2.position - oculusRing1.position);

			// Align Avatar Ring2 with Oculus "Ring3 - Ring2" line 
			Transform oculusRing3 = handTracking.GetOculusHandBone(BoneId.Hand_Ring3);
			Transform avatarSDKRing3 = handTracking.GetAvatarSDKHandBone(BoneId.Hand_Ring3);
			if (oculusRing2 != null && oculusRing3 != null && avatarSDKRing2 != null && avatarSDKRing3 != null)
				avatarSDKRing2.RotateTransform(avatarSDKRing3.position - avatarSDKRing2.position, oculusRing3.position - oculusRing2.position);

			// Align Avatar Middle1 with Oculus "Middle2 - Middle1" line 
			Transform oculusMiddle2 = handTracking.GetOculusHandBone(BoneId.Hand_Middle2);
			Transform avatarSDKMiddle2 = handTracking.GetAvatarSDKHandBone(BoneId.Hand_Middle2);
			if (oculusMiddle1 != null && oculusMiddle2 != null && avatarSDKMiddle1 != null && avatarSDKMiddle2 != null)
				avatarSDKMiddle1.RotateTransform(avatarSDKMiddle2.position - avatarSDKMiddle1.position, oculusMiddle2.position - oculusMiddle1.position);

			// Align Avatar Middle2 with Oculus "Middle3 - Middle2" line 
			Transform oculusMiddle3 = handTracking.GetOculusHandBone(BoneId.Hand_Middle3);
			Transform avatarSDKMiddle3 = handTracking.GetAvatarSDKHandBone(BoneId.Hand_Middle3);
			if (oculusMiddle3 != null && oculusMiddle2 != null && avatarSDKMiddle3 != null && avatarSDKMiddle2 != null)
				avatarSDKMiddle2.RotateTransform(avatarSDKMiddle3.position - avatarSDKMiddle2.position, oculusMiddle3.position - oculusMiddle2.position);

			// Align Avatar Index1 with Oculus "Index2 - Index1" line 
			Transform oculusIndex1 = handTracking.GetOculusHandBone(BoneId.Hand_Index1);
			Transform avatarSDKIndex1 = handTracking.GetAvatarSDKHandBone(BoneId.Hand_Index1);
			Transform oculusIndex2 = handTracking.GetOculusHandBone(BoneId.Hand_Index2);
			Transform avatarSDKIndex2 = handTracking.GetAvatarSDKHandBone(BoneId.Hand_Index2);
			if (oculusIndex1 != null && oculusIndex2 != null && avatarSDKIndex1 != null && avatarSDKIndex2 != null)
				avatarSDKIndex1.RotateTransform(avatarSDKIndex2.position - avatarSDKIndex1.position, oculusIndex2.position - oculusIndex1.position);

			// Align Avatar Index2 with Oculus "Index3 - Index2" line 
			Transform oculusIndex3 = handTracking.GetOculusHandBone(BoneId.Hand_Index3);
			Transform avatarSDKIndex3 = handTracking.GetAvatarSDKHandBone(BoneId.Hand_Index3);
			if (oculusIndex3 != null && oculusIndex2 != null && avatarSDKIndex3 != null && avatarSDKIndex2 != null)
				avatarSDKIndex2.RotateTransform(avatarSDKIndex3.position - avatarSDKIndex2.position, oculusIndex3.position - oculusIndex2.position);

			// Align Avatar Thumb1 with Oculus "Thumb2 - Thumb1" line 
			Transform oculusThumb2 = handTracking.GetOculusHandBone(BoneId.Hand_Thumb2);
			Transform avatarSDKThumb2 = handTracking.GetAvatarSDKHandBone(BoneId.Hand_Thumb2);
			if (oculusThumb1 != null && oculusThumb2 != null && avatarSDKThumb1 != null && avatarSDKThumb2 != null)
				avatarSDKThumb1.RotateTransform(avatarSDKThumb2.position - avatarSDKThumb1.position, oculusThumb2.position - oculusThumb1.position);

			// Align Avatar Thumb2 with Oculus "Thumb3 - Thumb2" line 
			Transform oculusThumb3 = handTracking.GetOculusHandBone(BoneId.Hand_Thumb3);
			Transform avatarSDKThumb3 = handTracking.GetAvatarSDKHandBone(BoneId.Hand_Thumb3);
			if (oculusThumb3 != null && oculusThumb2 != null && avatarSDKThumb3 != null && avatarSDKThumb2 != null)
				avatarSDKThumb2.RotateTransform(avatarSDKThumb3.position - avatarSDKThumb2.position, oculusThumb3.position - oculusThumb2.position);
		}

		private void AutoMapBones(AvatarSDKHandTracking handTracking)
		{
			if (handTracking.skeletonType == OVRSkeleton.SkeletonType.None)
				return;

			string oculusPrefix = handTracking.skeletonType == OVRSkeleton.SkeletonType.HandLeft ? "b_l_" : "b_r_";
			string metaPersonPrefix = handTracking.skeletonType == OVRSkeleton.SkeletonType.HandLeft ? "LeftHand" : "RightHand";
			string fitPersonPrefix = handTracking.skeletonType == OVRSkeleton.SkeletonType.HandLeft ? "l" : "r";
			Transform oculusRoot = handTracking.oculusHandRoot != null ? handTracking.oculusHandRoot : handTracking.transform;
			Transform avatarRoot = handTracking.avatarSdkHandRoot != null ? handTracking.avatarSdkHandRoot : handTracking.transform;
			for (int i=0; i<handTracking.availableBones.Length; i++)
			{
				handTracking.oculusHandsBones[i] = oculusRoot.GetChildByName(oculusPrefix + expectedOculusBonesNames[handTracking.availableBones[i]]);
				handTracking.avatarSdkModelBones[i] = avatarRoot.GetChildByName(metaPersonPrefix + expectedMetaPersonBonesNames[handTracking.availableBones[i]]);
				if (handTracking.avatarSdkModelBones[i] == null)
					handTracking.avatarSdkModelBones[i] = avatarRoot.GetChildByName(fitPersonPrefix + expectedFitPersonBonesNames[handTracking.availableBones[i]]);
			}
		}

	}
}
