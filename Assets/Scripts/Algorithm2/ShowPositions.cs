using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowPositions : MonoBehaviour {

	public class PositionProperty{
		public List<IndexOfList2D> positions;
		public Color color;
		public PositionProperty(List<IndexOfList2D> _positions, Color _color) {
			positions = _positions;
			color = _color;
		}
	}
	public List<PositionProperty> positionsList = new List<PositionProperty>();
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
