using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length, startPosX, startPosY;
    public Collider2D groupBounds;
    public GameObject cam;
    public float parallaxSpeedX, parallaxSpeedY;



    void Start()
    {
        groupBounds = GetComponent<Collider2D>();
        startPosX = transform.position.x;
        startPosY = transform.position.y;
        length = groupBounds.bounds.size.x;
    }


    void Update()
    {

        float relativeDistance = cam.transform.position.x * parallaxSpeedX;
        float relativeDistanceY = cam.transform.position.y * parallaxSpeedY;
        transform.position = new Vector3(startPosX + relativeDistance, startPosY + relativeDistanceY, transform.position.z);





    }
}
