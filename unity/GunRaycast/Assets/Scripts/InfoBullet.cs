using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InfoBullet : MonoBehaviour
{

		/*
		* Public
		*/
		public Text Origin;
		public Text Range;
		public Text info_X;
		public Text info_Y;
		public Text info_Z;

		public Ray Origin_start;
		public float range_hit = 0f;
		public float Bullet_X = 0f;
		public float Bullet_Y = 0f;
		public float Bullet_Z = 0f;


		/*
		* Private
		*/

		/*
		* Constructor
		*/

		void Start ()
		{
				
		}

		void Update ()
		{
				if (Input.GetKey (KeyCode.Mouse0)) {
						//-----------debug------------------------------------------------------------------------------
						Origin_start = Shoot.transmitOrigin;
						Origin.text = Origin_start.ToString ();
						//print (Shoot.transmitDistance);
						//print (" x=" + Shoot.transmitRayX);
						//print (Shoot.transmitRayY);
						//print (Shoot.transmitRayZ);
						//------------Bullet GUI -----------------------------------------------------------------------
						range_hit = Shoot.transmitDistance;
						Range.text = range_hit.ToString ();


						//Bullet_X = Shoot.transmitRayX;
						//info_X.text = Bullet_X.ToString ();

						//Bullet_X = Shoot.valTourelleX;
						//info_X.text = Bullet_X.ToString ();

						Shoot.valTourelleX = float.Parse (info_X.text);
						Shoot.valTourelleY = float.Parse (info_Y.text);

						//Bullet_Y = Shoot.transmitRayY;
						//info_Y.text = Bullet_Y.ToString ();



						Bullet_X = Shoot.transmitRayX;
						Bullet_Y = Shoot.transmitRayY;
						info_Z.text = "Y:"  + Bullet_Y.ToString () + " X:"  + Bullet_X.ToString () ;
				}
	
		}
}
