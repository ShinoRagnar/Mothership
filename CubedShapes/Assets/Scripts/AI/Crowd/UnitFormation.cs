using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlacementStrategy
{
    MiddleAndOut,
    LeftToRight,
    RightToLeft
}
/*public enum Direction
{
    Towards,
    Against,
    Right,
    Left
}*/
public class UnitFormation  {

    public static float GROUND_MARGIN = 0.5f;

    public PlacementStrategy currentStrategy;

    public Dictionary<int, Vector3> placements;

    public Dictionary<GameUnit, int> placedUnits;

    public Ground currentlyOn;
    public float currentlyAtX;

    public int reserves;
    public int placed;

    public UnitFormation(PlacementStrategy strategy)
    {
        this.currentStrategy = strategy;
        this.placements = new Dictionary<int, Vector3>();
        this.placedUnits = new Dictionary<GameUnit,int>();
        this.reserves = 0;
        this.placed = 0;
    }
    public int Move(Ground ground, float xStart, float unitWidth)
    {
        return ProjectFormationOn(ground, xStart, unitWidth, placedUnits.Count);
    }
    public void Place(int i, GameUnit unit)
    {
        placedUnits.Add(unit,i);
    }
    public Vector3 GetMoveFor(GameUnit unit)
    {
        if (placedUnits.ContainsKey(unit))
        {
            return placements[placedUnits[unit]];
        }
        return Vector3.zero;
    }
    public Vector3 GetFormationCenter()
    {
        float x = currentlyAtX;
        float y = currentlyOn.GetMidPoint().y;
        float z = 0;

        return new Vector3(x, y, z);
    }
    public int ProjectFormationOn(Ground ground, float xStart, float unitWidth, int numberOfUnits)
    {
        reserves = 0;
        placed = 0;

        currentlyOn = ground;
        currentlyAtX = xStart;

        Vector3 center = GetFormationCenter();

        float x = center.x;
        float y = center.y;
        float z = center.z;

        float width = ground.obj.localScale.z - GROUND_MARGIN * 2;
        float unitsInRank = Mathf.FloorToInt(width / unitWidth);
        float maxRanks = ground.obj.localScale.x/ unitWidth;
        Debug.Log("Width: " + width + " unitsinRank: " + unitsInRank + " maxRanks" + maxRanks);

        for (int i = 0; i < unitsInRank*(maxRanks); i++)
        {
            reserves = numberOfUnits - placed;
            if(reserves == 0)
            {
                break;
            }
            int currentRank = Mathf.FloorToInt(((float)i) / unitsInRank);
            int positionWithinRank = i % (int)unitsInRank;

            if (currentStrategy == PlacementStrategy.LeftToRight)
            {
                x = xStart + currentRank * unitWidth;
            }
            else if (currentStrategy == PlacementStrategy.LeftToRight)
            {
                x = xStart - currentRank * unitWidth;
            }
            else if (currentStrategy == PlacementStrategy.MiddleAndOut)
            {

                float rankLeft = Mathf.Ceil(((float)currentRank) / 2.0f);
                float rankRight = currentRank / 2;
                float rank = 0;

                if (currentRank % 2 == 1)
                {
                    rank = -rankLeft;
                }
                else
                {
                    rank = rankRight;
                }

                x = xStart - rank * unitWidth;
            }

            if (positionWithinRank % 2 == 1)
            {
                z = -Mathf.Ceil(((float)positionWithinRank) / 2.0f) * unitWidth;
            }
            else
            {
                z = (positionWithinRank / 2) * unitWidth;
            }
            Vector3 placement = new Vector3(x, y, z);
            if(CanPlace(ground, placement))
            {
                if (placements.ContainsKey(placed))
                {
                    placements[placed] = placement;
                }
                else
                {
                    placements.Add(placed, placement);
                }
                placed++;
            }
        }
        
        return reserves;
    }
    

    private bool CanPlace(Ground g, Vector3 position)
    {
        if(Mathf.Abs(position.z-g.obj.position.z) > Mathf.Abs(g.obj.transform.localScale.z/2 - GROUND_MARGIN))
        {
            return false;
        }else if (Mathf.Abs(position.x-g.obj.position.x) > Mathf.Abs(g.obj.transform.localScale.x / 2 - GROUND_MARGIN))
        {
            return false;
        }
        return true;
    }

    /*
	public static ArrayList ProjectFormationOn(PlacementStrategy strategy, Ground ground, float xStart, float unitWidth, int numberOfUnits)
    {
    }*/
}
