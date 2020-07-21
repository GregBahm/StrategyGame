﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleStageSide
{
    public IEnumerable<BattalionState> AllUnits { get; }

    public bool StillFighting { get; }

    public BattleStageSide(List<BattalionState> units)
    {
        AllUnits = units.AsReadOnly();
        StillFighting = GetIsStillFighting();
    }


    private bool GetIsStillFighting()
    {
        return AllUnits.Any(unit => unit.IsAlive);
    }

    public BattleStageSide GetRepositionedSurvivors()
    {
        IEnumerable<BattalionState> states = AllUnits.Where(item => item.IsAlive);
        
        Repositioner currentRepositioner = new Repositioner(states.ToList());
        List<Repositioner> repositioningHistory = new List<Repositioner>() { currentRepositioner };
        while(currentRepositioner.RepositioningHappend)
        {
            currentRepositioner = currentRepositioner.GetNext();
            repositioningHistory.Add(currentRepositioner);
        }
        return currentRepositioner.ToBattleSide();
    }

    public class Repositioner
    {
        private readonly BattalionState[,] grid;
        private readonly int columns;
        private readonly int rows;
        private readonly IEnumerable<BattalionState> repositioners;

        public IEnumerable<BattalionState> RepositionedUnits { get; }
        public bool RepositioningHappend { get; private set; }

        public Repositioner(IEnumerable<BattalionState> units)
        {
            columns = units.Max(unit => unit.Position.X);
            rows = units.Max(unit => Mathf.Abs(unit.Position.Y) * 2);

            grid = CreateGrid(units);

            // Step 1: If entire column is eliminated, bring all units forward.
            CollapseEmptyColumns();

            // Step 2: Move each advancer forward
            BringAdvancersForward();

            // Step 3: Move units inward

            for (int x = 0; x < columns; x++)
            {
                BringUnitsInward(x);
            }
        }

        private void BringUnitsInward(int column)
        {
            bool doLeanPositive = GetShouldLeanPositive(column);
            List<BattalionState> states = GetStatesAsList(column);
            int offset = GetInwardOffset(states.Count, doLeanPositive);
            for (int y = 0; y < rows; y++)
            {
                grid[column, y] = null;
            }
            for (int i = 0; i < states.Count; i++)
            {
                grid[column, i + offset] = states[i];
            }
        }

        private int GetInwardOffset(int count, bool doLeanPositive)
        {
            float ret = (rows - count) / 2;
            if(doLeanPositive)
            {
                return Mathf.CeilToInt(ret);
            }
            return Mathf.FloorToInt(ret);
        }

        private List<BattalionState> GetStatesAsList(int column)
        {
            List<BattalionState> ret = new List<BattalionState>();
            for (int y = 0; y < rows; y++)
            {
                if(grid[column, y] != null)
                {
                    ret.Add(grid[column, y]);
                }
            }
            return ret;
        }

        private bool GetShouldLeanPositive(int column)
        {
            int highVal = 0;
            int lowVal = 0;
            for (int x = 0; x < rows; x++)
            {
                if (grid[x, column] != null)
                {
                    highVal = Mathf.Max(x, highVal);
                    lowVal = Mathf.Max((rows - 1) - x, lowVal);
                }
            }
            return highVal > lowVal;
        }

        private void BringAdvancersForward()
        {
            for (int x = 1; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    BattalionState state = grid[x, y];
                    if(state != null && GetIsAdvancer(state))
                    {
                        BattalionState targetPos = grid[x - 1, y];
                        if(targetPos == null)
                        {
                            grid[x, y] = null;
                            grid[x - 1, y] = state;
                            RepositioningHappend = true;
                        }
                    }
                }
            }
        }

        private bool GetIsAdvancer(BattalionState state)
        {
            int advancing = state.GetAttribute(BattalionAttribute.Advancing);
            return advancing > 0;
        }

        private void CollapseEmptyColumns()
        {
            for (int x = 0; x < columns; x++)
            {
                bool columnContainsUnits = GetDoesColumnContainUnits(x);
                if(!columnContainsUnits)
                {
                    MoveColumnsForward(x + 1);
                }
            }
        }

        private void MoveColumnsForward(int startingColumn)
        {
            for (int x = startingColumn; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    BattalionState state = grid[x, y];
                    if(state != null)
                    {
                        grid[x, y] = null;
                        grid[x - 1, y] = state;
                    }
                }
            }
        }

        private bool GetDoesColumnContainUnits(int x)
        {
            for (int y = 0; y < rows; y++)
            {
                if (grid[x, y] != null)
                {
                    return true;
                }
            }
            return false;
        }

        private BattalionState[,] CreateGrid(IEnumerable<BattalionState> units)
        {
            BattalionState[,] ret = new BattalionState[columns, rows];
            foreach (BattalionState unit in units)
            {
                int absoluteRow = GetAbsoluteRow(unit.Position.Y);
                ret[unit.Position.X, absoluteRow] = unit;
            }
            return ret;
        }

        internal BattleStageSide ToBattleSide()
        {
            List<BattalionState> newStates = new List<BattalionState>();

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    BattalionState oldState = grid[x, y];
                    if(oldState != null)
                    {
                        BattalionState newState = GetRepositionedState(oldState, x, y);
                        newStates.Add(newState);
                    }
                }
            }
            return new BattleStageSide(newStates);
        }

        private BattalionState GetRepositionedState(BattalionState oldState, int newXPos, int newYPos)
        {
            int actualYPos = GetOffsetRow(newYPos);
            if(oldState.Position.X == newXPos && oldState.Position.Y == actualYPos)
            {
                return oldState;
            }
            return oldState.GetWithNewPosition(newXPos, actualYPos);
        }

        private int GetAbsoluteRow(int relativeRowPosition)
        {
            if(relativeRowPosition > 0)
            {
                relativeRowPosition --;
            }
            return relativeRowPosition + (rows / 2);
        }

        private int GetOffsetRow(int absoluteRowPosition)
        {
            int ret = absoluteRowPosition - (rows / 2);
            if(ret >= 0) // 
            {
                ret ++;
            }
            return ret;
        }

        internal Repositioner GetNext()
        {
            return new Repositioner(ToBattleSide().AllUnits);
        }
    }
}