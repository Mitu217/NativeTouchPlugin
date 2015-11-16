using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class CreateObject {
	static CreateObject() {
		Create ();
	}

	private static void Create() {
		GameObject createdObj = null;
		foreach (GameObject obj in UnityEngine.Object.FindObjectsOfType(typeof(GameObject))) {
			if (obj.GetComponent<MobileTouchInput>()) {
				createdObj = obj;
			}
		}

		if (createdObj == null) {
			createdObj = new GameObject ();
			createdObj.name = "MobileTouchInputObject";
			createdObj.AddComponent<MobileTouchInput> ();
			Hide (createdObj);
		}
	}

	static void Hide (GameObject obj) {
		obj.hideFlags = HideFlags.HideInHierarchy;
	}
}