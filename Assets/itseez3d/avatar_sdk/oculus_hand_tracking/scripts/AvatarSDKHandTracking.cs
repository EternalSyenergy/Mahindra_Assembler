/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, March 2022
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static OVRSkeleton;

namespace ItSeez3D.AvatarSdk.Oculus.HandTracking
{
	class BoneRotationMapper
	{
		private Transform target;

		List<BoneId> boneIds = new List<BoneId>();
		List<Matrix4x4> sourcesLocalToWorldMat = new List<Matrix4x4>();
		List<Matrix4x4> sourcesLocalRotationMat = new List<Matrix4x4>();

		Matrix4x4 targetWorldToLocalMat = Matrix4x4.identity;
		Quaternion rotationTargetInit = Quaternion.identity;

		public BoneRotationMapper(BoneId boneId, Transform source, Transform target)
		{
			this.target = target;

			boneIds.Add(boneId);
			sourcesLocalToWorldMat.Add(source.localToWorldMatrix);
			sourcesLocalRotationMat.Add(Matrix4x4.Rotate(source.localRotation));

			targetWorldToLocalMat = target.worldToLocalMatrix;
			rotationTargetInit = target.localRotation;
		}

		public void AddAdditionalSourceBone(BoneId boneId, Transform source)
		{
			boneIds.Add(boneId);
			sourcesLocalToWorldMat.Add(source.localToWorldMatrix);
			sourcesLocalRotationMat.Add(Matrix4x4.Rotate(source.localRotation));
		}

		public Quaternion GetTargetNodeRotation(OVRPlugin.Quatf[] rotations)
		{
			Quaternion resultRotation = rotationTargetInit;
			Matrix4x4 worldToLocal = targetWorldToLocalMat;

			for (int i = 0; i < boneIds.Count; i++)
			{
				BoneId boneId = boneIds[i];
				Quaternion sourceLocalRotation = rotations[(int)boneId].FromFlippedXQuatf();

				Matrix4x4 rotMat = sourcesLocalRotationMat[i].inverse * Matrix4x4.Rotate(sourceLocalRotation);
				if (rotMat.ValidTRS())
				{

					float angle = 0;
					Vector3 axis = Vector3.zero;
					rotMat.rotation.ToAngleAxis(out angle, out axis);

					// Thumb2 bone's rotation should be restricted for more smoothly mapping on the Oculus hand
					if (boneId == BoneId.Hand_Thumb2)
						angle *= 0.5f;

					Quaternion targetLocalRotation = Quaternion.AngleAxis(angle, worldToLocal * sourcesLocalToWorldMat[i] * axis);
					resultRotation = resultRotation * targetLocalRotation;
					worldToLocal = Matrix4x4.Rotate(targetLocalRotation).inverse * worldToLocal;
				}
			}
			return resultRotation;
		}

		public Quaternion GetInitialRotation()
		{
			return rotationTargetInit;
		}

		public Transform TargetTransform
		{
			get { return target; }
		}

		public BoneId SourceBoneId
		{
			get
			{
				if (boneIds.Count > 0)
					return boneIds[0];
				return BoneId.Invalid;
			}
		}
	}

	public class AvatarSDKHandTracking : MonoBehaviour
	{
		public SkeletonType skeletonType = SkeletonType.None;

		[HideInInspector]
		public BoneId[] availableBones = null;

		[SerializeField]
		public Transform oculusHandRoot;
		[SerializeField]
		public Transform avatarSdkHandRoot;

		[SerializeField]
		public Transform[] oculusHandsBones = null;
		[SerializeField]
		public Transform[] avatarSdkModelBones = null;

		private List<BoneRotationMapper> boneRotationMappers = new List<BoneRotationMapper>();

		private Matrix4x4 oculusRootToWorldMat = Matrix4x4.identity;
		private Matrix4x4 avatarSdkRootToWorldMat = Matrix4x4.identity;
		private Quaternion avatarSdkRootRotationInit = Quaternion.identity;

		private IOVRSkeletonDataProvider dataProvider;

		protected Animator animator;

