using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NoiseGen : MonoBehaviour {
	public float radius = 20;
	public int size = 10;
	public float scale = 6.5f;
	public float power = 3;
	private Vector2 v2SampleStart = new Vector2(0, 0);

	void Start(){

		Mesh mesh = transform.GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;

		for(int i = 0; i < vertices.Length; i++){
			vertices[i] = vertices[i].normalized * radius;
		}

		mesh.vertices = vertices;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();

		SetHeights();
	}

	void Update(){
		if(Input.GetKeyUp(KeyCode.Space)){
			v2SampleStart = new Vector2(Random.Range(0, 100), Random.Range(0, 100));
			SetHeights();
		}
	}

	void SetHeights(){
		Mesh mesh = transform.GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;
		for (int i = 0; i < vertices.Length; i++) {    
			float xCoord = v2SampleStart.x + vertices[i].x  * scale * Random.Range(0f,0.1f);
			float yCoord = v2SampleStart.y + vertices[i].z  * scale * Random.Range(0f,0.1f);
			vertices[i] = vertices[i] * Mathf.PerlinNoise (xCoord, yCoord); 
		}
		mesh.vertices = vertices;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();

	}
}
