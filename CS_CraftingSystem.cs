using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_CraftingSystem : MonoBehaviour {

	public GameObject CraftingBench;
	public Transform spawnArea;
	public GameObject green;
	public bool boolRed;
	public bool boolBlue;
	public bool boolYellow;
	GameObject objBlue;
	GameObject objRed;
	GameObject objYellow;

	//Sets a bool to true when a compatable item is placed in the collider.
	void OnTriggerStay(Collider col)
	{
		if (col.gameObject.tag == "Blue")
		{
			Debug.Log("Blue Detected");
			boolBlue = true;
			objBlue = col.gameObject;
			CraftRecipe();
		}
		if (col.gameObject.tag == "Red")
		{
			Debug.Log("Red Detected");
			boolRed = true;
			objRed = col.gameObject;
			CraftRecipe();
		}
		if (col.gameObject.tag == "Yellow")
		{
			Debug.Log("Yellow Detected");
			boolYellow = true;
			objYellow = col.gameObject;
			CraftRecipe();
		}
	}
	//Reset the bools to false when one is removed. The remaining ones are set back to true
	//by the function above.
	void OnTriggerExit(Collider col)
	{
		boolRed = false;
		objRed = null;
		boolBlue = false;
		objBlue = null;
		boolYellow = false;
		objYellow = null;
	}

	void CraftRecipe()
	{
		if(boolBlue == true && boolYellow == true && boolRed == false)
		{
			Destroy(objBlue);
			Destroy(objYellow);
			GameObject Green = Instantiate(green, spawnArea.transform.position, spawnArea.transform.rotation);
		}
	}
}
