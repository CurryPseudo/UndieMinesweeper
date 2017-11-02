using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Precalculate : MonoBehaviour {

	public PrecacTableBase tableBase;
	// Use this for initialization
	public MainData mainData;
	public GamePart gamePart;
	public event System.Action afterPrecacAction;
	public Dictionary<ConnectedAreas, SearchForCa> problemsAndResults;
	public void Init() {
		mainData = Singleton.MainData;
		gamePart = Singleton.GamePart;
		tableBase = new PrecacTableBase(mainData.XSize, mainData.YSize);
		problemsAndResults = new Dictionary<ConnectedAreas, SearchForCa>();
		Flip(new List<FlipNode>());
		visualizedData();
	}
	void Awake () {
		Init();
		gamePart.flipAction += Flip;
		gamePart.initAction += Init;
	}
	
	public void visualizedData() {
		Singleton.DestroyAllChilds(transform);
		int caIndex = 0;
		foreach(var ca in tableBase.cas.list) {
			GameObject caGb = new GameObject("ConnectedArea " + caIndex.ToString());
			caGb.transform.parent = transform;
			ShowCa sca = caGb.AddComponent<ShowCa>();
			sca.ca = ca;
			GameObject searchGb = new GameObject("Search");
			searchGb.transform.parent = caGb.transform;
			SearchForCaMB sfca = searchGb.AddComponent<SearchForCaMB>();
			sfca.searchForCa = problemsAndResults[ca];
			caIndex++;
		}
	}
	public void Flip(List<FlipNode> flipNodes) {
		foreach(var node in flipNodes) {
			var area = tableBase.map.GetArea(node.pos);
			if(area != null) {
				area.father.RemoveOutsides(area);
			}
			ConnectedAreas ca = tableBase.cas.CreateCa();
			area = tableBase.map.CreateNumber(node.pos, ca);
			gamePart.flipBoard.ChangeAround(node.x, node.y, 
				(int aroundX, int aroundY, int get)=>{
					IndexOfList2D pos = new IndexOfList2D(aroundX, aroundY);
					Area neighbour = tableBase.map.GetArea(pos);
					if(get == 0) {
						if(neighbour == null) {
							neighbour = tableBase.map.CreateOutside(pos, ca);
						}else{
							tableBase.cas.MergeCas(neighbour.father, ca);
							ca = neighbour.father;
						}
						neighbour.neighbours.Add(area);
						area.neighbours.Add(neighbour);
					}
					return get;
				}
			);
			if(area.neighbours.Count == 0) {
				tableBase.map.RemovePos(area.pos);
				tableBase.cas.Remove(ca);
			}	
		}
		problemsAndResults = new Dictionary<ConnectedAreas, SearchForCa>();
		foreach(var ca in tableBase.cas.list) {
			problemsAndResults[ca] = new SearchForCa(ca);
			problemsAndResults[ca].Process();
		}
		visualizedData();
		if(afterPrecacAction != null) {
			afterPrecacAction();
		}
	}
	// Update is called once per frame
	void Update () {
		
	}
}
public class Area{
	public IndexOfList2D pos;
	public List<Area> neighbours;
	public ConnectedAreas father;
	public Area(IndexOfList2D _pos, ConnectedAreas _father) {
		Debug.Assert(_father != null);
		pos = _pos;
		neighbours = new List<Area>(8);
		father = _father;
	}
	public Area(Area other) {
		pos = new IndexOfList2D(other.pos);
		neighbours = new List<Area>(other.neighbours);
	}
}
public class ConnectedAreas{
	public HashSet<Area> numbers;
	public HashSet<Area> outsides;
	public Cas father;
	public ConnectedAreas(Cas _father) {
		numbers = new HashSet<Area>();
		outsides = new HashSet<Area>();
		father = _father;
	}
	public ConnectedAreas(ConnectedAreas other) {
		numbers = new HashSet<Area>(other.numbers);
	}
	public void Merge(ConnectedAreas other) {
	
		foreach(var number in other.numbers) {
			numbers.Add(number);
			number.father = this;
		}
		foreach(var outside in other.outsides) {
			outsides.Add(outside);
			outside.father = this;
		}
	}
	public void RemoveOutsides(Area area) {
		foreach(var number in area.neighbours) {
			
			number.neighbours.Remove(area);
			if(number.neighbours.Count == 0) {
				numbers.Remove(number);
				father.father.map.RemovePos(number.pos);
			}
		}
		outsides.Remove(area);
		father.father.map.RemovePos(area.pos);
	}
}
public class Cas{
	public PrecacTableBase father;
	public List<ConnectedAreas> list;
	public delegate void CaAction(ConnectedAreas ca);
	public event CaAction addCaAction;
	public event CaAction removeCaAction;
	public Cas(PrecacTableBase _father) {
		father = _father;
		list = new List<ConnectedAreas>();
	}
	public ConnectedAreas CreateCa() {
		ConnectedAreas ca = new ConnectedAreas(this);
		list.Add(ca);
		if(addCaAction != null) {
			addCaAction(ca);
		}
		return ca;
	}
	public ConnectedAreas MergeCas(ConnectedAreas main, params ConnectedAreas[] args) {
		foreach(ConnectedAreas ca in args) {
			if(main != ca) {
				main.Merge(ca);
				Remove(ca);
			}
		}
		return main;
	}
	public void Remove(ConnectedAreas ca) {
		list.Remove(ca);
		if(removeCaAction != null) {
			removeCaAction(ca);
		}
	}
	
}
public class AreaMap{
	public PrecacTableBase father;
	public List2D<Area> map;
	public AreaMap(int xSize, int ySize, PrecacTableBase _father) {
		map = new List2D<Area>(xSize, ySize, null);
		father = _father;
	}
	public Area GetArea(IndexOfList2D pos) {
		return map[pos];
	}
	public Area GetArea(int x, int y) {
		return map[x, y];
	}
	public Area CreateNumber(IndexOfList2D pos, ConnectedAreas father) {
		Area area = new Area(pos, father);
		map[pos] = area;
		father.numbers.Add(area);
		return area;
	}
	public Area CreateOutside(IndexOfList2D pos, ConnectedAreas father) {
		Area area = new Area(pos, father);
		map[pos] = area;
		father.outsides.Add(area);
		return area;
	}
	public void RemovePos(IndexOfList2D pos) {
		
		map[pos] = null;
	}
}
public class PrecacTableBase{
	public AreaMap map;
	public Cas cas;
	public PrecacTableBase(int xSize, int ySize) {
		map = new AreaMap(xSize, ySize, this);
		cas = new Cas(this);
	}
}