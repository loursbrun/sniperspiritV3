using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

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
		*   XML
		*/
		public TextAsset GameAsset;

		static string Cube_Character = "";
		static string Cylinder_Character = "";
		static string Capsule_Character = "";
		static string Sphere_Character = "";

		List<Dictionary<string,string>> levels = new List<Dictionary<string,string>>();
		Dictionary<string,string> obj;



		/*
		* Private vars Balistic
		*/



		private int[] tableDistance;
		// Table de tir distance
		private int[] tablePression;
		// Table de tir pression
		private int[] tableTemperature;
		// Table de tir temperature
		private int[] tableWind;
		// Table de tir wind
		private int[] deriveGyro;
		// Table de tir derive gyroscopique

		private int distanceTemp = 100;
		/// distance temporaire avant le retour recast  
		private int pressionTemp = 900;
		// mLbar ou HPA
		private int temperatureTemp = 20;
		// °C
		private int wind = 5;
		// m/s
		private int windDirection = 3;
		// Heure 1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23


		// modifucation de la hauteur du tir
		private int correctionDistance;
		// correction distance
		private int correctionPression;
		// correction pression
		private int correctionTemperature;
		// correction temperature
		private int correctionWindHauteur;
		// correction du vent en hauteur

		// modifucation de la direction du tir
		private int correctionWindDirection;
		// correction du vent en direction
		private int correctionDeriveGyro;
		// correction derive gyroscopique




		// Variable index tableau
		private int indexDistance;
		private int indexPression;
		private int indexTemperature;


		private int[] returnCorrectionArray;

	

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



		public static Vector3 LoadFromXml(string elementName,string typename,int stage)
		{
				float X = 0;
				float Y = 0;
				float Z = 0;
				string p1 = "x";
				string p2 = "y";
				string p3 = "z";
				//string filepath = Application.dataPath + "/Data/data.xml";
				string filepath = File.ReadAllText(Application.dataPath + "/Data/data.xml");
				//string filepath = Resources.Load("Data");


				XmlDocument xmlDoc = new XmlDocument(); 




						

						//xmlDoc.Load(File.ReadAllText(filepath)); 




						xmlDoc.LoadXml(File.ReadAllText(Application.dataPath + "/Data/data.xml")); // load the file.
				 


						//XmlNodeList transformList = xmlDoc.GetElementsByTagName(elementName);


				Debug.Log("Coucou" + xmlDoc.ChildNodes.Count); 
				    XmlNodeList transformList = xmlDoc.GetElementsByTagName(elementName);
				    


						foreach (XmlNode transformInfo in transformList)
						{



								XmlNodeList transformcontent = transformInfo.ChildNodes;

								foreach (XmlNode transformItens in transformcontent) 
								{
										if(transformItens.Name == typename)
										{
												XmlNodeList newtransformList = transformItens.ChildNodes;

												foreach (XmlNode newtransformItens in newtransformList)
												{
														if(newtransformItens.Name == "x")
														{
																X = float.Parse(newtransformItens.InnerText); // convert the strings to float and apply to the Y variable.
														}
														if(newtransformItens.Name == "y")
														{
																Y = float.Parse(newtransformItens.InnerText); // convert the strings to float and apply to the Y variable.
														}
														if(newtransformItens.Name == "z")
														{
																Z = float.Parse(newtransformItens.InnerText); // convert the strings to float and apply to the Y variable.
														}
												} 
										}
								}
						}





				return new Vector3(X,Y,Z);;
		}







		public void LoadTable()
		{
				XmlDocument xmlDoc = new XmlDocument(); // xmlDoc is the new xml document.
				xmlDoc.LoadXml(File.ReadAllText(Application.dataPath + "/Data/gamexmldata.xml")); // load the file.
				XmlNodeList levelsList = xmlDoc.GetElementsByTagName("level"); // array of the level nodes.


				levels.Add(obj); // add whole obj dictionary in the levels[].
			

		}






		public void GetTable()
		{
				XmlDocument xmlDoc = new XmlDocument(); // xmlDoc is the new xml document.
				xmlDoc.LoadXml(File.ReadAllText(Application.dataPath + "/Data/gamexmldata.xml")); // load the file.
				XmlNodeList levelsList = xmlDoc.GetElementsByTagName("level"); // array of the level nodes.

				foreach (XmlNode levelInfo in levelsList)
				{
						XmlNodeList levelcontent = levelInfo.ChildNodes;
						obj = new Dictionary<string,string>(); // Create a object(Dictionary) to colect the both nodes inside the level node and then put into levels[] array.


						/*
						foreach (XmlNode levelsItens in levelcontent) // levels itens nodes.
						{
								if(levelsItens.Name == "name")
								{
										obj.Add("name",levelsItens.InnerText); // put this in the dictionary.
								}

								if(levelsItens.Name == "tutorial")
								{
										obj.Add("tutorial",levelsItens.InnerText); // put this in the dictionary.
								}

								if(levelsItens.Name == "object")
								{
										switch(levelsItens.Attributes["name"].Value)
										{
										case "Cube": obj.Add("Cube",levelsItens.InnerText);break; // put this in the dictionary.
										case "Cylinder":obj.Add("Cylinder",levelsItens.InnerText); break; // put this in the dictionary.
										case "Capsule":obj.Add("Capsule",levelsItens.InnerText); break; // put this in the dictionary.
										case "Sphere": obj.Add("Sphere",levelsItens.InnerText);break; // put this in the dictionary.
										}
								}

								if(levelsItens.Name == "finaltext")
								{
										obj.Add("finaltext",levelsItens.InnerText); // put this in the dictionary.
								}
						}
						*/
						levels.Add(obj); // add whole obj dictionary in the levels[].

				}
		}






		void CalculTable (int MyFocus)
		{

				//-------------------------recuperation valeur javascript----------------
				//print (ReceptionC.toto);
				//print ("number :" + ReceptionC.toto);

				//print ("number :" + ReceptionC.var1 * 2);
				//print ("objet :" + ReceptionC.objet);


				//-----------------------------------------------------------------------


				// ------ test  XML   ---------  //

				//GetTable();
				//LoadTable();
				//print ("size of dictionary:" + levels.Count);

				//XmlDocument xmlDoc = new XmlDocument(); // xmlDoc is the new xml document.
				//xmlDoc.LoadXml(File.ReadAllText(Application.dataPath + "/Data/gamexmldata.xml")); // load the file.
				//XmlNodeList levelsList = xmlDoc.GetElementsByTagName("level"); // array of the level nodes.




				//print(LoadFromXml ("FacePosition", "Happy", 1));
				print(LoadFromXml ("Happy", "FacePosition", 1));







				// ------- END XML  ------ --//




				//print (hit.distance);
				//tableReturn ();
				//print (tableDistance[2]);



				distanceTemp = 300;      /// distance temporaire avant le retour recast  
				pressionTemp = 900;                // mLbar ou HPA
				temperatureTemp = 20;             // °C

				wind = 5;   				 // m/s
				windDirection = 3; 		// Heure 1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23




				//calculReturn (distanceTemp);
				returnCorrectionArray = calculReturn (distanceTemp, pressionTemp, temperatureTemp);



				//print ("correction distance :" + returnCorrectionArray [0] + "\n");
				//print ("correction pression :" + returnCorrectionArray [1] + "\n");
				//print ("correction temperature :" + returnCorrectionArray [2] + "\n");





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








		int[] calculReturn (int distanceValue, int pressionValue, int temperatureValue)
		{

				// Tableau distance
				int[] tableDistanceRef = new int[] { 100, 200, 300, 400, 500 };  
				int[] tableDistanceCorrection = new int[] { 6, 0, 10, 21, 34 };  


				// Tableau pression
				int[] tablePressionRef = new int[] { 990, 960, 930, 900, 870 };  
				int[] tablePressionCorrection = new int[] { 6, 0, 10, 21, 34 };  


				// Tableau temperature
				int[] tableTemperatureRef = new int[] { 0, 5, 10, 15, 20, 25, 30, 35, 40 };  
				int[] tableTemperatureCorrection = new int[] { 0, 2, 4, 6, 8, 10, 12, 14, 16 };  


			
				// FindIndex in array distance

				for (int i = 0; i < tableDistanceRef.Length; i++) {
						if (tableDistanceRef [i] == distanceValue) {
								indexDistance = i; 
						}
				}


				// FindIndex in array pression

				for (int i = 0; i < tablePressionRef.Length; i++) {
						if (tablePressionRef [i] == pressionValue) {
								indexPression = i; 
						}
				}




				// FindIndex in array temperature

				for (int i = 0; i < tableTemperatureRef.Length; i++) {
						if (tableTemperatureRef [i] == temperatureValue) {
								indexTemperature = i; 
						}
				}






				int[] correctionArray = new int[] {
						tableDistanceCorrection [indexDistance],
						tablePressionCorrection [indexPression],
						tableTemperatureCorrection [indexTemperature]
				};  
		

				return correctionArray;
		}







}