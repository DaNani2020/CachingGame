// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

using Wave.OpenXR;
using Wave.Native;
using Wave.Essence.Tracker;
using System.Text;
using UnityEngine.XR;
using System.Linq;

namespace Wave.Essence.BodyTracking
{
	public enum BodyTrackingResult : Byte
	{
		SUCCESS = 0,
		ERROR_IK_NOT_UPDATED = 1,
		ERROR_INVALID_ARGUMENT = 2,
		ERROR_IK_NOT_DESTROYED = 3,

		ERROR_BODYTRACKINGMODE_NOT_FOUND = 100,
		ERROR_TRACKER_AMOUNT_FAILED = 101,
		ERROR_SKELETONID_NOT_FOUND = 102,
		ERROR_INPUTPOSE_NOT_VALID = 103,
		ERROR_NOT_CALIBRATED = 104,
		ERROR_BODYTRACKINGMODE_NOT_ALIGNED = 105,
		ERROR_AVATAR_INIT_FAILED = 106,
		ERROR_CALIBRATE_FAILED = 107,
		ERROR_COMPUTE_FAILED = 108,
		ERROR_TABLE_STATIC = 109,
		ERROR_SOLVER_NOT_FOUND = 110,
		ERROR_NOT_INITIALIZATION = 111,
		ERROR_JOINT_NOT_FOUND = 112,

		ERROR_FATAL_ERROR = 255,
	}
	public enum DeviceExtRole : UInt64
	{
		Unknown = 0,

		Arm_Wrist = (UInt64)(1 << (Int32)TrackedDeviceRole.ROLE_HEAD
			| 1 << (Int32)TrackedDeviceRole.ROLE_LEFTWRIST | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTWRIST),
		UpperBody_Wrist = (UInt64)(Arm_Wrist | 1 << (Int32)TrackedDeviceRole.ROLE_HIP),
		FullBody_Wrist_Ankle = (UInt64)(UpperBody_Wrist | 1 << (Int32)TrackedDeviceRole.ROLE_LEFTANKLE | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTANKLE),
		FullBody_Wrist_Foot = (UInt64)(UpperBody_Wrist | 1 << (Int32)TrackedDeviceRole.ROLE_LEFTFOOT | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTFOOT),

		Arm_Handheld_Hand = (UInt64)(1 << (Int32)TrackedDeviceRole.ROLE_HEAD
			| 1 << (Int32)TrackedDeviceRole.ROLE_LEFTHAND | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTHAND
			| 1 << (Int32)TrackedDeviceRole.ROLE_LEFTHANDHELD | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTHANDHELD),
		UpperBody_Handheld_Hand = (UInt64)(Arm_Handheld_Hand | 1 << (Int32)TrackedDeviceRole.ROLE_HIP),
		FullBody_Handheld_Hand_Ankle = (UInt64)(UpperBody_Handheld_Hand | 1 << (Int32)TrackedDeviceRole.ROLE_LEFTANKLE | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTANKLE),
		FullBody_Handheld_Hand_Foot = (UInt64)(UpperBody_Handheld_Hand | 1 << (Int32)TrackedDeviceRole.ROLE_LEFTFOOT | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTFOOT),

		UpperBody_Handheld_Hand_Knee_Ankle = (UInt64)(UpperBody_Handheld_Hand
			| 1 << (Int32)TrackedDeviceRole.ROLE_LEFTKNEE | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTKNEE
			| 1 << (Int32)TrackedDeviceRole.ROLE_LEFTANKLE | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTANKLE),

		// Total 9 Device Extrinsic Roles.
	}
	public enum BodyPoseRole : UInt64
	{
		Unknown = 0,

		// Using Tracker
		Arm_Wrist = (UInt64)(1 << (Int32)TrackedDeviceRole.ROLE_HEAD | 1 << (Int32)TrackedDeviceRole.ROLE_LEFTWRIST | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTWRIST),
		UpperBody_Wrist = (UInt64)(Arm_Wrist | 1 << (Int32)TrackedDeviceRole.ROLE_HIP),
		FullBody_Wrist_Ankle = (UInt64)(UpperBody_Wrist | 1 << (Int32)TrackedDeviceRole.ROLE_LEFTANKLE | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTANKLE),
		FullBody_Wrist_Foot = (UInt64)(UpperBody_Wrist | 1 << (Int32)TrackedDeviceRole.ROLE_LEFTFOOT | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTFOOT),

		// Using Controller
		Arm_Handheld = (UInt64)(1 << (Int32)TrackedDeviceRole.ROLE_HEAD | 1 << (Int32)TrackedDeviceRole.ROLE_LEFTHANDHELD | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTHANDHELD),
		UpperBody_Handheld = (UInt64)(Arm_Handheld | 1 << (Int32)TrackedDeviceRole.ROLE_HIP),
		FullBody_Handheld_Ankle = (UInt64)(UpperBody_Handheld | 1 << (Int32)TrackedDeviceRole.ROLE_LEFTANKLE | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTANKLE),
		FullBody_Handheld_Foot = (UInt64)(UpperBody_Handheld | 1 << (Int32)TrackedDeviceRole.ROLE_LEFTFOOT | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTFOOT),

		// Using Natural Hand
		Arm_Hand = (UInt64)(1 << (Int32)TrackedDeviceRole.ROLE_HEAD | 1 << (Int32)TrackedDeviceRole.ROLE_LEFTHAND | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTHAND),
		UpperBody_Hand = (UInt64)(Arm_Hand | 1 << (Int32)TrackedDeviceRole.ROLE_HIP),
		FullBody_Hand_Ankle = (UInt64)(UpperBody_Hand | 1 << (Int32)TrackedDeviceRole.ROLE_LEFTANKLE | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTANKLE),
		FullBody_Hand_Foot = (UInt64)(UpperBody_Hand | 1 << (Int32)TrackedDeviceRole.ROLE_LEFTFOOT | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTFOOT),

		// Head + Controller/Hand + Hip + Knee + Ankle
		UpperBody_Handheld_Knee_Ankle = (UInt64)(UpperBody_Handheld
			| 1 << ((Int32)TrackedDeviceRole.ROLE_LEFTKNEE) | 1 << ((Int32)TrackedDeviceRole.ROLE_RIGHTKNEE)
			| 1 << (Int32)TrackedDeviceRole.ROLE_LEFTANKLE | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTANKLE),
		UpperBody_Hand_Knee_Ankle = (UInt64)(UpperBody_Hand
			| 1 << ((Int32)TrackedDeviceRole.ROLE_LEFTKNEE) | 1 << ((Int32)TrackedDeviceRole.ROLE_RIGHTKNEE)
			| 1 << (Int32)TrackedDeviceRole.ROLE_LEFTANKLE | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTANKLE),

		// Total 14 Body Pose Roles.
	}

	public struct TransformData
	{
		public Vector3 position;
		public Vector3 localPosition;
		public Quaternion rotation;
		public Quaternion localRotation;
		public Vector3 localScale;

		public TransformData(Vector3 in_pos, Vector3 in_localPos, Quaternion in_rot, Quaternion in_localRot, Vector3 in_scale)
		{
			position = in_pos;
			localPosition = in_localPos;

			rotation = in_rot;
			BodyTrackingUtils.Validate(ref rotation);
			localRotation = in_localRot;
			BodyTrackingUtils.Validate(ref localRotation);

			localScale = in_scale;
		}
		public TransformData(Transform trans)
		{
			position = trans.position;
			localPosition = trans.localPosition;

			rotation = trans.rotation;
			BodyTrackingUtils.Validate(ref rotation);
			localRotation = trans.localRotation;
			BodyTrackingUtils.Validate(ref localRotation);

			localScale = trans.localScale;
		}
		public static TransformData identity {
			get {
				return new TransformData(Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity, Vector3.zero);
			}
		}
		public void Update(Transform trans)
		{
			if (trans == null) { return; }
			position = trans.position;
			localPosition = trans.localPosition;
			BodyTrackingUtils.Update(ref rotation, trans.rotation);
			BodyTrackingUtils.Update(ref localRotation, trans.localRotation);
			localScale = trans.localScale;
		}
		public void Update(ref Transform trans)
		{
			if (trans == null) { return; }
			trans.position = position;
			trans.localPosition = localPosition;
			trans.rotation = rotation;
			trans.localRotation = localRotation;
			trans.localScale = localScale;
		}
	}
	public class BodyAvatar
	{
		const string LOG_TAG = "Wave.Essence.BodyTracking.BodyAvatar";
		StringBuilder m_sb = null;
		StringBuilder sb {
			get {
				if (m_sb == null) { m_sb = new StringBuilder(); }
				return m_sb;
			}
		}
		void DEBUG(StringBuilder msg)
		{
			if (Log.EnableDebugLog)
				Log.d(LOG_TAG, msg, true);
		}

		public float height = 0;

		public Joint hip = Joint.identity;

		public Joint leftThigh = Joint.identity;
		public Joint leftLeg = Joint.identity;
		public Joint leftAnkle = Joint.identity;
		public Joint leftFoot = Joint.identity;

		public Joint rightThigh = Joint.identity;
		public Joint rightLeg = Joint.identity;
		public Joint rightAnkle = Joint.identity;
		public Joint rightFoot = Joint.identity;

		public Joint waist = Joint.identity;

		public Joint spineLower = Joint.identity;
		public Joint spineMiddle = Joint.identity;
		public Joint spineHigh = Joint.identity;

		public Joint chest = Joint.identity;
		public Joint neck = Joint.identity;
		public Joint head = Joint.identity;

		public Joint leftClavicle = Joint.identity;
		public Joint leftScapula = Joint.identity;
		public Joint leftUpperarm = Joint.identity;
		public Joint leftForearm = Joint.identity;
		public Joint leftHand = Joint.identity;

		public Joint rightClavicle = Joint.identity;
		public Joint rightScapula = Joint.identity;
		public Joint rightUpperarm = Joint.identity;
		public Joint rightForearm = Joint.identity;
		public Joint rightHand = Joint.identity;

		public float scale = 1;

		private Joint[] s_AvatarJoints = null;
		private void UpdateJoints()
		{
			if (s_AvatarJoints == null || s_AvatarJoints.Length <= 0) { return; }

			int jointCount = 0;
			s_AvatarJoints[jointCount++].Update(hip);

			s_AvatarJoints[jointCount++].Update(leftThigh);
			s_AvatarJoints[jointCount++].Update(leftLeg);
			s_AvatarJoints[jointCount++].Update(leftAnkle);
			s_AvatarJoints[jointCount++].Update(leftFoot);

			s_AvatarJoints[jointCount++].Update(rightThigh);
			s_AvatarJoints[jointCount++].Update(rightLeg);
			s_AvatarJoints[jointCount++].Update(rightAnkle);
			s_AvatarJoints[jointCount++].Update(rightFoot);

			s_AvatarJoints[jointCount++].Update(waist);

			s_AvatarJoints[jointCount++].Update(spineLower);
			s_AvatarJoints[jointCount++].Update(spineMiddle);
			s_AvatarJoints[jointCount++].Update(spineHigh);

			s_AvatarJoints[jointCount++].Update(chest);
			s_AvatarJoints[jointCount++].Update(neck);
			s_AvatarJoints[jointCount++].Update(head);

			s_AvatarJoints[jointCount++].Update(leftClavicle);
			s_AvatarJoints[jointCount++].Update(leftScapula);
			s_AvatarJoints[jointCount++].Update(leftUpperarm);
			s_AvatarJoints[jointCount++].Update(leftForearm);
			s_AvatarJoints[jointCount++].Update(leftHand);

			s_AvatarJoints[jointCount++].Update(rightClavicle);
			s_AvatarJoints[jointCount++].Update(rightScapula);
			s_AvatarJoints[jointCount++].Update(rightUpperarm);
			s_AvatarJoints[jointCount++].Update(rightForearm);
			s_AvatarJoints[jointCount++].Update(rightHand);
		}
		public BodyAvatar()
		{
			int jointCount = 0;

			height = 0;
			// Joint initialization
			{
				hip.jointType = JointType.HIP; jointCount++;

				leftThigh.jointType = JointType.LEFTTHIGH; jointCount++;
				leftLeg.jointType = JointType.LEFTLEG; jointCount++;
				leftAnkle.jointType = JointType.LEFTANKLE; jointCount++;
				leftFoot.jointType = JointType.LEFTFOOT; jointCount++; // 5

				rightThigh.jointType = JointType.RIGHTTHIGH; jointCount++;
				rightLeg.jointType = JointType.RIGHTLEG; jointCount++;
				rightAnkle.jointType = JointType.RIGHTANKLE; jointCount++;
				rightFoot.jointType = JointType.RIGHTFOOT; jointCount++;

				waist.jointType = JointType.WAIST; jointCount++; // 10

				spineLower.jointType = JointType.SPINELOWER; jointCount++;
				spineMiddle.jointType = JointType.SPINEMIDDLE; jointCount++;
				spineHigh.jointType = JointType.SPINEHIGH; jointCount++;

				chest.jointType = JointType.CHEST; jointCount++;
				neck.jointType = JointType.NECK; jointCount++; // 15
				head.jointType = JointType.HEAD; jointCount++;

				leftClavicle.jointType = JointType.LEFTCLAVICLE; jointCount++;
				leftScapula.jointType = JointType.LEFTSCAPULA; jointCount++;
				leftUpperarm.jointType = JointType.LEFTUPPERARM; jointCount++;
				leftForearm.jointType = JointType.LEFTFOREARM; jointCount++; // 20
				leftHand.jointType = JointType.LEFTHAND; jointCount++;

				rightClavicle.jointType = JointType.RIGHTCLAVICLE; jointCount++;
				rightScapula.jointType = JointType.RIGHTSCAPULA; jointCount++;
				rightUpperarm.jointType = JointType.RIGHTUPPERARM; jointCount++;
				rightForearm.jointType = JointType.RIGHTFOREARM; jointCount++; // 25
				rightHand.jointType = JointType.RIGHTHAND; jointCount++;
			}
			scale = 1;

			s_AvatarJoints = new Joint[jointCount];
		}
		public void Update(BodyAvatar in_avatar)
		{
			if (in_avatar == null) { return; }

			height = in_avatar.height;

			head.Update(in_avatar.head);
			neck.Update(in_avatar.neck);
			chest.Update(in_avatar.chest);
			waist.Update(in_avatar.waist);
			hip.Update(in_avatar.hip);

			leftClavicle.Update(in_avatar.leftClavicle);
			leftScapula.Update(in_avatar.leftScapula);
			leftUpperarm.Update(in_avatar.leftUpperarm);
			leftForearm.Update(in_avatar.leftForearm);
			leftHand.Update(in_avatar.leftHand);

			leftThigh.Update(in_avatar.leftThigh);
			leftLeg.Update(in_avatar.leftLeg);
			leftAnkle.Update(in_avatar.leftAnkle);
			leftFoot.Update(in_avatar.leftFoot);

			rightClavicle.Update(in_avatar.rightClavicle);
			rightScapula.Update(in_avatar.rightScapula);
			rightUpperarm.Update(in_avatar.rightUpperarm);
			rightForearm.Update(in_avatar.rightForearm);
			rightHand.Update(in_avatar.rightHand);

			rightThigh.Update(in_avatar.rightThigh);
			rightLeg.Update(in_avatar.rightLeg);
			rightAnkle.Update(in_avatar.rightAnkle);
			rightFoot.Update(in_avatar.rightFoot);

			scale = in_avatar.scale;
		}
		public void Update(Joint joint)
		{
			if (joint.jointType == JointType.HIP) { hip.Update(joint); }

			if (joint.jointType == JointType.LEFTTHIGH) { leftThigh.Update(joint); }
			if (joint.jointType == JointType.LEFTLEG) { leftLeg.Update(joint); }
			if (joint.jointType == JointType.LEFTANKLE) { leftAnkle.Update(joint); }
			if (joint.jointType == JointType.LEFTFOOT) { leftFoot.Update(joint); } // 5

			if (joint.jointType == JointType.RIGHTTHIGH) { rightThigh.Update(joint); }
			if (joint.jointType == JointType.RIGHTLEG) { rightLeg.Update(joint); }
			if (joint.jointType == JointType.RIGHTANKLE) { rightAnkle.Update(joint); }
			if (joint.jointType == JointType.RIGHTFOOT) { rightFoot.Update(joint); }

			if (joint.jointType == JointType.WAIST) { waist.Update(joint); } // 10

			if (joint.jointType == JointType.SPINELOWER) { spineLower.Update(joint); }
			if (joint.jointType == JointType.SPINEMIDDLE) { spineMiddle.Update(joint); }
			if (joint.jointType == JointType.SPINEHIGH) { spineHigh.Update(joint); }

			if (joint.jointType == JointType.CHEST) { chest.Update(joint); }
			if (joint.jointType == JointType.NECK) { neck.Update(joint); } // 15
			if (joint.jointType == JointType.HEAD) { head.Update(joint); }

			if (joint.jointType == JointType.LEFTCLAVICLE) { leftClavicle.Update(joint); }
			if (joint.jointType == JointType.LEFTSCAPULA) { leftScapula.Update(joint); }
			if (joint.jointType == JointType.LEFTUPPERARM) { leftUpperarm.Update(joint); }
			if (joint.jointType == JointType.LEFTFOREARM) { leftForearm.Update(joint); } // 20
			if (joint.jointType == JointType.LEFTHAND) { leftHand.Update(joint); }

			if (joint.jointType == JointType.RIGHTCLAVICLE) { rightClavicle.Update(joint); }
			if (joint.jointType == JointType.RIGHTSCAPULA) { rightScapula.Update(joint); }
			if (joint.jointType == JointType.RIGHTUPPERARM) { rightUpperarm.Update(joint); }
			if (joint.jointType == JointType.RIGHTFOREARM) { rightForearm.Update(joint); } // 25
			if (joint.jointType == JointType.RIGHTHAND) { rightHand.Update(joint); }
		}

		private void Update([In] Transform trans, [In] Vector3 velocity, [In] Vector3 angularVelocity, ref Joint joint)
		{
			if (trans == null) { return; }
			joint.poseState = (PoseState.ROTATION | PoseState.TRANSLATION);
			joint.translation = trans.position;
			BodyTrackingUtils.Update(ref joint.rotation, trans.rotation);
			joint.velocity = velocity;
			joint.angularVelocity = angularVelocity;
			//sb.Clear().Append("Update() ").Append(joint.Log()); DEBUG(sb);
		}
		public void Update(JointType jointType, Transform trans, Vector3 velocity, Vector3 angularVelocity)
		{
			if (trans == null) { return; }
			if (jointType == JointType.HEAD) { Update(trans, velocity, angularVelocity, ref head); }
			if (jointType == JointType.NECK) { Update(trans, velocity, angularVelocity, ref neck); }
			if (jointType == JointType.CHEST) { Update(trans, velocity, angularVelocity, ref chest); }
			if (jointType == JointType.WAIST) { Update(trans, velocity, angularVelocity, ref waist); }
			if (jointType == JointType.HIP) { Update(trans, velocity, angularVelocity, ref hip); }

			if (jointType == JointType.LEFTCLAVICLE) { Update(trans, velocity, angularVelocity, ref leftClavicle); }
			if (jointType == JointType.LEFTSCAPULA) { Update(trans, velocity, angularVelocity, ref leftScapula); }
			if (jointType == JointType.LEFTUPPERARM) { Update(trans, velocity, angularVelocity, ref leftUpperarm); }
			if (jointType == JointType.LEFTFOREARM) { Update(trans, velocity, angularVelocity, ref leftForearm); }
			if (jointType == JointType.LEFTHAND) { Update(trans, velocity, angularVelocity, ref leftHand); }

			if (jointType == JointType.LEFTTHIGH) { Update(trans, velocity, angularVelocity, ref leftThigh); }
			if (jointType == JointType.LEFTLEG) { Update(trans, velocity, angularVelocity, ref leftLeg); }
			if (jointType == JointType.LEFTANKLE) { Update(trans, velocity, angularVelocity, ref leftAnkle); }
			if (jointType == JointType.LEFTFOOT) { Update(trans, velocity, angularVelocity, ref leftFoot); }

			if (jointType == JointType.RIGHTCLAVICLE) { Update(trans, velocity, angularVelocity, ref rightClavicle); }
			if (jointType == JointType.RIGHTSCAPULA) { Update(trans, velocity, angularVelocity, ref rightScapula); }
			if (jointType == JointType.RIGHTUPPERARM) { Update(trans, velocity, angularVelocity, ref rightUpperarm); }
			if (jointType == JointType.RIGHTFOREARM) { Update(trans, velocity, angularVelocity, ref rightForearm); }
			if (jointType == JointType.RIGHTHAND) { Update(trans, velocity, angularVelocity, ref rightHand); }

			if (jointType == JointType.RIGHTTHIGH) { Update(trans, velocity, angularVelocity, ref rightThigh); }
			if (jointType == JointType.RIGHTLEG) { Update(trans, velocity, angularVelocity, ref rightLeg); }
			if (jointType == JointType.RIGHTANKLE) { Update(trans, velocity, angularVelocity, ref rightAnkle); }
			if (jointType == JointType.RIGHTFOOT) { Update(trans, velocity, angularVelocity, ref rightFoot); }

			if (jointType == JointType.SPINELOWER) { Update(trans, velocity, angularVelocity, ref spineLower); }
			if (jointType == JointType.SPINEMIDDLE) { Update(trans, velocity, angularVelocity, ref spineMiddle); }
			if (jointType == JointType.SPINEHIGH) { Update(trans, velocity, angularVelocity, ref spineHigh); }
		}
		private void Update([In] Transform trans, [In] Vector3 velocity, ref Joint joint)
		{
			if (trans == null) { return; }
			joint.translation = trans.transform.position;
			BodyTrackingUtils.Update(ref joint.rotation, trans.transform.rotation);
			joint.poseState = (PoseState.ROTATION | PoseState.TRANSLATION);
			joint.velocity = velocity;
			//sb.Clear().Append("Update() ").Append(joint.Log()); DEBUG(sb);
		}
		public void Update(JointType jointType, Transform trans, Vector3 velocity)
		{
			if (trans == null) { return; }
			if (jointType == JointType.HEAD) { Update(trans, velocity, ref head); }
			if (jointType == JointType.NECK) { Update(trans, velocity, ref neck); }
			if (jointType == JointType.CHEST) { Update(trans, velocity, ref chest); }
			if (jointType == JointType.WAIST) { Update(trans, velocity, ref waist); }
			if (jointType == JointType.HIP) { Update(trans, velocity, ref hip); }

			if (jointType == JointType.LEFTCLAVICLE) { Update(trans, velocity, ref leftClavicle); }
			if (jointType == JointType.LEFTSCAPULA) { Update(trans, velocity, ref leftScapula); }
			if (jointType == JointType.LEFTUPPERARM) { Update(trans, velocity, ref leftUpperarm); }
			if (jointType == JointType.LEFTFOREARM) { Update(trans, velocity, ref leftForearm); }
			if (jointType == JointType.LEFTHAND) { Update(trans, velocity, ref leftHand); }

			if (jointType == JointType.LEFTTHIGH) { Update(trans, velocity, ref leftThigh); }
			if (jointType == JointType.LEFTLEG) { Update(trans, velocity, ref leftLeg); }
			if (jointType == JointType.LEFTANKLE) { Update(trans, velocity, ref leftAnkle); }
			if (jointType == JointType.LEFTFOOT) { Update(trans, velocity, ref leftFoot); }

			if (jointType == JointType.RIGHTCLAVICLE) { Update(trans, velocity, ref rightClavicle); }
			if (jointType == JointType.RIGHTSCAPULA) { Update(trans, velocity, ref rightScapula); }
			if (jointType == JointType.RIGHTUPPERARM) { Update(trans, velocity, ref rightUpperarm); }
			if (jointType == JointType.RIGHTFOREARM) { Update(trans, velocity, ref rightForearm); }
			if (jointType == JointType.RIGHTHAND) { Update(trans, velocity, ref rightHand); }

			if (jointType == JointType.RIGHTTHIGH) { Update(trans, velocity, ref rightThigh); }
			if (jointType == JointType.RIGHTLEG) { Update(trans, velocity, ref rightLeg); }
			if (jointType == JointType.RIGHTANKLE) { Update(trans, velocity, ref rightAnkle); }
			if (jointType == JointType.RIGHTFOOT) { Update(trans, velocity, ref rightFoot); }

			if (jointType == JointType.SPINELOWER) { Update(trans, velocity, ref spineLower); }
			if (jointType == JointType.SPINEMIDDLE) { Update(trans, velocity, ref spineMiddle); }
			if (jointType == JointType.SPINEHIGH) { Update(trans, velocity, ref spineHigh); }
		}
		private void Update([In] Transform trans, ref Joint joint)
		{
			if (trans == null) { return; }
			joint.translation = trans.position;
			BodyTrackingUtils.Update(ref joint.rotation, trans.rotation);
			joint.poseState = (PoseState.ROTATION | PoseState.TRANSLATION);
			//sb.Clear().Append("Update() ").Append(joint.Log()); DEBUG(sb);
		}
		public void Update(JointType jointType, Transform trans)
		{
			if (trans == null) { return; }
			if (jointType == JointType.HEAD) { Update(trans, ref head); }
			if (jointType == JointType.NECK) { Update(trans, ref neck); }
			if (jointType == JointType.CHEST) { Update(trans, ref chest); }
			if (jointType == JointType.WAIST) { Update(trans, ref waist); }
			if (jointType == JointType.HIP) { Update(trans, ref hip); }

			if (jointType == JointType.LEFTCLAVICLE) { Update(trans, ref leftClavicle); }
			if (jointType == JointType.LEFTSCAPULA) { Update(trans, ref leftScapula); }
			if (jointType == JointType.LEFTUPPERARM) { Update(trans, ref leftUpperarm); }
			if (jointType == JointType.LEFTFOREARM) { Update(trans, ref leftForearm); }
			if (jointType == JointType.LEFTHAND) { Update(trans, ref leftHand); }

			if (jointType == JointType.LEFTTHIGH) { Update(trans, ref leftThigh); }
			if (jointType == JointType.LEFTLEG) { Update(trans, ref leftLeg); }
			if (jointType == JointType.LEFTANKLE) { Update(trans, ref leftAnkle); }
			if (jointType == JointType.LEFTFOOT) { Update(trans, ref leftFoot); }

			if (jointType == JointType.RIGHTCLAVICLE) { Update(trans, ref rightClavicle); }
			if (jointType == JointType.RIGHTSCAPULA) { Update(trans, ref rightScapula); }
			if (jointType == JointType.RIGHTUPPERARM) { Update(trans, ref rightUpperarm); }
			if (jointType == JointType.RIGHTFOREARM) { Update(trans, ref rightForearm); }
			if (jointType == JointType.RIGHTHAND) { Update(trans, ref rightHand); }

			if (jointType == JointType.RIGHTTHIGH) { Update(trans, ref rightThigh); }
			if (jointType == JointType.RIGHTLEG) { Update(trans, ref rightLeg); }
			if (jointType == JointType.RIGHTANKLE) { Update(trans, ref rightAnkle); }
			if (jointType == JointType.RIGHTFOOT) { Update(trans, ref rightFoot); }

			if (jointType == JointType.SPINELOWER) { Update(trans, ref spineLower); }
			if (jointType == JointType.SPINEMIDDLE) { Update(trans, ref spineMiddle); }
			if (jointType == JointType.SPINEHIGH) { Update(trans, ref spineHigh); }
		}
		private void Update([In] Joint joint, ref Transform trans, float scale = 1)
		{
			if (trans == null) { return; }
			if (joint.poseState.HasFlag(PoseState.TRANSLATION)) { trans.position = joint.translation * scale; }
			if (joint.poseState.HasFlag(PoseState.ROTATION))
			{
				trans.rotation = joint.rotation;
				if (trans.rotation.isZero()) { trans.rotation = Quaternion.identity; }
			}
		}
		public void Update([In] JointType jointType, ref Transform trans, float scale = 1)
		{
			if (trans == null) { return; }
			if (jointType == JointType.HEAD) { Update(head, ref trans, scale); }
			if (jointType == JointType.NECK) { Update(neck, ref trans, scale); }
			if (jointType == JointType.CHEST) { Update(chest, ref trans, scale); }
			if (jointType == JointType.WAIST) { Update(waist, ref trans, scale); }
			if (jointType == JointType.HIP) { Update(hip, ref trans, scale); }

			if (jointType == JointType.LEFTCLAVICLE) { Update(leftClavicle, ref trans, scale); }
			if (jointType == JointType.LEFTSCAPULA) { Update(leftScapula, ref trans, scale); }
			if (jointType == JointType.LEFTUPPERARM) { Update(leftUpperarm, ref trans, scale); }
			if (jointType == JointType.LEFTFOREARM) { Update(leftForearm, ref trans, scale); }
			if (jointType == JointType.LEFTHAND) { Update(leftHand, ref trans, scale); }

			if (jointType == JointType.LEFTTHIGH) { Update(leftThigh, ref trans, scale); }
			if (jointType == JointType.LEFTLEG) { Update(leftLeg, ref trans, scale); }
			if (jointType == JointType.LEFTANKLE) { Update(leftAnkle, ref trans, scale); }
			if (jointType == JointType.LEFTFOOT) { Update(leftFoot, ref trans, scale); }

			if (jointType == JointType.RIGHTCLAVICLE) { Update(rightClavicle, ref trans, scale); }
			if (jointType == JointType.RIGHTSCAPULA) { Update(rightScapula, ref trans, scale); }
			if (jointType == JointType.RIGHTUPPERARM) { Update(rightUpperarm, ref trans, scale); }
			if (jointType == JointType.RIGHTFOREARM) { Update(rightForearm, ref trans, scale); }
			if (jointType == JointType.RIGHTHAND) { Update(rightHand, ref trans, scale); }

			if (jointType == JointType.RIGHTTHIGH) { Update(rightThigh, ref trans, scale); }
			if (jointType == JointType.RIGHTLEG) { Update(rightLeg, ref trans, scale); }
			if (jointType == JointType.RIGHTANKLE) { Update(rightAnkle, ref trans, scale); }
			if (jointType == JointType.RIGHTFOOT) { Update(rightFoot, ref trans, scale); }

			if (jointType == JointType.SPINELOWER) { Update(spineLower, ref trans, scale); }
			if (jointType == JointType.SPINEMIDDLE) { Update(spineMiddle, ref trans, scale); }
			if (jointType == JointType.SPINEHIGH) { Update(spineHigh, ref trans, scale); }
		}

