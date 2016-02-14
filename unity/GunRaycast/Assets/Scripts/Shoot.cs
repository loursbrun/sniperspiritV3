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
		private RaycastHit horCible;
		private Ray ray;
		private float decalageX;
		private float decalageY;
		private bool confirmRay = false;



		/*
		*   XML
		*/


		static float distanceSuperieur;
		static float positionNoeudCorrection;
		static float positionNoeudCorrectionTemp;
		static float forceVentFace;
		static float forceVentDirection;



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



		static float correctionTotaleHauteur;
		static float correctionTotaleDirection;


		// modifucation de la hauteur du tir
		static float correctionDistance;
		static float correctionDistanceInferieur;
		// correction distance
		static float correctionPression;
		static float correctionPressionInferieur;
		// correction pression
		static float correctionTemperature;
		static float correctionTemperatureInferieur;
		// correction temperature
		static float correctionVentY;
		static float correctionVentYInferieur;
		// correction du vent en hauteur

		// modifucation de la direction du tir
		static float correctionVentX;
		static float correctionVentXInferieur;
		// correction du vent en direction
		static float correctionDeriveGyro;
		static float correctionDeriveGyroInferieur;
		// correction derive gyroscopique
		static float tempsDeVol;
		static float tempsDeVolInferieur;
		// temps de vol




		// Variable index tableau
		private int indexDistance;
		private int indexPression;
		private int indexTemperature;

		static float distanceInferieurTrouve;
		static float distanceSuperieurTrouve;
		static float coeficientExtrapolation;



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
								//------------------------------------------Appel function CalculTable------------------
								if (hit.distance < 100) {
										CalculTable (100);
								}
								if (hit.distance < 200 && hit.distance > 100) {
										CalculTable (200);	
								}
								if (hit.distance < 300 && hit.distance > 200) {
										CalculTable (300);	
								}
								//----------------------------verif si hors cible---------------------------------------
								ConfirmRay ();
								if (confirmRay == false) {
										Instantiate (bulletHole, hit.point, Quaternion.FromToRotation (Vector3.up, hit.normal));
								}
								if (confirmRay == true) {
										Instantiate (bulletHole, horCible.point, Quaternion.FromToRotation (Vector3.up, hit.normal));
										confirmRay = false;
								}
						}
				}
		}

		void ConfirmRay ()
		{
				RaycastHit hitVerif = new RaycastHit ();
				Ray rayVerif = new Ray (transform.position, transform.forward);
				Physics.Raycast (rayVerif, out hitVerif, 3000f);
				RaycastHit hitConfirm = new RaycastHit ();
				Ray rayConfirm = new Ray (new Vector3 (hit.point.x, hit.point.y, hit.point.z), transform.forward);	
				Physics.Raycast (rayConfirm, out hitConfirm, 3000f);

				//print (("pointX = ") + hit.distance);
				//print ("VerifX = " + (int)hitVerif.point.x);
				//print ("ConfirmX = " + (int)hitConfirm.point.x);

				if ((int)hitVerif.point.x < (int)hitConfirm.point.x) { 
						confirmRay = true;
						horCible = hitConfirm;
				}
		}
	
	


		//    --------------     Function Tables   XML ---------------   //
		public static Vector3 calculatorFromXml (int distance, int temperature, int pression, int vent, int directionVent)
		{

			
				// Calcul Tigonometric du Vent
				// Info direction du vent 
				// 12h00 => Cos(0) = 1
				// 1h00 => Cos(30) = 0.866025
				// 2h00 => Cos(60) = 0.5
				// 3h00 => Cos(90) = 0
				// 4h00 => Cos(120) = -0.5
				// 5h00 => Cos(150) = -0.866025
				// 6h00 => Cos(180) = 1
				// 7h00 => Cos(210) = 1
				// 8h00 => Cos(240) = 1
				// 9h00 => Cos(270) = 1
				// 10h00 => Cos(300) = 1
				// 11h00 => Cos(330) = 1

				//print ("force du vent :<color=red>" + vent + "</color>direction du vent en degres :<color=red>" + directionVent + "</color>");
				forceVentFace = Mathf.Round(Mathf.Cos (Mathf.PI / 180 * directionVent )* vent);
				//print ("force vent Face = " + forceVentFace);
				forceVentDirection = Mathf.Round(Mathf.Sin (Mathf.PI / 180 * directionVent )* vent);
				//print ("force vent Direction = " + forceVentDirection);




				//string filepath = Application.dataPath + "/Data/data.xml";
				string filepath = File.ReadAllText (Application.dataPath + "/Data/tables.xml");
				XmlDocument xmlDoc = new XmlDocument (); 
				xmlDoc.LoadXml (File.ReadAllText (Application.dataPath + "/Data/tables.xml")); // load the file.
		
	
				// Recherches Distances
				positionNoeudCorrection = 0;  // detrmine la position du noeud correction
				XmlNodeList tablesList = xmlDoc.GetElementsByTagName ("tables");
				foreach (XmlNode tablesDistance in tablesList) {
						XmlNodeList tablescontent = tablesDistance.ChildNodes;
						foreach (XmlNode tablesItens in tablescontent) {

								if (tablesItens.Name == "distance") {

										XmlNodeList newtablesList = tablesItens.ChildNodes;

										distanceSuperieur = float.Parse (tablesItens.InnerText);
										// si la distance est trouvée le foreach break
										//Debug.Log("Distance" + distanceSuperieur);
										// on incremente la positionNoeudCorrection
										positionNoeudCorrection++;

										// Extrapolation distances


										if (distanceSuperieur <= distance) {
												distanceInferieurTrouve = distanceSuperieur;
										}

										if (distanceSuperieur >= distance) {
												distanceSuperieurTrouve = distanceSuperieur;
												break;
										}
												
										if (distance == distanceSuperieur) {
												print ("Distance Inferieur Trouvé : " + distanceSuperieur + "m !!!!!!!!!!!!!! position du noeud xml=>" + positionNoeudCorrection);
												// Break !!!!!!!!
												break;
										}
									
								}
						}
				}


				// Coeficient Extrapolation deistance
				if (distance != distanceSuperieurTrouve && distance != distanceInferieurTrouve) {
						coeficientExtrapolation = 1f - (distanceSuperieurTrouve - distance) / (distanceSuperieurTrouve - distanceInferieurTrouve);
				} else {
						coeficientExtrapolation = 1f;
				}

				print ("Distance Inferieur:" + distanceInferieurTrouve);
				print ("Distance rellele :" + distance);
				print ("Distance Superieur:" + distanceSuperieurTrouve);
				print ("Coeficient Extrapolation:" + coeficientExtrapolation);





				// Fin recherche Distances




				// Recherches Corrections Distance
			
				XmlNodeList correctionList = xmlDoc.GetElementsByTagName ("corrections");



				// Save corrections distances inferieurs : 









				foreach (XmlNode tablesDistance in correctionList) {
						positionNoeudCorrectionTemp++;
						XmlNodeList tablescontent = tablesDistance.ChildNodes;
						foreach (XmlNode tablesItens in tablescontent) {
								XmlNodeList newcorrectionList = tablesItens.ChildNodes;
						

								if (tablesItens.Name == "hauteur_correction") {
										correctionDistanceInferieur = correctionDistance;
										correctionDistance = float.Parse (tablesItens.InnerText);
								}   
								if (tablesItens.Name == "VENT_X") {
										correctionVentXInferieur = correctionVentX;
										correctionVentX = float.Parse (tablesItens.InnerText);
										correctionVentX = Mathf.Round(correctionVentX * forceVentDirection / 10);
								}   
								if (tablesItens.Name == "VENT_Y") {
										correctionVentYInferieur = correctionVentY;
										correctionVentY = float.Parse (tablesItens.InnerText);
										correctionVentY = Mathf.Round(correctionVentY * forceVentFace / 10);
								}   
								if (tablesItens.Name == "t_vol") {
										tempsDeVolInferieur = tempsDeVol;
										tempsDeVol = float.Parse (tablesItens.InnerText);
								}   
								if (tablesItens.Name == "dervive_gyro") {
										correctionDeriveGyroInferieur = correctionDeriveGyro;
										correctionDeriveGyro = float.Parse (tablesItens.InnerText);
								}   
								if (tablesItens.Name == "temperature-10" && temperature == -10f) {
										correctionTemperatureInferieur = correctionTemperature;
										correctionTemperature = float.Parse (tablesItens.InnerText);
								} 
								if (tablesItens.Name == "temperature-5" && temperature == -5f) {
										correctionTemperatureInferieur = correctionTemperature;
										correctionTemperature = float.Parse (tablesItens.InnerText);
								} 
								if (tablesItens.Name == "temperature0" && temperature == 0f) {
										correctionTemperatureInferieur = correctionTemperature;
										correctionTemperature = float.Parse (tablesItens.InnerText);
								} 
								if (tablesItens.Name == "temperature5" && temperature == 5f) {
										correctionTemperatureInferieur = correctionTemperature;
										correctionTemperature = float.Parse (tablesItens.InnerText);
								} 
								if (tablesItens.Name == "temperature10" && temperature == 10f) {
										correctionTemperatureInferieur = correctionTemperature;
										correctionTemperature = float.Parse (tablesItens.InnerText);
								} 
								if (tablesItens.Name == "temperature15" && temperature == 15f) {
										correctionTemperatureInferieur = correctionTemperature;
										correctionTemperature = float.Parse (tablesItens.InnerText);
								} 
								if (tablesItens.Name == "temperature5" && temperature == 5f) {
										correctionTemperatureInferieur = correctionTemperature;
										correctionTemperature = float.Parse (tablesItens.InnerText);
								} 
								if (tablesItens.Name == "temperature25" && temperature == 25f) {
										correctionTemperatureInferieur = correctionTemperature;
										correctionTemperature = float.Parse (tablesItens.InnerText);
								} 
								if (tablesItens.Name == "temperature30" && temperature == 30f) {
										correctionTemperatureInferieur = correctionTemperature;
										correctionTemperature = float.Parse (tablesItens.InnerText);
								} 
								if (tablesItens.Name == "temperature35" && temperature == 35f) {
										correctionTemperatureInferieur = correctionTemperature;
										correctionTemperature = float.Parse (tablesItens.InnerText);
								} 
								if (tablesItens.Name == "temperature40" && temperature == 40f) {
										correctionTemperatureInferieur = correctionTemperature;
										correctionTemperature = float.Parse (tablesItens.InnerText);
								} 
								if (tablesItens.Name == "temperature45" && temperature == 45f) {
										correctionTemperatureInferieur = correctionTemperature;
										correctionTemperature = float.Parse (tablesItens.InnerText);
								} 
								if (tablesItens.Name == "temperature50" && temperature == 50f) {
										correctionTemperatureInferieur = correctionTemperature;
										correctionTemperature = float.Parse (tablesItens.InnerText);
								} 
								if (tablesItens.Name == "pression700" && pression == 700f) {
										correctionPressionInferieur = correctionPression;
										correctionPression = float.Parse (tablesItens.InnerText);
								} 
								if (tablesItens.Name == "pression800" && pression == 800f) {
										correctionPressionInferieur = correctionPression;
										correctionPression = float.Parse (tablesItens.InnerText);
								} 
								if (tablesItens.Name == "pression900" && pression == 900f) {
										correctionPressionInferieur = correctionPression;
										correctionPression = float.Parse (tablesItens.InnerText);
								} 
								if (tablesItens.Name == "pression1000" && pression == 1000f) {
										correctionPressionInferieur = correctionPression;
										correctionPression = float.Parse (tablesItens.InnerText);
								} 

			
						}



						if (positionNoeudCorrectionTemp == positionNoeudCorrection) {
								print ("correction DistanceInferieur : " + correctionDistanceInferieur);
								print ("Correction Distance : " + correctionDistance);
								print ("Correction Vent Inferieur Y : " + correctionVentYInferieur);
								print ("Correction Vent Y : " + correctionVentY);
								print ("Correction Temperature Inferieur : " + correctionTemperatureInferieur);
								print ("Correction Temperature : " + correctionTemperature);
								print ("Correction Pression Inferieur: " + correctionPressionInferieur);
								print ("Correction Pression : " + correctionPression);



								// Correction après extrapolation linéraire coéficient Distance 

								correctionDistance = correctionDistanceInferieur + ((correctionDistance - correctionDistanceInferieur)) * coeficientExtrapolation  ;
								print ("Correction Distance Extrapolée : " + correctionDistance);

								correctionVentY = correctionVentYInferieur + ((correctionVentY - correctionVentYInferieur)) * coeficientExtrapolation  ;
								print ("Correction correctionVentY Extrapolée : " + correctionVentY);

								correctionTemperature = correctionTemperatureInferieur + ((correctionTemperature - correctionTemperatureInferieur)) * coeficientExtrapolation  ;
								print ("Correction Temperature Extrapolée : " + correctionTemperature);

								correctionPression = correctionPressionInferieur + ((correctionPression - correctionPressionInferieur)) * coeficientExtrapolation  ;
								print ("Correction Pression Extrapolée : " + correctionPression);



								// Total Correction Hauteur
								correctionTotaleHauteur = correctionDistance + correctionVentY + correctionTemperature + correctionPression;
							//	print ("<color=red>Total Correction Hauteur : " + correctionTotaleHauteur + "</color>");

								print ("Correction Derive gyro Inferieur : " + correctionDeriveGyroInferieur);
								print ("Correction Derive gyro : " + correctionDeriveGyro);

								print ("Correction Vent X Inferieur : " + correctionVentXInferieur);
								print ("Correction Vent X : " + correctionVentX);


								correctionDeriveGyro = correctionDeriveGyroInferieur + ((correctionDeriveGyro - correctionDeriveGyroInferieur)) * coeficientExtrapolation  ;
								print ("Correction Derive Gyro Extrapolée : " + correctionDeriveGyro);

								correctionVentX = correctionVentXInferieur + ((correctionVentX - correctionVentXInferieur)) * coeficientExtrapolation  ;
								print ("Correction Vent X Extrapolée : " + correctionVentX);


								// Total Correction Hauteur
								correctionTotaleDirection = correctionVentX	+ correctionDeriveGyro;	
						//		print ("<color=red>Total Correction Direction : " + correctionTotaleDirection + "</color>");

						//		print ("Temps de vol : " + tempsDeVol);
								positionNoeudCorrectionTemp = 0;
								break;
						}
					
				}

				// Fin recherche correction Distances






				return new Vector3 (correctionTotaleHauteur, correctionTotaleDirection, tempsDeVol);
		}


		// link tuto http://answers.unity3d.com/questions/255756/retrive-value-from-xml.html



		//    --------------    END Function Tables ---------------   //

		void CalculTable (int MyFocus)
		{
				// Function Tables XML
				//print (calculatorFromXml (200, -5, 900, 10, 90)); // distance;temperature;pression;vent;direction du vent Degres
				print(calculatorFromXml (100, 10, 1000, 10, 90)); // distance;temperature;pression;vent;direction du vent Degres

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
				//print(LoadFromXml ("Happy", "FacePosition", 1));

						


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
				//returnCorrectionArray = calculReturn (distanceTemp, pressionTemp, temperatureTemp);





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