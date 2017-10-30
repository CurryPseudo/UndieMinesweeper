using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class ChangeMap{
    
    private PrecacTableBase tableBase = null;
    private Dictionary<ConnectedAreas, SearchForCa> sfcas = null;
    private List2DInt mineData = null;
    private List2DInt flipBoard = null;
    private IndexOfList2D clickPos = null;
    public Area clickArea = null;
    public List<SetMap> result = null;
    public int insideCount = 0;
    public List<IndexOfList2D> insideTargets = null;
    public int insideTargetValue = 0;
    public ChangeMap(IndexOfList2D _clickPos, Dictionary<ConnectedAreas, SearchForCa> _sfcas, PrecacTableBase _tableBase){
        //Debug.Assert(_sfcas.Count != 0);
        clickPos = _clickPos;
        sfcas = _sfcas;
        mineData = Singleton.MainData.mineDatas;
        flipBoard = Singleton.GamePart.flipBoard;
        tableBase = _tableBase;
    }
    public void ChangeMineData(){
        foreach(var setMap in result){
            Singleton.MainData.SetMineData(setMap.pos.x, setMap.pos.y, setMap.value);
        }
    }
    public void Calculate(){
        clickArea = tableBase.map.GetArea(clickPos);
        result = new List<SetMap>();
        if(mineData[clickPos] != -1){
            return;
        }
        int allMinesCount = Singleton.MainData.mineCount;
        if(clickArea != null){
            ConnectedAreas mainCa = clickArea.father;
            var table = GetMinDifferenceResults(mainCa);
            FindInsideTarget(-1);
            var solution = FindFinalSolution(table, insideCount, allMinesCount, insideTargets.Count);
            if(solution == null){
                return;
            }
            int nextOutSideMineCount = 0;
            foreach(var ca in solution.Keys){
                nextOutSideMineCount += solution[ca].Count;
                SetOutsideResult(ca, solution[ca]);
            }
            int nextInsideMineCount = allMinesCount - nextOutSideMineCount;
            int deltaInsideMineCount = nextInsideMineCount - insideTargets.Count;
            if(deltaInsideMineCount > 0){
                FindInsideTarget(0);
                ReverseRandomInsideTarget(deltaInsideMineCount);
            }else{
                ReverseRandomInsideTarget(-deltaInsideMineCount);
            }
        }else{
            FindInsideTarget(-1);
            if(insideCount != insideTargets.Count){
                result.Add(new SetMap(clickPos, 0));
                FindInsideTarget(0);
                ReverseRandomInsideTarget(1, null);
            }else{
                var table = GetMinDifferenceResults(null);
                var solution = FindFinalSolution(table, insideCount - 1, allMinesCount, insideCount);
                if(solution == null){
                    return;
                }
                int nextOutSideMineCount = 0;
                foreach(var ca in solution.Keys){
                    nextOutSideMineCount += solution[ca].Count;
                    SetOutsideResult(ca, solution[ca]);
                }
                result.Add(new SetMap(clickPos, 0));
                int deltaInsideMineCount = insideCount - allMinesCount + nextOutSideMineCount - 1;
                ReverseRandomInsideTarget(deltaInsideMineCount, clickPos);
            }

        }
    }
    public int CountingMines(ConnectedAreas ca){
        int count = 0;
        foreach(var area in ca.outsides){
            if(mineData[area.pos] == -1){
                count++;
            }
        }
        return count;
    }
    public void SetOutsideResult(ConnectedAreas ca, List<IndexOfList2D> target){
        List2DInt map = Singleton.CreateNewList2DInt();
        foreach(var pos in target){
            map[pos] = -1;
        }
        foreach(var area in ca.outsides){
            result.Add(new SetMap(area.pos,map[area.pos]));
        }
    }
    public void FindInsideTarget(int targetValue){
        insideTargetValue = targetValue;
        insideCount = 0;
        insideTargets = new List<IndexOfList2D>();
        for(int i = 0; i < flipBoard.XSize; i++){
            for(int j = 0; j < flipBoard.YSize; j++){
                if(flipBoard[i, j] == 0 && tableBase.map.GetArea(i, j) == null){
                    insideCount++;
                    bool equal = (targetValue == -1 && mineData[i, j] == -1) || (targetValue != -1 && mineData[i, j] != -1);
                    if(equal){
                        insideTargets.Add(new IndexOfList2D(i, j));
                    }
                }
            }
        }
    }
    public void ReverseRandomInsideTarget(int count, IndexOfList2D exceptPos = null){
        int setValue = -1 - insideTargetValue;
        List<IndexOfList2D> randomList = new List<IndexOfList2D>(insideTargets);
        if(exceptPos != null){
            foreach(IndexOfList2D pos in randomList){
                if(pos.x == exceptPos.x && pos.y == exceptPos.y){
                    randomList.Remove(pos);
                    break;
                }
            }
        }
       for(int i = 0; i < count; i++){
            IndexOfList2D randomPos = randomList[(int)(Random.value * randomList.Count)];
            randomList.Remove(randomPos);
            result.Add(new SetMap(randomPos, setValue));
        }
    }
    public Dictionary<ConnectedAreas, List<IndexOfList2D>> FindFinalSolution(Dictionary<ConnectedAreas, Dictionary<int, MinesWithDifference>> table, int insideAreaCount, int allMineCount, int insideMineCount){
        Dictionary<ConnectedAreas, List<IndexOfList2D>> solution = null;
        List<ConnectedAreas> caList = new List<ConnectedAreas>(table.Keys);
        if(caList.Count == 0) return null;
        Dictionary<ConnectedAreas, List<int>> caTable = new Dictionary<ConnectedAreas, List<int>>();
        List<int> stack = new List<int>();
        foreach(var ca in caList){
            caTable[ca] = new List<int>(table[ca].Keys);
            caTable[ca].Sort();
        }
        int sum = 0;
        int caIndex = 0;
        int minesIndex = 0;
        int minDifference = 0;
        while(true){
            if(caIndex < caList.Count && minesIndex < caTable[caList[caIndex]].Count && caTable[caList[caIndex]][minesIndex] + sum <= allMineCount){
                var ca = caList[caIndex];
                var mineCount = caTable[ca][minesIndex];
                if(caIndex == caList.Count - 1 && mineCount + sum + insideAreaCount < allMineCount){
                    minesIndex++;
                }else{
                    stack.Add(minesIndex);
                    sum += mineCount;
                    caIndex++;
                    minesIndex = 0;
                }
            }else{
                if(caIndex >= caList.Count){
                    Dictionary<ConnectedAreas, List<IndexOfList2D>> newSolution = new Dictionary<ConnectedAreas, List<IndexOfList2D>>();
                    int difference = 0;
                    int tempSum = 0;
                    int stackIndex= 0;
                    for(;stackIndex < stack.Count; stackIndex++){
                        var ca = caList[stackIndex];
                        int mineCount = caTable[ca][stack[stackIndex]];
                        tempSum += mineCount;
                        MinesWithDifference mwd = table[ca][mineCount];
                        difference += mwd.difference;
                        newSolution[ca] = mwd.mines;
                    }
                    difference += Mathf.Abs(tempSum + insideMineCount - allMineCount);
                    if(solution == null || difference < minDifference){
                        minDifference = difference;
                        solution = newSolution;
                    }
                }
                if(stack.Count == 0) break;
                int lastIndex = stack[stack.Count - 1];
                stack.RemoveAt(stack.Count - 1);
                caIndex--;
                sum -= caTable[caList[caIndex]][lastIndex];
                minesIndex = lastIndex + 1;
            }
        }
        return solution;
    }
    public Dictionary<ConnectedAreas, Dictionary<int, MinesWithDifference>> GetMinDifferenceResults(ConnectedAreas mainCa){
        var table = new Dictionary<ConnectedAreas, Dictionary<int, MinesWithDifference>>();
        foreach(var ca in sfcas.Keys){
            table[ca] = new Dictionary<int, MinesWithDifference>();
            foreach(var keyAndValue in sfcas[ca].searchResults){
                int mineCount = keyAndValue.Key;
                List<IndexOfList2D> minDifference = null;
                int minDifferenceValue = 0;
                foreach(var mines in keyAndValue.Value){
                    if(ca != mainCa || !CheckContainClickPos(mines)){
                        int difference = CalculateDifference(ca, mines);
                        if(minDifference == null || difference < minDifferenceValue){
                            minDifference = mines;
                            minDifferenceValue = difference;
                        }
                    }
                }
                if(minDifference != null){
                    table[ca][mineCount] = new MinesWithDifference(minDifferenceValue, minDifference);
                }
            }
        }
        return table;
    }
    public bool CheckContainClickPos(List<IndexOfList2D> mines){
        foreach(var pos in mines){
            if(pos.x == clickPos.x && pos.y == clickPos.y){
                return true;
            }
        }
        return false;
    }
    public int CalculateDifference(ConnectedAreas ca, List<IndexOfList2D> mines){
        int result = 0;
        List2DInt map = Singleton.CreateNewList2DInt();
        foreach(var pos in mines){
            map[pos] = -1;
        }
        foreach(var area in ca.outsides){
            if((map[area.pos] == -1 && mineData[area.pos] != -1 ) || (map[area.pos] != -1 && mineData[area.pos] == -1)){
                result++;
            }
        }
        return result;
    }
}
[System.Serializable]
public class MinesWithDifference{
    public int difference;
    public List<IndexOfList2D> mines;
    public MinesWithDifference(int _difference, List<IndexOfList2D> _mines){
        difference = _difference;
        mines = _mines;
    }
}
[System.Serializable]
public class SetMap{
    public IndexOfList2D pos;
    public int value;
    public SetMap(IndexOfList2D _pos, int _value){
        pos  = _pos;
        value = _value;
    }
}