		public void Update([In] Body body)
		{
			if (body == null) { return; }

			Update(JointType.HIP, body.root); // 0

			Update(JointType.LEFTTHIGH, body.leftThigh);
			Update(JointType.LEFTLEG, body.leftLeg);
			Update(JointType.LEFTANKLE, body.leftAnkle);
			Update(JointType.LEFTFOOT, body.leftFoot);

			Update(JointType.RIGHTTHIGH, body.rightThigh); // 5
			Update(JointType.RIGHTLEG, body.rightLeg);
			Update(JointType.RIGHTANKLE, body.rightAnkle);
			Update(JointType.RIGHTFOOT, body.rightFoot);

			Update(JointType.WAIST, body.waist);

			Update(JointType.SPINELOWER, body.spineLower); // 10
			Update(JointType.SPINEMIDDLE, body.spineMiddle);
			Update(JointType.SPINEHIGH, body.spineHigh);

			Update(JointType.CHEST, body.chest);
			Update(JointType.NECK, body.neck);
			Update(JointType.HEAD, body.head); // 15

			Update(JointType.LEFTCLAVICLE, body.leftClavicle);
			Update(JointType.LEFTSCAPULA, body.leftScapula);
			Update(JointType.LEFTUPPERARM, body.leftUpperarm);
			Update(JointType.LEFTFOREARM, body.leftForearm);
			Update(JointType.LEFTHAND, body.leftHand); // 20

			Update(JointType.RIGHTCLAVICLE, body.rightClavicle);
			Update(JointType.RIGHTSCAPULA, body.rightScapula);
			Update(JointType.RIGHTUPPERARM, body.rightUpperarm);
			Update(JointType.RIGHTFOREARM, body.rightForearm);
			Update(JointType.RIGHTHAND, body.rightHand);

			height = body.height;
		}
		/// <summary>
		/// Update full body poses. Note that your avatar should have joints in specified order.
		/// E.g. You avatar's toe should be the child of foot and the foot should be the child of leg.
		/// </summary>
		/// <param name="body">Reference to the avatar body.</param>
		public void Update(ref Body body)
		{
			if (body == null) { return; }

			body.height = height;

			if (body.root != null) Update(JointType.HIP, ref body.root); // 0

			if (body.leftThigh != null) Update(JointType.LEFTTHIGH, ref body.leftThigh);
			if (body.leftLeg != null) Update(JointType.LEFTLEG, ref body.leftLeg);
			if (body.leftAnkle != null) Update(JointType.LEFTANKLE, ref body.leftAnkle);
			if (body.leftFoot != null) Update(JointType.LEFTFOOT, ref body.leftFoot);

			if (body.rightThigh != null) Update(JointType.RIGHTTHIGH, ref body.rightThigh); // 5
			if (body.rightLeg != null) Update(JointType.RIGHTLEG, ref body.rightLeg);
			if (body.rightAnkle != null) Update(JointType.RIGHTANKLE, ref body.rightAnkle);
			if (body.rightFoot != null) Update(JointType.RIGHTFOOT, ref body.rightFoot);

			if (body.waist != null) Update(JointType.WAIST, ref body.waist);

			if (body.spineLower != null) Update(JointType.SPINELOWER, ref body.spineLower); // 10
			if (body.spineMiddle != null) Update(JointType.SPINEMIDDLE, ref body.spineMiddle);
			if (body.spineHigh != null) Update(JointType.SPINEHIGH, ref body.spineHigh);

			if (body.chest != null) Update(JointType.CHEST, ref body.chest);
			if (body.neck != null) Update(JointType.NECK, ref body.neck);
			if (body.head != null) Update(JointType.HEAD, ref body.head); // 15

			if (body.leftClavicle != null) Update(JointType.LEFTCLAVICLE, ref body.leftClavicle);
			if (body.leftScapula != null) Update(JointType.LEFTSCAPULA, ref body.leftScapula);
			if (body.leftUpperarm != null) Update(JointType.LEFTUPPERARM, ref body.leftUpperarm);
			if (body.leftForearm != null) Update(JointType.LEFTFOREARM, ref body.leftForearm);
			if (body.leftHand != null) Update(JointType.LEFTHAND, ref body.leftHand); // 20

			if (body.rightClavicle != null) Update(JointType.RIGHTCLAVICLE, ref body.rightClavicle);
			if (body.rightScapula != null) Update(JointType.RIGHTSCAPULA, ref body.rightScapula);
			if (body.rightUpperarm != null) Update(JointType.RIGHTUPPERARM, ref body.rightUpperarm);
			if (body.rightForearm != null) Update(JointType.RIGHTFOREARM, ref body.rightForearm);
			if (body.rightHand != null) Update(JointType.RIGHTHAND, ref body.rightHand); // 25
		}

		private List<Joint> joints = new List<Joint>();
		public bool GetJoints(out Joint[] avatarJoints, out UInt32 avatarJointCount, bool is6DoF = false)
		{
			if (!is6DoF) // including NODATA joints.
			{
				UpdateJoints();
				avatarJoints = s_AvatarJoints;
				avatarJointCount = (UInt32)(avatarJoints.Length & 0x7FFFFFFF);
				return true;
			}

			avatarJoints = null;
			avatarJointCount = 0;

			joints.Clear();
			if (hip.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(hip); }

			if (leftThigh.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(leftThigh); }
			if (leftLeg.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(leftLeg); }
			if (leftAnkle.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(leftAnkle); }
			if (leftFoot.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(leftFoot); }

			if (rightThigh.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(rightThigh); }
			if (rightLeg.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(rightLeg); }
			if (rightAnkle.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(rightAnkle); }
			if (rightFoot.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(rightFoot); }

			if (waist.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(waist); }

			if (spineLower.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(spineLower); }
			if (spineMiddle.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(spineMiddle); }
			if (spineHigh.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(spineHigh); }

			if (chest.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(chest); }
			if (neck.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(neck); }
			if (head.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(head); }

			if (leftClavicle.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(leftClavicle); }
			if (leftScapula.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(leftScapula); }
			if (leftUpperarm.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(leftUpperarm); }
			if (leftForearm.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(leftForearm); }
			if (leftHand.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(leftHand); }

			if (rightClavicle.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(rightClavicle); }
			if (rightScapula.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(rightScapula); }
			if (rightUpperarm.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(rightUpperarm); }
			if (rightForearm.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(rightForearm); }
			if (rightHand.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(rightHand); }

			if (joints.Count > 0)
			{
				avatarJoints = joints.ToArray();
				avatarJointCount = (UInt32)(joints.Count & 0x7FFFFFFF);
				return true;
			}

			return false;
		}
		public void Set6DoFJoints(Joint[] avatarJoints, UInt32 avatarJointCount)
		{
			for (UInt32 i = 0; i < avatarJointCount; i++)
			{
				Update(avatarJoints[i]);
			}
		}
	}

	[Serializable]
	public struct ExtrinsicVector4_t
	{
		public Vector3 translation;
		[SerializeField]
		private Vector4 m_rotation;
		public Vector4 rotation {
			get {
				if (m_rotation == Vector4.zero) { m_rotation.w = 1; }
				return m_rotation;
			}
			set { m_rotation = value; }
		}

		private Extrinsic ext;
		private void UpdateExtrinsic()
		{
			ext.translation = translation;
			BodyTrackingUtils.Update(rotation, ref ext.rotation);
		}
		public Extrinsic GetExtrinsic()
		{
			UpdateExtrinsic();
			return ext;
		}

		public ExtrinsicVector4_t(Vector3 in_tra, Vector4 in_rot)
		{
			translation = in_tra;
			m_rotation = in_rot;

			ext = Extrinsic.identity;
			UpdateExtrinsic();
		}
		public static ExtrinsicVector4_t identity {
			get {
				return new ExtrinsicVector4_t(Vector3.zero, new Vector4(0, 0, 0, 1));
			}
		}
		public void Update(ExtrinsicVector4_t in_ext)
		{
			translation = in_ext.translation;
			rotation = in_ext.rotation;
		}
		public void Update(Extrinsic in_ext)
		{
			translation = in_ext.translation;
			BodyTrackingUtils.Update(in_ext.rotation, ref m_rotation);
		}
		public void Update(WVR_Pose_t in_pose)
		{
			Coordinate.GetVectorFromGL(in_pose.position, out translation);
			m_rotation.x = in_pose.rotation.x;
			m_rotation.y = in_pose.rotation.y;
			m_rotation.z = -in_pose.rotation.z;
			m_rotation.w = -in_pose.rotation.w;
		}

		public string Log()
		{
			string log = "position(" + translation.x.ToString() + ", " + translation.y.ToString() + ", " + translation.z.ToString() + ")";
			log += ", rotation(" + rotation.x.ToString() + ", " + rotation.y.ToString() + ", " + rotation.z.ToString() + ", " + rotation.w.ToString() + ")";
			return log;
		}
	}
	[Serializable]
	public struct ExtrinsicInfo_t
	{
		const string LOG_TAG = "Wave.Essence.BodyTracking.ExtrinsicInfo_t";
		static StringBuilder m_sb = null;
		static StringBuilder sb {
			get {
				if (m_sb == null) { m_sb = new StringBuilder(); }
				return m_sb;
			}
		}
		void DEBUG(StringBuilder msg)
		{
			if (Log.EnableDebugLog)
				Log.d(LOG_TAG, msg, true);
		}

		public bool isTracking;
		public ExtrinsicVector4_t extrinsic;
		public ExtrinsicInfo_t(bool in_isTracking, ExtrinsicVector4_t in_extrinsic)
		{
			isTracking = in_isTracking;
			extrinsic = in_extrinsic;
		}
		public static ExtrinsicInfo_t identity {
			get {
				return new ExtrinsicInfo_t(false, ExtrinsicVector4_t.identity);
			}
		}
		public void Init()
		{
			isTracking = false;
			extrinsic = ExtrinsicVector4_t.identity;
		}
		public void Update(ExtrinsicInfo_t in_info)
		{
			isTracking = in_info.isTracking;
			extrinsic.Update(in_info.extrinsic);
		}
		public void Update(ExtrinsicVector4_t in_ext)
		{
			isTracking = true;
			extrinsic.Update(in_ext);
		}
		public void Update(WVR_Pose_t pose)
		{
			isTracking = true;
			extrinsic.Update(pose);
		}
		public void Update(Extrinsic in_ext)
		{
			isTracking = true;
			extrinsic.Update(in_ext);
		}
		public void printLog(string prefix)
		{
			sb.Clear().Append(prefix)
				.Append(", position(").Append(extrinsic.translation.x).Append(", ").Append(extrinsic.translation.y).Append(", ").Append(extrinsic.translation.z).Append(")")
				.Append(", rotation(").Append(extrinsic.rotation.x).Append(", ").Append(extrinsic.rotation.y).Append(", ").Append(extrinsic.rotation.z).Append(", ").Append(extrinsic.rotation.w).Append(")");
			DEBUG(sb);
		}
	}

	public struct TrackedDeviceExtrinsicState_t
	{
		const string LOG_TAG = "Wave.Essence.BodyTracking.TrackedDeviceExtrinsicState_t";
		static StringBuilder m_sb = null;
		static StringBuilder sb {
			get {
				if (m_sb == null) { m_sb = new StringBuilder(); }
				return m_sb;
			}
		}
		static void DEBUG(StringBuilder msg) { Log.d(LOG_TAG, msg, true); }

		public bool isTracking;
		public TrackedDeviceExtrinsic deviceExtrinsic;

		public TrackedDeviceExtrinsicState_t(bool in_isTracking, TrackedDeviceExtrinsic in_deviceExtrinsic)
		{
			isTracking = in_isTracking;
			deviceExtrinsic = in_deviceExtrinsic;
		}
		public static TrackedDeviceExtrinsicState_t identity {
			get {
				return new TrackedDeviceExtrinsicState_t(false, TrackedDeviceExtrinsic.identity);
			}
		}
		public void Update(TrackedDeviceExtrinsicState_t in_info)
		{
			isTracking = in_info.isTracking;
			deviceExtrinsic.Update(in_info.deviceExtrinsic);
		}
		public void Update(ExtrinsicInfo_t extInfo)
		{
			isTracking = extInfo.isTracking;
			deviceExtrinsic.extrinsic.Update(extInfo.extrinsic.GetExtrinsic());

			sb.Clear().Append(deviceExtrinsic.trackedDeviceRole.Name())
				.Append(", isTracking: ").Append(isTracking)
				.Append(", position(")
					.Append(deviceExtrinsic.extrinsic.translation.x).Append(", ")
					.Append(deviceExtrinsic.extrinsic.translation.y).Append(", ")
					.Append(deviceExtrinsic.extrinsic.translation.z)
				.Append(")")
				.Append(", rotation(")
					.Append(deviceExtrinsic.extrinsic.rotation.x).Append(", ")
					.Append(deviceExtrinsic.extrinsic.rotation.y).Append(", ")
					.Append(deviceExtrinsic.extrinsic.rotation.z).Append(", ")
					.Append(deviceExtrinsic.extrinsic.rotation.w)
				.Append(")");
			DEBUG(sb);
		}
	}
	/// <summary>
	/// A class records the developer's choices of tracking devices.
	/// The developer selects which devices to be tracked in the Inspector of BodyManager.Body.
	/// The selections will be imported as a BodyTrackedDevice instance.
	/// </summary>
	public class BodyTrackedDevice
	{
		const string LOG_TAG = "Wave.Essence.BodyTracking.BodyTrackedDevice";
		static StringBuilder m_sb = null;
		static StringBuilder sb {
			get {
				if (m_sb == null) { m_sb = new StringBuilder(); }
				return m_sb;
			}
		}
		void DEBUG(StringBuilder msg)
		{
			if (Log.EnableDebugLog)
				Log.d(LOG_TAG, msg, true);
		}
		static int logFrame = 0;
		bool printIntervalLog = false;
		void ERROR(StringBuilder msg) { Log.e(LOG_TAG, msg, true); }

		public TrackedDeviceExtrinsicState_t hip = TrackedDeviceExtrinsicState_t.identity;
		public TrackedDeviceExtrinsicState_t chest = TrackedDeviceExtrinsicState_t.identity;
		public TrackedDeviceExtrinsicState_t head = TrackedDeviceExtrinsicState_t.identity;

		public TrackedDeviceExtrinsicState_t leftElbow = TrackedDeviceExtrinsicState_t.identity;
		public TrackedDeviceExtrinsicState_t leftWrist = TrackedDeviceExtrinsicState_t.identity;
		public TrackedDeviceExtrinsicState_t leftHand = TrackedDeviceExtrinsicState_t.identity;
		public TrackedDeviceExtrinsicState_t leftHandheld = TrackedDeviceExtrinsicState_t.identity;

		public TrackedDeviceExtrinsicState_t rightElbow = TrackedDeviceExtrinsicState_t.identity;
		public TrackedDeviceExtrinsicState_t rightWrist = TrackedDeviceExtrinsicState_t.identity;
		public TrackedDeviceExtrinsicState_t rightHand = TrackedDeviceExtrinsicState_t.identity;
		public TrackedDeviceExtrinsicState_t rightHandheld = TrackedDeviceExtrinsicState_t.identity;

		public TrackedDeviceExtrinsicState_t leftKnee = TrackedDeviceExtrinsicState_t.identity;
		public TrackedDeviceExtrinsicState_t leftAnkle = TrackedDeviceExtrinsicState_t.identity;
		public TrackedDeviceExtrinsicState_t leftFoot = TrackedDeviceExtrinsicState_t.identity;

		public TrackedDeviceExtrinsicState_t rightKnee = TrackedDeviceExtrinsicState_t.identity;
		public TrackedDeviceExtrinsicState_t rightAnkle = TrackedDeviceExtrinsicState_t.identity;
		public TrackedDeviceExtrinsicState_t rightFoot = TrackedDeviceExtrinsicState_t.identity;

		private Dictionary<DeviceExtRole, List<TrackedDeviceExtrinsic>> s_TrackedDeviceExtrinsics = new Dictionary<DeviceExtRole, List<TrackedDeviceExtrinsic>>();
		private Dictionary<DeviceExtRole, TrackedDeviceExtrinsic[]> trackedDevicesArrays = new Dictionary<DeviceExtRole, TrackedDeviceExtrinsic[]>();
		private bool getDeviceExtrinsicsFirstTime = true;
		private void UpdateTrackedDevicesArray()
		{
			if (s_TrackedDeviceExtrinsics == null) { s_TrackedDeviceExtrinsics = new Dictionary<DeviceExtRole, List<TrackedDeviceExtrinsic>>(); }
			s_TrackedDeviceExtrinsics.Clear();

			s_TrackedDeviceExtrinsics.Add(DeviceExtRole.Arm_Wrist, new List<TrackedDeviceExtrinsic>());
			s_TrackedDeviceExtrinsics.Add(DeviceExtRole.UpperBody_Wrist, new List<TrackedDeviceExtrinsic>());
			s_TrackedDeviceExtrinsics.Add(DeviceExtRole.FullBody_Wrist_Ankle, new List<TrackedDeviceExtrinsic>());
			s_TrackedDeviceExtrinsics.Add(DeviceExtRole.FullBody_Wrist_Foot, new List<TrackedDeviceExtrinsic>());

			s_TrackedDeviceExtrinsics.Add(DeviceExtRole.Arm_Handheld_Hand, new List<TrackedDeviceExtrinsic>());
			s_TrackedDeviceExtrinsics.Add(DeviceExtRole.UpperBody_Handheld_Hand, new List<TrackedDeviceExtrinsic>());
			s_TrackedDeviceExtrinsics.Add(DeviceExtRole.FullBody_Handheld_Hand_Ankle, new List<TrackedDeviceExtrinsic>());
			s_TrackedDeviceExtrinsics.Add(DeviceExtRole.FullBody_Handheld_Hand_Foot, new List<TrackedDeviceExtrinsic>());

			s_TrackedDeviceExtrinsics.Add(DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle, new List<TrackedDeviceExtrinsic>());

			// 7 roles use hip.
			if (hip.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(hip.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);

				s_TrackedDeviceExtrinsics[DeviceExtRole.UpperBody_Wrist].Add(hip.deviceExtrinsic);
				s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Wrist_Ankle].Add(hip.deviceExtrinsic);
				s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Wrist_Foot].Add(hip.deviceExtrinsic);

