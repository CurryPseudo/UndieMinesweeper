using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GamePart : MonoBehaviour {
    public List2DInt flipBoard = null;
    public List2D<AreaView> areaViews = null;
    public float flipDelayTime = 0.3f;
    public float flipTime = 0.5f;
    public delegate void FlipAction(List<FlipNode> flipNodes);
    public event FlipAction flipAction;
    public delegate void ClickAction(IndexOfList2D clickPos);
    public event ClickAction clickAction;
    public List<List<AreaView>> beforesList;
    public List<List<AreaView>> aftersList;
    public event System.Action initAction;
    public bool GameValid = true;
    public bool flipMine = false;
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    public int UnFlipedAreaCount{
        get{
            int count = 0;
            foreach(var pos in flipBoard.Positions()) {
                if(flipBoard[pos] == 0) {
                    count++;
                }
            }
            return count;
        }
    }
    void Init() {
        var areaGbs = Singleton.MainData.areaGbs;
        flipBoard = new List2DInt(areaGbs.XSize, areaGbs.YSize, 0);
        areaViews = new List2D<AreaView>(areaGbs.XSize, areaGbs.YSize, null);
        for(int i = 0; i < areaGbs.XSize; i++) {
            for(int j = 0; j < areaGbs.YSize; j++) {
                areaViews[i, j] = areaGbs[i, j].GetComponentInChildren<AreaView>();
            }
        }
        beforesList = new List<List<AreaView>>();
        aftersList = new List<List<AreaView>>();
        GameValid = true;
        flipMine = false;
        if(initAction != null) {
            initAction();
        }
    }
    void Awake()
    {
        Init();
        Singleton.MainData.BeforeDestroyAction += GetFlipsDone;
        Singleton.MainData.mb.AfterReGenerateAction += Init;
    }
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        flipAction += 
            (List<FlipNode> flipNodes)=>{
                StartCoroutine(Flip(flipNodes));
                return;
            };
        
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
   IEnumerator Flip(List<FlipNode> flipNodes) {
       if(flipNodes.Count == 0) yield break;
        GameObject[] prefabs = {Resources.Load("Number") as GameObject, Resources.Load("Mine") as GameObject};
        List<AreaView> befores = new List<AreaView>();
        List<AreaView> afters = new List<AreaView>();
        beforesList.Add(befores);
        aftersList.Add(afters);
        foreach(FlipNode node in flipNodes) {
            befores.Add(areaViews[node.x, node.y]);
            int index = Singleton.MainData.mineDatas[node.x, node.y] != -1 ? 0 : 1;
            var gb = Instantiate(prefabs[index], Singleton.MainData.transform);
            gb.transform.localPosition = Singleton.MainData.AreaPosLocal(node.x, node.y);
            var av = gb.GetComponentInChildren<AreaView>();
            av.x = node.x;
            av.y = node.y;
            av.SetSize(0);
            afters.Add(av);
        }
        float delayCountTime = 0;
        while(delayCountTime <= flipNodes[flipNodes.Count - 1].startFlipTime + flipTime) {
            int nodeIndex = 0;
            while(nodeIndex < flipNodes.Count) {
                if(delayCountTime > flipNodes[nodeIndex].startFlipTime) {
                    float value = Mathf.Clamp((delayCountTime - flipNodes[nodeIndex].startFlipTime) / flipTime, 0, 1);
                    befores[nodeIndex].SetSize(Mathf.Clamp(1 - value / 0.5f, 0, 1));
                    afters[nodeIndex].SetSize(Mathf.Clamp((value - 0.5f) / 0.5f, 0, 1));
                }
                nodeIndex++; 
            }
            yield return null;
            delayCountTime += Time.deltaTime;
        }
        GetBeforesDone(befores);
        beforesList.Remove(befores);
        GetAftersDone(afters);
        aftersList.Remove(afters); 
        yield break;
    }
    public void GetFlipsDone() {
        StopAllCoroutines();
        foreach(var befores in beforesList) {
            GetBeforesDone(befores);
        }
        foreach(var afters in aftersList) {
            GetAftersDone(afters);
        }
        beforesList.Clear();
        aftersList.Clear();
    }
    public void GetBeforesDone(List<AreaView> befores) {
        foreach(var av in befores) {
            av.DestroyAll();
        }
    }
    public void GetAftersDone(List<AreaView> afters) {
        foreach(var av in afters) {
            av.SetSize(1);
            areaViews[av.x, av.y] = av;
            Singleton.MainData.areaGbs[av.x, av.y] = av.transform.parent.gameObject;
        }
    }
    public void AreaClick(int x, int y) {
        if(!GameValid) {
            return;
        }
        Debug.Assert(flipBoard.Inside(x, y));
        if(clickAction != null) {
            clickAction.Invoke(new IndexOfList2D(x, y));
        }
        Queue BFS = new Queue();
        List<FlipNode> flipNodes = new List<FlipNode>();
        if(flipBoard[x, y] == 0) {
           FlipNode head = new FlipNode(0, x, y, 0);
           BFS.Enqueue(head);
           while(BFS.Count > 0) {
               FlipNode node = BFS.Dequeue() as FlipNode;
               if(flipBoard[node.x, node.y] == 0) {
                   if(Singleton.MainData.mineDatas[node.x, node.y] == -1) {
                       GameValid = false;
                       flipMine = true;
                   }
                   flipBoard[node.x, node.y] = 1;
                   flipNodes.Add(node);
                   if(Singleton.MainData.mineDatas[node.x, node.y] == 0) {
                        flipBoard.ChangeAround(node.x, node.y, 
                            (int aroundX, int aroundY, int get)=>{
                                FlipNode newNode = new FlipNode(node.depth + 1, aroundX, aroundY, node.depth * flipDelayTime);
                                BFS.Enqueue(newNode);
                                return get;
                            }
                        );
                   }
               }
           }
        }
        if(flipAction != null) {
            flipAction.Invoke(flipNodes);
        }
    }
}
public class FlipNode{
    public int depth;
    public int x;
    public int y;
    public IndexOfList2D pos{
        get{
            return new IndexOfList2D(x, y);
        }
    }
    public float startFlipTime;
    public FlipNode(int _depth, int _x, int _y, float _startFlipTime) {
        depth = _depth;
        x = _x;
        y = _y;
        startFlipTime = _startFlipTime;
    }
}