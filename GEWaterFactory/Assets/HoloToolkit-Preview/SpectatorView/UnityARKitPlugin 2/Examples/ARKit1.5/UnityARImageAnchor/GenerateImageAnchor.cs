using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class GenerateImageAnchor : MonoBehaviour {


	[SerializeField]
	private ARReferenceImage referenceImage;

	[SerializeField]
	private GameObject prefabToGenerate;
	[SerializeField]
	private Transform worldroot;

	private Vector3 offset;
	private float eulerangleYoffset;

	private bool issetparent = false;
	private int searchcount = 0;
	private int searchlimit = 5;

	private GameObject imageAnchorGO;
	private TextMesh lockedDoneTitle;
	// Use this for initialization
	void Start () {
		UnityARSessionNativeInterface.ARImageAnchorAddedEvent += AddImageAnchor;
		UnityARSessionNativeInterface.ARImageAnchorUpdatedEvent += UpdateImageAnchor;
		UnityARSessionNativeInterface.ARImageAnchorRemovedEvent += RemoveImageAnchor;

	}

	void AddImageAnchor(ARImageAnchor arImageAnchor)
	{
		Debug.Log ("image anchor added");
		if (arImageAnchor.referenceImageName == referenceImage.imageName) {
			Vector3 position = UnityARMatrixOps.GetPosition (arImageAnchor.transform);
			Quaternion rotation = UnityARMatrixOps.GetRotation (arImageAnchor.transform);

			imageAnchorGO = Instantiate<GameObject> (prefabToGenerate, position, rotation);
			lockedDoneTitle = imageAnchorGO.GetComponentInChildren<TextMesh> ();
			lockedDoneTitle.gameObject.SetActive (false);
		}
	}

	void UpdateImageAnchor(ARImageAnchor arImageAnchor)
	{
		Debug.Log ("image anchor updated");
		if (arImageAnchor.referenceImageName == referenceImage.imageName) {
			imageAnchorGO.transform.position = UnityARMatrixOps.GetPosition (arImageAnchor.transform);
			imageAnchorGO.transform.rotation = UnityARMatrixOps.GetRotation (arImageAnchor.transform);
			searchcount++;
			if (!issetparent && searchcount > searchlimit) {
				issetparent = true;
				offset = worldroot.position - imageAnchorGO.transform.position;
				eulerangleYoffset = worldroot.eulerAngles.y - imageAnchorGO.transform.eulerAngles.y;
				lockedDoneTitle.gameObject.SetActive (true);
				Debug.Log ("记录初始偏移  ==  "+ offset.ToString() +"   "+ eulerangleYoffset.ToString());
			} else if (issetparent) 
			{
				Debug.Log ("位置恢复！！！！！！");
				worldroot.position = imageAnchorGO.transform.position + offset;
				worldroot.eulerAngles = new Vector3 (0, imageAnchorGO.transform.eulerAngles.y + eulerangleYoffset, 0);
			}
		}

	}

	void RemoveImageAnchor(ARImageAnchor arImageAnchor)
	{
		Debug.Log ("image anchor removed");
		if (imageAnchorGO) {
			GameObject.Destroy (imageAnchorGO);
		}

	}

	void OnDestroy()
	{
		UnityARSessionNativeInterface.ARImageAnchorAddedEvent -= AddImageAnchor;
		UnityARSessionNativeInterface.ARImageAnchorUpdatedEvent -= UpdateImageAnchor;
		UnityARSessionNativeInterface.ARImageAnchorRemovedEvent -= RemoveImageAnchor;

	}

	// Update is called once per frame
	void Update () {
		
	}
}
