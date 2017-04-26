using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IPetActions {
	void WalkLeft();
	void WalkRight();
	bool WalkLeftCarefully(float target);
	bool WalkRightCarefully(float target);
	void StartSatisfyNeed(NeedActions action);
	void EndSatisfyNeed();
	void SetAnimation(string anim);

	float GetNeedValue(Needs need);
	int GetCurrentSection();
}

public class Pet : MonoBehaviour, IPetActions {

	private Dictionary<Needs, Need> coreNeeds, secondaryNeeds;
	private Plan? currentPlan;
	private PlanElement currentAction;
	private NeedActions? satisfyingNeed;
	private int targetSection = -1;
	private Biome expectedBiome;

	[SerializeField]
	private float MoveSpeed;
	[SerializeField]
	private float EatDrinkSpeed;
	[SerializeField]
	private float EatDrinkSlowSpeed;
	[SerializeField]
	private float SleepSpeed;
	[SerializeField]
	private float NeedDecaySpeed;
	[SerializeField]
	private float HealthLossSpeed;
	[SerializeField]
	private Animator animator;

	[SerializeField]
	[Range(0,10)]
	private float CriticalNeedValue;
	public const float NeedSatisfiedValue = 9.5f;
	[Range(0,10)]
	[SerializeField]
	private float SatisfyNeedBelow;
	[Range(0, 3)]
	[SerializeField]
	private float TemperatureTolerance;

	// Use this for initialization
	void Start() {
		//food, water, sleeps, temperature, happiness, health
		coreNeeds = new Dictionary<Needs, Need>();
		coreNeeds.Add(Needs.HUNGER, new Need(5, () => (
			(satisfyingNeed == NeedActions.EAT ? EatDrinkSpeed :
				satisfyingNeed == NeedActions.EAT_SLOW ? EatDrinkSlowSpeed : 0
				- NeedDecaySpeed)
		)));
		coreNeeds.Add(Needs.THIRST, new Need(5, () => (
			(satisfyingNeed == NeedActions.DRINK ? EatDrinkSpeed :
				satisfyingNeed == NeedActions.DRINK_SLOW ? EatDrinkSlowSpeed : 0
				- NeedDecaySpeed)
		)));
		coreNeeds.Add(Needs.TIREDNESS, new Need(5, () => (
			(satisfyingNeed == NeedActions.SLEEP ? SleepSpeed : 0
			- NeedDecaySpeed)
		)));

		secondaryNeeds = new Dictionary<Needs, Need>();
		secondaryNeeds.Add(Needs.HAPPINESS, new Need(3, () => {
			return 0;
		}));
		secondaryNeeds.Add(Needs.HEALTH, new Need(10, () => (
			Planet.instance.GetBiomeForSection(Planet.instance.GetSectionForRotation(transform.rotation.eulerAngles.z)) == Biome.DESERT ||
			Planet.instance.GetBiomeForSection(Planet.instance.GetSectionForRotation(transform.rotation.eulerAngles.z)) == Biome.MOUNTAIN ||
			coreNeeds[Needs.HUNGER].value < 0.5f ||
			coreNeeds[Needs.THIRST].value < 0.5f ||
			coreNeeds[Needs.TIREDNESS].value < 0.05f ?
			-HealthLossSpeed : 0
		)));

		//TESTING:
		//currentAction = new MoveToLocation(this, 5, -1);
	}

	// Update is called once per frame
	void Update() {
		if(currentPlan == null) FormulatePlan();
		foreach(Need n in coreNeeds.Values) {
			n.Update();
		}
		foreach(Need n in secondaryNeeds.Values) {
			n.Update();
		}
		if(secondaryNeeds[Needs.HEALTH].value <= 0.01) Die();
		if(currentAction.Update()) {
			if(targetSection != -1) {
				if(expectedBiome != Planet.instance.GetBiomeForSection(targetSection)) {
					currentAction = null;
					currentPlan = null;
					EndSatisfyNeed();
					FormulatePlan();
					return;
				}
			}
			if(currentPlan.Value.HasNext()) {
				currentAction = currentPlan.Value.NextAction();
			} else {
				FormulatePlan();
			}
		}
		Debug.Log(secondaryNeeds[Needs.HEALTH].value);
	}

	public void SetAnimation(string anim) {
		animator.CrossFade(anim, 0.1f);
		Debug.Log("SetAnimation " + anim);
	}

	public void WalkLeft() {
		transform.parent.Rotate(new Vector3(0, 0, -MoveSpeed * Time.deltaTime));
	}

	public void WalkRight() {
		transform.parent.Rotate(new Vector3(0, 0,  MoveSpeed * Time.deltaTime));
	}

	public bool WalkLeftCarefully(float target) {
		if(MoveSpeed * Time.deltaTime > Mathf.Abs(target - transform.parent.rotation.eulerAngles.z)) {
			transform.parent.rotation = Quaternion.Euler(0, 0, target);
			return true;
		}
		WalkLeft();
		return false;
	}

	public bool WalkRightCarefully(float target) {
		if(MoveSpeed * Time.deltaTime > Mathf.Abs(target - transform.parent.rotation.eulerAngles.z)) {
			transform.parent.rotation = Quaternion.Euler(0, 0, target);
			return true;
		}
		WalkRight();
		return false;
	}

