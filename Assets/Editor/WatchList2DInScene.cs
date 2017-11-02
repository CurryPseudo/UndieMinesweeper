using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
[CustomEditor(typeof(WatchList2DInScene))]
public class WatchList2DInSceneEditor : EditWithList2DIntEditor {
    public override List2D<int> GetMainList2D() {
        WatchList2DInScene wl = target as WatchList2DInScene;
        return wl.list;
    }
}