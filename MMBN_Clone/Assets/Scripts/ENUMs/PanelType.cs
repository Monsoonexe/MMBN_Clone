using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PanelType
{
    NULL, //standard tile
    BROKEN, //tile cannot be stood on. restores to NULL overtime
    CRACKED, //if stepped on, becomes BROKEN
    DARKHOLE,
    GRASS,
    HOLY, 
    ICE, 
    LAVA,
    METAL, 
    MISSING, 
    POISON,
    SAND, 
    SEA,
    WARNING//flashes yellow to alert to danger
}
