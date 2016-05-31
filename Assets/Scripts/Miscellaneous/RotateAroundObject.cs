using UnityEngine;
using System.Collections;

public class RotateAroundObject : MonoBehaviour {

  public GameObject gameObjectToRotateAround = null;
  public Vector3 rotationVector = Vector3.zero;
  public float rotateSpeed = 100;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
    this.gameObject.transform.RotateAround(gameObjectToRotateAround.transform.position, rotationVector, rotateSpeed * Time.deltaTime);
	}
}
