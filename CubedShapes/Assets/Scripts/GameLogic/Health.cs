using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health {

    public float maxHealth;
    public float maxShield;
    public float healthRegenBase;
    public float shieldRegenBase;

	public Health(float maxHealthVal, float maxShieldVal, float healthRegenBaseVal, float shieldRegenBaseVal)
    {
        this.maxHealth = maxHealthVal;
        this.maxShield = maxShieldVal;
        this.healthRegenBase = healthRegenBaseVal;
        this.shieldRegenBase = shieldRegenBaseVal;
    }

    public Health Clone()
    {
        return new Health(maxHealth, maxShield, healthRegenBase, shieldRegenBase);
    }
}
