using System.Collections.Generic;

public class BattleBuilder
{
    private const int GridDimension = 8;

    private readonly List<BattalionState> leftUnitsList = new List<BattalionState>();
    private readonly BattalionState[,] leftUnits = new BattalionState[GridDimension, GridDimension];

    private readonly List<BattalionState> rightUnitsList = new List<BattalionState>();
    private readonly BattalionState[,] rightUnits = new BattalionState[GridDimension, GridDimension];

    public Battle ToBattle()
    {
        BattleStageSide baseLeftSide = new BattleStageSide(leftUnitsList);
        BattleStageSide baseRightSide = new BattleStageSide(rightUnitsList);

        BattleStageSide leftSide = baseLeftSide.GetRepositionedSurvivors();
        BattleStageSide rightSide = baseRightSide.GetRepositionedSurvivors();
        return new Battle(leftSide, rightSide);
    }

    public void AddLeft(BattalionTemplate template, int column)
    {
        int row = GetNextRow(leftUnits, column);
        BattalionPosition pos = new BattalionPosition(column, row + 1);
        BattalionState state = new BattalionState(template.Id, pos, template.Modifiers, template.EffectSources);
        leftUnits[column, row] = state;
        leftUnitsList.Add(state);
    }

    public void AddRight(BattalionTemplate template, int column)
    {
        int row = GetNextRow(rightUnits, column);
        BattalionPosition pos = new BattalionPosition(column, row + 1);
        BattalionState state = new BattalionState(template.Id, pos, template.Modifiers, template.EffectSources);
        rightUnits[column, row] = state;
        rightUnitsList.Add(state);
    }

    private int GetNextRow(BattalionState[,] grid, int column)
    {
        for (int y = 0; y < GridDimension; y++)
        {
            if(grid[column, y] == null)
            {
                return y;
            }
        }
        throw new System.Exception("grid full");
    }
}