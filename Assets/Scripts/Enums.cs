using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum Biome {
	MUD00,
	MUD01,
	MUD10,
	LAKE,
	FOREST,
	DESERT,
	SWAMP,
	PLAINS,
	MOUNTAIN
}

public enum Needs {
	HUNGER,
	THIRST,
	TIREDNESS,
	HAPPINESS,
	HEALTH
}

public enum NeedActions {
	EAT,
	EAT_SLOW,
	DRINK,
	DRINK_SLOW,
	SLEEP
}

public static class EnumHelpers {
	public static string GetAnimationString(this Biome biome) {
		switch(biome) {
			case Biome.MUD00:
				return "Mud00";
			case Biome.MUD01:
				return "Mud01";
			case Biome.MUD10:
				return "Mud10";
			case Biome.LAKE:
				return "Lake";
			case Biome.FOREST:
				return "Forest";
			case Biome.SWAMP:
				return "Swamp";
			case Biome.DESERT:
				return "Desert";
			case Biome.PLAINS:
				return "Plains";
			case Biome.MOUNTAIN:
				return "Mountain";
		}
		return "";
	}
}