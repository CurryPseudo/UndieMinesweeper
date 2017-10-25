using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class List2D<T> : System.Object{
	static int[] aroundXs = {-1, 0, 1, -1, 1, -1, 0, 1};
	static int[] aroundYs = {-1, -1, -1, 0, 0, 1, 1, 1};
    public delegate T SetAround(int x, int y,T get);
    [SerializeField]
    int xSize = 0;
    public int XSize{
        get{
            return xSize;
        }
    }
    [SerializeField]
    int ySize = 0;
    public int YSize{
        get{
            return ySize;
        }
    }
    public List<T> list;
    public List2D(int _xSize, int _ySize, T t){
        list = new List<T>();
        xSize = _xSize;
        ySize = _ySize;
        for(int i = 0; i < xSize; i++){
            for(int j = 0; j < ySize; j++){
                list.Add(t);
            }
        }
    }
    public T this[int x, int y]{
        get{
            return list[y * xSize + x];
        }
        set{
            list[y * xSize + x] = value;
        }
    }
    public T this[List2DIndex index]{
        get{
            return this[index.x, index.y];
        }
        set{
            this[index.x, index.y] = value;
        }
    }
    public bool Inside(int x, int y){
        return x >= 0 && x < xSize && y >= 0 && y < ySize;
    }
    public bool Inside(List2DIndex index){
        return Inside(index.x, index.y);
    }
    public void ChangeAround(int x, int y, SetAround setAround){
        for(int i = 0; i < 8; i++){
            int aroundX = x + aroundXs[i];
            int aroundY = y + aroundYs[i];
            if(Inside(aroundX, aroundY)){
                this[aroundX, aroundY] = setAround(aroundX, aroundY, this[aroundX, aroundY]);
            }
        }
    }
}
[System.Serializable]
public class List2DInt : List2D<int> {
    public List2DInt(int _xSize, int _ySize, int t) : base(_xSize, _ySize, t){}
}
[System.Serializable]
public class List2DGameObject : List2D<GameObject> {
    public List2DGameObject(int _xSize, int _ySize, GameObject t) : base(_xSize, _ySize, t){}
}
public class List2DIndex{
    public int x;
    public int y;
    public List2DIndex(int _x, int _y){
        x = _x;
        y = _y;
    }
}