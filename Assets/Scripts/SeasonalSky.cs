using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeasonalSky : MonoBehaviour {

	[SerializeField]
	private float fadeIn;
	[SerializeField]
	private float fadeOut;
	[SerializeField]
	private bool active;

	private SpriteRenderer renderer;

	private readonly Color SOLID = new Color(1, 1, 1, 1);
	private readonly Color CLEAR = new Color(1, 1, 1, 0);

	// Use this for initialization
	void Start () {
		renderer = GetComponent<SpriteRenderer>();
		renderer.color = active ? SOLID : CLEAR;
	}
	
	// Update is called once per frame
	void Update () {
		if(active) {
			if(Planet.instance.season > fadeOut || Planet.instance.season < fadeIn)
				StartCoroutine(DoFadeOut());
		} else {
			if(Planet.instance.season >= fadeIn && Planet.instance.season <= fadeOut)
				StartCoroutine(DoFadeIn());
		}
	}

	private const float FADE_TIME = 4f;
	IEnumerator DoFadeOut() {
		Debug.Log("Start Fade Out");
		active = false;
		transform.localPosition = new Vector3(0f, 0f, 2f);
		yield return new WaitForSeconds(FADE_TIME);
		renderer.color = CLEAR;
		Debug.Log("Start Fade In");
	}

	IEnumerator DoFadeIn() {
		Debug.Log("Start Fade In");
		active = true;
		transform.localPosition = new Vector3(0f, 0f, 1f);
		var startTime = Time.time;
		while(Time.time < startTime + FADE_TIME) {
			renderer.color = Color.Lerp(CLEAR, SOLID, (Time.time - startTime) / FADE_TIME);
			yield return null;
		}
		renderer.color = SOLID;
		Debug.Log("End Fade In");
	}
}
