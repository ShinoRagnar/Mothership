using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visor : Item {

    public bool visorIsUp;

    public Visor(string name, Transform pivotVisor, Alignment alignmentVal)
    {
        this.itemName = name;
        this.prefab = pivotVisor;
        this.visorIsUp = true;
        this.alignment = alignmentVal;
    }

    public void VisorUp()
    {

    }
    public void VisorDown()
    {

    }

    public Visor Clone()
    {
        return new Visor(this.itemName, this.prefab, this.alignment);
    }
}
