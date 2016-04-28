using UnityEngine;
using System.Collections;

public class TerrainGen : MonoBehaviour {
	public GameObject asteroid;
	float xPos = 0;
	float yPos = 0;
	float zPos = 0;
	public float xBound = 100;
	public float yBound = 100;
	public float zBound = 100;
	public float amountOfAsteroids = 100;
	bool hasGenerated = false;
	int i = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(!hasGenerated)
		{
			for(i = 0; i < amountOfAsteroids; i++)
			{
				xPos = Random.Range(0,xBound);
				yPos = Random.Range(0,yBound);
				zPos = Random.Range(0,zBound);
				Instantiate(asteroid,new Vector3(xPos,yPos,zPos),Random.rotation);
			}
		}
		hasGenerated = true;
	}
}
