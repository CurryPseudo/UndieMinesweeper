using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SmartList<T> : IEnumerable{
    List<T> items;
    HashSet<int> emptyIndex;
    public SmartList(){
        items = new List<T>();
        emptyIndex = new HashSet<int>();
    }
    public int Count{
        get{
            return items.Count - emptyIndex.Count;
        }
    }
    public T this[int index]{
        get{
            Debug.Assert(!emptyIndex.Contains(index));
            return items[index];
        }
        set{
            items[index] = value;
        }
    }
    public void RemoveAt(int index){
        items[index] = default(T);
        emptyIndex.Add(index);
    }
    public int Add(T t){
        if(emptyIndex.Count == 0){
            items.Add(t);
            return items.Count - 1;
        }else{
            var e = emptyIndex.GetEnumerator();
            e.MoveNext();
            int index = e.Current;
            emptyIndex.Remove(index);
            items[index] = t;
            return index;
        }
    }


    public SmartListEnum<T> GetEnumerator(){
        return new SmartListEnum<T>(items, emptyIndex);
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
public class SmartListEnum<T> : IEnumerator
{
    List<T> items;
    HashSet<int> emptyIndex;
    int index;

    public SmartListEnum(List<T> _items, HashSet<int> _emptyIndex)
    {
        items = _items;
        emptyIndex = _emptyIndex;
        index = -1;
    }

    object IEnumerator.Current{
        get{
            return Current;
        }
    }
    public T Current{
        get{
            Debug.Assert(index >= 0 && index < items.Count && !emptyIndex.Contains(index));
            return items[index];
        }
    }

    bool IEnumerator.MoveNext()
    {
        return MoveNext();
    }
    public bool MoveNext(){
        index++;
        while((index < items.Count && emptyIndex.Contains(index))){
            index++;
        }
        return index != items.Count;
    }
    void IEnumerator.Reset()
    {
        index = -1;
    }
}