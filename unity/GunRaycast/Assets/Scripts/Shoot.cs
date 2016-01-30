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
		private bool initTable = false;
		private RaycastHit hit;
		private Ray ray;
		private float decalageZ;

		/*
		* Constructor
		*/


	
		

		void Update ()
		{
				if (Input.GetKey (KeyCode.Mouse0)) {
						ShootNow ();
				}
				counter += Time.deltaTime;
		}

		void ShootNow ()
		{
				if (counter > delayTime) {
						Instantiate (bullet, transform.position, transform.rotation);
						GetComponent<AudioSource> ().Play ();
						counter = 0;

						//------------------------Range Hit Raycast-----------------------------------

						ray = new Ray (transform.position, transform.forward);
						if (Physics.Raycast (ray, out hit, 3000f)) {
								//------------------------------------------Appel function CalculTable-----------------------
								if (hit.distance < 100) {
										CalculTable (1);	
								}
								if (hit.distance < 200 && hit.distance > 100) {
										CalculTable (2);	
								}
								//-------------------------puis affichage après Calcul--------------------------------
								Instantiate (bulletHole, hit.point, Quaternion.FromToRotation (Vector3.up, hit.normal));
						}
				}
		}

		void CalculTable (int MyFocus)
		{
				switch (MyFocus) {
				case 1:
				//----------------------------------------100 m--------------------------------------
						decalageZ = hit.point.z + 0.1f;
						hit.point = new Vector3 (hit.point.x, hit.point.y, decalageZ);
						break;
				case 2:
				//----------------------------------------200 m--------------------------------------
						decalageZ = hit.point.z + 0.2f;
						hit.point = new Vector3 (hit.point.x, hit.point.y, decalageZ);
						break;
				}






				//-------------------------distribute impact + range pour affichage  (InfoBullet.cs)-----------------------------------
				transmitOrigin = ray;
				transmitDistance = hit.distance;
				transmitRayX = hit.point.x;
				transmitRayY = hit.point.y;
				transmitRayZ = hit.point.z;
		}
}