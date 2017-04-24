using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pet : MonoBehaviour {

	private Dictionary<Needs, Need> coreNeeds, secondaryNeeds;
	private Plan? currentPlan;
	[SerializeField]
	[Range(0,10)]
	private float CriticalNeedValue;
	private IPlanElement currentAction;

	// Use this for initialization
	void Start() {
		//food, water, sleeps, temperature, happiness, health
		coreNeeds = new Dictionary<Needs, Need>();
		coreNeeds.Add(Needs.HUNGER, new Need(0, () => (
			currentAction.GetType() == typeof(Eat) ? 1 : 0 - 0.05f
		)));
		coreNeeds.Add(Needs.THIRST, new Need(0, () => (
			currentAction.GetType() == typeof(Drink) ? 1 : 0 - 0.05f
		)));
		coreNeeds.Add(Needs.TIREDNESS, new Need(0, () => (
			currentAction.GetType() == typeof(Sleep) ? 1 : 0 - 0.05f
		)));

		secondaryNeeds = new Dictionary<Needs, Need>();
		secondaryNeeds.Add(Needs.WARMTH, new Need(0, () => {
			return 0;
		}));
		secondaryNeeds.Add(Needs.HAPPINESS, new Need(0, () => {
			return 0;
		}));
		secondaryNeeds.Add(Needs.HEALTH, new Need(0, () => {
			return 0;
		}));
	}

	// Update is called once per frame
	void Update() {
		if(currentPlan == null) FormulatePlan();
		foreach(Need n in coreNeeds.Values) {
			n.Update();
		}
		if(currentAction.Update()) {
			if(currentPlan.Value.HasNext()) {
				currentAction = currentPlan.Value.NextAction();
				currentAction.Start();
			} else {
				currentPlan = null;
			}
		}
	}

	private void FormulatePlan() {
		KeyValuePair<Needs, Need> lowestNeed = coreNeeds
			.Aggregate((currMin, n) => ( (n.Value.value) < currMin.Value.value && BiomeAvailable(n.Key) ? n : currMin));
		Queue<IPlanElement> elements = new Queue<IPlanElement>();
		if(lowestNeed.Value.value > CriticalNeedValue) {
			//Dawdle, or possibly consider satisfying the need for temperature
		} else {
			//Get a move on
			//1) Find nearest biome that can satisfy this need
			//2) Create the move left/right Plan Element
			//3) Add the relevant SatifyNeed thingy
			//4) Construct the Plan object
		}
		currentPlan = new Plan(lowestNeed.Key, elements);
		currentAction = currentPlan.Value.NextAction();
	}

	public bool BiomeAvailable(Needs need) {
		if(need == Needs.HUNGER)
			return Planet.instance.BiomeAvailable(Biome.FOREST) || Planet.instance.BiomeAvailable(Biome.SWAMP);
		if(need == Needs.THIRST)
			return Planet.instance.BiomeAvailable(Biome.LAKE) || Planet.instance.BiomeAvailable(Biome.SWAMP);
		if(need == Needs.TIREDNESS)
			return Planet.instance.BiomeAvailable(Biome.PLAINS);
		Debug.LogError("Bad call to BiomeAvailable " + need);
		return false;
	}

	public class Need {
		public float value { get; private set; }
		private Func<float> Decay;

		public Need(float initialValue, Func<float> decayFunction) {
			value = initialValue;
			Decay = decayFunction;
		}

		public void Update() {
			value = Mathf.Clamp(value + (Decay() * Time.deltaTime), 0f, 10f);
		}
	}
}

public struct Plan {
	Needs needToSatisfy;
	Queue<IPlanElement> elements;

	public Plan(Needs n, Queue<IPlanElement> elems) {
		needToSatisfy = n;
		elements = elems;
	}

	public bool HasNext() {
		return elements.Count > 0;
	}

	public IPlanElement NextAction() {
		if(elements.Count == 0) return null;
		return elements.Dequeue();
	}
}

public interface IPlanElement {
	void Start();
	bool Update();
}

public class MoveToLocation : IPlanElement {
	public void Start() {
		throw new NotImplementedException();
	}

	public bool Update() {
		throw new NotImplementedException();
	}
}

public abstract class SatisfyNeed : IPlanElement { 
	public void Start() {
		throw new NotImplementedException();
	}

	public bool Update() {
		throw new NotImplementedException();
	}
}

public class Eat : SatisfyNeed {

}

public class Drink : SatisfyNeed {

}

public class Sleep : SatisfyNeed {

}