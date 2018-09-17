using System;

public class CombatLocation
{
    public bool IsCollision { get; }
    /// <summary>
    /// The site of the battle if not collision
    /// </summary>
    public Province Province { get; }
    /// <summary>
    /// The battle is between Province and ProvinceB if a collision
    /// </summary>
    public Province ProvinceB { get; }

    public CombatLocation(Province province)
    {
        Province = province;
        IsCollision = false;
    }

    public CombatLocation(Province province,
        Province provinceB)
    {
        Province = province;
        ProvinceB = provinceB;
        IsCollision = true;
    }
}