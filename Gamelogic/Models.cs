using UnityEngine;
using System.Collections;

public class Models : MonoBehaviour {

	
	
    public void GetModel(Player Customer,string s)
	{

        var model = Customer.transform.FindChild(s + "_Model");

		//model.transform.localPosition = new Vector3(0,-5f,1.76f);
		//model.transform.localScale = new Vector3(7.63f,7.64f,7.64f);
	}
}
