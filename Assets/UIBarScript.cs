using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBarScript : MonoBehaviour {

	[SerializeField]
	private Pet pet;
	[SerializeField]
	private bool useSecondaryNeeds;
	[SerializeField]
	private bool scaleVertical;
	[SerializeField]
	private Needs need;

	private void OnGUI() {
		Vector3 scale = transform.localScale;
		if(scaleVertical)
			scale.y = pet.GetNeedValue(need) / 10f;
		else
			scale.x = pet.GetNeedValue(need) / 10f;
		transform.localScale = scale;
	}
}