	public int GetCurrentSection() {
		return Planet.instance.GetSectionForRotation(transform.rotation.eulerAngles.z);
	}

	public float GetNeedValue(Needs need) {
		if(need == Needs.HEALTH || need == Needs.HAPPINESS) return secondaryNeeds[need].value;
		return coreNeeds[need].value;
	}

	public void StartSatisfyNeed(NeedActions action) {
		satisfyingNeed = action;
	}

	public void EndSatisfyNeed() {
		satisfyingNeed = null;
	}

	private void Die() {
		SetAnimation("Dead");
		animator.transform.Translate(1, -1.0f, 0);
		Destroy(this);
	}

	private int GetNearestSectionForNeed(int currentSection, Needs need) {
		if(need == Needs.HUNGER) {
			int nearestSwamp = Planet.instance.GetNearestSectionOfType(currentSection, Biome.SWAMP);
			int nearestForest = Planet.instance.GetNearestSectionOfType(currentSection, Biome.FOREST);
			if(nearestSwamp == -1)
				return nearestForest;
			else {
				if(nearestForest == -1)
					return nearestSwamp;
				else
					return Planet.instance.DistanceBetweenTwoSectors(currentSection, nearestSwamp) < Planet.instance.DistanceBetweenTwoSectors(currentSection, nearestForest) ? nearestSwamp : nearestForest;
			}
		}
		if(need == Needs.THIRST) {
			int nearestSwamp = Planet.instance.GetNearestSectionOfType(currentSection, Biome.SWAMP);
			int nearestLake = Planet.instance.GetNearestSectionOfType(currentSection, Biome.LAKE);
			if(nearestSwamp == -1)
				return nearestLake;
			else {
				if(nearestLake == -1)
					return nearestSwamp;
				else
					return Planet.instance.DistanceBetweenTwoSectors(currentSection, nearestSwamp) < Planet.instance.DistanceBetweenTwoSectors(currentSection, nearestLake) ? nearestSwamp : nearestLake;
			}
		}
		if(need == Needs.TIREDNESS) {
			return Planet.instance.GetNearestSectionOfType(currentSection, Biome.PLAINS);
		}
		Debug.LogError("Bad call to GetNearestSectionForNeed " + need);
		return -1;
	}

	private void FormulatePlan() {
		targetSection = -1;
		Debug.Log("Formulate Plan");
		KeyValuePair<Needs, Need>? lowestNeed = null;
		foreach(KeyValuePair<Needs,Need> n in coreNeeds) {
			if(!BiomeAvailable(n.Key) || n.Value.value > SatisfyNeedBelow) continue;
			if(!lowestNeed.HasValue || lowestNeed.Value.Value.value < n.Value.value)
				lowestNeed = n;
		}
		if(!lowestNeed.HasValue) {
			DoSpecialPlan();
			return;
		}
		KeyValuePair<Needs, Need> needToSatisfy = lowestNeed.Value;
		Queue<PlanElement> elements = new Queue<PlanElement>();

		//Debug.Log(needToSatisfy.Key);
		if(false/*needToSatisfy.Value.value > CriticalNeedValue*/) {
			//Dawdle, or possibly consider satisfying the need for temperature
		} else {
			//Get a move on
			//1) Find nearest biome that can satisfy this need
			int currentSection = GetCurrentSection();
			int nearestSectionForNeed = GetNearestSectionForNeed(currentSection, needToSatisfy.Key);
			//2) Create the move left/right Plan Element
			int travelDirection = Planet.instance.GetTravelDirection(currentSection, nearestSectionForNeed);
			elements.Enqueue(new MoveToLocation(this, nearestSectionForNeed, travelDirection));
			elements.Enqueue(new MoveToHotspot(this, Planet.instance.GetHotspotForSection(nearestSectionForNeed), travelDirection));
			//3) Add the relevant SatifyNeed thingy
			if(needToSatisfy.Key == Needs.HUNGER) {
				elements.Enqueue(new Eat(this, Planet.instance.GetBiomeForSection(nearestSectionForNeed) == Biome.SWAMP));
				elements.Enqueue(new EndEatDrink(this));
			}else if(needToSatisfy.Key == Needs.THIRST) {
				elements.Enqueue(new Drink(this, Planet.instance.GetBiomeForSection(nearestSectionForNeed) == Biome.SWAMP));
				elements.Enqueue(new EndEatDrink(this));
			}else if(needToSatisfy.Key == Needs.TIREDNESS) {
				elements.Enqueue(new Sleep(this));
				elements.Enqueue(new EndSleep(this));
			}
			targetSection = nearestSectionForNeed;
			expectedBiome = Planet.instance.GetBiomeForSection(nearestSectionForNeed);
		}
		currentPlan = new Plan(needToSatisfy.Key, elements);
		currentAction = currentPlan.Value.NextAction();
	}

	public void DoSpecialPlan() {
		Queue<PlanElement> queue = new Queue<PlanElement>();
		queue.Enqueue(new DoNothing(this));
		currentPlan = new Plan(Needs.HAPPINESS, queue);
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