		private readonly Dictionary<BoneId, HumanBodyBones> leftBoneIdToHumanBones = new Dictionary<BoneId, HumanBodyBones>()
		{
			{ BoneId.Hand_Thumb0, HumanBodyBones.LeftThumbProximal },
			{ BoneId.Hand_Thumb1, HumanBodyBones.LeftThumbProximal },
			{ BoneId.Hand_Thumb2, HumanBodyBones.LeftThumbIntermediate },
			{ BoneId.Hand_Thumb3, HumanBodyBones.LeftThumbDistal },
			{ BoneId.Hand_Index1, HumanBodyBones.LeftIndexProximal },
			{ BoneId.Hand_Index2, HumanBodyBones.LeftIndexIntermediate },
			{ BoneId.Hand_Index3, HumanBodyBones.LeftIndexDistal },
			{ BoneId.Hand_Middle1, HumanBodyBones.LeftMiddleProximal },
			{ BoneId.Hand_Middle2, HumanBodyBones.LeftMiddleIntermediate },
			{ BoneId.Hand_Middle3, HumanBodyBones.LeftMiddleDistal },
			{ BoneId.Hand_Ring1, HumanBodyBones.LeftRingProximal },
			{ BoneId.Hand_Ring2, HumanBodyBones.LeftRingIntermediate },
			{ BoneId.Hand_Ring3, HumanBodyBones.LeftRingDistal },
			{ BoneId.Hand_Pinky1, HumanBodyBones.LeftLittleProximal },
			{ BoneId.Hand_Pinky2, HumanBodyBones.LeftLittleIntermediate },
			{ BoneId.Hand_Pinky3, HumanBodyBones.LeftLittleDistal }
		};

		private readonly Dictionary<BoneId, HumanBodyBones> rightBoneIdToHumanBones = new Dictionary<BoneId, HumanBodyBones>()
		{
			{ BoneId.Hand_Thumb0, HumanBodyBones.RightThumbProximal },
			{ BoneId.Hand_Thumb1, HumanBodyBones.RightThumbProximal },
			{ BoneId.Hand_Thumb2, HumanBodyBones.RightThumbIntermediate },
			{ BoneId.Hand_Thumb3, HumanBodyBones.RightThumbDistal },
			{ BoneId.Hand_Index1, HumanBodyBones.RightIndexProximal },
			{ BoneId.Hand_Index2, HumanBodyBones.RightIndexIntermediate },
			{ BoneId.Hand_Index3, HumanBodyBones.RightIndexDistal },
			{ BoneId.Hand_Middle1, HumanBodyBones.RightMiddleProximal },
			{ BoneId.Hand_Middle2, HumanBodyBones.RightMiddleIntermediate },
			{ BoneId.Hand_Middle3, HumanBodyBones.RightMiddleDistal },
			{ BoneId.Hand_Ring1, HumanBodyBones.RightRingProximal },
			{ BoneId.Hand_Ring2, HumanBodyBones.RightRingIntermediate },
			{ BoneId.Hand_Ring3, HumanBodyBones.RightRingDistal },
			{ BoneId.Hand_Pinky1, HumanBodyBones.RightLittleProximal },
			{ BoneId.Hand_Pinky2, HumanBodyBones.RightLittleIntermediate },
			{ BoneId.Hand_Pinky3, HumanBodyBones.RightLittleDistal }
		};

		public AvatarSDKHandTracking()
		{
			availableBones = new BoneId[]
			{
				BoneId.Hand_Thumb0,
				BoneId.Hand_Thumb1,
				BoneId.Hand_Thumb2,
				BoneId.Hand_Thumb3,
				BoneId.Hand_Index1,
				BoneId.Hand_Index2,
				BoneId.Hand_Index3,
				BoneId.Hand_Middle1,
				BoneId.Hand_Middle2,
				BoneId.Hand_Middle3,
				BoneId.Hand_Ring1,
				BoneId.Hand_Ring2,
				BoneId.Hand_Ring3,
				BoneId.Hand_Pinky1,
				BoneId.Hand_Pinky2,
				BoneId.Hand_Pinky3,
			};

			oculusHandsBones = new Transform[availableBones.Length];
			avatarSdkModelBones = new Transform[availableBones.Length];
		}

		private readonly Quaternion wristFixupRotation = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);

		private void Awake()
		{
			if (dataProvider == null)
			{
				dataProvider = GetComponents<IOVRSkeletonDataProvider>().FirstOrDefault(p => p.GetSkeletonType() == skeletonType);
			}

			for (int i = 0; i < availableBones.Length; i++)
			{
				if (oculusHandsBones[i] != null && avatarSdkModelBones[i] != null)
				{
					BoneRotationMapper boneMapper = boneRotationMappers.FirstOrDefault(m => m.TargetTransform == avatarSdkModelBones[i]);
					if (boneMapper == null)
						boneRotationMappers.Add(new BoneRotationMapper(availableBones[i], oculusHandsBones[i], avatarSdkModelBones[i]));
					else
						boneMapper.AddAdditionalSourceBone(availableBones[i], oculusHandsBones[i]);
				}
			}

			oculusRootToWorldMat = oculusHandRoot.localToWorldMatrix;
			avatarSdkRootToWorldMat = avatarSdkHandRoot.localToWorldMatrix;
			avatarSdkRootRotationInit = avatarSdkHandRoot.rotation;
		}

