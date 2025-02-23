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
using UnityEngine;
using static OVRSkeleton;

namespace ItSeez3D.AvatarSdk.Oculus.HandTracking
{
	public static class AvatarSDKHandTrackingExtensions
	{
		public static Transform GetOculusHandBone(this AvatarSDKHandTracking handTracking, BoneId boneId)
		{
			for (int i = 0; i < handTracking.availableBones.Length; i++)
			{
				if (handTracking.availableBones[i] == boneId)
					return handTracking.oculusHandsBones[i];
			}
			return null;
		}

		public static Transform GetAvatarSDKHandBone(this AvatarSDKHandTracking handTracking, BoneId boneId)
		{
			for (int i = 0; i < handTracking.availableBones.Length; i++)
			{
				if (handTracking.availableBones[i] == boneId)
					return handTracking.avatarSdkModelBones[i];
			}
			return null;
		}

		public static void RotateTransform(this Transform target, Vector3 fromDirection, Vector3 toDirection)
		{
			Quaternion q = Quaternion.FromToRotation(fromDirection, toDirection);
			Vector3 axis;
			float angle;
			q.ToAngleAxis(out angle, out axis);
			target.Rotate(target.worldToLocalMatrix * axis, angle);
		}

		public static void RotateTransform(this Transform target, Vector3 fromPoint, Vector3 toPoint, Vector3 rotateAroundLineStart, Vector3 rotateAroundLineEnd)
		{
			Vector3 rotateAroundLine = rotateAroundLineEnd - rotateAroundLineStart;
			Vector3 fromPointNorm = GetNormalFromPointToLine(rotateAroundLine, fromPoint - rotateAroundLineStart);
			Vector3 toPointNorm = GetNormalFromPointToLine(rotateAroundLine, toPoint - rotateAroundLineStart);

			Quaternion q = Quaternion.FromToRotation(fromPointNorm, toPointNorm);
			Vector3 axis;
			float angle;
			q.ToAngleAxis(out angle, out axis);
			target.Rotate(target.worldToLocalMatrix * axis, angle);
		}

		public static Transform GetChildByName(this Transform root, string name)
		{
			foreach (var t in root.GetComponentsInChildren<Transform>(true))
				if (t.name == name)
					return t;
			return null;
		}

		private static Vector3 GetNormalFromPointToLine(Vector3 line, Vector3 point)
		{
			line = line.normalized;
			float dot = Vector3.Dot(point.normalized, line);
			return point - line * dot * point.magnitude;
		}
	}
}
