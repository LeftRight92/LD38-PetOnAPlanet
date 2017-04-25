using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISeason : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnGUI() {
		string val = "Spring";
		float season = Planet.instance.season;
		if(season > 1) {
			if(season > 2) {
				if(season > 3) {
					val = "Winter";
				} else {
					val = "Autumn";
				}
			} else {
				val = "Summer";
			}
		}
		GetComponent<UnityEngine.UI.Text>().text = val + ", Year " + Planet.instance.Year;
	}
}
