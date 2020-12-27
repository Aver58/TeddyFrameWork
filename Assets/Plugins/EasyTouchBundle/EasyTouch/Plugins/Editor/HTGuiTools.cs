using UnityEngine;
using System.Collections;
using UnityEditor;

public class HTGuiTools{

	private static Texture2D gradientTexture;

	#region Widget
	public static bool BeginFoldOut(string text,bool foldOut, bool endSpace=true){
	
		text = "<b><size=11>" + text + "</size></b>";
		if (foldOut){
			text = "\u25BC " + text;
		}
		else{
			text = "\u25BA " + text;
		}

		if ( !GUILayout.Toggle(true,text,"dragtab")){
			foldOut=!foldOut;
		}

		if (!foldOut && endSpace)GUILayout.Space(5f);

		return foldOut;
	}

	public static void BeginGroup(int padding=0){


		GUILayout.BeginHorizontal();
		GUILayout.Space(padding);
		EditorGUILayout.BeginHorizontal("As TextArea", GUILayout.MinHeight(10f));
		GUILayout.BeginVertical();
		GUILayout.Space(2f);


	}

	public static void EndGroup(bool endSpace = true){
		GUILayout.Space(3f);
		GUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();
		GUILayout.Space(3f);
		GUILayout.EndHorizontal();

		if (endSpace){
			GUILayout.Space(10f);
		}
	}


	public static bool Toggle(string text, bool value,bool leftToggle=false){

		if (value) GUI.backgroundColor = Color.green; else GUI.backgroundColor = Color.red;
			
		if (leftToggle){
			value = EditorGUILayout.ToggleLeft(text,value); 
		}
		else{
			value = EditorGUILayout.Toggle(text,value); 
		}

		GUI.backgroundColor = Color.white;

		return value;

	}

	public static bool Toggle (string text, bool value,int width,bool leftToggle=false){

		if (value) GUI.backgroundColor = Color.green; else GUI.backgroundColor = Color.red;
		
		if (leftToggle){
			value = EditorGUILayout.ToggleLeft(text,value,GUILayout.Width( width)); 
		}
		else{
			value = EditorGUILayout.Toggle(text,value,GUILayout.Width( width)); 
		}
		
		GUI.backgroundColor = Color.white;
		
		return value;
	}

	static public bool Button(string label,Color color,int width, bool leftAligment=false, int height=0){

		GUI.backgroundColor  = color;
		GUIStyle buttonStyle = new GUIStyle("Button");
		
		if (leftAligment)
			buttonStyle.alignment = TextAnchor.MiddleLeft;
		
		if (height==0){
			if (GUILayout.Button( label,buttonStyle,GUILayout.Width(width))){
				GUI.backgroundColor = Color.white;
				return true;	
			}
		}
		else{
			if (GUILayout.Button( label,buttonStyle,GUILayout.Width(width),GUILayout.Height(height))){
				GUI.backgroundColor = Color.white;
				return true;	
			}			
		}
		GUI.backgroundColor = Color.white;		

		return false;
	}
	#endregion

	#region 2d effect
	public static void DrawSeparatorLine(int padding=0){
		
		EditorGUILayout.Space();
		DrawLine(Color.gray, padding);
		EditorGUILayout.Space();
	}
	
	private static void DrawLine(Color color,int padding=0){
		
		GUILayout.Space(10);
		Rect lastRect = GUILayoutUtility.GetLastRect();
		
		GUI.color = color;
		GUI.DrawTexture(new Rect(padding, lastRect.yMax -lastRect.height/2f, Screen.width, 1f), EditorGUIUtility.whiteTexture);
		GUI.color = Color.white;
	}
	#endregion

