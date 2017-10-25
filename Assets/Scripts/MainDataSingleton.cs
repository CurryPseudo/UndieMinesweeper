using UnityEngine;
public class MainDataSingleton{
    static MainData mainData = null;
    public static MainData value{
        get{
            if(mainData == null){
                mainData = GameObject.Find("MainData").GetComponent<MainData>();
            }
            return mainData;
        }
    }
}