public enum UnitAllegiance
{
    Attacker,
    Defender,
    Neutral, // Will attack attackers and defenders, but not neutrals. For example, a spell that creates a swarm of demons.
    Berzerk // Will attack anything it can
}