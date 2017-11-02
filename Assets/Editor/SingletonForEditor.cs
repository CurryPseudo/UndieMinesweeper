using UnityEditor;
using UnityEngine;
public class SingletonForEditor{
    public static void DrawABeaultifulButton(IndexOfList2D pos, Color color, System.Action pushButton) {
        Handles.color = color;
        if(Handles.Button(Singleton.MainData.AreaPosWorld(pos), Quaternion.identity, 0.5f, 0.5f, Handles.CylinderHandleCap)) {
            if(pushButton != null) {
                pushButton();
            }
        }
    }
    public static void DrawABeautifulSmallButton(IndexOfList2D pos, Color color, System.Action pushButton) {
        Handles.color = color;
        if(Handles.Button(Singleton.MainData.AreaPosWorld(pos), Quaternion.identity, 0.5f, 0.5f, Handles.ArrowHandleCap)) {
            if(pushButton != null) {
                pushButton();
            }
        }
    }
    public static void DrawABeaultifulLabel(IndexOfList2D pos, string text) {
        Handles.Label(Singleton.MainData.AreaPosWorld(pos), text);
    }
}
