using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Planet : MonoBehaviour {

	public static Planet instance;

	[SerializeField]
	private List<Section> sections;
	public static BiomeMap biomeMap { get; private set; }

	public AnimationCurve SeasonTemperatureCurve;
	public float season { get; private set; }
	[SerializeField]
	private float seasonRate;
	public float AmbientTemperature {
		get {
			return SeasonTemperatureCurve.Evaluate(season);
		}
	}

	public int Year { get; private set; }

	// Use this for initialization
	void Start () {
		if(instance != null) Debug.LogError("Cannot have more than 1 Planet");
		instance = this;
		biomeMap = GetComponent<BiomeMap>();
		Year = 1;
	}
	
	// Update is called once per frame
	void Update () {
		season += seasonRate * Time.deltaTime;
		if(season > 4f) {
			season -= 4f;
			Year++;
		}
	}

	public Section GetLeftNeighbour(Section section) {
		if(!sections.Contains(section)) Debug.LogError("Section not registered to Planet");
		int index = sections.IndexOf(section);
		if(index == 0) return sections[sections.Count - 1];
		return sections[index - 1];
	}

	public Section GetRightNeighbour(Section section) {
		if(!sections.Contains(section)) Debug.LogError("Section not registered to Planet");
		int index = sections.IndexOf(section);
		if(index == sections.Count - 1) return sections[0];
		return sections[index + 1];
	}

	public Section[] GetNeighbours(Section section) {
		if(!sections.Contains(section)) Debug.LogError("Section not registered to Planet");
		int index = sections.IndexOf(section);
		Section[] result = new Section[2];
		result[0] = index == 0 ? sections[sections.Count - 1] : sections[index - 1];
		result[1] = index == sections.Count - 1 ? sections[0] : sections[index + 1];
		return result;
	}

	public bool BiomeAvailable(Biome biome) {
		return sections.Any((s) => (s.CurrentBiome == biome));
	}
}
