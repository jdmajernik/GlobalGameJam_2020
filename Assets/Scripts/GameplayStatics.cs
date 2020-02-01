using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;
using UnityEngine;


public enum RepairerInput
{
    Repairer_UseItem,
    Repairer_DropItem
}

public enum BearInput
{
    Bear_MoveHorizontal,
    Bear_Jump,
    Bear_Attack
}

public enum HouseFloors
{
    House_Basement,
    House_Middle,
    House_Top
}
public class GameplayStatics
{
    // This is a class that holds all our static lookup variables. Stuff like
    public readonly Dictionary<RepairerInput, string> RepairerInputLookup = new Dictionary<RepairerInput, string>()
    {
        {RepairerInput.Repairer_UseItem, "PlayerFix" },
        {RepairerInput.Repairer_DropItem, "PlayerDrop" },
    };
    public readonly Dictionary<BearInput, string> BearInputLookup = new Dictionary<BearInput, string>()
    {
        {BearInput.Bear_Attack, "BearAttack" },
        {BearInput.Bear_MoveHorizontal, "Horizontal" },
        {BearInput.Bear_Jump, "Jump" },
    };
    public readonly Dictionary<HouseFloors, Vector3> FloorPositionLookup = new Dictionary<HouseFloors, Vector3>()
    {
        {HouseFloors.House_Basement, Vector3.zero },

    };
}
