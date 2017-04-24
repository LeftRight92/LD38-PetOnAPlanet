using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BiomeMap : MonoBehaviour {
	[Header("Biome Sprites")]
	[SerializeField] private Sprite MudSprite;
	[SerializeField] private Sprite LakeSprite;
	[SerializeField] private Sprite ForestSprite;
	[SerializeField] private Sprite DesertSprite;
	[SerializeField] private Sprite PlainsSprite;
	[SerializeField] private Sprite SwampSprite;
	[SerializeField] private Sprite MountainSprite;

	public Dictionary<Biome, Sprite> sprites;

	void Start() {
		sprites = new Dictionary<Biome, Sprite>();
		sprites.Add(Biome.MUD,		MudSprite);
		sprites.Add(Biome.LAKE,		LakeSprite);
		sprites.Add(Biome.FOREST,	ForestSprite);
		sprites.Add(Biome.DESERT,	DesertSprite);
		sprites.Add(Biome.PLAINS,	PlainsSprite);
		sprites.Add(Biome.SWAMP,	SwampSprite);
		sprites.Add(Biome.MOUNTAIN,	MountainSprite);
	}

	public Biome GetBiome(int waterLevel, int magmaLevel) {
		switch(waterLevel) {
			case 0:
				switch(magmaLevel) {
					case 0:
						return Biome.MUD;
					case 1:
						return Biome.MUD;
					case 2:
						return Biome.DESERT;
					default:
						Debug.LogError("Invalid magma value");
						return Biome.MUD;
				}
			case 1:
				switch(magmaLevel) {
					case 0:
						return Biome.MUD;
					case 1:
						return Biome.FOREST;
					case 2:
						return Biome.PLAINS;
					default:
						Debug.LogError("Invalid magma value");
						return Biome.MUD;
				}
			case 2:
				switch(magmaLevel) {
					case 0:
						return Biome.LAKE;
					case 1:
						return Biome.SWAMP;
					case 2:
						return Biome.MOUNTAIN;
					default:
						Debug.LogError("Invalid magma value");
						return Biome.MUD;
				}
			default:
				Debug.LogError("Invalid water value");
				return Biome.MUD;
		}
	}
}
