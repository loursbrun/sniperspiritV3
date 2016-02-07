
//create a variable to access the C# script
private var csScript : EmissionC; 
public static var toto_js;
public static var var1_js;
	
function Awake()
{
	//Get the CSharp Script
	csScript = this.GetComponent("EmissionC");//ne pas deplacer les fichiers emissionJ et EmissionC
}
	//--------------------recuperation de la valeur contenu ds C#  -----------------
	toto_js = csScript.toto_charp;

	var1_js = csScript.var1_charp;

	//print(toto_js);