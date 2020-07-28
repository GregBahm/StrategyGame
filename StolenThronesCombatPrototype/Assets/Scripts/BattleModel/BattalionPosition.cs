using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

public class BattalionPosition
{
    public int X { get; }
    public int Y { get; }

    public bool IsFrontLine { get; }

    public BattalionPosition(int x, int y)
    {
        if (y == 0)
        {
            throw new System.Exception("BattalionPosition Y can never equal zero");
        }
        X = x;
        Y = y;
        IsFrontLine = x == 0;
    }

    public override bool Equals(Object obj)
    {
        return this.Equals(obj as BattalionPosition);
    }

    public bool Equals(BattalionPosition p)
    {
        if (Object.ReferenceEquals(p, null))
        {
            return false;
        }
        if (Object.ReferenceEquals(this, p))
        {
            return true;
        }
        if (this.GetType() != p.GetType())
        {
            return false;
        }
        return (X == p.X) && (Y == p.Y);
    }

    public override int GetHashCode()
    {
        return X * 0x00010000 + Y;
    }
    public static bool operator ==(BattalionPosition x, BattalionPosition y)
    {
        if (Object.ReferenceEquals(x, null))
        {
            if (Object.ReferenceEquals(y, null))
            {
                return true;
            }

            return false;
        }
        return x.Equals(y);
    }

    public static bool operator !=(BattalionPosition x, BattalionPosition y)
    {
        return !(x == y);
    }

    public IEnumerable<BattalionPosition> GetAdjacentPositions()
    {
        int lowerY = Y == 1 ? -1 : Y - 1;
        int higherY = Y == -1 ? 1 : Y + 1;

        yield return new BattalionPosition(X, lowerY);
        yield return new BattalionPosition(X, higherY);

        yield return new BattalionPosition(X + 1, lowerY);
        yield return new BattalionPosition(X + 1, Y);
        yield return new BattalionPosition(X + 1, higherY);

        if (!IsFrontLine)
        {
            yield return new BattalionPosition(X - 1, lowerY);
            yield return new BattalionPosition(X - 1, Y);
            yield return new BattalionPosition(X - 1, higherY);
        }
    }
}