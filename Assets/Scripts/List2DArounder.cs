using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class List2DArounder<T>{
	static int[] aroundXs = {-1, 0, 1, -1, 1, -1, 0, 1};
	static int[] aroundYs = {-1, -1, -1, 0, 0, 1, 1, 1};
    public delegate T SetAround(T t);
    public static void ChangeAround(List2D<T> list2D, int x, int y, SetAround setAround){
        for(int i = 0; i < 8; i++){
            int aroundX = x + aroundXs[i];
            int aroundY = y + aroundYs[i];
            if(aroundX >= 0 && aroundX < list2D.XSize && aroundY >= 0 && aroundY < list2D.YSize){
                list2D[aroundX, aroundY] = setAround(list2D[aroundX, aroundY]);
            }
        }
    }
}