		void Start()
		{
			animator = GetComponent<Animator>();
		}

		private void Update()
		{
			if (dataProvider != null && (animator == null || !animator.enabled))
			{
				var data = dataProvider.GetSkeletonPoseData();

				if (data.IsDataValid)
				{
					//SyncRootsRotation(data.RootPose.Orientation.FromFlippedZQuatf() * wristFixupRotation, data.RootPose.Position.FromFlippedZVector3f());

					var boneIdToHumanBonesMap = GetBoneIdToHumanBodyBonesMap();
					var boneRotations = data.BoneRotations;
					foreach (BoneRotationMapper rotationMapper in boneRotationMappers)
					{
						Quaternion rotation = rotationMapper.GetTargetNodeRotation(boneRotations);
						rotationMapper.TargetTransform.localRotation = rotation;
					}
				}
			}
		}

		void OnAnimatorIK()
		{
			if (animator != null && dataProvider != null)
			{
				var data = dataProvider.GetSkeletonPoseData();

				if (data.IsDataValid)
				{
					//SyncIKRootsRotation(data.RootPose.Orientation.FromFlippedZQuatf() * wristFixupRotation, data.RootPose.Position.FromFlippedZVector3f());

					var boneIdToHumanBonesMap = GetBoneIdToHumanBodyBonesMap();
					var boneRotations = data.BoneRotations;
					foreach (BoneRotationMapper rotationMapper in boneRotationMappers)
					{
						Quaternion rotation = rotationMapper.GetTargetNodeRotation(boneRotations);
						animator.SetBoneLocalRotation(boneIdToHumanBonesMap[rotationMapper.SourceBoneId], rotation);
					}
				}
			}
		}

		//private void SyncRootsRotation(Quaternion oculusRootRotation, Vector3 oculusRootPosition)
		//{
		//	Matrix4x4 oculusMat = oculusRootToWorldMat.inverse * Matrix4x4.TRS(oculusRootPosition, oculusRootRotation, Vector3.one);

		//	avatarSdkHandRoot.rotation = avatarSdkRootRotationInit;
		//	avatarSdkHandRoot.position = oculusRootPosition;

		//	if (oculusMat.ValidTRS())
		//	{
		//		float angle = 0;
		//		Vector3 axis = Vector3.zero;
		//		oculusMat.rotation.ToAngleAxis(out angle, out axis);
		//		avatarSdkHandRoot.Rotate(avatarSdkRootToWorldMat.inverse * oculusRootToWorldMat * axis, angle);
		//	}
		//}

		//private void SyncIKRootsRotation(Quaternion oculusRootRotation, Vector3 oculusRootPosition)
		//{
		//	if (skeletonType != SkeletonType.HandLeft && skeletonType != SkeletonType.HandRight)
		//		return;

		//	AvatarIKGoal ikHandGoal = skeletonType == SkeletonType.HandLeft ? AvatarIKGoal.LeftHand : AvatarIKGoal.RightHand;
		//	Quaternion ikGoalRot = Quaternion.Inverse(animator.rootRotation) * animator.GetIKRotation(ikHandGoal);
		//	Matrix4x4 ikGoalLocalToWorldMat = Matrix4x4.Rotate(ikGoalRot);

		//	animator.SetIKPositionWeight(ikHandGoal, 1);
		//	animator.SetIKRotationWeight(ikHandGoal, 1);
		//	animator.SetIKPosition(ikHandGoal, oculusRootPosition);

		//	Matrix4x4 rotMat = oculusRootToWorldMat.inverse * Matrix4x4.Rotate(oculusRootRotation);

		//	Quaternion rootRotation = ikGoalRot;
		//	if (rotMat.ValidTRS())
		//	{
		//		float angle = 0;
		//		Vector3 axis = Vector3.zero;
		//		rotMat.rotation.ToAngleAxis(out angle, out axis);

		//		Quaternion targetLocalRotation = Quaternion.AngleAxis(angle, ikGoalLocalToWorldMat.inverse * oculusRootToWorldMat * axis);
		//		rootRotation = rootRotation * targetLocalRotation;
		//	}
		//	animator.SetIKRotation(ikHandGoal, rootRotation);
		//}

		private Dictionary<BoneId, HumanBodyBones> GetBoneIdToHumanBodyBonesMap()
		{
			if (skeletonType == SkeletonType.HandLeft)
				return leftBoneIdToHumanBones;
			if (skeletonType == SkeletonType.HandRight)
				return rightBoneIdToHumanBones;
			return null;
		}
	}
}
