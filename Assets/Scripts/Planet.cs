using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Planet : MonoBehaviour {

	public static Planet instance;

	[SerializeField]
	private List<Section> sections;

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
		Year = 1;
	}
	
	// Update is called once per frame
	void Update () {
		season += seasonRate * Time.deltaTime;
		if(season > 4f) {
			season -= 4f;
			Year++;
		}
		Debug.Log(sections[0].heat);
	}

	public Section GetLeftNeighbour(Section section) {
		if(!sections.Contains(section)) Debug.LogError("Section not registered to Planet");
		int index = sections.IndexOf(section);
		if(index == 0) return sections[sections.Count - 1];
		return sections[index - 1];
	}
	
	public Section Get2LeftNeighbour(Section section) {
		return GetLeftNeighbour(GetLeftNeighbour(section));
	}

	public Section GetRightNeighbour(Section section) {
		if(!sections.Contains(section)) Debug.LogError("Section not registered to Planet");
		int index = sections.IndexOf(section);
		if(index == sections.Count - 1) return sections[0];
		return sections[index + 1];
	}

	public Section Get2RightNeighbour(Section section) {
		return GetRightNeighbour(GetRightNeighbour(section));
	}

	public int GetNeighbourCountOfType(Section section, Biome b) {
		int result = 0;
		result += GetLeftNeighbour(section).CurrentBiome == b ? 1 : 0;
		result += Get2LeftNeighbour(section).CurrentBiome == b ? 1 : 0;
		result += GetRightNeighbour(section).CurrentBiome == b ? 1 : 0;
		result += Get2RightNeighbour(section).CurrentBiome == b ? 1 : 0;
		return result;
	}

	public Section[] GetNeighbours(Section section) {
		if(!sections.Contains(section)) Debug.LogError("Section not registered to Planet");
		int index = sections.IndexOf(section);
		Section[] result = new Section[2];
		result[0] = index == 0 ? sections[sections.Count - 1] : sections[index - 1];
		result[1] = index == sections.Count - 1 ? sections[0] : sections[index + 1];
		return result;
	}

	public Biome GetBiomeForSection(int section) {
		return sections[section].CurrentBiome;
	}

	public float GetTemperatureForSection(int section) {
		return sections[section].heat;
	}

	public int GetNearestSectionOfType(int currentSection, Biome biome) {
		List<int> viableCandidates = sections.Where(x => (x.CurrentBiome == biome)).Select(x => sections.IndexOf(x)).ToList();
		int bestCandidate = -1;
		int route = int.MaxValue;
		foreach(int n in viableCandidates) {
			if(DistanceBetweenTwoSectors(currentSection, n) < route) bestCandidate = n;
		}
		return bestCandidate;
	}

	public int DistanceBetweenTwoSectors(int a, int b) {
		int dist = Mathf.Abs(a - b);
		while(dist > 4) dist -= 4;
		return dist;
	}

	public int GetTravelDirection(int from, int to) {
		if(Mathf.Abs(to - from) == DistanceBetweenTwoSectors(from, to)) {
			if(from < to)
				return 1;
			else
				return 0;
		} else {
			if(to < from)
				return 1;
			else
				return 0;
		}
	}

	public Hotspot GetHotspotForSection(int section) {
		return sections[section].GetHotspot();
	}


	public bool BiomeAvailable(Biome biome) {
		return sections.Any((s) => (s.CurrentBiome == biome));
	}

	public int GetSectionForRotation(float rotation) {
		foreach(Section s in sections) {
			float secRotation = s.transform.rotation.eulerAngles.z;
			if(secRotation < 0) secRotation += 365;
			if(secRotation - 22.5f < rotation && rotation < secRotation + 22.5f) return sections.IndexOf(s);
			if(secRotation -22.5f < 0) {
				secRotation += 360;
				if(secRotation - 22.5f < rotation) return sections.IndexOf(s);
			}
				
		}
		string str = "" + (360f - 22.5f) + "\n";
		foreach(Section s in sections) {
			float r = s.transform.rotation.eulerAngles.z;
			str += "" + (r - 22.5) + ", " + (r + 22.5) + "\n";
		}
		Debug.LogError(str);
		throw new System.Exception("Err in GetSectionForRotation: " + rotation);
	}
}
