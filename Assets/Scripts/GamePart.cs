using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GamePart : MonoBehaviour {
    public List2DInt flipBoard = null;
    public List2DInt mineDatas = null;
    public MainData mainData = null;
    public List2D<AreaView> areaViews = null;
    public float flipDelayTime = 0.3f;
    public float flipTime = 0.5f;
    public delegate void FlipAction(List<FlipNode> flipNodes);
    public event FlipAction flipAction;
    public delegate void ClickAction(IndexOfList2D clickPos);
    public event ClickAction clickAction;
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        
        mainData = GameObject.Find("MainData").GetComponent<MainData>();
        var areaGbs = mainData.areaGbs;
        flipBoard = new List2DInt(areaGbs.XSize, areaGbs.YSize, 0);
        areaViews = new List2D<AreaView>(areaGbs.XSize, areaGbs.YSize, null);
        for(int i = 0; i < areaGbs.XSize; i++){
            for(int j = 0; j < areaGbs.YSize; j++){
                areaViews[i, j] = areaGbs[i, j].GetComponentInChildren<AreaView>();
                areaViews[i, j].gamePart = this;
            }
        }
        mineDatas = mainData.mineDatas;
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
    void Update()
    {

    }
    IEnumerator Flip(List<FlipNode> flipNodes){
        GameObject[] prefabs = {Resources.Load("Number") as GameObject, Resources.Load("Mine") as GameObject};
        List<AreaView> befores = new List<AreaView>();
        List<AreaView> afters = new List<AreaView>();
        foreach(FlipNode node in flipNodes){
            befores.Add(areaViews[node.x, node.y]);
            int index = mineDatas[node.x, node.y] != -1 ? 0 : 1;
            var gb = Instantiate(prefabs[index], mainData.transform);
            gb.transform.localPosition = mainData.AreaPosLocal(node.x, node.y);
            var av = gb.GetComponentInChildren<AreaView>();
            av.x = node.x;
            av.y = node.y;
            av.gamePart = this;
            av.SetSize(0);
            areaViews[node.x, node.y] = av;
            afters.Add(av);
        }
        float delayCountTime = 0;
        while(true){
            int nodeIndex = 0;
            while(nodeIndex < flipNodes.Count){
                if(delayCountTime > flipNodes[nodeIndex].startFlipTime){
                    float value = Mathf.Clamp((delayCountTime - flipNodes[nodeIndex].startFlipTime) / flipTime, 0, 1);
                    befores[nodeIndex].SetSize(Mathf.Clamp(1 - value / 0.5f, 0, 1));
                    afters[nodeIndex].SetSize(Mathf.Clamp((value - 0.5f) / 0.5f, 0, 1));
                }
                nodeIndex++; 
            }
            yield return null;
            delayCountTime += Time.deltaTime;
        }
        foreach(var av in befores){
            av.DestroyAll();
        }
        foreach(var av in afters){
            av.SetSize(1);
        }
        yield break;
    }
    public void AreaClick(int x, int y){
        Debug.Assert(flipBoard.Inside(x, y));
        if(clickAction != null){
            clickAction.Invoke(new IndexOfList2D(x, y));
        }
        Queue BFS = new Queue();
        List<FlipNode> flipNodes = new List<FlipNode>();
        if(flipBoard[x, y] == 0){
           FlipNode head = new FlipNode(0, x, y, 0);
           BFS.Enqueue(head);
           while(BFS.Count > 0){
               FlipNode node = BFS.Dequeue() as FlipNode;
               if(flipBoard[node.x, node.y] == 0 && mineDatas[node.x, node.y] != -1){
                   flipBoard[node.x, node.y] = 1;
                   flipNodes.Add(node);
                   if(mineDatas[node.x, node.y] == 0){
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
        if(flipAction != null){
            flipAction.Invoke(flipNodes);
        }
    }
}
public class FlipNode{
    public int depth;
    public int x;
    public int y;
    public float startFlipTime;
    public FlipNode(int _depth, int _x, int _y, float _startFlipTime){
        depth = _depth;
        x = _x;
        y = _y;
        startFlipTime = _startFlipTime;
    }
}