	#region Texture
	private static Rect DrawGradient(int padding, int width, int height=35){

		GUILayout.Space(height);
		Rect lastRect = GUILayoutUtility.GetLastRect();
		lastRect.yMin = lastRect.yMin + 7;
		lastRect.yMax = lastRect.yMax - 7;
		lastRect.width =  Screen.width;

		GUI.DrawTexture(new Rect(padding,lastRect.yMin+1,width, lastRect.yMax- lastRect.yMin), GetGradientTexture());
		
		return lastRect;
	}
	
	private static Texture2D GetGradientTexture(){
		
		if (gradientTexture==null){
			gradientTexture = CreateGradientTexture();
		}
		return gradientTexture;
		
	}
	
	private static Texture2D CreateGradientTexture(){
	
		int height =18;
		
		Texture2D myTexture = new Texture2D(1, height);
		myTexture.hideFlags = HideFlags.HideInInspector;
		myTexture.hideFlags = HideFlags.DontSave;
		myTexture.filterMode = FilterMode.Bilinear;
		
		Color startColor= new Color(0.4f,0.4f,0.4f);
		Color endColor= new Color(0.6f,0.6f,0.6f);
		
		float stepR = (endColor.r - startColor.r)/18f;
		float stepG = (endColor.g - startColor.g)/18f;
		float stepB = (endColor.b - startColor.b)/18f;
		
		Color pixColor = startColor;
		
		for (int i = 1; i < height-1; i++)
		{
			pixColor = new Color(pixColor.r + stepR,pixColor.g + stepG , pixColor.b + stepB);
			myTexture.SetPixel(0, i, pixColor);
		}
		
		myTexture.SetPixel(0, 0, new Color(0,0,0));
		myTexture.SetPixel(0, 17, new Color(1,1,1));
		
		myTexture.Apply();
		
		return myTexture;
	}
	#endregion
}

