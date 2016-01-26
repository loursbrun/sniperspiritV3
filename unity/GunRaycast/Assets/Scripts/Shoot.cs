using UnityEngine;
using System.Collections;

public class Shoot : MonoBehaviour
{

		/*
		* Public
		*/

		public GameObject bullet;
		public GameObject bulletHole;
		public float delayTime = 0.5f;

		public static Ray transmitOrigin;
		public static float transmitDistance;
		public static float transmitRayX;
		public static float transmitRayY;
		public static float transmitRayZ;

		/*
		* Private
		*/

		private float counter = 0;

		/*
		* Constructor
		*/


	
		

		void Update ()
		{
				if (Input.GetKey (KeyCode.Mouse0) && counter > delayTime) {
						Instantiate (bullet, transform.position, transform.rotation);
						GetComponent<AudioSource> ().Play ();
						counter = 0;

						//------------------------Range Hit Raycast-----------------------------------
						RaycastHit hit;
						Ray ray = new Ray (transform.position, transform.forward);
						if (Physics.Raycast (ray, out hit, 3000f)) {
								Instantiate (bulletHole, hit.point, Quaternion.FromToRotation (Vector3.up, hit.normal));
								//-------------------------distribute impact + range (InfoBullet.cs)--------------------------------------------
								transmitOrigin = ray;
								transmitDistance = hit.distance;
								transmitRayX = hit.point.x;
								transmitRayY = hit.point.y;
								transmitRayZ = hit.point.z;
						}
				}
				counter += Time.deltaTime;
		}
}
