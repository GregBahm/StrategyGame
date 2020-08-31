using System.Collections.Generic;
using System.Linq;

public class BattleVisualiser
{
    private readonly IEnumerable<BattalionVisualizer> unitVisualizers;
    private readonly MainScript mainScript;

    public BattleVisualiser(Battle battle, MainScript mainScript)
    {
        this.mainScript = mainScript;
        Dictionary<BattalionIdentifier, BattalionVisualizer> dictionary = InitializeVisualizers(battle);
        this.unitVisualizers = dictionary.Values;
    }

    private Dictionary<BattalionIdentifier, BattalionVisualizer> InitializeVisualizers(Battle battle)
    {
        Dictionary<BattalionIdentifier, BattalionVisualizer> ret = new Dictionary<BattalionIdentifier, BattalionVisualizer>();
        for (int i = 0; i < battle.Progression.Count; i++)
        {
            BattleRound round = battle.Progression[i];
            IEnumerable<BattalionState> allUnits = round.InitialState.LeftSide.AllUnits.Concat(round.InitialState.RightSide.AllUnits);
            foreach (BattalionState unitState in allUnits)
            {
                if (!ret.ContainsKey(unitState.Id))
                {
                    BattalionVisualizer visualizer = mainScript.CreateVisualsFor(unitState.Id.Type, battle.Progression.Count);
                    ret.Add(unitState.Id, visualizer);
                }
                ret[unitState.Id].InsertState(unitState, battle.Progression[i], i);
            }
        }
        return ret;
    }
    
    public void Display(float normalizedTime)
    {
        foreach (BattalionVisualizer unit in unitVisualizers)
        {
            unit.Dispay(normalizedTime);
        }
    }
}
