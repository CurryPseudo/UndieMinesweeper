using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class List2DArounder<T>{
	static int[] aroundXs = {-1, 0, 1, -1, 1, -1, 0, 1};
	static int[] aroundYs = {-1, -1, -1, 0, 0, 1, 1, 1};
    public delegate T SetAround(int x, int y, T t);
    public static void ChangeAround(List2D<T> list2D, int x, int y, SetAround setAround){
        for(int i = 0; i < 8; i++){
            int aroundX = x + aroundXs[i];
            int aroundY = y + aroundYs[i];
            if(list2D.Inside(aroundX, aroundY)){
                list2D[aroundX, aroundY] = setAround(aroundX, aroundY, list2D[aroundX, aroundY]);
            }
        }
    }
}