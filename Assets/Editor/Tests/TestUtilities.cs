using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

internal static class TestUtilities
{
    public static string JsonResourceFromFile(string file)
    {
        string root = "C:\\PROJECTS\\DungeonCrawler2\\Assets\\Editor\\Tests\\Resources\\{0}.json";
        return File.ReadAllText(string.Format(root, file));
    }
}