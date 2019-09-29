using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity.Preview;
using HoloToolkit.Unity.Preview.SpectatorView;
namespace ShowNowMrKit{
	public class HoloConnectionStatusController : MonoBehaviour {

		/// <summary>
		/// UI Textfield to display status
		/// </summary>
		[SerializeField]
		[Tooltip("UI Textfield to display status")]
		private Text text;

		/// <summary>
		/// UI Textfield to display status
		/// </summary>
		public Text Text
		{
			get
			{
				return text;
			}

			set
			{
				text = value;
			}
		}


		/// <summary>
		/// Object responsible for aligning holograms on mobile and HoloLens
		/// </summary>
		[Tooltip("Object responsible for aligning holograms on mobile and HoloLens")]
		[SerializeField]
		private HoloWorldSync worldSync;
		/// <summary>
		/// WorldSync
		/// </summary>
		public HoloWorldSync WorldSync
		{
			get
			{
				return worldSync;
			}

			set
			{
				worldSync = value;
			}
		}

		/// <summary>
		/// Object to detect whether the world anchor has been located
		/// </summary>
		[Tooltip("Object to detect whether the world anchor has been located")]
		[SerializeField]
		private AnchorLocated anchorLocated;
		/// <summary>
		/// AnchorLocated
		/// </summary>
		public AnchorLocated AnchorLocated
		{
			get
			{
				return anchorLocated;
			}

			set
			{
				anchorLocated = value;
			}
		}

		void Start ()
		{
			if (Text == null)
			{
				Text = GetComponent<Text>();
			}


			if (WorldSync == null)
			{
				WorldSync = FindObjectOfType<HoloWorldSync>();
			}

			if (AnchorLocated == null)
			{
				AnchorLocated = FindObjectOfType<AnchorLocated>();
			}

			// Suscribe to Anchor and Network events
			AnchorLocated.OnAnchorLocated += PromptShowToHoloLens;

			// First status
			if (Text != null)
			{
				Text.text = "Locating Floor...";
			}
		}
		private bool MarkerGeneratedEventSent = false;
		/// <summary>
		/// Sets text displayed on screen before marker detected
		/// </summary>
		private void PromptShowToHoloLens()
		{
			Text.text = "Show to HoloLens";
			if(MarkerGeneratedEventSent == false){
				MarkerGeneratedEventSent = true;
				Debug.Log ("PromptShowToHoloLens Send Marker Generated");
				SpectatorViewManager._instance.MarkerGenerated();
			}
		}

		/// <summary>
		/// Sets text displayed on screen once marker has been detected,
		/// before mobile has connected to session
		/// </summary>
		private void PromptConnecting()
		{
			Text.text = "Connecting...";
		}

		/// <summary>
		/// Sets text displayed on screen once marker has been detected,
		/// mobile has connected to the HoloLens session, but before
		/// the world space has been synchronized
		/// </summary>
		private void PromptAlmostThere()
		{
			Text.text = "Almost there...";
		}

		private void OnDestroy()
		{
			// Unsubscribe from events
			AnchorLocated.OnAnchorLocated -= PromptShowToHoloLens;

		}
	}
}