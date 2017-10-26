using UnityEngine;
using UnityEditor;
public class Singleton{
    static MainData mainData = null;
    static GamePart gamePart = null;
    public static MainData MainData{
        get{
            if(mainData == null){
                mainData = GameObject.Find("MainData").GetComponent<MainData>();
            }
            return mainData;
        }
    }
    public static GamePart GamePart{
        get{
            if(gamePart == null){
                gamePart = GameObject.Find("MainData").GetComponent<GamePart>();
            }
            return gamePart;
        }
    }
    public static void DrawABeaultifulButton(IndexOfList2D pos, Color color, System.Action pushButton){
        Handles.color = color;
        if(Handles.Button(MainData.AreaPosWorld(pos), Quaternion.identity, 0.5f, 0.5f, Handles.CylinderHandleCap)){
            if(pushButton != null){
                pushButton();
            }
        }
    }
    public static void DrawABeautifulSmallButton(IndexOfList2D pos, Color color, System.Action pushButton){
        Handles.color = color;
        if(Handles.Button(MainData.AreaPosWorld(pos), Quaternion.identity, 0.5f, 0.5f, Handles.ArrowHandleCap)){
            if(pushButton != null){
                pushButton();
            }
        }
    }
    public static void DrawABeaultifulLabel(IndexOfList2D pos, string text){
        Handles.Label(MainData.AreaPosWorld(pos), text);
    }
}