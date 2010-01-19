using UnityEngine;
using System;
using System.Runtime.InteropServices;

//using Supershape;

public class Bevy : MonoBehaviour {
	// Unity component interface params

	//private float min_phi = (float) - Math.PI;
	//private float max_phi = (float) Math.PI;
	//private float min_theta = (float) - Math.PI/2;
	//private float max_theta = (float) Math.PI/2;
	//public bool Enable_Spiral = false;
	
	//public Transform obj;
	
	public int xSteps = 128;
	public int ySteps = 128;
	public int count = 100;
		
    void Start () {
		// Create supershapes
		
		
		
		for(int s = 0; s < count; s++)
		{

            GameObject newGameObject = new GameObject();
            newGameObject.AddComponent<Supershape>();
            
            
            //Instantiate(obj, new Vector3(0, 0, 0), Quaternion.identity);
			//Supershape shape = gameObject.AddComponent<Supershape>();
			/*Supershape shape =*/ //gameObject.AddComponent(shape);
			//gameObject.AddComponent<MeshRenderer>();
		}
		
        //    Debug.Log("Game object has no renderer or gui texture to assign the generated texture to!");
    }
    
    void OnDisable() {

    }
    
    // Now we can simply call UpdateTexture which gets routed directly into the plugin
    void Update () {

    }
}