/*
public static Texture2D gradientTextureStyle;
public static Texture2D checkerTexture;

// Title


// FoldOut
public static bool FoldOut( ref bool foldOut,string text, bool addButton=false, string buttonText="+"){
	
	GUIStyle foldOutStyle =  new GUIStyle( EditorStyles.foldout);
	foldOutStyle.fontStyle = FontStyle.Bold;
	foldOutStyle.fontSize = 12;
	foldOutStyle.stretchWidth = false;		
	Color textColor = Color.white;
	
	int offsety=2;
	
	foldOutStyle.onActive.textColor = textColor;
	foldOutStyle.onFocused.textColor = textColor;
	foldOutStyle.onHover.textColor = textColor;
	foldOutStyle.onNormal.textColor = textColor;
	foldOutStyle.active.textColor = textColor;
	foldOutStyle.focused.textColor = textColor;
	foldOutStyle.hover.textColor = textColor;;
	foldOutStyle.normal.textColor = textColor;
	
	int width=Screen.width;
	if (addButton)
		width = Screen.width-37;
	
	Rect lastRect = DrawTitleGradient(0,width);
	
	
	GUI.backgroundColor = Color.black;
	
	GUIContent foldContent = new GUIContent();
	foldContent.text = " " + text;
	
	foldOut =  EditorGUI.Foldout(new Rect(lastRect.x + 3, lastRect.y+offsety, width , lastRect.height),foldOut,foldContent,true,foldOutStyle);	
	GUI.backgroundColor = Color.white;
	
	if (addButton){
		GUI.backgroundColor = Color.green;
		if (GUI.Button( new Rect(Screen.width-37, lastRect.y,  20 , lastRect.height),buttonText)){
			GUI.backgroundColor = Color.white;
			return true;	
		}
		GUI.backgroundColor = Color.white;
	}
	
	return false;
	
}

public static Rect DrawTitleChapter(string text,int size, bool bold, int style, int width)
{
	Rect lastRect = DrawTitleGradient(0,width);
	
	GUIStyle labelStyle =  new GUIStyle( EditorStyles.label);
	if (bold){
		labelStyle.fontStyle = FontStyle.Bold;
	}
	labelStyle.fontSize = size;
	
	Color textColor = Color.white;
	
	labelStyle.onActive.textColor = textColor;
	labelStyle.onFocused.textColor = textColor;
	labelStyle.onHover.textColor = textColor;
	labelStyle.onNormal.textColor = textColor;
	labelStyle.active.textColor = textColor;
	labelStyle.focused.textColor = textColor;
	labelStyle.hover.textColor = textColor;;
	labelStyle.normal.textColor = textColor;
	
	int offsety = 2;
	if (style==2) offsety=0;
	
	//GUI.Label(new Rect(lastRect.x + 3, lastRect.y+offsety, lastRect.width - 5, lastRect.height), text,labelStyle);
	GUI.Label(new Rect(lastRect.x + 3, lastRect.y+offsety, width, lastRect.height), text,labelStyle);
	
	return lastRect;
}

public static bool ChildFoldOut(bool foldOut,string text, Color color, int width){
	
	string label="[+] " + text;
	if (foldOut) label="[-] " + text;
	if (HTGuiTools.Button(label,color,width,true)){
		foldOut = !foldOut;		
	}
	
	return foldOut;
}

// Vector2
public static Vector2 Vector2Field(string label,Vector2 vector){
	Rect r = EditorGUILayout.BeginHorizontal();	
	EditorGUILayout.PrefixLabel(label);
	r.y-= 18;
	r.x = 50;
	vector = EditorGUI.Vector2Field(r,"",vector);
	EditorGUILayout.EndHorizontal();
	
	return vector;
}


// Depth control
static public int GuiDepth(int depth){
	
	EditorGUILayout.BeginHorizontal();		
	EditorGUILayout.PrefixLabel("Gui depth");
	if (GUILayout.Button("Back",GUILayout.Width(50))){
		depth--;	
	}
	depth =EditorGUILayout.IntField(depth,GUILayout.Width(32));
	
	if (GUILayout.Button("Front",GUILayout.Width(50))){
		depth++;	
	}			
	EditorGUILayout.EndHorizontal();
	
	
	return depth;
}

// Picture
public static void DrawTexturePreview( Rect rect, Texture tex){
	DrawTileTexture( rect, HTGuiTools.GetCheckerTexture());
	if (tex!=null){
		GUI.DrawTexture( rect, tex,ScaleMode.ScaleToFit);	
	}
}

public static void DrawTextureRectPreview( Rect rect, Rect textureRect, Texture2D tex, Color color){
	
	GUI.color = color;
	GUI.DrawTexture( rect, EditorGUIUtility.whiteTexture);
	GUI.color = Color.white;
	
	rect = new Rect(rect.x+3,rect.y+3,rect.width-6,rect.height-6);
	DrawTileTexture( rect, HTGuiTools.GetCheckerTexture());
	
	
	
	if (tex!=null){	

		GUI.DrawTextureWithTexCoords( rect, tex,textureRect );
	}
	
}

// Draw separator line
public static void DrawSeparatorLine(int padding=0)
{
	DrawSeparatorLine(Color.gray, padding);
}

public static void DrawSeparatorLine(Color color,int padding=0)
{
	
	GUILayout.Space(10);
	Rect lastRect = GUILayoutUtility.GetLastRect();
	
	GUI.color = color;
	GUI.DrawTexture(new Rect(padding, lastRect.yMax -lastRect.height/2f, Screen.width, 1f), EditorGUIUtility.whiteTexture);
	GUI.color = Color.white;
}

public static void DrawTileTexture (Rect rect, Texture tex)
{
	GUI.BeginGroup(rect);
	{
		int width  = Mathf.RoundToInt(rect.width);
		int height = Mathf.RoundToInt(rect.height);
		
		for (int y = 0; y < height; y += tex.height)
		{
			for (int x = 0; x < width; x += tex.width)
			{
				GUI.DrawTexture(new Rect(x, y, tex.width, tex.height), tex);
			}
		}
	}
	GUI.EndGroup();
}

public static void DrawLine(float x1,float y1,float x2,float y2, Color color){
	GUI.color = color;
	GUI.DrawTexture( new Rect(x1,y1,x2-x1,y2-y1), EditorGUIUtility.whiteTexture);
	GUI.color = Color.white;
}

// Gradient
private static Rect DrawTitleGradient(int padding, int width)
{
	int space=35;
	
	GUILayout.Space(space);
	Rect lastRect = GUILayoutUtility.GetLastRect();
	lastRect.yMin = lastRect.yMin + 7;
	lastRect.yMax = lastRect.yMax - 7;
	lastRect.width =  Screen.width;
	
	if (width==-1)
		GUI.DrawTexture(new Rect(padding,lastRect.yMin+1,Screen.width, lastRect.yMax- lastRect.yMin), GetGradientTexture());
	else
		GUI.DrawTexture(new Rect(padding,lastRect.yMin+1,width, lastRect.yMax- lastRect.yMin), GetGradientTexture());
	
	return lastRect;
}

private static Texture2D GetGradientTexture(){
	
	if (HTGuiTools.gradientTextureStyle==null){
		HTGuiTools.gradientTextureStyle = CreateGradientTexture();
	}
	return gradientTextureStyle;
	
}

private static Texture2D CreateGradientTexture()
{
	// new texture
	int height =18;
	
	Texture2D myTexture = new Texture2D(1, height);
	myTexture.hideFlags = HideFlags.HideInInspector;
	myTexture.hideFlags = HideFlags.DontSave;
	myTexture.filterMode = FilterMode.Bilinear;
	
	Color startColor= new Color(0.4f,0.4f,0.4f);
	Color endColor= new Color(0.6f,0.6f,0.6f);
	
	float stepR = (endColor.r - startColor.r)/18f;
	float stepG = (endColor.g - startColor.g)/18f;
	float stepB = (endColor.b - startColor.b)/18f;
	
	Color pixColor = startColor;
	
	for (int i = 1; i < height-1; i++)
	{
		pixColor = new Color(pixColor.r + stepR,pixColor.g + stepG , pixColor.b + stepB);
		myTexture.SetPixel(0, i, pixColor);
	}
	
	myTexture.SetPixel(0, 0, new Color(0,0,0));
	myTexture.SetPixel(0, 17, new Color(1,1,1));
	
	myTexture.Apply();
	
	return myTexture;
}

// Checker
private static Texture2D GetCheckerTexture(){
	if (HTGuiTools.checkerTexture==null){
		HTGuiTools.checkerTexture = CreateCheckerTexture();
	}
	return checkerTexture;		
}

private static Texture2D CreateCheckerTexture(){
	
	Texture2D myTexture = new Texture2D(16, 16);
	myTexture.hideFlags = HideFlags.DontSave;
	
	Color color1 = new Color(0.5f,0.5f,0.5f);
	for( int x=0;x<8;x++){
		for( int y=0;y<8;y++){
			myTexture.SetPixel(x, y, color1);
			myTexture.SetPixel(x+8, y+8, color1);
		}
	}
	
	color1 = new Color(0.3f,0.3f,0.3f);
	for( int x=0;x<8;x++){
		for( int y=0;y<8;y++){
			myTexture.SetPixel(x+8, y, color1);
			myTexture.SetPixel(x, y+8, color1);
		}
	}
	
	myTexture.Apply();
	
	return myTexture;
}	


#region AssetDatabase tool
public static bool CreateAssetDirectory(string rootPath,string name){
	string directory = rootPath + "/" +  name;
	if (!System.IO.Directory.Exists(directory)){
		AssetDatabase.CreateFolder(rootPath,name);
		AssetDatabase.Refresh();
		return true;
	}
	return false;
}

public static string GetAssetRootPath( string path){
	
	string[] tokens = path.Split('/');
	
	path="";
	for (int i=0;i<tokens.Length-1;i++){
		path+= tokens[i] +"/";
	}
	return path;
}
#endregion
*/