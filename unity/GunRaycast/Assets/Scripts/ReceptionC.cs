using UnityEngine;
using System.Collections;

public  class ReceptionC : MonoBehaviour
{

		public  static string toto;
		public  static int var1;
		public  static string objet;
		//---------------------creation de la variable pour javascript ------------------------------
		public  EmissionJ jsScript;

		void Awake ()
		{
				//------------recuperation de la valeur contenu ds javascript------------------------
				jsScript = this.GetComponent<EmissionJ> ();//ne pas deplacer les fichiers emissionJ et EmissionC

				toto = jsScript.toto_script;
				var1 = jsScript.var1_script;
				objet = jsScript.objet_script;



		}
}
