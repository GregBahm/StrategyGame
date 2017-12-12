public enum UnitAllegiance
{
    Attackers,
    Defenders,
    Neutrals, // The unit belongs to a new faction, aligned against both attackers and defenders. For example, a spell that creates a swarm of demons.
    AttacksAll // The unit is berzerk and will attack anything it can
}