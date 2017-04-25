using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseController : MonoBehaviour {

	private static MouseController instance;
	[SerializeField]public MouseMode currentMouseMode { get; private set; }

	[SerializeField]
	private Button waterButton, magmaButton;
	[SerializeField]
	private Sprite MagmaOn, WaterOn, MagmaOff, WaterOff;

	// Use this for initialization
	void Start () {
		if(instance != null) Debug.LogError("OMG >1 MOUSECONTROLLER");
		instance = this;
		currentMouseMode = MouseMode.NONE;
	}
	
	// Update is called once per frame
	void Update () {
		if(EventSystem.current.IsPointerOverGameObject()) return;
		if(Input.GetMouseButtonDown(0)) {
			TakeActionOnMouseOverObject(
				(RaycastHit2D hit) => hit.collider.GetComponent<Section>().AddWater(),
				(RaycastHit2D hit) => hit.collider.GetComponent<Section>().AddMagma()
			);
		}

		if(Input.GetMouseButtonDown(1)) {
			TakeActionOnMouseOverObject(
				(RaycastHit2D hit) => hit.collider.GetComponent<Section>().SubWater(),
				(RaycastHit2D hit) => hit.collider.GetComponent<Section>().SubMagma()
				);
		}
	}

	private void TakeActionOnMouseOverObject(Action<RaycastHit2D> waterAction, Action<RaycastHit2D> magmaAction) {
		if(currentMouseMode == MouseMode.NONE) return;
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
		if(hit.collider != null) {
			if(currentMouseMode == MouseMode.WATER_TOOL)
				waterAction(hit);
			else
				magmaAction(hit);
		}
		
	}

	public void ToggleMouseModeWater() {
		if(currentMouseMode == MouseMode.WATER_TOOL)
			currentMouseMode = MouseMode.NONE;
		else
			currentMouseMode = MouseMode.WATER_TOOL;
		UpdateButtons();
	}

	public void ToggleMouseModeMagma() {
		if(currentMouseMode == MouseMode.MAGMA_TOOL)
			currentMouseMode = MouseMode.NONE;
		else
			currentMouseMode = MouseMode.MAGMA_TOOL;
		UpdateButtons();
	}

	private void UpdateButtons() {
		if(currentMouseMode == MouseMode.WATER_TOOL) {
			waterButton.GetComponent<Image>().sprite = WaterOn;
			magmaButton.GetComponent<Image>().sprite = MagmaOff;
		}else if(currentMouseMode == MouseMode.MAGMA_TOOL) {
			waterButton.GetComponent<Image>().sprite = WaterOff;
			magmaButton.GetComponent<Image>().sprite = MagmaOn;
		} else {
			waterButton.GetComponent<Image>().sprite = WaterOff;
			magmaButton.GetComponent<Image>().sprite = MagmaOff;
		}
	}

	public enum MouseMode {
		NONE,
		WATER_TOOL,
		MAGMA_TOOL
	}
}