				s_TrackedDeviceExtrinsics[DeviceExtRole.UpperBody_Handheld_Hand].Add(hip.deviceExtrinsic);
				s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Handheld_Hand_Ankle].Add(hip.deviceExtrinsic);
				s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Handheld_Hand_Foot].Add(hip.deviceExtrinsic);

				s_TrackedDeviceExtrinsics[DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle].Add(hip.deviceExtrinsic);
			}
			if (chest.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(chest.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);
			}
			// 9 roles use head.
			if (head.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(head.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);

				s_TrackedDeviceExtrinsics[DeviceExtRole.Arm_Wrist].Add(head.deviceExtrinsic);
				s_TrackedDeviceExtrinsics[DeviceExtRole.UpperBody_Wrist].Add(head.deviceExtrinsic);
				s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Wrist_Ankle].Add(head.deviceExtrinsic);
				s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Wrist_Foot].Add(head.deviceExtrinsic);

				s_TrackedDeviceExtrinsics[DeviceExtRole.Arm_Handheld_Hand].Add(head.deviceExtrinsic);
				s_TrackedDeviceExtrinsics[DeviceExtRole.UpperBody_Handheld_Hand].Add(head.deviceExtrinsic);
				s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Handheld_Hand_Ankle].Add(head.deviceExtrinsic);
				s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Handheld_Hand_Foot].Add(head.deviceExtrinsic);

				s_TrackedDeviceExtrinsics[DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle].Add(head.deviceExtrinsic);
			}

			if (leftElbow.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(leftElbow.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);
			}
			// 4 roles use leftWrist.
			if (leftWrist.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(leftWrist.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);

				s_TrackedDeviceExtrinsics[DeviceExtRole.Arm_Wrist].Add(leftWrist.deviceExtrinsic);
				s_TrackedDeviceExtrinsics[DeviceExtRole.UpperBody_Wrist].Add(leftWrist.deviceExtrinsic);
				s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Wrist_Ankle].Add(leftWrist.deviceExtrinsic);
				s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Wrist_Foot].Add(leftWrist.deviceExtrinsic);
			}
			// 5 roles use leftHand
			if (leftHand.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(leftHand.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);

				s_TrackedDeviceExtrinsics[DeviceExtRole.Arm_Handheld_Hand].Add(leftHand.deviceExtrinsic);
				s_TrackedDeviceExtrinsics[DeviceExtRole.UpperBody_Handheld_Hand].Add(leftHand.deviceExtrinsic);
				s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Handheld_Hand_Ankle].Add(leftHand.deviceExtrinsic);
				s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Handheld_Hand_Foot].Add(leftHand.deviceExtrinsic);

				s_TrackedDeviceExtrinsics[DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle].Add(leftHand.deviceExtrinsic);
			}
			// 5 roles use leftHandheld
			if (leftHandheld.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(leftHandheld.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);

				s_TrackedDeviceExtrinsics[DeviceExtRole.Arm_Handheld_Hand].Add(leftHandheld.deviceExtrinsic);
				s_TrackedDeviceExtrinsics[DeviceExtRole.UpperBody_Handheld_Hand].Add(leftHandheld.deviceExtrinsic);
				s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Handheld_Hand_Ankle].Add(leftHandheld.deviceExtrinsic);
				s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Handheld_Hand_Foot].Add(leftHandheld.deviceExtrinsic);

				s_TrackedDeviceExtrinsics[DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle].Add(leftHandheld.deviceExtrinsic);
			}

			if (rightElbow.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(rightElbow.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);
			}
			// 4 roles use rightWrist.
			if (rightWrist.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(rightWrist.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);

				s_TrackedDeviceExtrinsics[DeviceExtRole.Arm_Wrist].Add(rightWrist.deviceExtrinsic);
				s_TrackedDeviceExtrinsics[DeviceExtRole.UpperBody_Wrist].Add(rightWrist.deviceExtrinsic);
				s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Wrist_Ankle].Add(rightWrist.deviceExtrinsic);
				s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Wrist_Foot].Add(rightWrist.deviceExtrinsic);
			}
			// 5 roles use rightHand
			if (rightHand.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(rightHand.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);

				s_TrackedDeviceExtrinsics[DeviceExtRole.Arm_Handheld_Hand].Add(rightHand.deviceExtrinsic);
				s_TrackedDeviceExtrinsics[DeviceExtRole.UpperBody_Handheld_Hand].Add(rightHand.deviceExtrinsic);
				s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Handheld_Hand_Ankle].Add(rightHand.deviceExtrinsic);
				s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Handheld_Hand_Foot].Add(rightHand.deviceExtrinsic);

				s_TrackedDeviceExtrinsics[DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle].Add(rightHand.deviceExtrinsic);
			}
			// 5 roles use rightHandheld
			if (rightHandheld.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(rightHandheld.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);

				s_TrackedDeviceExtrinsics[DeviceExtRole.Arm_Handheld_Hand].Add(rightHandheld.deviceExtrinsic);
				s_TrackedDeviceExtrinsics[DeviceExtRole.UpperBody_Handheld_Hand].Add(rightHandheld.deviceExtrinsic);
				s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Handheld_Hand_Ankle].Add(rightHandheld.deviceExtrinsic);
				s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Handheld_Hand_Foot].Add(rightHandheld.deviceExtrinsic);

				s_TrackedDeviceExtrinsics[DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle].Add(rightHandheld.deviceExtrinsic);
			}

			// Only 1 role uses leftKnee.
			if (leftKnee.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(leftKnee.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);

				s_TrackedDeviceExtrinsics[DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle].Add(leftKnee.deviceExtrinsic);
			}
			// 3 roles use leftAnkle
			if (leftAnkle.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(leftAnkle.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);

				s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Wrist_Ankle].Add(leftAnkle.deviceExtrinsic);
				s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Handheld_Hand_Ankle].Add(leftAnkle.deviceExtrinsic);

				s_TrackedDeviceExtrinsics[DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle].Add(leftAnkle.deviceExtrinsic);
			}
			// 2 roles use leftFoot
			if (leftFoot.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(leftFoot.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);

				s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Wrist_Foot].Add(leftFoot.deviceExtrinsic);
				s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Handheld_Hand_Foot].Add(leftFoot.deviceExtrinsic);
			}

			// Only 1 role uses rightKnee.
			if (rightKnee.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(rightKnee.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);

				s_TrackedDeviceExtrinsics[DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle].Add(rightKnee.deviceExtrinsic);
			}
			// 3 roles use rightAnkle
			if (rightAnkle.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(rightAnkle.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);

				s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Wrist_Ankle].Add(rightAnkle.deviceExtrinsic);
				s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Handheld_Hand_Ankle].Add(rightAnkle.deviceExtrinsic);

				s_TrackedDeviceExtrinsics[DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle].Add(rightAnkle.deviceExtrinsic);
			}
			// 2 roles use rightFoot
			if (rightFoot.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(rightFoot.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);

				s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Wrist_Foot].Add(rightFoot.deviceExtrinsic);
				s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Handheld_Hand_Foot].Add(rightFoot.deviceExtrinsic);
			}

			if (trackedDevicesArrays == null) { trackedDevicesArrays = new Dictionary<DeviceExtRole, TrackedDeviceExtrinsic[]>(); }
			trackedDevicesArrays.Clear();

			trackedDevicesArrays.Add(DeviceExtRole.Arm_Wrist, s_TrackedDeviceExtrinsics[DeviceExtRole.Arm_Wrist].ToArray());
			trackedDevicesArrays.Add(DeviceExtRole.UpperBody_Wrist, s_TrackedDeviceExtrinsics[DeviceExtRole.UpperBody_Wrist].ToArray());
			trackedDevicesArrays.Add(DeviceExtRole.FullBody_Wrist_Ankle, s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Wrist_Ankle].ToArray());
			trackedDevicesArrays.Add(DeviceExtRole.FullBody_Wrist_Foot, s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Wrist_Foot].ToArray());

			trackedDevicesArrays.Add(DeviceExtRole.Arm_Handheld_Hand, s_TrackedDeviceExtrinsics[DeviceExtRole.Arm_Handheld_Hand].ToArray());
			trackedDevicesArrays.Add(DeviceExtRole.UpperBody_Handheld_Hand, s_TrackedDeviceExtrinsics[DeviceExtRole.UpperBody_Handheld_Hand].ToArray());
			trackedDevicesArrays.Add(DeviceExtRole.FullBody_Handheld_Hand_Ankle, s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Handheld_Hand_Ankle].ToArray());
			trackedDevicesArrays.Add(DeviceExtRole.FullBody_Handheld_Hand_Foot, s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Handheld_Hand_Foot].ToArray());

			trackedDevicesArrays.Add(DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle, s_TrackedDeviceExtrinsics[DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle].ToArray());

			getDeviceExtrinsicsFirstTime = true;
		}

		public BodyTrackedDevice()
		{
			hip.deviceExtrinsic.trackedDeviceRole = TrackedDeviceRole.ROLE_HIP;
			chest.deviceExtrinsic.trackedDeviceRole = TrackedDeviceRole.ROLE_CHEST;
			head.deviceExtrinsic.trackedDeviceRole = TrackedDeviceRole.ROLE_HEAD;

			leftElbow.deviceExtrinsic.trackedDeviceRole = TrackedDeviceRole.ROLE_LEFTELBOW;
			leftWrist.deviceExtrinsic.trackedDeviceRole = TrackedDeviceRole.ROLE_LEFTWRIST;
			leftHand.deviceExtrinsic.trackedDeviceRole = TrackedDeviceRole.ROLE_LEFTHAND;
			leftHandheld.deviceExtrinsic.trackedDeviceRole = TrackedDeviceRole.ROLE_LEFTHANDHELD;

			rightElbow.deviceExtrinsic.trackedDeviceRole = TrackedDeviceRole.ROLE_RIGHTELBOW;
			rightWrist.deviceExtrinsic.trackedDeviceRole = TrackedDeviceRole.ROLE_RIGHTWRIST;
			rightHand.deviceExtrinsic.trackedDeviceRole = TrackedDeviceRole.ROLE_RIGHTHAND;
			rightHandheld.deviceExtrinsic.trackedDeviceRole = TrackedDeviceRole.ROLE_RIGHTHANDHELD;

			leftKnee.deviceExtrinsic.trackedDeviceRole = TrackedDeviceRole.ROLE_LEFTKNEE;
			leftAnkle.deviceExtrinsic.trackedDeviceRole = TrackedDeviceRole.ROLE_LEFTANKLE;
			leftFoot.deviceExtrinsic.trackedDeviceRole = TrackedDeviceRole.ROLE_LEFTFOOT;

			rightKnee.deviceExtrinsic.trackedDeviceRole = TrackedDeviceRole.ROLE_RIGHTKNEE;
			rightAnkle.deviceExtrinsic.trackedDeviceRole = TrackedDeviceRole.ROLE_RIGHTANKLE;
			rightFoot.deviceExtrinsic.trackedDeviceRole = TrackedDeviceRole.ROLE_RIGHTFOOT;
		}
		public void Update(BodyTrackedDevice in_device)
		{
			if (in_device == null) { return; }

			hip.Update(in_device.hip);
			chest.Update(in_device.chest);
			head.Update(in_device.head);

			leftElbow.Update(in_device.leftElbow);
			leftWrist.Update(in_device.leftWrist);
			leftHand.Update(in_device.leftHand);
			leftHandheld.Update(in_device.leftHandheld);

			rightElbow.Update(in_device.rightElbow);
			rightWrist.Update(in_device.rightWrist);
			rightHand.Update(in_device.rightHand);
			rightHandheld.Update(in_device.rightHandheld);

			leftKnee.Update(in_device.leftKnee);
			leftAnkle.Update(in_device.leftAnkle);
			leftFoot.Update(in_device.leftFoot);

			rightKnee.Update(in_device.rightKnee);
			rightAnkle.Update(in_device.rightAnkle);
			rightFoot.Update(in_device.rightFoot);

			UpdateTrackedDevicesArray();
		}
		/// <summary> Should only be called in CreateBodyTracking() </summary>
		public void Update([In] TrackerExtrinsic in_ext)
		{
			if (in_ext == null) { return; }
			sb.Clear().Append("Update() TrackerExtrinsic of each device."); DEBUG(sb);

			hip.Update(in_ext.hip); // 0
			chest.Update(in_ext.chest);
			head.Update(in_ext.head);

			leftElbow.Update(in_ext.leftElbow);
			leftWrist.Update(in_ext.leftWrist);
			leftHand.Update(in_ext.leftHand); // 5
			leftHandheld.Update(in_ext.leftHandheld);

			rightElbow.Update(in_ext.rightElbow);
			rightWrist.Update(in_ext.rightWrist);
			rightHand.Update(in_ext.rightHand);
			rightHandheld.Update(in_ext.rightHandheld); // 10

			leftKnee.Update(in_ext.leftKnee);
			leftAnkle.Update(in_ext.leftAnkle);
			leftFoot.Update(in_ext.leftFoot);

			rightKnee.Update(in_ext.rightKnee);
			rightAnkle.Update(in_ext.rightAnkle); // 15
			rightFoot.Update(in_ext.rightFoot);

			UpdateTrackedDevicesArray();
		}

		/// <summary> The device extrinsics for use depends on the calibration pose role. </summary>
		public bool GetDevicesExtrinsics(BodyPoseRole calibRole, out TrackedDeviceExtrinsic[] bodyTrackedDevices, out UInt32 bodyTrackedDeviceCount)
		{
			logFrame++;
			logFrame %= 500;
			printIntervalLog = (logFrame == 0);

			// Upper Body + Leg FK
			if (calibRole == BodyPoseRole.UpperBody_Handheld_Knee_Ankle || calibRole == BodyPoseRole.UpperBody_Hand_Knee_Ankle)
			{
				bodyTrackedDevices = trackedDevicesArrays[DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle];
				bodyTrackedDeviceCount = (UInt32)(bodyTrackedDevices.Length & 0x7FFFFFFF);

				if (printIntervalLog || getDeviceExtrinsicsFirstTime)
				{
					sb.Clear().Append("GetDevicesExtrinsics() of extrinsic role ").Append(DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle.Name()); DEBUG(sb);
					sb.Clear();
					for (int i = 0; i < bodyTrackedDeviceCount; i++)
					{
						sb.Append("GetDevicesExtrinsics() Add extrinsic[").Append(i).Append("] ")
							.Append(bodyTrackedDevices[i].trackedDeviceRole.Name()).Append("\n");
					}
					DEBUG(sb);
				}
				return bodyTrackedDeviceCount > 0;
			}

			// Full Body
			if (calibRole == BodyPoseRole.FullBody_Wrist_Ankle)
			{
				bodyTrackedDevices = trackedDevicesArrays[DeviceExtRole.FullBody_Wrist_Ankle];
				bodyTrackedDeviceCount = (UInt32)(bodyTrackedDevices.Length & 0x7FFFFFFF);

				if (printIntervalLog || getDeviceExtrinsicsFirstTime)
				{
					sb.Clear().Append("GetDevicesExtrinsics() of extrinsic role ").Append(DeviceExtRole.FullBody_Wrist_Ankle.Name()); DEBUG(sb);
					sb.Clear();
					for (int i = 0; i < bodyTrackedDeviceCount; i++)
					{
						sb.Append("GetDevicesExtrinsics() Add extrinsic[").Append(i).Append("] ")
							.Append(bodyTrackedDevices[i].trackedDeviceRole.Name()).Append("\n");
					}
					DEBUG(sb);
				}
				return bodyTrackedDeviceCount > 0;
			}
			if (calibRole == BodyPoseRole.FullBody_Wrist_Foot)
			{
				bodyTrackedDevices = trackedDevicesArrays[DeviceExtRole.FullBody_Wrist_Foot];
				bodyTrackedDeviceCount = (UInt32)(bodyTrackedDevices.Length & 0x7FFFFFFF);

				if (printIntervalLog || getDeviceExtrinsicsFirstTime)
				{
					sb.Clear().Append("GetDevicesExtrinsics() of extrinsic role ").Append(DeviceExtRole.FullBody_Wrist_Foot.Name()); DEBUG(sb);
					sb.Clear();
					for (int i = 0; i < bodyTrackedDeviceCount; i++)
					{
						sb.Append("GetDevicesExtrinsics() Add extrinsic[").Append(i).Append("] ")
							.Append(bodyTrackedDevices[i].trackedDeviceRole.Name()).Append("\n");
					}
					DEBUG(sb);
				}
				return bodyTrackedDeviceCount > 0;
			}
			if (calibRole == BodyPoseRole.FullBody_Handheld_Ankle || calibRole == BodyPoseRole.FullBody_Hand_Ankle)
			{
				bodyTrackedDevices = trackedDevicesArrays[DeviceExtRole.FullBody_Handheld_Hand_Ankle];
				bodyTrackedDeviceCount = (UInt32)(bodyTrackedDevices.Length & 0x7FFFFFFF);

				if (printIntervalLog || getDeviceExtrinsicsFirstTime)
				{
					sb.Clear().Append("GetDevicesExtrinsics() of extrinsic role ").Append(DeviceExtRole.FullBody_Handheld_Hand_Ankle.Name()); DEBUG(sb);
					sb.Clear();
					for (int i = 0; i < bodyTrackedDeviceCount; i++)
					{
						sb.Append("GetDevicesExtrinsics() Add extrinsic[").Append(i).Append("] ")
							.Append(bodyTrackedDevices[i].trackedDeviceRole.Name()).Append("\n");
					}
					DEBUG(sb);
				}
				return bodyTrackedDeviceCount > 0;
			}
			if (calibRole == BodyPoseRole.FullBody_Handheld_Foot || calibRole == BodyPoseRole.FullBody_Hand_Foot)
			{
				bodyTrackedDevices = trackedDevicesArrays[DeviceExtRole.FullBody_Handheld_Hand_Foot];
				bodyTrackedDeviceCount = (UInt32)(bodyTrackedDevices.Length & 0x7FFFFFFF);

				if (printIntervalLog || getDeviceExtrinsicsFirstTime)
				{
					sb.Clear().Append("GetDevicesExtrinsics() of extrinsic role ").Append(DeviceExtRole.FullBody_Handheld_Hand_Foot.Name()); DEBUG(sb);
					sb.Clear();
					for (int i = 0; i < bodyTrackedDeviceCount; i++)
					{
						sb.Append("GetDevicesExtrinsics() Add extrinsic[").Append(i).Append("] ")
							.Append(bodyTrackedDevices[i].trackedDeviceRole.Name()).Append("\n");
					}
					DEBUG(sb);
				}
				return bodyTrackedDeviceCount > 0;
			}

			// Upper Body
			if (calibRole == BodyPoseRole.UpperBody_Wrist)
			{
				bodyTrackedDevices = trackedDevicesArrays[DeviceExtRole.UpperBody_Wrist];
				bodyTrackedDeviceCount = (UInt32)(bodyTrackedDevices.Length & 0x7FFFFFFF);

				if (printIntervalLog || getDeviceExtrinsicsFirstTime)
				{
					sb.Clear().Append("GetDevicesExtrinsics() of extrinsic role ").Append(DeviceExtRole.UpperBody_Wrist.Name()); DEBUG(sb);
					sb.Clear();
					for (int i = 0; i < bodyTrackedDeviceCount; i++)
					{
						sb.Append("GetDevicesExtrinsics() Add extrinsic[").Append(i).Append("] ")
							.Append(bodyTrackedDevices[i].trackedDeviceRole.Name()).Append("\n");
					}
					DEBUG(sb);
				}
				return bodyTrackedDeviceCount > 0;
			}
			if (calibRole == BodyPoseRole.UpperBody_Handheld || calibRole == BodyPoseRole.UpperBody_Hand)
			{
				bodyTrackedDevices = trackedDevicesArrays[DeviceExtRole.UpperBody_Handheld_Hand];
				bodyTrackedDeviceCount = (UInt32)(bodyTrackedDevices.Length & 0x7FFFFFFF);

				if (printIntervalLog || getDeviceExtrinsicsFirstTime)
				{
					sb.Clear().Append("GetDevicesExtrinsics() of extrinsic role ").Append(DeviceExtRole.UpperBody_Handheld_Hand.Name()); DEBUG(sb);
					sb.Clear();
					for (int i = 0; i < bodyTrackedDeviceCount; i++)
					{
						sb.Append("GetDevicesExtrinsics() Add extrinsic[").Append(i).Append("] ")
							.Append(bodyTrackedDevices[i].trackedDeviceRole.Name()).Append("\n");
					}
					DEBUG(sb);
				}
				return bodyTrackedDeviceCount > 0;
			}

			// Arm
			if (calibRole == BodyPoseRole.Arm_Wrist)
			{
				bodyTrackedDevices = trackedDevicesArrays[DeviceExtRole.Arm_Wrist];
				bodyTrackedDeviceCount = (UInt32)(bodyTrackedDevices.Length & 0x7FFFFFFF);

				if (printIntervalLog || getDeviceExtrinsicsFirstTime)
				{
					sb.Clear().Append("GetDevicesExtrinsics() of extrinsic role ").Append(DeviceExtRole.Arm_Wrist.Name()); DEBUG(sb);
					sb.Clear();
					for (int i = 0; i < bodyTrackedDeviceCount; i++)
					{
						sb.Append("GetDevicesExtrinsics() Add extrinsic[").Append(i).Append("] ")
							.Append(bodyTrackedDevices[i].trackedDeviceRole.Name()).Append("\n");
					}
					DEBUG(sb);
				}
				return bodyTrackedDeviceCount > 0;
			}
			if (calibRole == BodyPoseRole.Arm_Handheld || calibRole == BodyPoseRole.Arm_Hand)
			{
				bodyTrackedDevices = trackedDevicesArrays[DeviceExtRole.Arm_Handheld_Hand];
				bodyTrackedDeviceCount = (UInt32)(bodyTrackedDevices.Length & 0x7FFFFFFF);

				if (printIntervalLog || getDeviceExtrinsicsFirstTime)
				{
					sb.Clear().Append("GetDevicesExtrinsics() of extrinsic role ").Append(DeviceExtRole.Arm_Handheld_Hand.Name()); DEBUG(sb);
					sb.Clear();
					for (int i = 0; i < bodyTrackedDeviceCount; i++)
					{
						sb.Append("GetDevicesExtrinsics() Add extrinsic[").Append(i).Append("] ")
							.Append(bodyTrackedDevices[i].trackedDeviceRole.Name()).Append("\n");
					}
					DEBUG(sb);
				}
				return bodyTrackedDeviceCount > 0;
			}

			bodyTrackedDevices = null;
			bodyTrackedDeviceCount = 0;
			return false;
		}

		private int ikFrame = -1;
		private DeviceExtRole m_IKRoles = DeviceExtRole.Unknown;
		public DeviceExtRole GetIKRoles(BodyPoseRole calibRole)
		{
			if (printIntervalLog || getDeviceExtrinsicsFirstTime) { sb.Clear().Append("GetIKRoles()"); DEBUG(sb); }

			if (ikFrame == Time.frameCount) { return m_IKRoles; }
			else { m_IKRoles = DeviceExtRole.Unknown; ikFrame = Time.frameCount; }

			if (GetDevicesExtrinsics(calibRole, out TrackedDeviceExtrinsic[] bodyTrackedDevices, out UInt32 bodyTrackedDeviceCount))
				m_IKRoles = BodyTrackingUtils.GetDeviceExtRole(calibRole, bodyTrackedDevices, bodyTrackedDeviceCount);

			return m_IKRoles;
		}
	}
	public class BodyIKInfo
	{
		public BodyAvatar avatar = new BodyAvatar();
		public BodyTrackedDevice trackedDevice = new BodyTrackedDevice();

		public BodyIKInfo() { }
		public BodyIKInfo(BodyAvatar in_avatar, BodyTrackedDevice in_device)
		{
			avatar = in_avatar;
			trackedDevice = in_device;
		}
		public void Update(BodyIKInfo in_info)
		{
			if (in_info == null) { return; }

			avatar.Update(in_info.avatar);
			trackedDevice.Update(in_info.trackedDevice);
		}
	}

	internal struct TrackingInfos_t
	{
		internal struct TrackingInfo_t
		{
			public WVR_BodyTrackingType type;
			public UInt32[] ids;
			public UInt32 count;

			public TrackingInfo_t(WVR_BodyTrackingType in_type, UInt32[] in_ids, UInt32 in_count)
			{
				type = in_type;
				ids = in_ids;
				count = in_count;
			}
			public void Update(TrackingInfo_t in_info)
			{
				type = in_info.type;
				if (count != in_info.count && in_info.count > 0)
				{
					count = in_info.count;
					ids = new UInt32[count];
				}
				for (UInt32 i = 0; i < count; i++)
					ids[i] = in_info.ids[i];
			}
		}
		
		public TrackingInfo_t[] s_info;
		public UInt32 size;
		public TrackingInfos_t(TrackingInfo_t[] in_info, UInt32 in_size)
		{
			s_info = in_info;
			size = in_size;
		}
		public void Update(TrackingInfos_t in_infos)
		{
			if (size != in_infos.size && in_infos.size > 0)
			{
				size = in_infos.size;
				s_info = new TrackingInfo_t[size];
				for (UInt32 i = 0; i < size; i++)
					s_info[i].Update(in_infos.s_info[i]);
			}
		}
		public void Update(TrackingInfo_t in_info)
		{
			for (int i = 0; i < s_info.Length; i++)
			{
				if (s_info[i].type == in_info.type)
					s_info[i].Update(in_info);
			}
		}
	}
	public class BodyPose
	{
		const string LOG_TAG = "Wave.Essence.BodyTracking.BodyPose";
		StringBuilder m_sb = null;
		StringBuilder sb {
			get {
				if (m_sb == null) { m_sb = new StringBuilder(); }
				return m_sb;
			}
		}
		void DEBUG(StringBuilder msg)
		{
			if (Log.EnableDebugLog)
				Log.d(LOG_TAG, msg, true);
		}
		static int logFrame = 0;
		bool printIntervalLog = false;
		void ERROR(StringBuilder msg) { Log.e(LOG_TAG, msg, true); }

		public TrackedDevicePose hip = TrackedDevicePose.identity;
		public TrackedDevicePose chest = TrackedDevicePose.identity;
		public TrackedDevicePose head = TrackedDevicePose.identity;

		public TrackedDevicePose leftElbow = TrackedDevicePose.identity;
		public TrackedDevicePose leftWrist = TrackedDevicePose.identity;
		public TrackedDevicePose leftHand = TrackedDevicePose.identity;
		public TrackedDevicePose leftHandheld = TrackedDevicePose.identity;

		public TrackedDevicePose rightElbow = TrackedDevicePose.identity;
		public TrackedDevicePose rightWrist = TrackedDevicePose.identity;
		public TrackedDevicePose rightHand = TrackedDevicePose.identity;
		public TrackedDevicePose rightHandheld = TrackedDevicePose.identity;

		public TrackedDevicePose leftKnee = TrackedDevicePose.identity;
		public TrackedDevicePose leftAnkle = TrackedDevicePose.identity;
		public TrackedDevicePose leftFoot = TrackedDevicePose.identity;

		public TrackedDevicePose rightKnee = TrackedDevicePose.identity;
		public TrackedDevicePose rightAnkle = TrackedDevicePose.identity;
		public TrackedDevicePose rightFoot = TrackedDevicePose.identity;

		internal TrackingInfos_t trackingInfos;

		internal Dictionary<TrackedDeviceRole, WVR_BodyTrackingType> s_TrackedDeviceType = null;
		private void InitTrackedDeviceType()
		{
			if (s_TrackedDeviceType == null)
			{
				s_TrackedDeviceType = new Dictionary<TrackedDeviceRole, WVR_BodyTrackingType>();

				s_TrackedDeviceType.Add(TrackedDeviceRole.ROLE_HIP, WVR_BodyTrackingType.WVR_BodyTrackingType_Invalid);
				s_TrackedDeviceType.Add(TrackedDeviceRole.ROLE_CHEST, WVR_BodyTrackingType.WVR_BodyTrackingType_Invalid);
				s_TrackedDeviceType.Add(TrackedDeviceRole.ROLE_HEAD, WVR_BodyTrackingType.WVR_BodyTrackingType_Invalid);

				s_TrackedDeviceType.Add(TrackedDeviceRole.ROLE_LEFTELBOW, WVR_BodyTrackingType.WVR_BodyTrackingType_Invalid);
				s_TrackedDeviceType.Add(TrackedDeviceRole.ROLE_LEFTWRIST, WVR_BodyTrackingType.WVR_BodyTrackingType_Invalid);
				s_TrackedDeviceType.Add(TrackedDeviceRole.ROLE_LEFTHAND, WVR_BodyTrackingType.WVR_BodyTrackingType_Invalid);
				s_TrackedDeviceType.Add(TrackedDeviceRole.ROLE_LEFTHANDHELD, WVR_BodyTrackingType.WVR_BodyTrackingType_Invalid);

				s_TrackedDeviceType.Add(TrackedDeviceRole.ROLE_RIGHTELBOW, WVR_BodyTrackingType.WVR_BodyTrackingType_Invalid);
				s_TrackedDeviceType.Add(TrackedDeviceRole.ROLE_RIGHTWRIST, WVR_BodyTrackingType.WVR_BodyTrackingType_Invalid);
				s_TrackedDeviceType.Add(TrackedDeviceRole.ROLE_RIGHTHAND, WVR_BodyTrackingType.WVR_BodyTrackingType_Invalid);
				s_TrackedDeviceType.Add(TrackedDeviceRole.ROLE_RIGHTHANDHELD, WVR_BodyTrackingType.WVR_BodyTrackingType_Invalid);

				s_TrackedDeviceType.Add(TrackedDeviceRole.ROLE_LEFTKNEE, WVR_BodyTrackingType.WVR_BodyTrackingType_Invalid);
				s_TrackedDeviceType.Add(TrackedDeviceRole.ROLE_LEFTANKLE, WVR_BodyTrackingType.WVR_BodyTrackingType_Invalid);
				s_TrackedDeviceType.Add(TrackedDeviceRole.ROLE_LEFTFOOT, WVR_BodyTrackingType.WVR_BodyTrackingType_Invalid);

				s_TrackedDeviceType.Add(TrackedDeviceRole.ROLE_RIGHTKNEE, WVR_BodyTrackingType.WVR_BodyTrackingType_Invalid);
				s_TrackedDeviceType.Add(TrackedDeviceRole.ROLE_RIGHTANKLE, WVR_BodyTrackingType.WVR_BodyTrackingType_Invalid);
				s_TrackedDeviceType.Add(TrackedDeviceRole.ROLE_RIGHTFOOT, WVR_BodyTrackingType.WVR_BodyTrackingType_Invalid);
			}
			else
			{
				for (int i = 0; i < s_TrackedDeviceType.Count; i++)
					s_TrackedDeviceType[s_TrackedDeviceType.ElementAt(i).Key] = WVR_BodyTrackingType.WVR_BodyTrackingType_Invalid;
			}
		}

		public BodyPose()
		{
			hip.trackedDeviceRole = TrackedDeviceRole.ROLE_HIP;
			chest.trackedDeviceRole = TrackedDeviceRole.ROLE_CHEST;
			head.trackedDeviceRole = TrackedDeviceRole.ROLE_HEAD;

			leftElbow.trackedDeviceRole = TrackedDeviceRole.ROLE_LEFTELBOW;
			leftWrist.trackedDeviceRole = TrackedDeviceRole.ROLE_LEFTWRIST;
			leftHand.trackedDeviceRole = TrackedDeviceRole.ROLE_LEFTHAND;
			leftHandheld.trackedDeviceRole = TrackedDeviceRole.ROLE_LEFTHANDHELD;

			rightElbow.trackedDeviceRole = TrackedDeviceRole.ROLE_RIGHTELBOW;
			rightWrist.trackedDeviceRole = TrackedDeviceRole.ROLE_RIGHTWRIST;
			rightHand.trackedDeviceRole = TrackedDeviceRole.ROLE_RIGHTHAND;
			rightHandheld.trackedDeviceRole = TrackedDeviceRole.ROLE_RIGHTHANDHELD;

			leftKnee.trackedDeviceRole = TrackedDeviceRole.ROLE_LEFTKNEE;
			leftAnkle.trackedDeviceRole = TrackedDeviceRole.ROLE_LEFTANKLE;
			leftFoot.trackedDeviceRole = TrackedDeviceRole.ROLE_LEFTFOOT;

			rightKnee.trackedDeviceRole = TrackedDeviceRole.ROLE_RIGHTKNEE;
			rightAnkle.trackedDeviceRole = TrackedDeviceRole.ROLE_RIGHTANKLE;
			rightFoot.trackedDeviceRole = TrackedDeviceRole.ROLE_RIGHTFOOT;

			InitTrackedDeviceType();
		}
		public void Update([In] BodyPose in_body)
		{
			hip.Update(in_body.hip);
			chest.Update(in_body.chest);
			head.Update(in_body.head);

			leftElbow.Update(in_body.leftElbow);
			leftWrist.Update(in_body.leftWrist);
			leftHand.Update(in_body.leftHand);
			leftHandheld.Update(in_body.leftHandheld);

			rightElbow.Update(in_body.rightElbow);
			rightWrist.Update(in_body.rightWrist);
			rightHand.Update(in_body.rightHand);
			rightHandheld.Update(in_body.rightHandheld);

			leftKnee.Update(in_body.leftKnee);
			leftAnkle.Update(in_body.leftAnkle);
			leftFoot.Update(in_body.leftFoot);

			rightKnee.Update(in_body.rightKnee);
			rightAnkle.Update(in_body.rightAnkle);
			rightFoot.Update(in_body.rightFoot);

			trackingInfos.Update(in_body.trackingInfos);

			if (s_TrackedDeviceType != null) { s_TrackedDeviceType.Clear(); }
			for (int i = 0; i < in_body.s_TrackedDeviceType.Count; i++)
			{
				s_TrackedDeviceType.Add(
					in_body.s_TrackedDeviceType.ElementAt(i).Key,
					in_body.s_TrackedDeviceType.ElementAt(i).Value);
			}
		}

		private void ResetPose()
		{
			hip.poseState = PoseState.NODATA;
			chest.poseState = PoseState.NODATA;
			head.poseState = PoseState.NODATA;

			leftElbow.poseState = PoseState.NODATA;
			leftWrist.poseState = PoseState.NODATA;
			leftHand.poseState = PoseState.NODATA;
			leftHandheld.poseState = PoseState.NODATA;

			rightElbow.poseState = PoseState.NODATA;
			rightWrist.poseState = PoseState.NODATA;
			rightHand.poseState = PoseState.NODATA;
			rightHandheld.poseState = PoseState.NODATA;

			leftKnee.poseState = PoseState.NODATA;
			leftAnkle.poseState = PoseState.NODATA;
			leftFoot.poseState = PoseState.NODATA;

			rightKnee.poseState = PoseState.NODATA;
			rightAnkle.poseState = PoseState.NODATA;
			rightFoot.poseState = PoseState.NODATA;

			InitTrackedDeviceType();
		}
		private bool getDevicePosesFirstTime = true;
		public void Clear()
		{
			ResetPose();
			getDevicePosesFirstTime = true;
		}

		#region Update Tracking Infos and Standard Pose from runtime.
		/// <summary>
		/// Updates 1.Body Pose from runtime cached data, 2.s_TrackedDeviceType.
		/// <br></br>
		/// Called Time: GetStandardPoseDefault() from StartBodyTracking().
		/// </summary>
		internal BodyTrackingResult UpdatePose(WVR_BodyTrackingType type, UInt32 id, WVR_Pose_t pose)
		{
			if (type == WVR_BodyTrackingType.WVR_BodyTrackingType_Invalid) { return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID; };

			if (type == WVR_BodyTrackingType.WVR_BodyTrackingType_HMD)
			{
				BodyTrackingUtils.Update(ref head.rotation, Coordinate.GetQuaternionFromGL(pose.rotation));
				head.translation = Coordinate.GetVectorFromGL(pose.position);
				head.poseState = PoseState.ROTATION | PoseState.TRANSLATION;
				s_TrackedDeviceType[head.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_HMD;
			}
			if (type == WVR_BodyTrackingType.WVR_BodyTrackingType_Controller)
			{
				if (id == (UInt32)WVR_DeviceType.WVR_DeviceType_Controller_Left)
				{
					BodyTrackingUtils.Update(ref leftHandheld.rotation, Coordinate.GetQuaternionFromGL(pose.rotation));
					leftHandheld.translation = Coordinate.GetVectorFromGL(pose.position);
					leftHandheld.poseState = PoseState.ROTATION | PoseState.TRANSLATION;
					s_TrackedDeviceType[leftHandheld.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_Controller;
				}
				if (id == (UInt32)WVR_DeviceType.WVR_DeviceType_Controller_Right)
				{
					BodyTrackingUtils.Update(ref rightHandheld.rotation, Coordinate.GetQuaternionFromGL(pose.rotation));
					rightHandheld.translation = Coordinate.GetVectorFromGL(pose.position);
					rightHandheld.poseState = PoseState.ROTATION | PoseState.TRANSLATION;
					s_TrackedDeviceType[rightHandheld.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_Controller;
				}
			}
			if (type == WVR_BodyTrackingType.WVR_BodyTrackingType_Hand)
			{
				if (id == (UInt32)WVR_DeviceType.WVR_DeviceType_NaturalHand_Left)
				{
					BodyTrackingUtils.Update(ref leftHand.rotation, Coordinate.GetQuaternionFromGL(pose.rotation));
					leftHand.translation = Coordinate.GetVectorFromGL(pose.position);
					leftHand.poseState = PoseState.ROTATION | PoseState.TRANSLATION;
					s_TrackedDeviceType[leftHand.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_Hand;
				}
				if (id == (UInt32)WVR_DeviceType.WVR_DeviceType_NaturalHand_Right)
				{
					BodyTrackingUtils.Update(ref rightHand.rotation, Coordinate.GetQuaternionFromGL(pose.rotation));
					rightHand.translation = Coordinate.GetVectorFromGL(pose.position);
					rightHand.poseState = PoseState.ROTATION | PoseState.TRANSLATION;
					s_TrackedDeviceType[rightHand.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_Hand;
				}
			}
			if (type == WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker)
			{
				TrackerId tracker = TrackerUtils.s_TrackerIds[id];
				TrackerRole role = TrackerManager.Instance.GetTrackerRole(tracker);

				if (role == TrackerRole.Wrist_Left)
				{
					BodyTrackingUtils.Update(ref leftWrist.rotation, Coordinate.GetQuaternionFromGL(pose.rotation));
					leftWrist.translation = Coordinate.GetVectorFromGL(pose.position);
					leftWrist.poseState = PoseState.ROTATION | PoseState.TRANSLATION;
					s_TrackedDeviceType[leftWrist.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker;
				}
				if (role == TrackerRole.Wrist_Right)
				{
					BodyTrackingUtils.Update(ref rightWrist.rotation, Coordinate.GetQuaternionFromGL(pose.rotation));
					rightWrist.translation = Coordinate.GetVectorFromGL(pose.position);
					rightWrist.poseState = PoseState.ROTATION | PoseState.TRANSLATION;
					s_TrackedDeviceType[rightWrist.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker;
				}
				if (role == TrackerRole.Waist)
				{
					BodyTrackingUtils.Update(ref hip.rotation, Coordinate.GetQuaternionFromGL(pose.rotation));
					hip.translation = Coordinate.GetVectorFromGL(pose.position);
					hip.poseState = PoseState.ROTATION | PoseState.TRANSLATION;
					s_TrackedDeviceType[hip.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker;
				}
				if (role == TrackerRole.Ankle_Left)
				{
					BodyTrackingUtils.Update(ref leftAnkle.rotation, Coordinate.GetQuaternionFromGL(pose.rotation));
					leftAnkle.translation = Coordinate.GetVectorFromGL(pose.position);
					leftAnkle.poseState = PoseState.ROTATION | PoseState.TRANSLATION;
					s_TrackedDeviceType[leftAnkle.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker;
				}
				if (role == TrackerRole.Ankle_Right)
				{
					BodyTrackingUtils.Update(ref rightAnkle.rotation, Coordinate.GetQuaternionFromGL(pose.rotation));
					rightAnkle.translation = Coordinate.GetVectorFromGL(pose.position);
					rightAnkle.poseState = PoseState.ROTATION | PoseState.TRANSLATION;
					s_TrackedDeviceType[rightAnkle.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker;
				}
				if (role == TrackerRole.Foot_Left)
				{
					BodyTrackingUtils.Update(ref leftFoot.rotation, Coordinate.GetQuaternionFromGL(pose.rotation));
					leftFoot.translation = Coordinate.GetVectorFromGL(pose.position);
					leftFoot.poseState = PoseState.ROTATION | PoseState.TRANSLATION;
					s_TrackedDeviceType[leftFoot.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker;
				}
				if (role == TrackerRole.Foot_Right)
				{
					BodyTrackingUtils.Update(ref rightFoot.rotation, Coordinate.GetQuaternionFromGL(pose.rotation));
					rightFoot.translation = Coordinate.GetVectorFromGL(pose.position);
					rightFoot.poseState = PoseState.ROTATION | PoseState.TRANSLATION;
					s_TrackedDeviceType[rightFoot.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker;
				}
			}
			if (type == WVR_BodyTrackingType.WVR_BodyTrackingType_WristTracker)
			{
				TrackerId tracker = TrackerUtils.s_TrackerIds[id];
				TrackerRole role = TrackerManager.Instance.GetTrackerRole(tracker);

				if (role == TrackerRole.Pair1_Left)
				{
					BodyTrackingUtils.Update(ref leftWrist.rotation, Coordinate.GetQuaternionFromGL(pose.rotation));
					leftWrist.translation = Coordinate.GetVectorFromGL(pose.position);
					leftWrist.poseState = PoseState.ROTATION | PoseState.TRANSLATION;
					s_TrackedDeviceType[leftWrist.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_WristTracker;
				}
				if (role == TrackerRole.Pair1_Right)
				{
					BodyTrackingUtils.Update(ref rightWrist.rotation, Coordinate.GetQuaternionFromGL(pose.rotation));
					rightWrist.translation = Coordinate.GetVectorFromGL(pose.position);
					rightWrist.poseState = PoseState.ROTATION | PoseState.TRANSLATION;
					s_TrackedDeviceType[rightWrist.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_WristTracker;
				}
			}
			if (type == WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTrackerIM)
			{
				TrackerId tracker = TrackerUtils.s_TrackerIds[id];
				TrackerRole role = TrackerManager.Instance.GetTrackerRole(tracker);

				if (role == TrackerRole.Knee_Left)
				{
					BodyTrackingUtils.Update(ref leftKnee.rotation, Coordinate.GetQuaternionFromGL(pose.rotation));
					leftKnee.poseState = PoseState.ROTATION; // Self Tracker IM has ROTATION only.
					s_TrackedDeviceType[leftKnee.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTrackerIM;
				}
				if (role == TrackerRole.Knee_Right)
				{
					BodyTrackingUtils.Update(ref rightKnee.rotation, Coordinate.GetQuaternionFromGL(pose.rotation));
					rightKnee.poseState = PoseState.ROTATION; // Self Tracker IM has ROTATION only.
					s_TrackedDeviceType[rightKnee.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTrackerIM;
				}
				if (role == TrackerRole.Ankle_Left)
				{
					BodyTrackingUtils.Update(ref leftAnkle.rotation, Coordinate.GetQuaternionFromGL(pose.rotation));
					leftAnkle.poseState = PoseState.ROTATION; // Self Tracker IM has ROTATION only.
					s_TrackedDeviceType[leftAnkle.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTrackerIM;
				}
				if (role == TrackerRole.Ankle_Right)
				{
					BodyTrackingUtils.Update(ref rightAnkle.rotation, Coordinate.GetQuaternionFromGL(pose.rotation));
					rightAnkle.poseState = PoseState.ROTATION; // Self Tracker IM has ROTATION only.
					s_TrackedDeviceType[rightAnkle.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTrackerIM;
				}
			}

			return BodyTrackingResult.SUCCESS;
		}
		internal void InitTrackingInfos([In] TrackingInfos_t in_trackingInfos)
		{
			trackingInfos.Update(in_trackingInfos);
		}
		#endregion

		#region Update Tracking Infos and Standard Pose in content.
		/// <summary>
		/// Updates 1. Body Pose according to trackingInfos, 2.s_TrackedDeviceType.
		/// <br></br>
		/// Called time: 1. After InitTrackingInfos. 2. Each frame from UpdateBodyTrackingOnce().
		/// </summary>
		internal BodyTrackingResult UpdatePose()
		{
			if (trackingInfos.size == 0)
			{
				sb.Clear().Append("UpdatePose() No pose to update."); ERROR(sb);
				return BodyTrackingResult.ERROR_IK_NOT_UPDATED;
			}
			ResetPose();
			for (int info_index = 0; info_index < trackingInfos.size; info_index++)
			{
				TrackingInfos_t.TrackingInfo_t info = trackingInfos.s_info[info_index];

				if (info.type == WVR_BodyTrackingType.WVR_BodyTrackingType_HMD)
				{
					if (InputDeviceControl.GetRotation(InputDeviceControl.ControlDevice.Head, out Quaternion rotation))
					{
						BodyTrackingUtils.Update(ref head.rotation, rotation);
						head.poseState |= PoseState.ROTATION;
						s_TrackedDeviceType[head.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_HMD;
					}
					else
					{
						sb.Clear().Append("UpdatePose() Invalid HMD rotation."); ERROR(sb);
						return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
					}
					if (InputDeviceControl.GetPosition(InputDeviceControl.ControlDevice.Head, out Vector3 position))
					{
						head.translation = position;
						head.poseState |= PoseState.TRANSLATION;
					}
					else
					{
						sb.Clear().Append("UpdatePose() Invalid HMD position."); ERROR(sb);
						return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
					}
					if (InputDeviceControl.GetVelocity(InputDeviceControl.ControlDevice.Head, out Vector3 velocity))
						head.velocity = velocity;
					else
					{
						sb.Clear().Append("UpdatePose() Invalid HMD velocity."); ERROR(sb);
						return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
					}
					if (InputDeviceControl.GetAngularVelocity(InputDeviceControl.ControlDevice.Head, out Vector3 angularVelocity))
						head.angularVelocity = angularVelocity;
					else
					{
						sb.Clear().Append("UpdatePose() Invalid HMD angularVelocity."); ERROR(sb);
						return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
					}
					if (InputDeviceControl.GetAcceleration(InputDeviceControl.ControlDevice.Head, out Vector3 acceleration))
						head.acceleration = acceleration;
					else
					{
						sb.Clear().Append("UpdatePose() Invalid HMD acceleration."); ERROR(sb);
						return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
					}
				}
				if (info.type == WVR_BodyTrackingType.WVR_BodyTrackingType_Controller ||
					info.type == WVR_BodyTrackingType.WVR_BodyTrackingType_Hand)
				{
					for (int ids_index = 0; ids_index < info.count; ids_index++)
					{
						if (info.ids[ids_index] == (UInt32)WVR_DeviceType.WVR_DeviceType_Controller_Left ||
							info.ids[ids_index] == (UInt32)WVR_DeviceType.WVR_DeviceType_NaturalHand_Left)
						{
							if (InputDeviceControl.GetRotation(InputDeviceControl.ControlDevice.Left, out Quaternion controllerRot) &&
								!WXRDevice.IsTableStatic(XR_Hand.Left))
							{
								BodyTrackingUtils.Update(ref leftHandheld.rotation, controllerRot);
								leftHandheld.poseState |= PoseState.ROTATION;
								s_TrackedDeviceType[leftHandheld.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_Controller;
							}
							else
							{
								// FORCE set type due to library supports NODATA when using Hand.
								s_TrackedDeviceType[leftHand.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_Hand;
								if (InputDeviceHand.IsTracked(true))
								{
									Bone palmLeft = InputDeviceHand.GetPalm(true);
									if (palmLeft.TryGetRotation(out Quaternion handRot))
									{
										BodyTrackingUtils.Update(ref leftHand.rotation, handRot);
										leftHand.poseState |= PoseState.ROTATION;
									}
								}
							}
							if (InputDeviceControl.GetPosition(InputDeviceControl.ControlDevice.Left, out Vector3 controllerPos) &&
								!WXRDevice.IsTableStatic(XR_Hand.Left))
							{
								leftHandheld.translation = controllerPos;
								leftHandheld.poseState |= PoseState.TRANSLATION;
							}
							else
							{
								if (InputDeviceHand.IsTracked(true))
								{
									Bone palmLeft = InputDeviceHand.GetPalm(true);
									if (palmLeft.TryGetPosition(out Vector3 handPos))
									{
										leftHand.translation = handPos;
										leftHand.poseState |= PoseState.TRANSLATION;
									}
								}
							}
							if (InputDeviceControl.GetVelocity(InputDeviceControl.ControlDevice.Left, out Vector3 velocity))
								leftHandheld.velocity = velocity;
							if (InputDeviceControl.GetAngularVelocity(InputDeviceControl.ControlDevice.Left, out Vector3 angularVelocity))
								leftHandheld.angularVelocity = angularVelocity;
							if (InputDeviceControl.GetAcceleration(InputDeviceControl.ControlDevice.Left, out Vector3 acceleration))
								leftHandheld.acceleration = acceleration;
						}
						if (info.ids[ids_index] == (UInt32)WVR_DeviceType.WVR_DeviceType_Controller_Right ||
							info.ids[ids_index] == (UInt32)WVR_DeviceType.WVR_DeviceType_NaturalHand_Right)
						{
							if (InputDeviceControl.GetRotation(InputDeviceControl.ControlDevice.Right, out Quaternion controllerRot) &&
								!WXRDevice.IsTableStatic(XR_Hand.Right))
							{
								BodyTrackingUtils.Update(ref rightHandheld.rotation, controllerRot);
								rightHandheld.poseState |= PoseState.ROTATION;
								s_TrackedDeviceType[rightHandheld.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_Controller;
							}
							else
							{
								// FORCE set type due to library supports NODATA when using Hand.
								s_TrackedDeviceType[rightHand.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_Hand;

								if (InputDeviceHand.IsTracked(false))
								{
									Bone palmRight = InputDeviceHand.GetPalm(false);
									if (palmRight.TryGetRotation(out Quaternion handRot))
									{
										BodyTrackingUtils.Update(ref rightHand.rotation, handRot);
										rightHand.poseState |= PoseState.ROTATION;
									}
								}
							}
							if (InputDeviceControl.GetPosition(InputDeviceControl.ControlDevice.Right, out Vector3 controllerPos) &&
								!WXRDevice.IsTableStatic(XR_Hand.Right))
							{
								rightHandheld.translation = controllerPos;
								rightHandheld.poseState |= PoseState.TRANSLATION;
							}
							else
							{
								if (InputDeviceHand.IsTracked(false))
								{
									Bone palmRight = InputDeviceHand.GetPalm(false);
									if (palmRight.TryGetPosition(out Vector3 handPos))
									{
										rightHand.translation = handPos;
										rightHand.poseState |= PoseState.TRANSLATION;
									}
								}
							}
							if (InputDeviceControl.GetVelocity(InputDeviceControl.ControlDevice.Right, out Vector3 velocity))
								rightHandheld.velocity = velocity;
							if (InputDeviceControl.GetAngularVelocity(InputDeviceControl.ControlDevice.Right, out Vector3 angularVelocity))
								rightHandheld.angularVelocity = angularVelocity;
							if (InputDeviceControl.GetAcceleration(InputDeviceControl.ControlDevice.Right, out Vector3 acceleration))
								rightHandheld.acceleration = acceleration;
						}
					}
				}
				if (info.type == WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker)
				{
					for (int ids_index = 0; ids_index < info.count && TrackerManager.Instance != null; ids_index++)
					{
						TrackerId id = TrackerUtils.s_TrackerIds[info.ids[ids_index]];
						TrackerRole role = TrackerManager.Instance.GetTrackerRole(id);

						if (printIntervalLog)
						{
							sb.Clear().Append("UpdatePose() type ").Append(info.type.Name())
								.Append(", id ").Append(id).Append(", role ").Append(role.Name());
							DEBUG(sb);
						}

						if (role == TrackerRole.Wrist_Left)
						{
							if (TrackerManager.Instance.GetTrackerRotation(id, out Quaternion rotation))
							{
								BodyTrackingUtils.Update(ref leftWrist.rotation, rotation);
								leftWrist.poseState |= PoseState.ROTATION;
								s_TrackedDeviceType[leftWrist.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker;
							}
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" rotation."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerPosition(id, out Vector3 position))
							{
								leftWrist.translation = position;
								leftWrist.poseState |= PoseState.TRANSLATION;
							}
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" position."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerVelocity(id, out Vector3 velocity))
								leftWrist.velocity = velocity;
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" velocity."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerAngularVelocity(id, out Vector3 angularVelocity))
								leftWrist.angularVelocity = angularVelocity;
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" angularVelocity."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerAcceleration(id, out Vector3 acceleration))
								leftWrist.acceleration = acceleration;
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" acceleration."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
						}
						if (role == TrackerRole.Wrist_Right)
						{
							if (TrackerManager.Instance.GetTrackerRotation(id, out Quaternion rotation))
							{
								BodyTrackingUtils.Update(ref rightWrist.rotation, rotation);
								rightWrist.poseState |= PoseState.ROTATION;
								s_TrackedDeviceType[rightWrist.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker;
							}
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" rotation."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerPosition(id, out Vector3 position))
							{
								rightWrist.translation = position;
								rightWrist.poseState |= PoseState.TRANSLATION;
							}
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" position."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerVelocity(id, out Vector3 velocity))
								rightWrist.velocity = velocity;
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" velocity."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerAngularVelocity(id, out Vector3 angularVelocity))
								rightWrist.angularVelocity = angularVelocity;
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" angularVelocity."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerAcceleration(id, out Vector3 acceleration))
								rightWrist.acceleration = acceleration;
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" acceleration."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
						}
						if (role == TrackerRole.Waist)
						{
							if (TrackerManager.Instance.GetTrackerRotation(id, out Quaternion rotation))
							{
								BodyTrackingUtils.Update(ref hip.rotation, rotation);
								hip.poseState |= PoseState.ROTATION;
								s_TrackedDeviceType[hip.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker;
							}
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" rotation."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerPosition(id, out Vector3 position))
							{
								hip.translation = position;
								hip.poseState |= PoseState.TRANSLATION;
							}
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" position."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerVelocity(id, out Vector3 velocity))
								hip.velocity = velocity;
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" velocity."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerAngularVelocity(id, out Vector3 angularVelocity))
								hip.angularVelocity = angularVelocity;
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" angularVelocity."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerAcceleration(id, out Vector3 acceleration))
								hip.acceleration = acceleration;
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" acceleration."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
						}
						if (role == TrackerRole.Ankle_Left)
						{
							if (TrackerManager.Instance.GetTrackerRotation(id, out Quaternion rotation))
							{
								BodyTrackingUtils.Update(ref leftAnkle.rotation, rotation);
								leftAnkle.poseState |= PoseState.ROTATION;
								s_TrackedDeviceType[leftAnkle.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker;
							}
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" rotation."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerPosition(id, out Vector3 position))
							{
								leftAnkle.translation = position;
								leftAnkle.poseState |= PoseState.TRANSLATION;
							}
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" position."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerVelocity(id, out Vector3 velocity))
								leftAnkle.velocity = velocity;
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" velocity."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerAngularVelocity(id, out Vector3 angularVelocity))
								leftAnkle.angularVelocity = angularVelocity;
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" angularVelocity."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerAcceleration(id, out Vector3 acceleration))
								leftAnkle.acceleration = acceleration;
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" acceleration."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
						}
						if (role == TrackerRole.Ankle_Right)
						{
							if (TrackerManager.Instance.GetTrackerRotation(id, out Quaternion rotation))
							{
								BodyTrackingUtils.Update(ref rightAnkle.rotation, rotation);
								rightAnkle.poseState |= PoseState.ROTATION;
								s_TrackedDeviceType[rightAnkle.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker;
							}
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" rotation."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerPosition(id, out Vector3 position))
							{
								rightAnkle.translation = position;
								rightAnkle.poseState |= PoseState.TRANSLATION;
							}
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" position."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerVelocity(id, out Vector3 velocity))
								rightAnkle.velocity = velocity;
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" velocity."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerAngularVelocity(id, out Vector3 angularVelocity))
								rightAnkle.angularVelocity = angularVelocity;
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" angularVelocity."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerAcceleration(id, out Vector3 acceleration))
								rightAnkle.acceleration = acceleration;
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" acceleration."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
						}
						if (role == TrackerRole.Foot_Left)
						{
							if (TrackerManager.Instance.GetTrackerRotation(id, out Quaternion rotation))
							{
								BodyTrackingUtils.Update(ref leftFoot.rotation, rotation);
								leftFoot.poseState |= PoseState.ROTATION;
								s_TrackedDeviceType[leftFoot.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker;
							}
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" rotation."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerPosition(id, out Vector3 position))
							{
								leftFoot.translation = position;
								leftFoot.poseState |= PoseState.TRANSLATION;
							}
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" position."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerVelocity(id, out Vector3 velocity))
								leftFoot.velocity = velocity;
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" velocity."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerAngularVelocity(id, out Vector3 angularVelocity))
								leftFoot.angularVelocity = angularVelocity;
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" angularVelocity."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerAcceleration(id, out Vector3 acceleration))
								leftFoot.acceleration = acceleration;
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" acceleration."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
						}
						if (role == TrackerRole.Foot_Right)
						{
							if (TrackerManager.Instance.GetTrackerRotation(id, out Quaternion rotation))
							{
								BodyTrackingUtils.Update(ref rightFoot.rotation, rotation);
								rightFoot.poseState |= PoseState.ROTATION;
								s_TrackedDeviceType[rightFoot.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker;
							}
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" rotation."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerPosition(id, out Vector3 position))
							{
								rightFoot.translation = position;
								rightFoot.poseState |= PoseState.TRANSLATION;
							}
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" position."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerVelocity(id, out Vector3 velocity))
								rightFoot.velocity = velocity;
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" velocity."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerAngularVelocity(id, out Vector3 angularVelocity))
								rightFoot.angularVelocity = angularVelocity;
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" angularVelocity."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerAcceleration(id, out Vector3 acceleration))
								rightFoot.acceleration = acceleration;
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" acceleration."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
						}
					}
				}
				if (info.type == WVR_BodyTrackingType.WVR_BodyTrackingType_WristTracker)
				{
					for (int ids_index = 0; ids_index < info.count && TrackerManager.Instance != null; ids_index++)
					{
						TrackerId id = TrackerUtils.s_TrackerIds[info.ids[ids_index]];
						TrackerRole role = TrackerManager.Instance.GetTrackerRole(id);

						if (printIntervalLog)
						{
							sb.Clear().Append("UpdatePose() type ").Append(info.type.Name())
								.Append(", id ").Append(id).Append(", role ").Append(role.Name());
							DEBUG(sb);
						}

						if (role == TrackerRole.Pair1_Left)
						{
							if (TrackerManager.Instance.GetTrackerRotation(id, out Quaternion rotation))
							{
								BodyTrackingUtils.Update(ref leftWrist.rotation, rotation);
								leftWrist.poseState |= PoseState.ROTATION;
								s_TrackedDeviceType[leftWrist.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_WristTracker;
							}
							else
							{
								sb.Clear().Append("UpdatePose() Invalid WristTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" rotation."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerPosition(id, out Vector3 position))
							{
								leftWrist.translation = position;
								leftWrist.poseState |= PoseState.TRANSLATION;
							}
							else
							{
								sb.Clear().Append("UpdatePose() Invalid WristTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" position."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerVelocity(id, out Vector3 velocity))
								leftWrist.velocity = velocity;
							else
							{
								sb.Clear().Append("UpdatePose() Invalid WristTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" velocity."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerAngularVelocity(id, out Vector3 angularVelocity))
								leftWrist.angularVelocity = angularVelocity;
							else
							{
								sb.Clear().Append("UpdatePose() Invalid WristTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" angularVelocity."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerAcceleration(id, out Vector3 acceleration))
								leftWrist.acceleration = acceleration;
							else
							{
								sb.Clear().Append("UpdatePose() Invalid WristTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" acceleration."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
						}
						if (role == TrackerRole.Pair1_Right)
						{
							if (TrackerManager.Instance.GetTrackerRotation(id, out Quaternion rotation))
							{
								BodyTrackingUtils.Update(ref rightWrist.rotation, rotation);
								rightWrist.poseState |= PoseState.ROTATION;
								s_TrackedDeviceType[rightWrist.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_WristTracker;
							}
							else
							{
								sb.Clear().Append("UpdatePose() Invalid WristTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" rotation."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerPosition(id, out Vector3 position))
							{
								rightWrist.translation = position;
								rightWrist.poseState |= PoseState.TRANSLATION;
							}
							else
							{
								sb.Clear().Append("UpdatePose() Invalid WristTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" position."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerVelocity(id, out Vector3 velocity))
								rightWrist.velocity = velocity;
							else
							{
								sb.Clear().Append("UpdatePose() Invalid WristTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" velocity."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerAngularVelocity(id, out Vector3 angularVelocity))
								rightWrist.angularVelocity = angularVelocity;
							else
							{
								sb.Clear().Append("UpdatePose() Invalid WristTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" angularVelocity."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerAcceleration(id, out Vector3 acceleration))
								rightWrist.acceleration = acceleration;
							else
							{
								sb.Clear().Append("UpdatePose() Invalid WristTracker ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" acceleration."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
						}
					}
				}
				if (info.type == WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTrackerIM)
				{
					for (int ids_index = 0; ids_index < info.count && TrackerManager.Instance != null; ids_index++)
					{
						TrackerId id = TrackerUtils.s_TrackerIds[info.ids[ids_index]];
						TrackerRole role = TrackerManager.Instance.GetTrackerRole(id);

						if (printIntervalLog)
						{
							sb.Clear().Append("UpdatePose() type ").Append(info.type.Name())
								.Append(", id ").Append(id).Append(", role ").Append(role.Name());
							DEBUG(sb);
						}

						if (role == TrackerRole.Knee_Left)
						{
							if (TrackerManager.Instance.GetTrackerRotation(id, out Quaternion rotation))
							{
								BodyTrackingUtils.Update(ref leftKnee.rotation, rotation);
								leftKnee.poseState = PoseState.ROTATION; // Self Tracker IM has ROTATION only.
								s_TrackedDeviceType[leftKnee.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTrackerIM;
							}
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTrackerIM ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" rotation."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerAngularVelocity(id, out Vector3 angularVelocity))
								leftKnee.angularVelocity = angularVelocity;
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTrackerIM ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" angularVelocity."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
						}
						if (role == TrackerRole.Knee_Right)
						{
							if (TrackerManager.Instance.GetTrackerRotation(id, out Quaternion rotation))
							{
								BodyTrackingUtils.Update(ref rightKnee.rotation, rotation);
								rightKnee.poseState = PoseState.ROTATION; // Self Tracker IM has ROTATION only.
								s_TrackedDeviceType[rightKnee.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTrackerIM;
							}
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTrackerIM ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" rotation."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerAngularVelocity(id, out Vector3 angularVelocity))
								rightKnee.angularVelocity = angularVelocity;
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTrackerIM ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" angularVelocity."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
						}
						if (role == TrackerRole.Ankle_Left)
						{
							if (TrackerManager.Instance.GetTrackerRotation(id, out Quaternion rotation))
							{
								BodyTrackingUtils.Update(ref leftAnkle.rotation, rotation);
								leftAnkle.poseState = PoseState.ROTATION; // Self Tracker IM has ROTATION only.
								s_TrackedDeviceType[leftAnkle.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTrackerIM;
							}
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTrackerIM ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" rotation."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerAngularVelocity(id, out Vector3 angularVelocity))
								leftAnkle.angularVelocity = angularVelocity;
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTrackerIM ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" angularVelocity."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
						}
						if (role == TrackerRole.Ankle_Right)
						{
							if (TrackerManager.Instance.GetTrackerRotation(id, out Quaternion rotation))
							{
								BodyTrackingUtils.Update(ref rightAnkle.rotation, rotation);
								rightAnkle.poseState = PoseState.ROTATION; // Self Tracker IM has ROTATION only.
								s_TrackedDeviceType[rightAnkle.trackedDeviceRole] = WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTrackerIM;
							}
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTrackerIM ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" rotation."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
							if (TrackerManager.Instance.GetTrackerAngularVelocity(id, out Vector3 angularVelocity))
								rightAnkle.angularVelocity = angularVelocity;
							else
							{
								sb.Clear().Append("UpdatePose() Invalid SelfTrackerIM ").Append(id).Append(", type: ").Append(info.type.Name()).Append(" angularVelocity."); ERROR(sb);
								return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
							}
						}
					}
				}
			}
			return BodyTrackingResult.SUCCESS;
		}

		// The Self Trackers used in Body Tracking Mode.
		private readonly Dictionary<BodyTrackingMode, List<TrackerRole>> s_SelfTrackerRoles = new Dictionary<BodyTrackingMode, List<TrackerRole>>()
		{
			// Arm uses tracker roles: wrist
			{ BodyTrackingMode.ARMIK, new List<TrackerRole>() {
				TrackerRole.Wrist_Left, TrackerRole.Wrist_Right }
			},
			// Upper Body uses self tracker roles: wrist, waist
			{ BodyTrackingMode.UPPERBODYIK, new List<TrackerRole>() {
				TrackerRole.Wrist_Left, TrackerRole.Wrist_Right, TrackerRole.Waist }
			},
			// Full Body uses self tracker roles: wrist, waist, ankle, foot
			{ BodyTrackingMode.FULLBODYIK, new List<TrackerRole>() {
				TrackerRole.Wrist_Left, TrackerRole.Wrist_Right, TrackerRole.Waist, TrackerRole.Ankle_Left, TrackerRole.Ankle_Right, TrackerRole.Foot_Left, TrackerRole.Foot_Right }
			},
			// Upper Body + Leg FK uses self tracker roles: waist
			{ BodyTrackingMode.UPPERIKANDLEGFK, new List<TrackerRole>() {
				TrackerRole.Waist }
			},
		};
		// The Wrist Trackers used in Body Tracking Mode.
		private readonly Dictionary<BodyTrackingMode, List<TrackerRole>> s_WristTrackerRoles = new Dictionary<BodyTrackingMode, List<TrackerRole>>()
		{
			// Arm uses wrist tracker roles: wrist
			{ BodyTrackingMode.ARMIK, new List<TrackerRole>() {
				TrackerRole.Pair1_Left, TrackerRole.Pair1_Right }
			},
			// Upper Body uses wrist tracker roles: wrist
			{ BodyTrackingMode.UPPERBODYIK, new List<TrackerRole>() {
				TrackerRole.Pair1_Left, TrackerRole.Pair1_Right }
			},
			// Full Body uses wrist tracker roles: wrist
			{ BodyTrackingMode.FULLBODYIK, new List<TrackerRole>() {
				TrackerRole.Pair1_Left, TrackerRole.Pair1_Right }
			},
		};
		// The Self IM Trackers used in Body Tracking Mode.
		private readonly Dictionary<BodyTrackingMode, List<TrackerRole>> s_SelfIMTrackerRoles = new Dictionary<BodyTrackingMode, List<TrackerRole>>()
		{
			// Only Upper Body + Leg FK uses self im trackers: knee, ankle
			{ BodyTrackingMode.UPPERIKANDLEGFK, new List<TrackerRole>() {
				TrackerRole.Knee_Left, TrackerRole.Knee_Right, TrackerRole.Ankle_Left, TrackerRole.Ankle_Right }
			}
		};
		internal BodyTrackingResult InitTrackingInfos(BodyTrackingMode mode)
		{
			sb.Clear().Append("InitTrackingInfos() ").Append(mode.Name()); DEBUG(sb);
			List<TrackingInfos_t.TrackingInfo_t> s_info = new List<TrackingInfos_t.TrackingInfo_t>();
			List<UInt32> ids = new List<uint>();

			// Checks Head
			if (WXRDevice.IsTracked(XR_Device.Head))
			{
				ids.Add((UInt32)WVR_DeviceType.WVR_DeviceType_HMD);
				sb.Clear().Append("InitTrackingInfos() add HMD"); DEBUG(sb);

			}
			if (ids.Count > 0)
			{
				s_info.Add(new TrackingInfos_t.TrackingInfo_t(
					WVR_BodyTrackingType.WVR_BodyTrackingType_HMD,
					ids.ToArray(),
					(UInt32)(ids.Count & 0x7FFFFFFF))
				);
			}

			// Checks Tracker first.
			bool hasLeftHand = false, hasRightHand = false;
			bool hasLeftAnkle = false, hasRightAnkle = false;
			if (TrackerManager.Instance != null)
			{
				// Checks Self Tracker first.
				sb.Clear().Append("InitTrackingInfos() Checks Self Tracker first."); DEBUG(sb);
				ids.Clear();
				for (int i = 0; i < TrackerUtils.s_TrackerIds.Length; i++)
				{
					if (TrackerManager.Instance.IsTrackerPoseValid(TrackerUtils.s_TrackerIds[i]))
					{
						if (GetBodyTrackingType(TrackerUtils.s_TrackerIds[i]) != WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker)
							continue;

						if (!s_SelfTrackerRoles.ContainsKey(mode)) { continue; }
						TrackerRole role = TrackerManager.Instance.GetTrackerRole(TrackerUtils.s_TrackerIds[i]);
						if (!s_SelfTrackerRoles[mode].Contains(role)) { continue; }

						if (role == TrackerRole.Wrist_Left)
						{
							if (hasLeftHand) { continue; }
							hasLeftHand = true;
						}
						if (role == TrackerRole.Wrist_Right)
						{
							if (hasRightHand) { continue; }
							hasRightHand = true;
						}
						if (role == TrackerRole.Ankle_Left)
						{
							if (hasLeftAnkle) { continue; }
							hasLeftAnkle = true;
						}
						if (role == TrackerRole.Ankle_Right)
						{
							if (hasRightAnkle) { continue; }
							hasRightAnkle = true;
						}

						sb.Clear().Append("InitTrackingInfos() add self tracker: ").Append(TrackerUtils.s_TrackerIds[i].Name()).Append(", role: ").Append(role.Name()); DEBUG(sb);
						ids.Add((UInt32)TrackerUtils.s_TrackerIds[i]);
					}
				}
				if (ids.Count > 0)
				{
					s_info.Add(new TrackingInfos_t.TrackingInfo_t(
						WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker,
						ids.ToArray(),
						(UInt32)(ids.Count & 0x7FFFFFFF))
					);
				}

				// Checks Wrist Tracker next.
				sb.Clear().Append("InitTrackingInfos() Checks Wrist Tracker next."); DEBUG(sb);
				ids.Clear();
				for (int i = 0; i < TrackerUtils.s_TrackerIds.Length; i++)
				{
					if (TrackerManager.Instance.IsTrackerPoseValid(TrackerUtils.s_TrackerIds[i]))
					{
						if (GetBodyTrackingType(TrackerUtils.s_TrackerIds[i]) != WVR_BodyTrackingType.WVR_BodyTrackingType_WristTracker)
							continue;

						if (!s_WristTrackerRoles.ContainsKey(mode)) { continue; }
						TrackerRole role = TrackerManager.Instance.GetTrackerRole(TrackerUtils.s_TrackerIds[i]);
						if (!s_WristTrackerRoles[mode].Contains(role)) { continue; }

						if (role == TrackerRole.Pair1_Left)
						{
							if (hasLeftHand) { continue; }
							hasLeftHand = true;
						}
						if (role == TrackerRole.Pair1_Right)
						{
							if (hasRightHand) { continue; }
							hasRightHand = true;
						}

						sb.Clear().Append("InitTrackingInfos() add wrist tracker: ").Append(TrackerUtils.s_TrackerIds[i].Name()).Append(", role: ").Append(role.Name()); DEBUG(sb);
						ids.Add((UInt32)TrackerUtils.s_TrackerIds[i]);
					}
				}
				if (ids.Count > 0)
				{
					s_info.Add(new TrackingInfos_t.TrackingInfo_t(
						WVR_BodyTrackingType.WVR_BodyTrackingType_WristTracker,
						ids.ToArray(),
						(UInt32)(ids.Count & 0x7FFFFFFF))
					);
				}

				// Checks Self Tracker IM last.
				sb.Clear().Append("InitTrackingInfos() Checks Self Tracker IM last."); DEBUG(sb);
				ids.Clear();
				for (int i = 0; i < TrackerUtils.s_TrackerIds.Length; i++)
				{
					if (TrackerManager.Instance.IsTrackerPoseValid(TrackerUtils.s_TrackerIds[i]))
					{
						if (GetBodyTrackingType(TrackerUtils.s_TrackerIds[i]) != WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTrackerIM)
							continue;

						if (!s_SelfIMTrackerRoles.ContainsKey(mode)) { continue; }
						TrackerRole role = TrackerManager.Instance.GetTrackerRole(TrackerUtils.s_TrackerIds[i]);
						if (!s_SelfIMTrackerRoles[mode].Contains(role)) { continue; }

						if (role == TrackerRole.Ankle_Left)
						{
							if (hasLeftAnkle) { continue; }
							hasLeftAnkle = true;
						}
						if (role == TrackerRole.Ankle_Right)
						{
							if (hasRightAnkle) { continue; }
							hasRightAnkle = true;
						}

						sb.Clear().Append("InitTrackingInfos() add self tracker im: ").Append(TrackerUtils.s_TrackerIds[i].Name()).Append(", role: ").Append(role.Name()); DEBUG(sb);
						ids.Add((UInt32)TrackerUtils.s_TrackerIds[i]);
					}
				}
				if (ids.Count > 0)
				{
					s_info.Add(new TrackingInfos_t.TrackingInfo_t(
						WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTrackerIM,
						ids.ToArray(),
						(UInt32)(ids.Count & 0x7FFFFFFF))
					);
				}
			}

			// Checks Controller next.
			if (!hasLeftHand || !hasRightHand)
			{
				ids.Clear();
				if (!hasLeftHand && WXRDevice.IsTracked(XR_Device.Left))
				{
					hasLeftHand = true;
					ids.Add((UInt32)WVR_DeviceType.WVR_DeviceType_Controller_Left);
					sb.Clear().Append("InitTrackingInfos() add controller left"); DEBUG(sb);
				}
				if (!hasRightHand && WXRDevice.IsTracked(XR_Device.Right))
				{
					hasRightHand = true;
					ids.Add((UInt32)WVR_DeviceType.WVR_DeviceType_Controller_Right);
					sb.Clear().Append("InitTrackingInfos() add controller right"); DEBUG(sb);
				}
				if (ids.Count > 0)
				{
					s_info.Add(new TrackingInfos_t.TrackingInfo_t(
						WVR_BodyTrackingType.WVR_BodyTrackingType_Controller,
						ids.ToArray(),
						(UInt32)(ids.Count & 0x7FFFFFFF))
					);
				}
			}

			// Force using Hand if no Tracker & Controller.
			if (!hasLeftHand || !hasRightHand)
			{
				ids.Clear();
				ids.Add((UInt32)WVR_DeviceType.WVR_DeviceType_NaturalHand_Left);
				sb.Clear().Append("InitTrackingInfos() FORCE add hand left."); DEBUG(sb);

				ids.Add((UInt32)WVR_DeviceType.WVR_DeviceType_NaturalHand_Right);
				sb.Clear().Append("InitTrackingInfos() FORCE add hand right."); DEBUG(sb);

				if (ids.Count > 0)
				{
					s_info.Add(new TrackingInfos_t.TrackingInfo_t(
						WVR_BodyTrackingType.WVR_BodyTrackingType_Hand,
						ids.ToArray(),
						(UInt32)(ids.Count & 0x7FFFFFFF))
					);
				}
			}

			trackingInfos = new TrackingInfos_t(s_info.ToArray(), (UInt32)(s_info.Count & 0x7FFFFFFF));
			BodyTrackingResult result = UpdatePose();

			if (result == BodyTrackingResult.SUCCESS)
			{
				BodyPoseRole ikRoles = GetIKRoles();
				sb.Clear().Append("InitTrackingInfos() ikRoles: ").Append(ikRoles.Name()); DEBUG(sb);
			}

			return result;
		}
		#endregion

		private List<TrackedDevicePose> poses = new List<TrackedDevicePose>();
		private TrackedDevicePose[] poseArray = null;
		private void UpdatePoseArray()
		{
			if (poses == null || poses.Count <= 0) { return; }
			if (poseArray == null || poseArray.Length != poses.Count) { poseArray = new TrackedDevicePose[poses.Count]; }
			for (int i = 0; i < poseArray.Length; i++) { poseArray[i] = poses[i]; }
		}

		public bool PoseStateAvailable(WVR_BodyTrackingType type, PoseState state, bool canIgnorePose = false)
		{
			if (type == WVR_BodyTrackingType.WVR_BodyTrackingType_HMD ||
				type == WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker ||
				type == WVR_BodyTrackingType.WVR_BodyTrackingType_WristTracker ||
				type == WVR_BodyTrackingType.WVR_BodyTrackingType_Controller ||
				type == WVR_BodyTrackingType.WVR_BodyTrackingType_Hand)
			{
				// Only Hand pose can be ignored.
				if (type == WVR_BodyTrackingType.WVR_BodyTrackingType_Hand)
				{
					if (canIgnorePose)
						return true;
				}
				if (state == (PoseState.ROTATION | PoseState.TRANSLATION))
					return true;
			}
			if (type == WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTrackerIM)
			{
				if ((state & PoseState.ROTATION) == PoseState.ROTATION)
					return true;
			}

			return false;
		}
		public bool GetTrackedDevicePoses(out TrackedDevicePose[] trackedDevicePoses, out UInt32 trackedDevicePoseCount)
		{
			logFrame++;
			logFrame %= 500;
			printIntervalLog = (logFrame == 0);

			poses.Clear();
			if (PoseStateAvailable(s_TrackedDeviceType[head.trackedDeviceRole], head.poseState))
			{
				if (printIntervalLog || getDevicePosesFirstTime)
				{
					sb.Clear().Append("GetTrackedDevicePoses() Add head with poseState ").Append(head.poseState)
						.Append(", device: ").Append(s_TrackedDeviceType[head.trackedDeviceRole].Name());
					DEBUG(sb);
				}
				poses.Add(head);
			}
			if (PoseStateAvailable(s_TrackedDeviceType[chest.trackedDeviceRole], chest.poseState)) { poses.Add(chest); }
			if (PoseStateAvailable(s_TrackedDeviceType[hip.trackedDeviceRole], hip.poseState))
			{
				if (printIntervalLog || getDevicePosesFirstTime)
				{
					sb.Clear().Append("GetTrackedDevicePoses() Add hip with poseState ").Append(hip.poseState)
						.Append(", device: ").Append(s_TrackedDeviceType[hip.trackedDeviceRole].Name());
					DEBUG(sb);
				}
				poses.Add(hip);
			}

			if (PoseStateAvailable(s_TrackedDeviceType[leftElbow.trackedDeviceRole], leftElbow.poseState)) { poses.Add(leftElbow); }
			if (PoseStateAvailable(s_TrackedDeviceType[leftWrist.trackedDeviceRole], leftWrist.poseState))
			{
				if (printIntervalLog || getDevicePosesFirstTime)
				{
					sb.Clear().Append("GetTrackedDevicePoses() Add leftWrist with poseState ").Append(leftWrist.poseState)
						.Append(", device: ").Append(s_TrackedDeviceType[leftWrist.trackedDeviceRole].Name());
					DEBUG(sb);
				}
				poses.Add(leftWrist);
			}
			// LeftHand uses Natural Hand pose which can be ignored in calibration.
			if (PoseStateAvailable(s_TrackedDeviceType[leftHand.trackedDeviceRole], leftHand.poseState, true))
			{
				if (printIntervalLog || getDevicePosesFirstTime)
				{
					sb.Clear().Append("GetTrackedDevicePoses() Add leftHand with poseState ").Append(leftHand.poseState)
						.Append(", device: ").Append(s_TrackedDeviceType[leftHand.trackedDeviceRole].Name());
					DEBUG(sb);
				}
				poses.Add(leftHand);
			}
			if (PoseStateAvailable(s_TrackedDeviceType[leftHandheld.trackedDeviceRole], leftHandheld.poseState))
			{
				if (printIntervalLog || getDevicePosesFirstTime)
				{
					sb.Clear().Append("GetTrackedDevicePoses() Add leftHandheld with poseState ").Append(leftHandheld.poseState)
						.Append(", device: ").Append(s_TrackedDeviceType[leftHandheld.trackedDeviceRole].Name());
					DEBUG(sb);
				}
				poses.Add(leftHandheld);
			}

			if (PoseStateAvailable(s_TrackedDeviceType[rightElbow.trackedDeviceRole], rightElbow.poseState)) { poses.Add(rightElbow); }
			if (PoseStateAvailable(s_TrackedDeviceType[rightWrist.trackedDeviceRole], rightWrist.poseState))
			{
				if (printIntervalLog || getDevicePosesFirstTime)
				{
					sb.Clear().Append("GetTrackedDevicePoses() Add rightWrist with poseState ").Append(rightWrist.poseState)
						.Append(", device: ").Append(s_TrackedDeviceType[rightWrist.trackedDeviceRole].Name());
					DEBUG(sb);
				}
				poses.Add(rightWrist);
			}
			// RightHand uses Natural Hand pose which can be ignored in calibration.
			if (PoseStateAvailable(s_TrackedDeviceType[rightHand.trackedDeviceRole], rightHand.poseState, true))
			{
				if (printIntervalLog || getDevicePosesFirstTime)
				{
					sb.Clear().Append("GetTrackedDevicePoses() Add rightHand with poseState ").Append(rightHand.poseState)
						.Append(", device: ").Append(s_TrackedDeviceType[rightHand.trackedDeviceRole].Name());
					DEBUG(sb);
				}
				poses.Add(rightHand);
			}
			if (PoseStateAvailable(s_TrackedDeviceType[rightHandheld.trackedDeviceRole], rightHandheld.poseState))
			{
				if (printIntervalLog || getDevicePosesFirstTime)
				{
					sb.Clear().Append("GetTrackedDevicePoses() Add rightHandheld with poseState ").Append(rightHandheld.poseState)
						.Append(", device: ").Append(s_TrackedDeviceType[rightHandheld.trackedDeviceRole].Name());
					DEBUG(sb);
				}
				poses.Add(rightHandheld);
			}

			// LeftKnee uses SelfTrackerIM which has rotation only.
			if (PoseStateAvailable(s_TrackedDeviceType[leftKnee.trackedDeviceRole], leftKnee.poseState))
			{
					if (printIntervalLog || getDevicePosesFirstTime)
					{
						sb.Clear().Append("GetTrackedDevicePoses() Add leftKnee with poseState ").Append(leftKnee.poseState)
							.Append(", device: ").Append(s_TrackedDeviceType[leftKnee.trackedDeviceRole].Name());
						DEBUG(sb);
					}
					poses.Add(leftKnee);
			}
			// LeftAnkle uses SelfTracker 6DoF pose or SelfTrackerIM pose which has rotation only.
			if (PoseStateAvailable(s_TrackedDeviceType[leftAnkle.trackedDeviceRole], leftAnkle.poseState))
			{
				if (printIntervalLog || getDevicePosesFirstTime)
				{
					sb.Clear().Append("GetTrackedDevicePoses() Add leftAnkle with poseState ").Append(leftAnkle.poseState)
						.Append(", device: ").Append(s_TrackedDeviceType[leftAnkle.trackedDeviceRole].Name());
					DEBUG(sb);
				}
				poses.Add(leftAnkle);
			}
			if (PoseStateAvailable(s_TrackedDeviceType[leftFoot.trackedDeviceRole], leftFoot.poseState))
			{
				if (printIntervalLog || getDevicePosesFirstTime)
				{
					sb.Clear().Append("GetTrackedDevicePoses() Add leftFoot with poseState ").Append(leftFoot.poseState)
						.Append(", device: ").Append(s_TrackedDeviceType[leftFoot.trackedDeviceRole].Name());
					DEBUG(sb);
				}
				poses.Add(leftFoot);
			}

			// RightKnee uses SelfTrackerIM which has rotation only.
			if (PoseStateAvailable(s_TrackedDeviceType[rightKnee.trackedDeviceRole], rightKnee.poseState))
			{
				if (printIntervalLog || getDevicePosesFirstTime)
				{
					sb.Clear().Append("GetTrackedDevicePoses() Add rightKnee with poseState ").Append(rightKnee.poseState)
						.Append(", device: ").Append(s_TrackedDeviceType[rightKnee.trackedDeviceRole].Name());
					DEBUG(sb);
				}
				poses.Add(rightKnee);
			}
			// RightAnkle uses SelfTracker 6DoF pose or SelfTrackerIM pose which has rotation only.
			if (PoseStateAvailable(s_TrackedDeviceType[rightAnkle.trackedDeviceRole], rightAnkle.poseState))
			{
				if (printIntervalLog || getDevicePosesFirstTime)
				{
					sb.Clear().Append("GetTrackedDevicePoses() Add rightAnkle with poseState ").Append(rightAnkle.poseState)
						.Append(", device: ").Append(s_TrackedDeviceType[rightAnkle.trackedDeviceRole].Name());
					DEBUG(sb);
				}
				poses.Add(rightAnkle);
			}
			if (PoseStateAvailable(s_TrackedDeviceType[rightFoot.trackedDeviceRole], rightFoot.poseState))
			{
				if (printIntervalLog || getDevicePosesFirstTime)
				{
					sb.Clear().Append("GetTrackedDevicePoses() Add rightFoot with poseState ").Append(rightFoot.poseState)
						.Append(", device: ").Append(s_TrackedDeviceType[rightFoot.trackedDeviceRole].Name());
					DEBUG(sb);
				}
				poses.Add(rightFoot);
			}

			getDevicePosesFirstTime = false;
			if (poses.Count > 0)
			{
				UpdatePoseArray();
				trackedDevicePoses = poseArray;
				trackedDevicePoseCount = (UInt32)(poseArray.Length & 0x7FFFFFFF);
				return true;
			}

			trackedDevicePoses = null;
			trackedDevicePoseCount = 0;
			return false;
		}

		private int ikFrame = -1;
		private BodyPoseRole m_IKRoles = BodyPoseRole.Unknown;
		public BodyPoseRole GetIKRoles()
		{
			if (printIntervalLog || getDevicePosesFirstTime) { sb.Clear().Append("GetIKRoles()"); DEBUG(sb); }

			if (ikFrame == Time.frameCount) { return m_IKRoles; }
			else { m_IKRoles = BodyPoseRole.Unknown; ikFrame = Time.frameCount; }

			if (GetTrackedDevicePoses(out TrackedDevicePose[] trackedDevicePoses, out UInt32 trackedDevicePoseCount))
				m_IKRoles = BodyTrackingUtils.GetBodyPoseRole(trackedDevicePoses, trackedDevicePoseCount);

			return m_IKRoles;
		}

		/// <summary> Checks if tracked device's extrinsic matches the calibration pose condition. </summary>
		public bool UseExtrinsic(WVR_TrackedDeviceRole wvrRole, WVR_BodyTrackingType wvrType)
		{
			UInt64 ikRoleValue = (UInt64)m_IKRoles;
			TrackedDeviceRole role = wvrRole.Role();
			UInt64 roleValue = (UInt64)(1 << (Int32)role);

			if ((ikRoleValue & roleValue) == roleValue)
			{
				// Multiple extrinsics may have the same role so need to confirm type.
				if (s_TrackedDeviceType.ContainsKey(role) && s_TrackedDeviceType[role] == wvrType)
				{
					//sb.Clear().Append("UseExtrinsic() ").Append(wvrRole.Name()).Append(" with type ").Append(wvrType.Name()); DEBUG(sb);
					return true;
				}
			}
			else
			{
				// Use the extrinsic anyway even not used in BodyPoseRole.
				//sb.Clear().Append("UseExtrinsic() ").Append(wvrRole.Name()).Append(" with type ").Append(wvrType.Name()); DEBUG(sb);
				return true;
			}

			return false;
		}

		// ToDo: Replaced by TrackerManager.Instance.GetTrackerName.
		private readonly Dictionary<WVR_BodyTrackingType, List<string>> s_TrackerNames = new Dictionary<WVR_BodyTrackingType, List<string>>()
		{
			{ WVR_BodyTrackingType.WVR_BodyTrackingType_WristTracker, new List<string>() {
				"Vive_Tracker_Wrist", "Vive_Wrist_Tracker" }
			},
			{ WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker, new List<string>() {
				"Vive_Tracker_OT", "Vive_Self_Tracker", "Vive_Ultimate_Tracker" }
			},
			{ WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTrackerIM, new List<string>() {
				"Vive_Tracker_IU", "Vive_Self_Tracker_IM", "Vive_3Dof_Tracker_A", "Vive_Tracking_Tag" }
			}
		};
		public WVR_BodyTrackingType GetBodyTrackingType(TrackerId id)
		{
			if (TrackerManager.Instance != null && TrackerManager.Instance.GetTrackerDeviceName(id, out string name))
			{
				sb.Clear().Append("GetBodyTrackingType() ").Append(id.Name()).Append(" name: ").Append(name); DEBUG(sb);
				// Checks self tracker first.
				for (int i = 0; i < s_TrackerNames[WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker].Count; i++)
				{
					if (name.Contains(s_TrackerNames[WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker][i]))
						return WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker;
				}
				// Checks self tracker im next.
				for (int i = 0; i < s_TrackerNames[WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTrackerIM].Count; i++)
				{
					if (name.Contains(s_TrackerNames[WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTrackerIM][i]))
						return WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTrackerIM;
				}
				// Checks wrist tracker last.
				for (int i = 0; i < s_TrackerNames[WVR_BodyTrackingType.WVR_BodyTrackingType_WristTracker].Count; i++)
				{
					if (name.Contains(s_TrackerNames[WVR_BodyTrackingType.WVR_BodyTrackingType_WristTracker][i]))
						return WVR_BodyTrackingType.WVR_BodyTrackingType_WristTracker;
				}
			}

			return WVR_BodyTrackingType.WVR_BodyTrackingType_Invalid;
		}
	}

	internal struct BodyRotationSpace_t
	{
		public RotateSpace[] spaces;
		public UInt32 count;

		public BodyRotationSpace_t(RotateSpace[] in_spaces, UInt32 in_count)
		{
			spaces = in_spaces;
			count = in_count;
		}
		public void Update(BodyRotationSpace_t in_brt)
		{
			if (count != in_brt.count && in_brt.count > 0)
			{
				count = in_brt.count;
				spaces = new RotateSpace[count];
			}
			for (UInt32 i = 0; i < count; i++)
				spaces[i] = in_brt.spaces[i];
		}
	}

	#region API v1.0.0.1
	public enum JointType : Int32
	{
		UNKNOWN = -1,
		HIP = 0,

		LEFTTHIGH = 1,
		LEFTLEG = 2,
		LEFTANKLE = 3,
		LEFTFOOT = 4,

		RIGHTTHIGH = 5,
		RIGHTLEG = 6,
		RIGHTANKLE = 7,
		RIGHTFOOT = 8,

		WAIST = 9,

		SPINELOWER = 10,
		SPINEMIDDLE = 11,
		SPINEHIGH = 12,

		CHEST = 13,
		NECK = 14,
		HEAD = 15,

		LEFTCLAVICLE = 16,
		LEFTSCAPULA = 17,
		LEFTUPPERARM = 18,
		LEFTFOREARM = 19,
		LEFTHAND = 20,

		RIGHTCLAVICLE = 21,
		RIGHTSCAPULA = 22,
		RIGHTUPPERARM = 23,
		RIGHTFOREARM = 24,
		RIGHTHAND = 25,

		NUMS_OF_JOINT,
		MAX_ENUM = 0x7fffffff
	}
	[Flags]
	public enum PoseState : UInt32
	{
		NODATA = 0,
		ROTATION = 1 << 0,
		TRANSLATION = 1 << 1
	}
	public enum BodyTrackingMode : Int32
	{
		UNKNOWNMODE = -1,
		ARMIK = 0,
		UPPERBODYIK = 1,
		FULLBODYIK = 2,

		UPPERIKANDLEGFK = 3, // controller or hand + hip tracker + leg fk
		SPINEIK = 4,    // used internal
		LEGIK = 5,    // used internal
		LEGFK = 6,    // used internal
		SPINEIKANDLEGFK = 7, // hip tracker + leg fk
		MAX = 0x7fffffff
	}
	public enum TrackedDeviceRole : Int32
	{
		ROLE_UNDEFINED = -1,
		ROLE_HIP = 0,
		ROLE_CHEST = 1,
		ROLE_HEAD = 2,

		ROLE_LEFTELBOW = 3,
		ROLE_LEFTWRIST = 4,
		ROLE_LEFTHAND = 5,
		ROLE_LEFTHANDHELD = 6,

		ROLE_RIGHTELBOW = 7,
		ROLE_RIGHTWRIST = 8,
		ROLE_RIGHTHAND = 9,
		ROLE_RIGHTHANDHELD = 10,

		ROLE_LEFTKNEE = 11,
		ROLE_LEFTANKLE = 12,
		ROLE_LEFTFOOT = 13,

		ROLE_RIGHTKNEE = 14,
		ROLE_RIGHTANKLE = 15,
		ROLE_RIGHTFOOT = 16,

		NUMS_OF_ROLE,
		ROLE_MAX = 0x7fffffff
	}
	public enum Result : Int32
	{
		SUCCESS = 0,
		ERROR_BODYTRACKINGMODE_NOT_FOUND = 100,
		ERROR_TRACKER_AMOUNT_FAILED = 101,
		ERROR_SKELETONID_NOT_FOUND = 102,
		ERROR_INPUTPOSE_NOT_VALID = 103,
		ERROR_NOT_CALIBRATED = 104,
		ERROR_BODYTRACKINGMODE_NOT_ALIGNED = 105,
		ERROR_AVATAR_INIT_FAILED = 200,
		ERROR_CALIBRATE_FAILED = 300,
		ERROR_COMPUTE_FAILED = 400,
		ERROR_TABLE_STATIC = 401,
		ERROR_SOLVER_NOT_FOUND = 402,
		ERROR_NOT_INITIALIZATION = 403,
		ERROR_JOINT_NOT_FOUND = 404,
		ERROR_FATAL_ERROR = 499,
		ERROR_MAX = 0x7fffffff,
	}
	public enum TrackerDirection : Int32
	{
		NODIRECTION = -1,
		FORWARD = 0,
		BACKWARD = 1,
		RIGHT = 2,
		LEFT = 3
	}
	public enum AvatarType : UInt32
	{
		TPOSE = 0,    // T-pose, pre-processing in SDK
		STANDARD_VRM = 1,    // any pose, standard vrm model (all joints' coordinate is identity)
		OTHERS = 2     // any pose, but need to meet the constraint defined by library
	}
	public enum CalibrationType : UInt32
	{
		DEFAULTCALIBRATION =
			0,    // User stands L-pose. Use tracked device poses to calibrate. Need tracked device pose.
		TOFFSETCALIBRATION =
			1,    // User stands straight. Only do translation offset calibration. Need tracked device pose.
		HEIGHTCALIBRATION = 2,    // Set user height directly. No need tracked device pose.
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct Joint
	{
		[FieldOffset(0)] public JointType jointType;
		[FieldOffset(4)] public PoseState poseState;
		[FieldOffset(8)] public Vector3 translation;
		[FieldOffset(20)] public Vector3 velocity;
		[FieldOffset(32)] public Vector3 angularVelocity;
		[FieldOffset(44)] public Quaternion rotation;

		public Joint(JointType in_jointType, PoseState in_poseState, Vector3 in_translation, Vector3 in_velocity, Vector3 in_angularVelocity, Quaternion in_rotation)
		{
			jointType = in_jointType;
			poseState = in_poseState;
			translation = in_translation;
			velocity = in_velocity;
			angularVelocity = in_angularVelocity;
			rotation = in_rotation;
			BodyTrackingUtils.Validate(ref rotation);
		}
		public static Joint identity {
			get {
				return new Joint(JointType.UNKNOWN, PoseState.NODATA, Vector3.zero, Vector3.zero, Vector3.zero, Quaternion.identity);
			}
		}
		public void Update(Joint in_joint)
		{
			jointType = in_joint.jointType;
			poseState = in_joint.poseState;
			translation = in_joint.translation;
			velocity = in_joint.velocity;
			angularVelocity = in_joint.angularVelocity;
			BodyTrackingUtils.Update(ref rotation, in_joint.rotation);
		}
		public static Joint init(JointType type)
		{
			return new Joint(type, PoseState.NODATA, Vector3.zero, Vector3.zero, Vector3.zero, Quaternion.identity);
		}
		public string Log()
		{
			string log = "jointType: " + jointType;
			log += ", poseState: " + poseState;
			log += ", position(" + translation.x.ToString() + ", " + translation.y.ToString() + ", " + translation.z.ToString() + ")";
			log += ", rotation(" + rotation.x.ToString() + ", " + rotation.y.ToString() + ", " + rotation.z.ToString() + ", " + rotation.w.ToString() + ")";
			log += ", velocity(" + velocity.x.ToString() + ", " + velocity.y.ToString() + ", " + velocity.z.ToString() + ")";
			log += ", angularVelocity(" + angularVelocity.x.ToString() + ", " + angularVelocity.y.ToString() + ", " + angularVelocity.z.ToString() + ")";
			return log;
		}
	}

	[StructLayout(LayoutKind.Explicit)]
	[Serializable]
	public struct Extrinsic
	{
		[FieldOffset(0)] public Vector3 translation;
		[FieldOffset(12)] public Quaternion rotation;

		public Extrinsic(Vector3 in_translation, Quaternion in_rotation)
		{
			translation = in_translation;
			rotation = in_rotation;
			BodyTrackingUtils.Validate(ref rotation);
		}
		public static Extrinsic identity { 
			get {
				return new Extrinsic(Vector3.zero, Quaternion.identity);
			}
		}
		public void Update(Extrinsic in_ext)
		{
			translation = in_ext.translation;
			BodyTrackingUtils.Update(ref rotation, in_ext.rotation);
		}
		public void Update(WVR_Pose_t in_pose)
		{
			Coordinate.GetVectorFromGL(in_pose.position, out translation);
			Coordinate.GetQuaternionFromGL(in_pose.rotation, out rotation);
		}

		public string Log()
		{
			string log = "position(" + translation.x.ToString() + ", " + translation.y.ToString() + ", " + translation.z.ToString() + ")";
			log += ", rotation(" + rotation.x.ToString() + ", " + rotation.y.ToString() + ", " + rotation.z.ToString() + ", " + rotation.w.ToString() + ")";
			return log;
		}
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct TrackedDeviceExtrinsic
	{
		[FieldOffset(0)] public TrackedDeviceRole trackedDeviceRole;
		[FieldOffset(4)] public Extrinsic extrinsic;

		public TrackedDeviceExtrinsic(TrackedDeviceRole in_trackedDeviceRole, Extrinsic in_extrinsic)
		{
			trackedDeviceRole = in_trackedDeviceRole;
			extrinsic = in_extrinsic;
		}
		public static TrackedDeviceExtrinsic identity {
			get {
				return new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_UNDEFINED, Extrinsic.identity);
			}
		}
		public static TrackedDeviceExtrinsic init(TrackedDeviceRole role)
		{
			return new TrackedDeviceExtrinsic(role, Extrinsic.identity);
		}
		public void Update(TrackedDeviceExtrinsic in_ext)
		{
			trackedDeviceRole = in_ext.trackedDeviceRole;
			extrinsic.Update(in_ext.extrinsic);
		}
		public string Log()
		{
			string log = "Role: " + trackedDeviceRole;
			log += ", extrinsic: " + extrinsic.Log();
			return log;
		}
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct TrackedDevicePose
	{
		[FieldOffset(0)] public TrackedDeviceRole trackedDeviceRole;
		[FieldOffset(4)] public PoseState poseState;
		[FieldOffset(8)] public Vector3 translation;
		[FieldOffset(20)] public Vector3 velocity;
		[FieldOffset(32)] public Vector3 angularVelocity;
		[FieldOffset(44)] public Vector3 acceleration;
		[FieldOffset(56)] public Quaternion rotation;

		public TrackedDevicePose(TrackedDeviceRole in_trackedDeviceRole, PoseState in_poseState, Vector3 in_translation, Vector3 in_velocity, Vector3 in_angularVelocity, Vector3 in_acceleration, Quaternion in_rotation)
		{
			trackedDeviceRole = in_trackedDeviceRole;
			poseState = in_poseState;
			translation = in_translation;
			velocity = in_velocity;
			angularVelocity = in_angularVelocity;
			acceleration = in_acceleration;
			rotation = in_rotation;
			BodyTrackingUtils.Validate(ref rotation);
		}
		public static TrackedDevicePose identity {
			get {
				return new TrackedDevicePose(TrackedDeviceRole.ROLE_UNDEFINED, PoseState.NODATA, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Quaternion.identity);
			}
		}
		public void Update([In] TrackedDevicePose in_pose)
		{
			trackedDeviceRole = in_pose.trackedDeviceRole;
			poseState = in_pose.poseState;
			translation = in_pose.translation;
			velocity = in_pose.velocity;
			angularVelocity = in_pose.angularVelocity;
			acceleration = in_pose.acceleration;
			BodyTrackingUtils.Update(ref rotation, in_pose.rotation);
		}
		public string Log()
		{
			string log = "trackedDeviceRole: " + trackedDeviceRole.Name();
			log += ", poseState: " + poseState;
			log += ", translation(" + translation.x.ToString() + ", " + translation.y.ToString() + ", " + translation.z.ToString() + ")";
			log += ", velocity(" + velocity.x.ToString() + ", " + velocity.y.ToString() + ", " + velocity.z.ToString() + ")";
			log += ", angularVelocity(" + angularVelocity.x.ToString() + ", " + angularVelocity.y.ToString() + ", " + angularVelocity.z.ToString() + ")";
			log += ", acceleration(" + acceleration.x.ToString() + ", " + acceleration.y.ToString() + ", " + acceleration.z.ToString() + ")";
			log += ", rotation(" + rotation.x.ToString() + ", " + rotation.y.ToString() + ", " + rotation.z.ToString() + ", " + rotation.w.ToString() + ")";
			return log;
		}
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct RotateSpace
	{
		[FieldOffset(0)] public JointType jointType;
		[FieldOffset(4)] public Quaternion rotation;

		public RotateSpace(JointType in_jointType, Quaternion in_rotation)
		{
			jointType = in_jointType;
			rotation = in_rotation;
			BodyTrackingUtils.Validate(ref rotation);
		}
		public static RotateSpace identity {
			get {
				return new RotateSpace(JointType.UNKNOWN, Quaternion.identity);
			}
		}
	}

	public class fbt
	{
		const string LOG_TAG = "Wave.Essence.BodyTracking.fbt";
		static StringBuilder m_sb = null;
		static StringBuilder sb {
			get {
				if (m_sb == null) { m_sb = new StringBuilder(); }
				return m_sb;
			}
		}
		static void DEBUG(StringBuilder msg)
		{
			if (Log.EnableDebugLog)
				Log.d(LOG_TAG, msg, true);
		}
		static int logFrame = -1;
		static bool fbtIntervalLog = false;

		[DllImport("bodytracking")]
		/**
		 *  @brief Initial body tracking algorithm with custom skeleton
		 *  @param[in] timestamp (unit:us)
		 *  @param[in] bodyTrackingMode. The body tracking mode which developer wants to use
		 *  @param[in] trackedDeviceExt. The tracked device extrinsic from avatar to tracked device
		 *  @param[in] deviceCount. The amount of tracked devices
		 *  @param[in] avatarJoints. The avatar's joints
		 *  @param[in] avatarJointCount. The amount of the avatar's joints
		 *  @param[in] avatarHeight. The avatar's height
		 *  @param[out] skeleton id.
		 *  @param[in] avatarType. The avatar's type (This paramenter is only for internal use. The default value is TPOSE.)
		 *  @param[out] success or not.
		 **/
		public static extern Result InitBodyTracking(UInt64 ts, BodyTrackingMode bodyTrackingMode,
			TrackedDeviceExtrinsic[] trackedDeviceExt, UInt32 deviceCount,
			Joint[] avatarJoints, UInt32 avatarJointCount, float avatarHeight,
			ref int skeletonId,
			AvatarType avatarType = AvatarType.TPOSE);
		public static Result InitBodyTrackingLog(UInt64 ts, BodyTrackingMode bodyTrackingMode,
			TrackedDeviceExtrinsic[] trackedDeviceExt, UInt32 deviceCount,
			Joint[] avatarJoints, UInt32 avatarJointCount, float avatarHeight,
			ref int skeletonId,
			AvatarType avatarType = AvatarType.TPOSE)
		{
			sb.Clear();
			sb.Append("InitBodyTracking() ").Append(ts).Append(", bodyTrackingMode: ").Append(bodyTrackingMode.Name()).Append("\n");

			sb.Append("deviceCount: ").Append(deviceCount).Append("\n");
			for (UInt32 i = 0; i < deviceCount; i++)
			{
				sb.Append("trackedDeviceExt[").Append(i).Append("] ").Append(trackedDeviceExt[i].Log()).Append("\n");
			}

			sb.Append("avatarJointCount: ").Append(avatarJointCount);
			DEBUG(sb);

			for (UInt32 i = 0; i < avatarJointCount; i++)
			{
				sb.Clear();
				sb.Append("avatarJoints[").Append(i).Append("] ").Append(avatarJoints[i].Log());
				DEBUG(sb);
			}

			sb.Clear();
			sb.Append("avatarHeight: ").Append(avatarHeight).Append("\n");
			sb.Append("skeletonId: ").Append(skeletonId).Append("\n");
			sb.Append("avatarType: ").Append(avatarType);
			DEBUG(sb);

			return InitBodyTracking(ts, bodyTrackingMode, trackedDeviceExt, deviceCount, avatarJoints, avatarJointCount, avatarHeight, ref skeletonId);
		}

		[DllImport("bodytracking")]
		/**
		 *  @brief Initial body trahcking algorithm with default skeleton
		 *  @param[in] timestamp (unit:us)
		 *  @param[in] bodyTrackingMode. The body tracking mode which developer wants to use
		 *  @param[in] trackedDeviceExt. The tracked device extrinsic from avatar to tracked device
		 *  @param[in] deviceCount. The amount of tracked devices
		 *  @param[out] skeleton id.
		 *  @param[out] success or not.
		 **/
		public static extern Result InitDefaultBodyTracking(UInt64 ts, BodyTrackingMode bodyTrackingMode,
			TrackedDeviceExtrinsic[] trackedDeviceExt, UInt32 deviceCount,
			ref int skeletonId);

		[DllImport("bodytracking")]
		/**
		 *  @brief Calibrate Body Tracking. Must be called after initail. User needs to stand L pose(stand straight, two arms straight forward and let the palm down)
		 *  @param[in] timestamp (unit:us)
		 *  @param[in] skeletonId.
		 *  @param[in] userHeight. The user height.
		 *  @param[in] bodyTrackingMode. The body tracking mode which developer wants to use
		 *  @param[in] trackedDevicePose. The tracked device poses.(Left-Handed coordinate sytstem. x right, y up, z forward. unit: m)
		 *  @param[in] deviceCount. The amount of tracked devices
		 *  @param[out] scale. If used custom skeleton, this value will be the scale of custom skeleton. Otherwise, the value will be 1.
		 *  @param[out] success or not.
		 **/
		public static extern Result CalibrateBodyTracking(UInt64 ts, int skeletonId, float userHeight,
			BodyTrackingMode bodyTrackingMode,
			TrackedDevicePose[] trackedDevicePose, UInt32 deviceCount,
			ref float scale, CalibrationType calibrationType = CalibrationType.DEFAULTCALIBRATION);
		public static Result CalibrateBodyTrackingLog(UInt64 ts, int skeletonId, float userHeight,
			BodyTrackingMode bodyTrackingMode,
			TrackedDevicePose[] trackedDevicePose, UInt32 deviceCount,
			ref float scale, CalibrationType calibrationType = CalibrationType.DEFAULTCALIBRATION)
		{
			sb.Clear().Append("CalibrateBodyTracking() ").Append(ts)
				.Append(", id: ").Append(skeletonId)
				.Append("bodyTrackingMode: ").Append(bodyTrackingMode.Name());
			DEBUG(sb);

			sb.Clear().Append("deviceCount: ").Append(deviceCount); DEBUG(sb);
			for (UInt32 i = 0; i < deviceCount; i++)
			{
				sb.Clear().Append("trackedDevicePose[").Append(i).Append("] ").Append(trackedDevicePose[i].Log());
				DEBUG(sb);
			}

			sb.Clear().Append("scale: ").Append(scale).Append(", calibrationType: ").Append(calibrationType.Name());
			DEBUG(sb);

			return CalibrateBodyTracking(ts, skeletonId, userHeight, bodyTrackingMode, trackedDevicePose, deviceCount, ref scale, calibrationType);
		}

		[DllImport("bodytracking")]
		/**
		 *  @brief Get the amount of output skeleton joints.
		 *  @param[in] timestamp (unit:us)
		 *  @param[in] skeleton id.
		 *  @param[out] the amount of output skeleton joints.
		 *  @param[out] success or not.
		 **/
		public static extern Result GetOutputJointCount(UInt64 ts, int skeletonId, ref UInt32 jointCount);

		[DllImport("bodytracking")]
		/**
		 *  @brief Update and get skeleton joints pose every frame. Must be called after calibrate.
		 *  @param[in] timestamp (unit:us)
		 *  @param[in] skeleton id.
		 *  @param[in] trackedDevicePose. The tracked device poses.(Left-Handed coordinate sytstem. x right, y up, z forward. unit: m)
		 *  @param[in] deviceCount. The amount of tracked devices
		 *  @param[out] output joints of skeleton. If the pose state of joint equals to 3(Translation|Rotation), it means the joint's pose is valid.
		 *  @param[in] jointCount. The amount of joints.
		 *  @param[out] success or not.
		 **/
		public static extern Result UpdateBodyTracking(UInt64 ts, int skeletonId,
			TrackedDevicePose[] trackedDevicePose, UInt32 deviceCount,
			[In, Out] Joint[] outJoint, UInt32 jointCount);
		public static Result UpdateBodyTrackingLog(UInt64 ts, int skeletonId,
			TrackedDevicePose[] trackedDevicePose, UInt32 deviceCount,
			[In, Out] Joint[] outJoint, UInt32 jointCount)
		{
			logFrame++;
			logFrame %= 300;
			fbtIntervalLog = (logFrame == 0);

			Result result = UpdateBodyTracking(ts, skeletonId, trackedDevicePose, deviceCount, outJoint, jointCount);
			if (fbtIntervalLog && result == Result.SUCCESS)
			{
				sb.Clear();
				sb.Append("UpdateBodyTracking() ").Append(ts).Append(", id: ").Append(skeletonId).Append("\n");
				sb.Append("deviceCount: ").Append(deviceCount).Append("\n");
				DEBUG(sb);

				for (UInt32 i = 0; i < deviceCount; i++)
				{
					sb.Clear();
					sb.Append("trackedDevicePose[").Append(i).Append("] ").Append(trackedDevicePose[i].Log());
					DEBUG(sb);
				}

				sb.Clear();
				sb.Append("jointCount: ").Append(jointCount);
				DEBUG(sb);

				for (UInt32 i = 0; i < jointCount; i++)
				{
					sb.Clear();
					sb.Append("outJoint[").Append(i).Append("] ").Append(outJoint[i].Log());
					DEBUG(sb);
				}
			}

			return result;
		}

		[DllImport("bodytracking")]
		/**
		 *  @brief Destroy body tracking.
		 *  @param[in] timestamp (unit:us)
		 *  @param[in] skeleton id.
		 *  @param[out] success or not.
		 **/
		public static extern Result DestroyBodyTracking(UInt64 ts, int skeletonId);

		[DllImport("bodytracking")]
		/**
		 *  @brief Get the amount of default skeleton joints.
		 *  @param[out] the amount of default skeleton joints.
		 *  @param[out] success or not.
		 **/
		public static extern Result GetDefaultSkeletonJointCount(ref UInt32 jointCount);

		[DllImport("bodytracking")]
		/**
		 *  @brief Get default skeleton rotate space.
		 *  @param[out] the rotate space of default skeleton.
		 *  @param[out] success or not.
		 * */
		public static extern Result GetDefaultSkeletonRotateSpace([In, Out] RotateSpace[] rotateSpace, UInt32 jointCount);
	}
	#endregion

	public static class BodyTrackingUtils
	{
		const string LOG_TAG = "Wave.Essence.BodyTracking.BodyTrackingUtils";
		static StringBuilder m_sb = null;
		static StringBuilder sb {
			get {
				if (m_sb == null) { m_sb = new StringBuilder(); }
				return m_sb;
			}
		}
		static void DEBUG(StringBuilder msg)
		{
			if (Log.EnableDebugLog)
				Log.d(LOG_TAG, msg, true);
		}

		public static bool isZero(this Quaternion qua)
		{
			if (qua.x == 0 &&
				qua.y == 0 &&
				qua.z == 0 &&
				qua.w == 0)
			{
				return true;
			}

			return false;
		}
		public static void Validate(ref Quaternion qua)
		{
			if (qua.isZero()) { qua = Quaternion.identity; }
		}
		public static void Update(ref Quaternion qua, Quaternion in_qua)
		{
			qua = in_qua;
			Validate(ref qua);
		}
		public static bool GetQuaternionDiff(Quaternion src, Quaternion dst, out Quaternion diff)
		{
			if (src.IsValid() && dst.IsValid())
			{
				diff = Quaternion.Inverse(src) * dst;
				Validate(ref diff);
				return true;
			}

			diff = Quaternion.identity;
			return false;
		}
		public static void Update(Quaternion qua, ref Vector4 vec)
		{
			vec.x = qua.x;
			vec.y = qua.y;
			vec.z = qua.z;
			vec.w = qua.w;
		}
		public static void Update(Vector4 vec, ref Quaternion qua)
		{
			qua.x = vec.x;
			qua.y = vec.y;
			qua.z = vec.z;
			qua.w = vec.w;
			Validate(ref qua);
		}

		public static string Name(this WVR_TrackedDeviceRole role)
		{
			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_Invalid) { return "Invalid"; }

			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_Hip) { return "Hip"; }
			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_Chest) { return "Chest"; }
			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_Head) { return "Head"; }

			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_LeftAnkle) { return "LeftAnkle"; }
			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_LeftElbow) { return "LeftElbow"; }
			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_LeftFoot) { return "LeftFoot"; }
			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_LeftHand) { return "LeftHand"; }
			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_LeftHandheld) { return "LeftHandheld"; }
			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_LeftKnee) { return "LeftKnee"; }
			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_LeftWrist) { return "LeftWrist"; }

			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_RightAnkle) { return "RightAnkle"; }
			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_RightElbow) { return "RightElbow"; }
			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_RightFoot) { return "RightFoot"; }
			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_RightHand) { return "RightHand"; }
			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_RightHandheld) { return "RightHandheld"; }
			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_RightKnee) { return "RightKnee"; }
			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_RightWrist) { return "RightWrist"; }

			sb.Clear().Append("WVR_TrackedDeviceRole = ").Append(role); DEBUG(sb);
			return "";
		}
		public static string Name(this TrackedDeviceRole role)
		{
			if (role == TrackedDeviceRole.ROLE_CHEST) { return "CHEST"; }
			if (role == TrackedDeviceRole.ROLE_HEAD) { return "HEAD"; }
			if (role == TrackedDeviceRole.ROLE_HIP) { return "HIP"; }

			if (role == TrackedDeviceRole.ROLE_LEFTANKLE) { return "LEFTANKLE"; }
			if (role == TrackedDeviceRole.ROLE_LEFTELBOW) { return "LEFTELBOW"; }
			if (role == TrackedDeviceRole.ROLE_LEFTFOOT) { return "LEFTFOOT"; }
			if (role == TrackedDeviceRole.ROLE_LEFTHAND) { return "LEFTHAND"; }
			if (role == TrackedDeviceRole.ROLE_LEFTHANDHELD) { return "LEFTHANDHELD"; }
			if (role == TrackedDeviceRole.ROLE_LEFTKNEE) { return "LEFTKNEE"; }
			if (role == TrackedDeviceRole.ROLE_LEFTWRIST) { return "LEFTWRIST"; }

			if (role == TrackedDeviceRole.ROLE_RIGHTANKLE) { return "RIGHTANKLE"; }
			if (role == TrackedDeviceRole.ROLE_RIGHTELBOW) { return "RIGHTELBOW"; }
			if (role == TrackedDeviceRole.ROLE_RIGHTFOOT) { return "RIGHTFOOT"; }
			if (role == TrackedDeviceRole.ROLE_RIGHTHAND) { return "RIGHTHAND"; }
			if (role == TrackedDeviceRole.ROLE_RIGHTHANDHELD) { return "RIGHTHANDHELD"; }
			if (role == TrackedDeviceRole.ROLE_RIGHTKNEE) { return "RIGHTKNEE"; }
			if (role == TrackedDeviceRole.ROLE_RIGHTWRIST) { return "RIGHTWRIST"; }

			if (role == TrackedDeviceRole.ROLE_UNDEFINED) { return "UNDEFINED"; }

			sb.Clear().Append("TrackedDeviceRole = ").Append(role); DEBUG(sb);
			return "";
		}
		public static string Name(this DeviceExtRole role)
		{
			if (role == DeviceExtRole.Arm_Wrist) { return "Arm_Wrist"; }
			if (role == DeviceExtRole.Arm_Handheld_Hand) { return "Arm_Handheld_Hand"; }

			if (role == DeviceExtRole.UpperBody_Wrist) { return "UpperBody_Wrist"; }
			if (role == DeviceExtRole.UpperBody_Handheld_Hand) { return "UpperBody_Handheld_Hand"; }

			if (role == DeviceExtRole.FullBody_Wrist_Ankle) { return "FullBody_Wrist_Ankle"; }
			if (role == DeviceExtRole.FullBody_Wrist_Foot) { return "FullBody_Wrist_Foot"; }
			if (role == DeviceExtRole.FullBody_Handheld_Hand_Ankle) { return "FullBody_Handheld_Hand_Ankle"; }
			if (role == DeviceExtRole.FullBody_Handheld_Hand_Foot) { return "FullBody_Handheld_Hand_Foot"; }

			if (role == DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle) { return "UpperBody_Handheld_Hand_Knee_Ankle"; }

			if (role == DeviceExtRole.Unknown) { return "Unknown"; }

			sb.Clear().Append("DeviceExtRole = ").Append(role); DEBUG(sb);
			return "";
		}
		public static string Name(this BodyPoseRole role)
		{
			if (role == BodyPoseRole.Arm_Wrist) { return "Arm_Wrist"; }
			if (role == BodyPoseRole.Arm_Handheld) { return "Arm_Handheld"; }
			if (role == BodyPoseRole.Arm_Hand) { return "Arm_Hand"; }

			if (role == BodyPoseRole.UpperBody_Wrist) { return "UpperBody_Wrist"; }
			if (role == BodyPoseRole.UpperBody_Handheld) { return "UpperBody_Handheld"; }
			if (role == BodyPoseRole.UpperBody_Hand) { return "UpperBody_Hand"; }

			if (role == BodyPoseRole.FullBody_Wrist_Ankle) { return "FullBody_Wrist_Ankle"; }
			if (role == BodyPoseRole.FullBody_Wrist_Foot) { return "FullBody_Wrist_Foot"; }
			if (role == BodyPoseRole.FullBody_Handheld_Ankle) { return "FullBody_Handheld_Ankle"; }
			if (role == BodyPoseRole.FullBody_Handheld_Foot) { return "FullBody_Handheld_Foot"; }
			if (role == BodyPoseRole.FullBody_Hand_Ankle) { return "FullBody_Hand_Ankle"; }
			if (role == BodyPoseRole.FullBody_Hand_Foot) { return "FullBody_Hand_Foot"; }

			if (role == BodyPoseRole.UpperBody_Handheld_Knee_Ankle) { return "UpperBody_Handheld_Knee_Ankle"; }
			if (role == BodyPoseRole.UpperBody_Hand_Knee_Ankle) { return "UpperBody_Hand_Knee_Ankle"; }

			if (role == BodyPoseRole.Unknown) { return "Unknown"; }

			sb.Clear().Append("BodyPoseRole = ").Append(role); DEBUG(sb);
			return "";
		}
		public static string Name(this WVR_BodyTrackingType type)
		{
			if (type == WVR_BodyTrackingType.WVR_BodyTrackingType_Controller) { return "Controller"; }
			if (type == WVR_BodyTrackingType.WVR_BodyTrackingType_Hand) { return "Hand"; }
			if (type == WVR_BodyTrackingType.WVR_BodyTrackingType_HMD) { return "HMD"; }
			if (type == WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker) { return "ViveSelfTracker"; }
			if (type == WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTrackerIM) { return "ViveSelfTrackerIM"; }
			if (type == WVR_BodyTrackingType.WVR_BodyTrackingType_WristTracker) { return "WristTracker"; }

			if (type == WVR_BodyTrackingType.WVR_BodyTrackingType_Invalid) { return "Invalid"; }

			sb.Clear().Append("WVR_BodyTrackingType = ").Append(type); DEBUG(sb);
			return "";
		}
		public static string Name(this TrackerRole role)
		{
			if (role == TrackerRole.Ankle_Left) { return "Ankle_Left"; }
			if (role == TrackerRole.Ankle_Right) { return "Ankle_Right"; }
			if (role == TrackerRole.Calf_Left) { return "Calf_Left"; }
			if (role == TrackerRole.Calf_Right) { return "Calf_Right"; }
			if (role == TrackerRole.Camera) { return "Camera"; }
			if (role == TrackerRole.Chest) { return "Chest"; }
			if (role == TrackerRole.Elbow_Left) { return "Elbow_Left"; }
			if (role == TrackerRole.Elbow_Right) { return "Elbow_Right"; }
			if (role == TrackerRole.Foot_Left) { return "Foot_Left"; }
			if (role == TrackerRole.Foot_Right) { return "Foot_Right"; }
			if (role == TrackerRole.Forearm_Left) { return "Forearm_Left"; }
			if (role == TrackerRole.Forearm_Right) { return "Forearm_Right"; }
			if (role == TrackerRole.Hand_Left) { return "Hand_Left"; }
			if (role == TrackerRole.Hand_Right) { return "Hand_Right"; }
			if (role == TrackerRole.Keyboard) { return "Keyboard"; }
			if (role == TrackerRole.Knee_Left) { return "Knee_Left"; }
			if (role == TrackerRole.Knee_Right) { return "Knee_Right"; }
			if (role == TrackerRole.Pair1_Left) { return "Pair1_Left"; }
			if (role == TrackerRole.Pair1_Right) { return "Pair1_Right"; }
			if (role == TrackerRole.Shoulder_Left) { return "Shoulder_Left"; }
			if (role == TrackerRole.Shoulder_Right) { return "Shoulder_Right"; }
			if (role == TrackerRole.Standalone) { return "Standalone"; }
			if (role == TrackerRole.Thigh_Left) { return "Thigh_Left"; }
			if (role == TrackerRole.Thigh_Right) { return "Thigh_Right"; }
			if (role == TrackerRole.Undefined) { return "Undefined"; }
			if (role == TrackerRole.Upper_Arm_Left) { return "Upper_Arm_Left"; }
			if (role == TrackerRole.Upper_Arm_Right) { return "Upper_Arm_Right"; }
			if (role == TrackerRole.Waist) { return "Waist"; }
			if (role == TrackerRole.Wrist_Left) { return "Wrist_Left"; }
			if (role == TrackerRole.Wrist_Right) { return "Wrist_Right"; }

			sb.Clear().Append("TrackerRole = ").Append(role); DEBUG(sb);
			return "";
		}
		public static string Name(this BodyTrackingMode mode)
		{
			if (mode == BodyTrackingMode.ARMIK) { return "ARMIK"; }
			if (mode == BodyTrackingMode.FULLBODYIK) { return "FULLBODYIK"; }
			if (mode == BodyTrackingMode.LEGFK) { return "LEGFK"; }
			if (mode == BodyTrackingMode.LEGIK) { return "LEGIK"; }
			if (mode == BodyTrackingMode.SPINEIK) { return "SPINEIK"; }
			if (mode == BodyTrackingMode.UNKNOWNMODE) { return "Unknown"; }
			if (mode == BodyTrackingMode.UPPERBODYIK) { return "UPPERBODYIK"; }
			if (mode == BodyTrackingMode.UPPERIKANDLEGFK) { return "UPPERIKANDLEGFK"; }

			sb.Clear().Append("BodyTrackingMode = ").Append(mode); DEBUG(sb);
			return "";
		}
		public static string Name(this BodyTrackingResult result)
		{
			if (result == BodyTrackingResult.ERROR_AVATAR_INIT_FAILED) { return "ERROR_AVATAR_INIT_FAILED"; }
			if (result == BodyTrackingResult.ERROR_BODYTRACKINGMODE_NOT_ALIGNED) { return "ERROR_BODYTRACKINGMODE_NOT_ALIGNED"; }
			if (result == BodyTrackingResult.ERROR_BODYTRACKINGMODE_NOT_FOUND) { return "ERROR_BODYTRACKINGMODE_NOT_FOUND"; }
			if (result == BodyTrackingResult.ERROR_CALIBRATE_FAILED) { return "ERROR_CALIBRATE_FAILED"; }
			if (result == BodyTrackingResult.ERROR_COMPUTE_FAILED) { return "ERROR_COMPUTE_FAILED"; }
			if (result == BodyTrackingResult.ERROR_FATAL_ERROR) { return "ERROR_FATAL_ERROR"; }
			if (result == BodyTrackingResult.ERROR_IK_NOT_DESTROYED) { return "ERROR_IK_NOT_DESTROYED"; }
			if (result == BodyTrackingResult.ERROR_IK_NOT_UPDATED) { return "ERROR_IK_NOT_UPDATED"; }
			if (result == BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID) { return "ERROR_INPUTPOSE_NOT_VALID"; }
			if (result == BodyTrackingResult.ERROR_INVALID_ARGUMENT) { return "ERROR_INVALID_ARGUMENT"; }
			if (result == BodyTrackingResult.ERROR_JOINT_NOT_FOUND) { return "ERROR_JOINT_NOT_FOUND"; }
			if (result == BodyTrackingResult.ERROR_NOT_CALIBRATED) { return "ERROR_NOT_CALIBRATED"; }
			if (result == BodyTrackingResult.ERROR_SKELETONID_NOT_FOUND) { return "ERROR_SKELETONID_NOT_FOUND"; }
			if (result == BodyTrackingResult.ERROR_SOLVER_NOT_FOUND) { return "ERROR_SOLVER_NOT_FOUND"; }
			if (result == BodyTrackingResult.ERROR_TABLE_STATIC) { return "ERROR_TABLE_STATIC"; }
			if (result == BodyTrackingResult.ERROR_TRACKER_AMOUNT_FAILED) { return "ERROR_TRACKER_AMOUNT_FAILED"; }
			if (result == BodyTrackingResult.SUCCESS) { return "SUCCESS"; }

			sb.Clear().Append("BodyTrackingResult = ").Append(result); DEBUG(sb);
			return "";
		}
		public static string Name(this JointType type)
		{
			if (type == JointType.HIP) { return "HIP"; }

			if (type == JointType.LEFTTHIGH) { return "LEFTTHIGH"; }
			if (type == JointType.LEFTLEG) { return "LEFTLEG"; }
			if (type == JointType.LEFTANKLE) { return "LEFTANKLE"; }
			if (type == JointType.LEFTFOOT) { return "LEFTFOOT"; }

			if (type == JointType.RIGHTTHIGH) { return "RIGHTTHIGH"; }
			if (type == JointType.RIGHTLEG) { return "RIGHTLEG"; }
			if (type == JointType.RIGHTANKLE) { return "RIGHTANKLE"; }
			if (type == JointType.RIGHTFOOT) { return "RIGHTFOOT"; }

			if (type == JointType.WAIST) { return "WAIST"; }

			if (type == JointType.SPINELOWER) { return "SPINELOWER"; }
			if (type == JointType.SPINEMIDDLE) { return "SPINEMIDDLE"; }
			if (type == JointType.SPINEHIGH) { return "SPINEHIGH"; }

			if (type == JointType.CHEST) { return "CHEST"; }
			if (type == JointType.NECK) { return "NECK"; }
			if (type == JointType.HEAD) { return "HEAD"; }

			if (type == JointType.LEFTCLAVICLE) { return "LEFTCLAVICLE"; }
			if (type == JointType.LEFTSCAPULA) { return "LEFTSCAPULA"; }
			if (type == JointType.LEFTUPPERARM) { return "LEFTUPPERARM"; }
			if (type == JointType.LEFTFOREARM) { return "LEFTFOREARM"; }
			if (type == JointType.LEFTHAND) { return "LEFTHAND"; }

			if (type == JointType.RIGHTCLAVICLE) { return "RIGHTCLAVICLE"; }
			if (type == JointType.RIGHTSCAPULA) { return "RIGHTSCAPULA"; }
			if (type == JointType.RIGHTUPPERARM) { return "RIGHTUPPERARM"; }
			if (type == JointType.RIGHTFOREARM) { return "RIGHTFOREARM"; }
			if (type == JointType.RIGHTHAND) { return "RIGHTHAND"; }

			sb.Clear().Append("JointType = ").Append(type); DEBUG(sb);
			return "";
		}
		public static string Name(this CalibrationType type)
		{
			if (type == CalibrationType.DEFAULTCALIBRATION) { return "DEFAULTCALIBRATION"; }
			if (type == CalibrationType.HEIGHTCALIBRATION) { return "HEIGHTCALIBRATION"; }
			if (type == CalibrationType.TOFFSETCALIBRATION) { return "TOFFSETCALIBRATION"; }

			sb.Clear().Append("CalibrationType = ").Append(type); DEBUG(sb);
			return "";
		}

		public static BodyTrackingResult Type(this Result result)
		{
			if (result == Result.ERROR_AVATAR_INIT_FAILED) { return BodyTrackingResult.ERROR_AVATAR_INIT_FAILED; }
			if (result == Result.ERROR_BODYTRACKINGMODE_NOT_ALIGNED) { return BodyTrackingResult.ERROR_BODYTRACKINGMODE_NOT_ALIGNED; }
			if (result == Result.ERROR_BODYTRACKINGMODE_NOT_FOUND) { return BodyTrackingResult.ERROR_BODYTRACKINGMODE_NOT_FOUND; }
			if (result == Result.ERROR_CALIBRATE_FAILED) { return BodyTrackingResult.ERROR_CALIBRATE_FAILED; }
			if (result == Result.ERROR_COMPUTE_FAILED) { return BodyTrackingResult.ERROR_COMPUTE_FAILED; }
			if (result == Result.ERROR_FATAL_ERROR) { return BodyTrackingResult.ERROR_FATAL_ERROR; }
			if (result == Result.ERROR_INPUTPOSE_NOT_VALID) { return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID; }
			if (result == Result.ERROR_JOINT_NOT_FOUND) { return BodyTrackingResult.ERROR_JOINT_NOT_FOUND; }
			if (result == Result.ERROR_NOT_CALIBRATED) { return BodyTrackingResult.ERROR_NOT_CALIBRATED; }
			if (result == Result.ERROR_NOT_INITIALIZATION) { return BodyTrackingResult.ERROR_NOT_INITIALIZATION; }
			if (result == Result.ERROR_SKELETONID_NOT_FOUND) { return BodyTrackingResult.ERROR_SKELETONID_NOT_FOUND; }
			if (result == Result.ERROR_SOLVER_NOT_FOUND) { return BodyTrackingResult.ERROR_SOLVER_NOT_FOUND; }
			if (result == Result.ERROR_TABLE_STATIC) { return BodyTrackingResult.ERROR_TABLE_STATIC; }
			if (result == Result.ERROR_TRACKER_AMOUNT_FAILED) { return BodyTrackingResult.ERROR_TRACKER_AMOUNT_FAILED; }

			return BodyTrackingResult.SUCCESS;
		}

		public static TrackedDeviceRole Role(this WVR_TrackedDeviceRole role)
		{
			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_Invalid) { return TrackedDeviceRole.ROLE_UNDEFINED; }

			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_Chest) { return TrackedDeviceRole.ROLE_CHEST; }
			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_Head) { return TrackedDeviceRole.ROLE_HEAD; }
			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_Hip) { return TrackedDeviceRole.ROLE_HIP; }

			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_LeftAnkle) { return TrackedDeviceRole.ROLE_LEFTANKLE; }
			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_LeftElbow) { return TrackedDeviceRole.ROLE_LEFTELBOW; }
			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_LeftFoot) { return TrackedDeviceRole.ROLE_LEFTFOOT; }
			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_LeftHand) { return TrackedDeviceRole.ROLE_LEFTHAND; }
			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_LeftHandheld) { return TrackedDeviceRole.ROLE_LEFTHANDHELD; }
			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_LeftKnee) { return TrackedDeviceRole.ROLE_LEFTKNEE; }
			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_LeftWrist) { return TrackedDeviceRole.ROLE_LEFTWRIST; }

			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_RightAnkle) { return TrackedDeviceRole.ROLE_RIGHTANKLE; }
			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_RightElbow) { return TrackedDeviceRole.ROLE_RIGHTELBOW; }
			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_RightFoot) { return TrackedDeviceRole.ROLE_RIGHTFOOT; }
			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_RightHand) { return TrackedDeviceRole.ROLE_RIGHTHAND; }
			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_RightHandheld) { return TrackedDeviceRole.ROLE_RIGHTHANDHELD; }
			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_RightKnee) { return TrackedDeviceRole.ROLE_RIGHTKNEE; }
			if (role == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_RightWrist) { return TrackedDeviceRole.ROLE_RIGHTWRIST; }

			return TrackedDeviceRole.ROLE_UNDEFINED;
		}

		public static void Update([In] Joint joint, ref Extrinsic ext)
		{
			ext.translation = joint.translation;
			BodyTrackingUtils.Update(ref ext.rotation, joint.rotation);
		}

		readonly static DateTime kBeginTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
		public static UInt64 GetTimeStamp(bool bflag = true)
		{
			TimeSpan ts = DateTime.UtcNow - kBeginTime;
			return Convert.ToUInt64(ts.TotalMilliseconds);
		}

		public readonly static WVR_BodyTrackingType[] s_BodyTrackingTypes =
		{
			WVR_BodyTrackingType.WVR_BodyTrackingType_HMD,
			WVR_BodyTrackingType.WVR_BodyTrackingType_Controller,
			WVR_BodyTrackingType.WVR_BodyTrackingType_Hand,
			WVR_BodyTrackingType.WVR_BodyTrackingType_WristTracker,
			WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker,
			WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTrackerIM,
		};

		/// <summary> Retrieves the body pose role according to the currently tracked device poses. </summary>
		public static BodyPoseRole GetBodyPoseRole([In] TrackedDevicePose[] trackedDevicePoses, [In] UInt32 trackedDevicePoseCount)
		{
			UInt64 ikRoles = 0;
			sb.Clear();
			for (UInt32 i = 0; i < trackedDevicePoseCount; i++)
			{
				sb.Append("GetBodyPoseRole() pose ").Append(i)
					.Append(" role ").Append(trackedDevicePoses[i].trackedDeviceRole.Name())
					.Append("\n");
#pragma warning disable CS0675 // Bitwise-or operator used on a sign-extended operand
				ikRoles |= (UInt64)(1 << (Int32)trackedDevicePoses[i].trackedDeviceRole);
#pragma warning restore CS0675 // Bitwise-or operator used on a sign-extended operand
			}
			DEBUG(sb);

			BodyPoseRole m_IKRoles = BodyPoseRole.Unknown;

			// Upper Body + Leg FK
			if ((ikRoles & (UInt64)BodyPoseRole.UpperBody_Handheld_Knee_Ankle) == (UInt64)BodyPoseRole.UpperBody_Handheld_Knee_Ankle)
				m_IKRoles = BodyPoseRole.UpperBody_Handheld_Knee_Ankle;
			else if ((ikRoles & (UInt64)BodyPoseRole.UpperBody_Hand_Knee_Ankle) == (UInt64)BodyPoseRole.UpperBody_Hand_Knee_Ankle)
				m_IKRoles = BodyPoseRole.UpperBody_Hand_Knee_Ankle;

			// ToDo: else if {Hybrid mode}

			// Full body
			else if ((ikRoles & (UInt64)BodyPoseRole.FullBody_Wrist_Ankle) == (UInt64)BodyPoseRole.FullBody_Wrist_Ankle)
				m_IKRoles = BodyPoseRole.FullBody_Wrist_Ankle;
			else if ((ikRoles & (UInt64)BodyPoseRole.FullBody_Wrist_Foot) == (UInt64)BodyPoseRole.FullBody_Wrist_Foot)
				m_IKRoles = BodyPoseRole.FullBody_Wrist_Foot;
			else if ((ikRoles & (UInt64)BodyPoseRole.FullBody_Handheld_Ankle) == (UInt64)BodyPoseRole.FullBody_Handheld_Ankle)
				m_IKRoles = BodyPoseRole.FullBody_Handheld_Ankle;
			else if ((ikRoles & (UInt64)BodyPoseRole.FullBody_Handheld_Foot) == (UInt64)BodyPoseRole.FullBody_Handheld_Foot)
				m_IKRoles = BodyPoseRole.FullBody_Handheld_Foot;
			else if ((ikRoles & (UInt64)BodyPoseRole.FullBody_Hand_Ankle) == (UInt64)BodyPoseRole.FullBody_Hand_Ankle)
				m_IKRoles = BodyPoseRole.FullBody_Hand_Ankle;
			else if ((ikRoles & (UInt64)BodyPoseRole.FullBody_Hand_Foot) == (UInt64)BodyPoseRole.FullBody_Hand_Foot)
				m_IKRoles = BodyPoseRole.FullBody_Hand_Foot;

			// Upper body
			else if ((ikRoles & (UInt64)BodyPoseRole.UpperBody_Wrist) == (UInt64)BodyPoseRole.UpperBody_Wrist)
				m_IKRoles = BodyPoseRole.UpperBody_Wrist;
			else if ((ikRoles & (UInt64)BodyPoseRole.UpperBody_Handheld) == (UInt64)BodyPoseRole.UpperBody_Handheld)
				m_IKRoles = BodyPoseRole.UpperBody_Handheld;
			else if ((ikRoles & (UInt64)BodyPoseRole.UpperBody_Hand) == (UInt64)BodyPoseRole.UpperBody_Hand)
				m_IKRoles = BodyPoseRole.UpperBody_Hand;

			// Arm
			else if ((ikRoles & (UInt64)BodyPoseRole.Arm_Wrist) == (UInt64)BodyPoseRole.Arm_Wrist)
				m_IKRoles = BodyPoseRole.Arm_Wrist;
			else if ((ikRoles & (UInt64)BodyPoseRole.Arm_Handheld) == (UInt64)BodyPoseRole.Arm_Handheld)
				m_IKRoles = BodyPoseRole.Arm_Handheld;
			else if ((ikRoles & (UInt64)BodyPoseRole.Arm_Hand) == (UInt64)BodyPoseRole.Arm_Hand)
				m_IKRoles = BodyPoseRole.Arm_Hand;

			sb.Clear().Append("GetBodyPoseRole() role: ").Append(m_IKRoles.Name()); DEBUG(sb);
			return m_IKRoles;
		}
		/// <summary> Checks if the body pose role and body tracking mode are matched./// </summary>
		public static bool MatchBodyTrackingMode(BodyTrackingMode mode, BodyPoseRole poseRole)
		{
			sb.Clear().Append("MatchBodyTrackingMode() mode: ").Append(mode.Name()).Append(", poseRole: ").Append(poseRole.Name()); DEBUG(sb);

			if (poseRole == BodyPoseRole.UpperBody_Handheld_Knee_Ankle || poseRole == BodyPoseRole.UpperBody_Hand_Knee_Ankle)
			{
				if (mode == BodyTrackingMode.UPPERIKANDLEGFK)
					return true;
			}
			if (poseRole == BodyPoseRole.FullBody_Wrist_Ankle || poseRole == BodyPoseRole.FullBody_Wrist_Foot ||
				poseRole == BodyPoseRole.FullBody_Handheld_Ankle || poseRole == BodyPoseRole.FullBody_Handheld_Foot ||
				poseRole == BodyPoseRole.FullBody_Hand_Ankle || poseRole == BodyPoseRole.FullBody_Hand_Foot)
			{
				if (mode == BodyTrackingMode.FULLBODYIK || mode == BodyTrackingMode.UPPERBODYIK || mode == BodyTrackingMode.ARMIK)
					return true;
			}
			if (poseRole == BodyPoseRole.UpperBody_Wrist || poseRole == BodyPoseRole.UpperBody_Handheld || poseRole == BodyPoseRole.UpperBody_Hand)
			{
				if (mode == BodyTrackingMode.UPPERBODYIK || mode == BodyTrackingMode.ARMIK)
					return true;
			}
			if (poseRole == BodyPoseRole.Arm_Wrist || poseRole == BodyPoseRole.Arm_Handheld || poseRole == BodyPoseRole.Arm_Hand)
			{
				if (mode == BodyTrackingMode.ARMIK)
					return true;
			}

			return false;
		}


		public static bool UseDeviceExtrinsic(BodyPoseRole calibRole, WVR_TrackedDeviceRole wvrRole, WVR_BodyTrackingType wvrType)
		{
			if (wvrRole == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_Head)
				return true;

			if (wvrRole == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_LeftWrist || wvrRole == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_RightWrist)
			{
				if (calibRole == BodyPoseRole.Arm_Wrist ||
					calibRole == BodyPoseRole.UpperBody_Wrist ||
					calibRole == BodyPoseRole.FullBody_Wrist_Ankle ||
					calibRole == BodyPoseRole.FullBody_Wrist_Foot)
				{
					if (wvrType == WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker ||
						wvrType == WVR_BodyTrackingType.WVR_BodyTrackingType_WristTracker)
						return true;
				}
			}
			if (wvrRole == WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_Hip)
			{
				if (calibRole == BodyPoseRole.UpperBody_Wrist || calibRole == BodyPoseRole.UpperBody_Handheld || calibRole == BodyPoseRole.UpperBody_Hand ||
					
					calibRole == BodyPoseRole.UpperBody_Handheld_Knee_Ankle || calibRole == BodyPoseRole.UpperBody_Hand_Knee_Ankle ||
					
					calibRole == BodyPoseRole.FullBody_Wrist_Ankle || calibRole == BodyPoseRole.FullBody_Wrist_Foot ||
					calibRole == BodyPoseRole.FullBody_Handheld_Ankle || calibRole == BodyPoseRole.FullBody_Handheld_Foot ||
					calibRole == BodyPoseRole.FullBody_Hand_Ankle || calibRole == BodyPoseRole.FullBody_Handheld_Foot)
				{
					if (wvrType == WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker)
						return true;
				}
			}

			return false;
		}
		/// <summary> Retrievs the device extrinsic role according to the calibration pose role and tracked device extrinsics in use. </summary>
		public static DeviceExtRole GetDeviceExtRole(BodyPoseRole calibRole, [In] TrackedDeviceExtrinsic[] bodyTrackedDevices, [In] UInt32 bodyTrackedDeviceCount)
		{
			sb.Clear().Append("GetDeviceExtRole() calibRole: ").Append(calibRole.Name()); DEBUG(sb);

			UInt64 ikRoles = 0;
			sb.Clear();
			for (UInt32 i = 0; i < bodyTrackedDeviceCount; i++)
			{
				sb.Append("GetDeviceExtRole() device ").Append(i)
					.Append(" role ").Append(bodyTrackedDevices[i].trackedDeviceRole.Name())
					.Append("\n");
#pragma warning disable CS0675 // Bitwise-or operator used on a sign-extended operand
				ikRoles |= (UInt64)(1 << (Int32)bodyTrackedDevices[i].trackedDeviceRole);
#pragma warning restore CS0675 // Bitwise-or operator used on a sign-extended operand
			}
			DEBUG(sb);

			DeviceExtRole m_IKRoles = DeviceExtRole.Unknown;

			// Upper Body + Leg FK
			if (calibRole == BodyPoseRole.UpperBody_Handheld_Knee_Ankle || calibRole == BodyPoseRole.UpperBody_Hand_Knee_Ankle)
			{
				if ((ikRoles & (UInt64)DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle) == (UInt64)DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle)
					m_IKRoles = DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle;
			}

			// Full Body
			if (calibRole == BodyPoseRole.FullBody_Wrist_Ankle)
			{
				if ((ikRoles & (UInt64)DeviceExtRole.FullBody_Wrist_Ankle) == (UInt64)DeviceExtRole.FullBody_Wrist_Ankle)
					m_IKRoles = DeviceExtRole.FullBody_Wrist_Ankle;
			}
			if (calibRole == BodyPoseRole.FullBody_Wrist_Foot)
			{
				if ((ikRoles & (UInt64)DeviceExtRole.FullBody_Wrist_Foot) == (UInt64)DeviceExtRole.FullBody_Wrist_Foot)
					m_IKRoles = DeviceExtRole.FullBody_Wrist_Foot;
			}
			if (calibRole == BodyPoseRole.FullBody_Handheld_Ankle || calibRole == BodyPoseRole.FullBody_Hand_Ankle)
			{
				if ((ikRoles & (UInt64)DeviceExtRole.FullBody_Handheld_Hand_Ankle) == (UInt64)DeviceExtRole.FullBody_Handheld_Hand_Ankle)
					m_IKRoles = DeviceExtRole.FullBody_Handheld_Hand_Ankle;
			}
			if (calibRole == BodyPoseRole.FullBody_Handheld_Foot || calibRole == BodyPoseRole.FullBody_Hand_Foot)
			{
				if ((ikRoles & (UInt64)DeviceExtRole.FullBody_Handheld_Hand_Foot) == (UInt64)DeviceExtRole.FullBody_Handheld_Hand_Foot)
					m_IKRoles = DeviceExtRole.FullBody_Handheld_Hand_Foot;
			}

			// Upper Body
			if (calibRole == BodyPoseRole.UpperBody_Wrist)
			{
				if ((ikRoles & (UInt64)DeviceExtRole.UpperBody_Wrist) == (UInt64)DeviceExtRole.UpperBody_Wrist)
					m_IKRoles = DeviceExtRole.UpperBody_Wrist;
			}
			if (calibRole == BodyPoseRole.UpperBody_Handheld || calibRole == BodyPoseRole.UpperBody_Hand)
			{
				if ((ikRoles & (UInt64)DeviceExtRole.UpperBody_Handheld_Hand) == (UInt64)DeviceExtRole.UpperBody_Handheld_Hand)
					m_IKRoles = DeviceExtRole.UpperBody_Handheld_Hand;
			}

			// Arm
			if (calibRole == BodyPoseRole.Arm_Wrist)
			{
				if ((ikRoles & (UInt64)DeviceExtRole.Arm_Wrist) == (UInt64)DeviceExtRole.Arm_Wrist)
					m_IKRoles = DeviceExtRole.Arm_Wrist;
			}
			if (calibRole == BodyPoseRole.Arm_Handheld || calibRole == BodyPoseRole.Arm_Hand)
			{
				if ((ikRoles & (UInt64)DeviceExtRole.Arm_Handheld_Hand) == (UInt64)DeviceExtRole.Arm_Handheld_Hand)
					m_IKRoles = DeviceExtRole.Arm_Handheld_Hand;
			}

			sb.Clear().Append("GetDeviceExtRole() role: ").Append(m_IKRoles.Name()); DEBUG(sb);
			return m_IKRoles;
		}
		/// <summary> Checks if the device extrinsic role and body tracking mode are matched./// </summary>
		public static bool MatchBodyTrackingMode(BodyTrackingMode mode, DeviceExtRole extRole)
		{
			sb.Clear().Append("MatchBodyTrackingMode() mode: ").Append(mode.Name()).Append(", extRole: ").Append(extRole.Name()); DEBUG(sb);

			if (mode == BodyTrackingMode.ARMIK)
			{
				if (extRole == DeviceExtRole.Arm_Wrist || extRole == DeviceExtRole.Arm_Handheld_Hand)
					return true;
			}
			if (mode == BodyTrackingMode.UPPERBODYIK)
			{
				if (extRole == DeviceExtRole.UpperBody_Wrist || extRole == DeviceExtRole.UpperBody_Handheld_Hand)
					return true;
			}
			if (mode == BodyTrackingMode.FULLBODYIK)
			{
				if (extRole == DeviceExtRole.FullBody_Wrist_Ankle ||
					extRole == DeviceExtRole.FullBody_Wrist_Foot ||
					extRole == DeviceExtRole.FullBody_Handheld_Hand_Ankle ||
					extRole == DeviceExtRole.FullBody_Handheld_Hand_Foot)
					return true;
			}
			if (mode == BodyTrackingMode.UPPERIKANDLEGFK)
			{
				if (extRole == DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle)
					return true;
			}

			return false;
		}
	}
}
