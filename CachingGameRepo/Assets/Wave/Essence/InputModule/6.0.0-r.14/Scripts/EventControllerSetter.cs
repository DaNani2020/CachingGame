using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using Wave.Native;

namespace Wave.Essence.InputModule
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Camera), typeof(PhysicsRaycaster))]
	public sealed class EventControllerSetter : MonoBehaviour
	{
		const string LOG_TAG = "Wave.Essence.InputModule.EventControllerSetter";
		private void DEBUG(string msg) { Log.d(LOG_TAG, m_ControllerType + " " + msg, true); }

		[SerializeField]
		private XR_Hand m_ControllerType = XR_Hand.Dominant;
		public XR_Hand ControllerType { get { return m_ControllerType; } set { m_ControllerType = value; } }

		// Customized fields delete if not needed anymore
		[SerializeField]
		private Vector3 magicWandLength = new Vector3(0f, 0f, 0.35f);
		[SerializeField][Tooltip("The offset of the magic wand. - Need to be null in Menu scene to render the regulary controller mode lwit hthe regular beam and pointer offset.")]
		private Transform magicWandOffset = null;
		[SerializeField]
		Vector3 controllerBeamOffset = new Vector3(0f, -0.023f, 0f);
		[SerializeField]
		Vector3 controllerPointerOffset = new Vector3(0f, -0.34f, 0f);
		[SerializeField]
		private float beamStartOffset = 0.0f;
		public bool activeBeam = true;
		public bool activePointer = true;
        // End of Customized fields delete if not needed anymore

        private GameObject beamObject = null;
		private ControllerBeam m_Beam = null;
		private GameObject pointerObject = null;
		private ControllerPointer m_Pointer = null;

		private List<GameObject> children = new List<GameObject>();
		private int childrenCount = 0;
		private List<bool> childrenStates = new List<bool>();
		private void CheckChildrenObjects()
		{
			if (childrenCount != transform.childCount)
			{
				DEBUG("CheckChildrenObjects() Children count old: " + childrenCount + ", new: " + transform.childCount);
				childrenCount = transform.childCount;
				children.Clear();
				childrenStates.Clear();
				for (int i = 0; i < childrenCount; i++)
				{
					children.Add(transform.GetChild(i).gameObject);
					childrenStates.Add(transform.GetChild(i).gameObject.activeSelf);
					DEBUG("CheckChildrenObjects() " + gameObject.name + " has child: " + children[i].name + ", active? " + childrenStates[i]);
				}
			}
		}
		private void ForceActivateTargetObjects(bool active)
		{
			for (int i = 0; i < children.Count; i++)
			{
				if (children[i] == null)
					continue;

				if (childrenStates[i])
				{
					DEBUG("ForceActivateTargetObjects() " + (active ? "Activate" : "Deactivate") + " " + children[i].name);
					children[i].SetActive(active);
				}
			}
		}

		private bool hasFocus = false;
		//private bool m_ControllerActive = true;

		private bool mEnabled = false;
		void OnEnable()
		{
			if (!mEnabled)
			{
				// Add a beam.
				beamObject = new GameObject(m_ControllerType.ToString() + "Beam");
				beamObject.transform.SetParent(transform, false);
				beamObject.transform.localPosition = Vector3.zero;
				beamObject.transform.localRotation = Quaternion.identity;
				beamObject.SetActive(false);
				m_Beam = beamObject.AddComponent<ControllerBeam>();
                // ##### Customized positions delete if not needed anymore
				m_Beam.StartOffset = beamStartOffset;
				if(magicWandOffset != null)
				{
					m_Beam.transform.position = magicWandOffset.position + magicWandLength;
				}
				else
				{
					
					m_Beam.transform.position = controllerBeamOffset;
				}
                
                // ##### End of customized position

                m_Beam.BeamType = m_ControllerType;
				beamObject.SetActive(true);

				// Add a pointer.
				pointerObject = new GameObject(m_ControllerType.ToString() + "Pointer");
				pointerObject.transform.SetParent(transform, false);
				pointerObject.transform.localPosition = Vector3.zero;
                pointerObject.transform.localRotation = Quaternion.identity;
				m_Pointer = pointerObject.AddComponent<ControllerPointer>();
				pointerObject.SetActive(false);
				// ##### Customized positions delete if not needed anymore
				if(magicWandOffset != null)
				{
					pointerObject.transform.position = magicWandOffset.position + magicWandLength;
				}
				else
				{
					pointerObject.transform.position = controllerPointerOffset;
				}
				m_Pointer.PointerType = m_ControllerType;
				// pointerObject.SetActive(true); // Currently the pointer will be enabled in the PointerPosition.cs script

				// ##### End of customized position

				hasFocus = ClientInterface.IsFocused;

				if (ControllerInputSwitch.Instance != null)
					Log.i(LOG_TAG, "OnEnable() Loaded ControllerInputSwitch.");

				EventControllerProvider.Instance.SetEventController(m_ControllerType, gameObject);

				mEnabled = true;
			}
		}

		void Start()
		{
			GetComponent<Camera>().stereoTargetEye = StereoTargetEyeMask.None;
			GetComponent<Camera>().enabled = false;
			DEBUG("Start() " + gameObject.name);
		}

		void Update()
		{
			// Customized code to enable and disable the beam and pointer during runtime
			if (!activeBeam){
				beamObject.SetActive(false);
			}else{
				beamObject.SetActive(true);
			}

			if(!activePointer)
			{
				pointerObject.SetActive(false);
			}else{
				pointerObject.SetActive(true);
			}
			// End of customized code
		}
	}
}
