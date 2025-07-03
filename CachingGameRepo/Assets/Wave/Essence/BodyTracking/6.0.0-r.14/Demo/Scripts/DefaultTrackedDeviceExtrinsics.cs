// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;

namespace Wave.Essence.BodyTracking.Demo
{
	public class DefaultTrackedDeviceExtrinsics : MonoBehaviour
	{
		public Vector3 hipPosition = Vector3.zero;
		public Quaternion hipRotation = Quaternion.identity;

		public Vector3 leftWristPosition = new Vector3(0.0f, -0.035f, 0.043f);
		public Quaternion leftWristRotation = new Quaternion(0.0f, 0.707f, 0.0f, 0.707f);

		public Vector3 rightWristPosition = new Vector3(0.0f, -0.035f, 0.043f);
		public Quaternion rightWristRotation = new Quaternion(0.0f, -0.707f, 0.0f, 0.707f);

		public Vector3 headPosition = new Vector3(0.0f, -0.055f, -0.095f);
		public Quaternion headRotation = Quaternion.identity;

		public Vector3 leftAnklePosition = new Vector3(0.0f, -0.05f, 0.0f);
		public Quaternion leftAnkleRotation = new Quaternion(-0.5f, 0.5f, 0.5f, -0.5f);

		public Vector3 rightAnklePosition = new Vector3(0.0f, -0.05f, 0.0f);
		public Quaternion rightAnkleRotation = new Quaternion(0.5f, 0.5f, 0.5f, 0.5f);
	}
}
