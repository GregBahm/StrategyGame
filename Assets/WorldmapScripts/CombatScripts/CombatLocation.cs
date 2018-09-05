using System;

public class CombatLocation
{
    public bool IsCollision { get; }
    /// <summary>
    /// The site of the battle if not collision
    /// </summary>
    public Guid? Province { get; }
    /// <summary>
    /// The battle is between Province and ProvinceB if a collision
    /// </summary>
    public Guid? ProvinceB { get; }

    public CombatLocation(Guid province)
    {
        Province = province;
        IsCollision = false;
    }

    public CombatLocation(Guid province,
        Guid provinceB)
    {
        Province = province;
        ProvinceB = provinceB;
        IsCollision = true;
    }
}