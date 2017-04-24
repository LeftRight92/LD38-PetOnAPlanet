using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Section : MonoBehaviour {

	public float heat {
		get {
			return 0;
		}
	}
	public float humidity {
		get {
			return 0;
		}
	}

	public int magma;
	public int water;

	[SerializeField]
	private Biome currentBiome;
	public Biome CurrentBiome { get { return currentBiome; } }



	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void AddMagma() {
		magma = Mathf.Min(magma + 1, 2);
		UpdateBiome();
	}

	public void SubMagma() {
		magma = Mathf.Max(magma - 1, 0);
		UpdateBiome();
	}

	public void AddWater() {
		water = Mathf.Min(water + 1, 2);
		UpdateBiome();
	}

	public void SubWater() {
		water = Mathf.Max(water - 1, 0);
		UpdateBiome();
	}


	private void UpdateBiome() {
		Biome newBiome = Planet.biomeMap.GetBiome(water, magma);
		if(newBiome != currentBiome) {
			GetComponent<SpriteRenderer>().sprite = Planet.biomeMap.sprites[newBiome];
			currentBiome = newBiome;
		}
	}
}
