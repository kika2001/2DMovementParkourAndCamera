using UnityEngine;
using Unity;
public class WallRun : MovementExtras
{
    public override void ActionStart()
    {
        base.ActionStart();
        Debug.Log("WallRun Extra Worked");
    }
}
