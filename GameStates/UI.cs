using UnityEngine;
using System.Collections;

public class UI : MonoBehaviour {

    //Normal button
	public static bool CreateButton(float x,float y,int width,int height,string text)
	{
		if(GUI.Button(new Rect(Screen.width * x,Screen.height * y,width,height),text))
		{
			return true;
		}
		else return false;
	}
	
    //Button with texture
    public static bool CreateButton(float x, float y, int width, int height,Texture2D texture2D)
    {
        if (GUI.Button(new Rect(Screen.width * x, Screen.height * y, texture2D.width, texture2D.height),texture2D))
        {
            return true;
        }
        else return false;
    }

	public static void CreateTextLabel(float x,float y,int width,int height,string text,Color color)
	{
		GUIStyle style = new GUIStyle();
		style.normal.textColor = color;
		GUI.Label(new Rect(Screen.width * x,Screen.height * y,width,height),text,style);
	}
	
	public static void CreateTextLabel(float x,float y,int width,int height,string text,GUIStyle style)
	{
		GUI.Label(new Rect(Screen.width * x,Screen.height * y,width,height),text,style);
	}
	
	
	public static string CreateTextBox(float x,float y,int width,int height,string variable)
	{
		return GUI.TextField(new Rect(Screen.width * x,Screen.height * y,width,height),variable);
		
	}
	
}
