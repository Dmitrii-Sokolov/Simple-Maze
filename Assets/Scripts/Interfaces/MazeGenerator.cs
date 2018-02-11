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
                return new PrimThin();
            case MazeGeneratorType.Kruskal:
                return new KruskalThin();
            case MazeGeneratorType.Wilson:
                return new WilsonThin();
            case MazeGeneratorType.AldousBroder:
                return new AldousBroderThin();
            case MazeGeneratorType.HuntAndKill:
                return new HuntAndKillThin();
            case MazeGeneratorType.Sidewinder:
                return new SidewinderThin();
            case MazeGeneratorType.BinaryTree:
                return new BinaryTreeThin();
            case MazeGeneratorType.DeepSearch:
                return new DeepSearchThin();
            case MazeGeneratorType.DeepSearchT:
                return new DeepSearchThick();
            case MazeGeneratorType.RandomString:
                return new RandomStringThin();
            default:
                Debug.LogError("Unknown Maze Generator Type: " + type);
                return null;
        }
    }
}

public interface MazeGenerator
{
    void Init(Maze TargetMaze);
    void Init();
    void Generate();
    bool NextStep();
}
