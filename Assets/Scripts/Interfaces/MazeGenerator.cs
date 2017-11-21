using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MazeGeneratorType
{
    RecursiveDivision, RecursiveUnion, Prim, Kruskal, Wilson, AldousBroder, HuntAndKill, Sidewinder, BinaryTree, DeepSearch, DeepSearchT, RandomString
}

public static class MazeGeneratorExtensions
{
    public static MazeGenerator CreateGenerator(MazeGeneratorType type, int width, int height)
    {
        switch (type)
        {
            case MazeGeneratorType.RecursiveDivision:
                return new RecursiveDivisionThin();
            case MazeGeneratorType.RecursiveUnion:
                return new RecursiveUnionThin();
            case MazeGeneratorType.Prim:
                return new PrimThin(width, height);
            case MazeGeneratorType.Kruskal:
                return new KruskalThin(width, height);
            case MazeGeneratorType.Wilson:
                return new WilsonThin(width, height);
            case MazeGeneratorType.AldousBroder:
                return new AldousBroderThin(width, height);
            case MazeGeneratorType.HuntAndKill:
                return new HuntAndKillThin(width, height);
            case MazeGeneratorType.Sidewinder:
                return new SidewinderThin(width, height);
            case MazeGeneratorType.BinaryTree:
                return new BinaryTreeThin(width, height);
            case MazeGeneratorType.DeepSearch:
                return new DeepSearchThin(width, height);
            case MazeGeneratorType.DeepSearchT:
                return new DeepSearchThick(width, height);
            case MazeGeneratorType.RandomString:
                return new RandomStringThin(width, height);
            default:
                Debug.LogError("Unknown Maze Generator Type: " + type);
                return null;
        }
    }
}

public interface MazeGenerator
{
    void Init(Maze TargetMaze);
    void Generate();
    bool NextStep();
}
