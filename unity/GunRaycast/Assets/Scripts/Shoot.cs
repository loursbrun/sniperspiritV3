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
		private float decalageX;
		private float decalageY;

		/*
		* Private vars Balistic
		*/

		private int[] tableDistance;				 // Table de tir distance
		private int[] tablePression;				 // Table de tir pression
		private int[] tableTemperature;				 // Table de tir temperature
		private int[] tableWind;				     // Table de tir wind
		private int[] deriveGyro;					 // Table de tir derive gyroscopique

		private int wind;   				 // m/s
		private int windDirection ; 		 // Heure 1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23
		private int pression;                // mLbar ou HPA
		private int temperature;             // °C


		private int correctionDistance ;           // correction distance
		private int correctionPression ;           // correction pression
		private int correctionTemperature ;        // correction temperature
		private int correctionWindHauteur ;        // correction wind hauteur
		private int correctionWindDirection ;      // correction wind distance
		private int correctionDeriveGyro ;         // correction derive gyroscopique


	

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
										CalculTable (100);	
								}
								if (hit.distance < 200 && hit.distance > 100) {
										CalculTable (200);	
								}
								if (hit.distance < 300 && hit.distance > 200) {
										CalculTable (300);	
								}
								//-------------------------puis affichage après Calcul--------------------------------
								Instantiate (bulletHole, hit.point, Quaternion.FromToRotation (Vector3.up, hit.normal));
						}
				}
		}

		void CalculTable (int MyFocus)
		{



				print (hit.distance);
				tableReturn ();
				print (tableDistance[0]);




				switch (MyFocus) {
				case 100:
				//----------------------------------------100 m--------------------------------------
						decalageX = hit.point.z - 0.2f;
						decalageY = hit.point.y + 0.1f;
						hit.point = new Vector3 (hit.point.x, decalageY, decalageX);
						break;
				case 200:
				//----------------------------------------200 m--------------------------------------
						decalageX = hit.point.z - 0.2f;
						decalageY = hit.point.y + 0.1f;
						hit.point = new Vector3 (hit.point.x, decalageY, decalageX);
						break;
				case 300:
						//----------------------------------------200 m--------------------------------------
						decalageX = hit.point.z + 0.2f;
						decalageY = hit.point.y - 0.1f;
						hit.point = new Vector3 (hit.point.x, decalageY, decalageX);
						break;
				}
					


				//-------------------------distribute impact + range pour affichage  (InfoBullet.cs)-----------------------------------
				transmitOrigin = ray;
				transmitDistance = hit.distance;
				transmitRayX = hit.point.x;
				transmitRayY = hit.point.y;
				transmitRayZ = hit.point.z;
		}


		void tableReturn ()
		{
				tableDistance = new int[] { 1, 3, 5, 7, 9 };   // OK
		}




}