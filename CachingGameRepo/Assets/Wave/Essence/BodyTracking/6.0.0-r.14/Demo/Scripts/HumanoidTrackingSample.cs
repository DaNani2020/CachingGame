// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Wave.Native;

namespace Wave.Essence.BodyTracking.Demo
{
	public class HumanoidTrackingSample : MonoBehaviour
	{
		const string LOG_TAG = "Wave.Essence.BodyTracking.Demo.HumanoidTrackingSample";
		private StringBuilder m_sb = null;
		private StringBuilder sb {
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

		public HumanoidTracking humanoidTracking = null;
		public Button beginBtn = null;
		public Text canvasTitle = null;

		private void Update()
		{
			if (humanoidTracking == null || canvasTitle == null) { return; }

			canvasTitle.text = humanoidTracking.Tracking + "\n" + "Manually Tracking";
		}

		public void SetArmMode()
		{
			if (humanoidTracking != null)
				humanoidTracking.Tracking = HumanoidTracking.TrackingMode.Arm;
		}
		public void SetUpperMode()
		{
			if (humanoidTracking != null)
				humanoidTracking.Tracking = HumanoidTracking.TrackingMode.UpperBody;
		}
		public void SetFullMode()
		{
			if (humanoidTracking != null)
				humanoidTracking.Tracking = HumanoidTracking.TrackingMode.FullBody;
		}
		public void SetUpperBodyAndLegMode()
		{
			if (humanoidTracking != null)
				humanoidTracking.Tracking = HumanoidTracking.TrackingMode.UpperBodyAndLeg;
		}	

		public void BeginTracking()
		{
			if (humanoidTracking != null)
			{
				if (beginBtn != null) { beginBtn.interactable = false; }
				humanoidTracking.BeginTracking();
			}
		}
		public void EndTracking()
		{
			if (humanoidTracking != null)
			{
				if (beginBtn != null) { beginBtn.interactable = true; }
				humanoidTracking.StopTracking();
			}
		}
	}
}
