using UnityEngine;
public class Singleton{
    static GameObject mainDataGb = null;
    static Precalculate precalculate = null;
    public static GameObject MainDataGb{
        get{
            if(mainDataGb == null) {
                mainDataGb = GameObject.Find("MainData");
            }
            return mainDataGb;
        }
    }
    public static ViewRect ViewRect{
        get{
            return MainDataGb.GetComponent<ViewRect>();
        }
    }
    public static MainData MainData{
        get{
            return MainDataGb.GetComponent<MainDataMB>().mainData;
        }
    }
    public static GamePart GamePart{
        get{
            return MainDataGb.GetComponent<GamePart>();
        }
    }
    public static FlagPart FlagPart{
        get{
            return MainDataGb.GetComponent<FlagPart>();
        }
    }
    public static Precalculate Precalculate{
        get{
            if(precalculate == null) {
                precalculate = GameObject.Find("Algorithm2").GetComponent<Precalculate>();
            }
            return precalculate;
        }
    }
    
    public static void DestroyAllChilds(Transform parent) {
        int count = parent.childCount;
		for(int i = count - 1; i >= 0; i--) {
			GameObject.Destroy(parent.GetChild(i).gameObject);
		}
    }
    public static List2DInt CreateNewList2DInt() {
        return new List2DInt(MainData.XSize, MainData.YSize, 0);
    }
    public static void ResetFlipBoardAndRandomGenerateMineData() {
        
    }
}