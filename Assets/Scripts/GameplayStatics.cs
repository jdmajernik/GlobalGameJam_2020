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
    public const string REPAIRER_INTERACTIBLE_OBJECTS_LAYER_NAME = "RepairerInteractableObjects";

    public const string LOADING_BAR_TAG = "Extinguisher_LoadingBar";
    public const string EXTINGUISH_COLLIDER_TAG = "ExtinguishesFires";
    public const string FIRE_BOUNDARY_TAG = "FireBoundry";
    public const string GAME_UI_TAG = "GameUI";
    public const string ANIMAL_CONTROL_TAG = "AnimalControl";

    // This is a class that holds all our static lookup variables. Stuff like
    public static Dictionary<RepairerInput, string> RepairerInputLookup = new Dictionary<RepairerInput, string>()
    {
        {RepairerInput.Repairer_UseItem, "PlayerFix" },
        {RepairerInput.Repairer_DropItem, "PlayerDrop" },
    };
    public static Dictionary<BearInput, string> BearInputLookup = new Dictionary<BearInput, string>()
    {
        {BearInput.Bear_Attack, "BearAttack" },
        {BearInput.Bear_MoveHorizontal, "Horizontal" },
        {BearInput.Bear_Jump, "Jump" },
    };
    public static Dictionary<HouseFloors, Vector3> FloorPositionLookup = new Dictionary<HouseFloors, Vector3>()
    {
        {HouseFloors.House_Basement, Vector3.zero },

    };
}
