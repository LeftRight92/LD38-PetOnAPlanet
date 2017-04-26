using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Section : MonoBehaviour {

	public const float HEAT_LEVEL = 1.0f;

	public float heat {
		get {
			if(currentBiome == Biome.MOUNTAIN) return -2;
			if(currentBiome == Biome.DESERT) return 2;
			float temp = Planet.instance.AmbientTemperature;
			temp += Planet.instance.GetNeighbourCountOfType(this, Biome.DESERT);
			temp -= Planet.instance.GetNeighbourCountOfType(this, Biome.MOUNTAIN);
			return temp;
		}
	}

	public int magma;
	public int water;

	[SerializeField]
	private Biome currentBiome;
	public Biome CurrentBiome { get { return currentBiome; } }

	[SerializeField]
	private GameObject transitionPrefab;
	[SerializeField]
	private Transform paffPoint;

	private Dictionary<Biome, List<Hotspot>> hotspots = new Dictionary<Biome, List<Hotspot>>();

	[SerializeField]
	[Header("Audio")]
	private AudioSource source;
	[SerializeField]
	private AudioClip birdSound, fliesSound, lavaSound, waterSound;

	// Use this for initialization
	void Start () {
		foreach(Hotspot h in GetComponentsInChildren<Hotspot>()) {
			if(!hotspots.ContainsKey(h.ActiveForBiome)) hotspots.Add(h.ActiveForBiome, new List<Hotspot>());
			hotspots[h.ActiveForBiome].Add(h);
		}
	}
	
	// Update is called once per frame
	void Update () {
		string heatS = currentBiome == Biome.MUD00 || currentBiome == Biome.MUD01 || currentBiome == Biome.MUD10 || currentBiome == Biome.DESERT || currentBiome == Biome.MOUNTAIN ?
				"" : GetHeatLevel();
		GetComponent<Animator>().CrossFade("Section " + currentBiome.GetAnimationString() + heatS, 0.0000001f);
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
		Biome newBiome = BiomeMap.GetBiome(water, magma);
		if(newBiome != currentBiome) {
			//GetComponent<SpriteRenderer>().sprite = Planet.biomeMap.sprites[newBiome];
			GameObject g = Instantiate(transitionPrefab, paffPoint.position, paffPoint.rotation);
			Destroy(g, 1);
			string heatS = newBiome == Biome.MUD00 || newBiome == Biome.MUD01 || newBiome == Biome.MUD10 || newBiome == Biome.DESERT || newBiome == Biome.MOUNTAIN ?
				"" : GetHeatLevel();
			GetComponent<Animator>().CrossFade("Section " + newBiome.GetAnimationString() + heatS, 0.1f);
			currentBiome = newBiome;

			if(newBiome == Biome.FOREST) {
				source.clip = birdSound;
			} else if(newBiome == Biome.SWAMP) {
				source.clip = fliesSound;
			} else if(newBiome == Biome.DESERT || newBiome == Biome.PLAINS) {
				source.clip = lavaSound;
			} else if(newBiome == Biome.LAKE) {
				source.clip = waterSound;
			} else {
				source.clip = null;
			}
			source.Play();
		}
	}

	private string GetHeatLevel() {
		if(heat > HEAT_LEVEL) return " Scorched";
		if(heat < -HEAT_LEVEL) return " Frozen";
		return "";
	}

	public Hotspot GetHotspot() {
		return hotspots[currentBiome][Mathf.FloorToInt(Random.Range(0, hotspots[currentBiome].Count))];
	}
}
