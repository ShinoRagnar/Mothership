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
    public ArrayList placements;
    public Dictionary<GameUnit, Vector3> placedUnits;

    public int reserves;

    public UnitFormation(PlacementStrategy strategy)
    {
        this.currentStrategy = strategy;
        this.placements = new ArrayList();
        this.placedUnits = new Dictionary<GameUnit, Vector3>();
        this.reserves = 0;
    }
    public void Place(Vector3 placement, GameUnit unit)
    {
        placedUnits.Add(unit, placement);
    }

    public int ProjectFormationOn(Ground ground, float xStart, float unitWidth, int numberOfUnits)
    {
        reserves = 0;

        this.placements = new ArrayList();
        this.placedUnits = new Dictionary<GameUnit, Vector3>();

        float x = xStart;
        float y = ground.GetMidPoint().y;
        float z = 0;

        float width = ground.obj.localScale.z - GROUND_MARGIN * 2;
        float unitsInRank = Mathf.FloorToInt(width / unitWidth);
        float maxRanks = ground.obj.localScale.x/ unitWidth;
        //Debug.Log("Width: " + width + " unitsinRank: " + unitsInRank + " maxRanks" + maxRanks);

        for (int i = 0; i < unitsInRank*(maxRanks); i++)
        {
            reserves = numberOfUnits - placements.Count;
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
                placements.Add(placement);
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
