using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public struct Plan {
	Needs needToSatisfy;
	Queue<PlanElement> elements;

	public Plan(Needs n, Queue<PlanElement> elems) {
		needToSatisfy = n;
		elements = elems;
	}

	public bool HasNext() {
		return elements.Count > 0;
	}

	public PlanElement NextAction() {
		if(elements.Count == 0) return null;
		var elem = elements.Dequeue();
		elem.Start();
		return elem;
	}
}

public abstract class PlanElement {
	protected IPetActions pet;
	public PlanElement(IPetActions pet) {
		this.pet = pet;
	}
	public abstract void Start();
	public abstract bool Update();
}

public class MoveToLocation : PlanElement {
	private int targetLocation;
	private int direction; //-1 or 1

	public MoveToLocation(IPetActions pet, int target, int direction) : base(pet) {
		targetLocation = target;
		this.direction = direction;
	}

	public override void Start() {
		pet.SetAnimation(direction != 1 ? "WalkLeft" : "WalkRight");
	}

	public override bool Update() {
		if(pet.GetCurrentSection() != targetLocation) {
			if(direction == 1)
				pet.WalkLeft();
			else
				pet.WalkRight();
			return false;
		}
		return true;
	}
}

public class DoNothing : PlanElement {
	public DoNothing(IPetActions pet) : base(pet) {

	}
	public override void Start() { }

	public override bool Update() {
		return true;
	}
}

public class MoveToHotspot : PlanElement {
	int direction;
	Hotspot targetHotspot;

	public MoveToHotspot(IPetActions pet, Hotspot h, int direction) : base(pet) {
		this.direction = direction;
		targetHotspot = h;
	}

	public override void Start() { }

	public override bool Update() {
		if(direction == 1)
			return pet.WalkLeftCarefully(targetHotspot.transform.rotation.eulerAngles.z);
		else
			return pet.WalkRightCarefully(targetHotspot.transform.rotation.eulerAngles.z);
	}
}

public abstract class SatisfyNeed : PlanElement {
	protected NeedActions action;
	protected Needs needToSatisfy;
	protected string animation;

	public SatisfyNeed(IPetActions pet, NeedActions action, Needs needToSatisfy) : base(pet) {
		this.action = action;
		this.needToSatisfy = needToSatisfy;
	}

	public override void Start() {
		pet.StartSatisfyNeed(action);
		pet.SetAnimation(animation);
	}

	public override bool Update() {
		if(pet.GetNeedValue(needToSatisfy) > Pet.NeedSatisfiedValue) {
			pet.EndSatisfyNeed();
			return true;
		}
		return false;
	}
}

public class Eat : SatisfyNeed {

	public Eat(IPetActions pet, bool eatSlow) : base(pet, eatSlow ? NeedActions.EAT_SLOW : NeedActions.EAT, Needs.HUNGER) {
		animation = eatSlow ? "EatSlow" : "Eat";
	}
}

public class Drink : SatisfyNeed {
	public Drink(IPetActions pet, bool drinkSlow) : base(pet, drinkSlow ? NeedActions.DRINK_SLOW : NeedActions.DRINK, Needs.THIRST) {
		animation = "Drink";
	}
}

public class Sleep : SatisfyNeed {
	public Sleep(IPetActions pet) : base(pet, NeedActions.SLEEP, Needs.TIREDNESS) {
		animation = "StartSleep";
	}
}

public class EndSleep : PlanElement {
	private float time = 0;

	public EndSleep(IPetActions pet) : base(pet) {

	}

	public override void Start() {
		pet.SetAnimation("EndSleep");
	}

	public override bool Update() {
		time += Time.deltaTime;
		if(time < 0.66) return false;
		return true;
	}
}

public class EndEatDrink : PlanElement {
	public EndEatDrink(IPetActions pet) : base(pet) {

	}

	public override void Start() {
		pet.SetAnimation("IdleRight");
	}

	public override bool Update() {
		return true;
	}
}