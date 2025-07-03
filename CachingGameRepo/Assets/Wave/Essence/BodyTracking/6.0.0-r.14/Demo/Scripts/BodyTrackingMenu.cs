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
using UnityEngine.UI;

namespace Wave.Essence.BodyTracking.Demo
{
	public class BodyTrackingMenu : MonoBehaviour
	{
		public BodyTrackingSample ikScript = null;
		public Text canvasTitle = null;
		public Button beginBtn = null;

		private void Update()
		{
			if (ikScript != null && canvasTitle != null)
			{
				string autoUpdateText = "Manually Tracking";
				canvasTitle.text = ikScript.TrackingMode.Name() + "\n" + autoUpdateText;
			}
		}

		public void SetArmMode()
		{
			if (ikScript != null)
				ikScript.SetArmMode();
		}
		public void SetUpperMode()
		{
			if (ikScript != null)
				ikScript.SetUpperMode();
		}
		public void SetFullMode()
		{
			if (ikScript != null)
				ikScript.SetFullMode();
		}
		public void SetUpperBodyAndLegMode()
		{
			if (ikScript != null)
				ikScript.SetUpperBodyAndLegMode();
		}
		public void BeginTracking()
		{
			if (ikScript != null)
			{
				if (beginBtn != null) { beginBtn.interactable = false; }
				ikScript.BeginTracking();
			}
		}
		public void StopTracking()
		{
			if (ikScript != null)
			{
				if (beginBtn != null) { beginBtn.interactable = true; }
				ikScript.StopTracking();
			}
		}
	}
}
