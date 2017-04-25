using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class BiomeMap {

	public static Biome GetBiome(int waterLevel, int magmaLevel) {
		switch(waterLevel) {
			case 0:
				switch(magmaLevel) {
					case 0:
						return Biome.MUD00;
					case 1:
						return Biome.MUD01;
					case 2:
						return Biome.DESERT;
					default:
						Debug.LogError("Invalid magma value");
						return Biome.MUD00;
				}
			case 1:
				switch(magmaLevel) {
					case 0:
						return Biome.MUD10;
					case 1:
						return Biome.FOREST;
					case 2:
						return Biome.PLAINS;
					default:
						Debug.LogError("Invalid magma value");
						return Biome.MUD00;
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
						return Biome.MUD00;
				}
			default:
				Debug.LogError("Invalid water value");
				return Biome.MUD00;
		}
	}
}
