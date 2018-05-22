using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Sense
{
    Seeing,
    Hearing
}
public class Senses {

    public static int MAX_RAY_CASTS_WHEN_TRY_TO_SEE = 3;

    public float visionRangeX;
    public float visionRangeY;
    public float hearingRangeX;
    public float hearingRangeY;

    public float reactionTime;

    public GameUnit owner;

	public Senses(float visionRangeXVal, float visionRangeYVal, float hearingRangeXVal, float hearingRangeYVal, float reactionTimeVal)
    {
        this.visionRangeX = visionRangeXVal;
        this.visionRangeY = visionRangeYVal;
        this.hearingRangeX = hearingRangeXVal;
        this.hearingRangeY = hearingRangeYVal;
        this.reactionTime = reactionTimeVal;
    }
    public bool CanSee(GameUnit target)
    {
        return SeeOrHear(Sense.Seeing, target);
    }
    public bool CanHear(GameUnit target)
    {
        return SeeOrHear(Sense.Hearing, target);
    }
    public RaycastHit TryToHit(Vector3 fromPosition, GameUnit target, int tries)
    {
        RaycastHit hit;

        for(int i = 0; i < tries; i++) {
            Vector3 toPosition = GetRandomPointFrom(fromPosition, target); //targetBody.position;
            Vector3 direction = toPosition - fromPosition;
        
            int layerMask = LayerMask.GetMask(Organizer.LAYERS_GAME_OBJECTS);

            if (Physics.Raycast(fromPosition, direction, out hit, layerMask))
            {
                ColliderOwner co = hit.collider.transform.gameObject.GetComponent<ColliderOwner>();
                if (co != null)
                {
                    if (co.owner == target)
                    {
                        //Debug.DrawRay(fromPosition, direction, Color.green);
                        return hit;
                    }
                }
            }
        }
        return new RaycastHit();
        //return Vector3.negativeInfinity;
    }
    public Vector3 GetRandomPointFrom(Vector3 fromPosition, GameUnit target)
    {
        float x = target.body.position.x;
        float y = target.body.position.y;
        float z = target.body.position.z;

        //Aim for roof if above
        if (target.body.position.y + target.body.localScale.y < fromPosition.y)
        {
            x += (float)Level.instance.rand.NextDouble() * target.body.localScale.x - target.body.localScale.x / 2;
            y += target.body.localScale.y / 2;

        }
        else if (target.body.position.x < fromPosition.x)
        {
            x += target.body.localScale.x / 2;
        }
        else
        {
            x -= target.body.localScale.x / 2;
        }
        //Aim for top half if  below
        if (target.body.position.y - target.body.localScale.y > fromPosition.y)
        {
            y += target.body.localScale.y / 2 - (float)Level.instance.rand.NextDouble() * target.body.localScale.y / 2;
        }
        else if (y == target.body.position.y)
        {
            y += (float)Level.instance.rand.NextDouble() * target.body.localScale.y - target.body.localScale.y / 2;
        }

        z += (float)Level.instance.rand.NextDouble() * target.body.localScale.z - target.body.localScale.z / 2;

        return new Vector3(x, y, z);
    }

    protected bool SeeOrHear(Sense sens, GameUnit target)
    {
        Transform selfBody = owner.body;
        Transform targetBody = target.body;

        float modifierX = visionRangeX;
        float modifierY = visionRangeY;

        if (sens == Sense.Hearing)
        {
            modifierX = hearingRangeX;
            modifierY = hearingRangeY;
        }

        if (
                (
                    (selfBody.forward.x < 0 || sens == Sense.Hearing)
                    && targetBody.transform.position.x < selfBody.transform.position.x
                    && targetBody.transform.position.x + modifierX > selfBody.position.x
                )
                ||
                (
                    (selfBody.forward.x > 0 || sens == Sense.Hearing)
                    && targetBody.transform.position.x > selfBody.transform.position.x
                    && targetBody.transform.position.x - modifierX < selfBody.position.x
                )
            )
        {
            if (
                (targetBody.transform.position.y < selfBody.transform.position.y
                    && targetBody.transform.position.y + modifierY > selfBody.position.y
                )
                ||
                (
                    targetBody.transform.position.y > selfBody.transform.position.y
                    && targetBody.transform.position.y - modifierY < selfBody.position.y
                )
            )
            {
                if (sens == Sense.Seeing)
                {
                    Vector3 fromPosition = selfBody.position;
                    fromPosition.y += owner.headYOffset;
                    RaycastHit hit = TryToHit(fromPosition, target, MAX_RAY_CASTS_WHEN_TRY_TO_SEE);
                    if(hit.collider != null)
                    {
                        return true;
                    }

                    /*Vector3 toPosition = GetRandomPointOn(owner.body.position,target); //targetBody.position;
                    Vector3 direction = toPosition - fromPosition;
                    RaycastHit hit;
                    


                    // Casts a ray against colliders in the scene
                    if (Physics.Raycast(fromPosition, direction, out hit))
                    {
                        ColliderOwner co = hit.transform.gameObject.GetComponent<ColliderOwner>();
                        if(co != null)
                        {
                            if(co.owner == target)
                            {
                                Debug.DrawRay(fromPosition, direction, Color.green);
                                return true;
                            }
                        }
                    }*/
                }
                else
                {
                    return true;
                }
            }
        }
        return false;
    }
    public Senses Clone()
    {
        return new Senses(visionRangeX, visionRangeY, hearingRangeX, hearingRangeY,reactionTime);
    }
}
