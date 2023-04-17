using System.Collections.Generic;
using System.Linq;
using BattleTech;
using HBS.Nav;
using UnityEngine;

namespace NavigationComputer.Features
{
    public static class ShiftClickMove
    {
        public static bool NextSelectIsShiftClick { get; set; }

        public static bool HandleClickSystem(Starmap starmap, StarSystemNode system)
        {
            if (!NextSelectIsShiftClick)
                return true;

            NextSelectIsShiftClick = false;

            //var plannedPath = Traverse.Create(starmap.Screen).Field("_plannedPath").GetValue<LineRenderer>();
            var plannedPath = starmap.Screen.plannedPath;
            if (starmap.PotentialPath == null || starmap.PotentialPath.Count == 0 || plannedPath == null ||
                plannedPath.positionCount == 0)
            {
                Main.HBSLog.Log("Shift clicked system but had no previous route");
                return true;
            }

            // set CurSelected to the new end of the route that we're making
            //Traverse.Create(starmap).Property("CurSelected").SetValue(system);
            starmap.CurSelected = system;

            var prevPath = new List<INavNode>(starmap.PotentialPath.ToArray());
            var prevPathLast = prevPath.Last();
            //var starmapPathfinder = Traverse.Create(starmap).Field("starmapPathfinder").GetValue<AStar.PathFinder>();
            var starmapPathfinder = starmap.starmapPathfinder;
            starmapPathfinder.InitFindPath(prevPathLast, system, 1, 1E-06f, result =>
            {
                if (result.status != PathStatus.Complete)
                {
                    Main.HBSLog.LogError("Something went wrong with pathfinding!");
                    return;
                }

                result.path.Remove(prevPathLast);
                result.path.InsertRange(0, prevPath);

                Main.HBSLog.Log($"Created new hybrid route of size {result.path.Count}");
               // Traverse.Create(starmap).Method("OnPathfindingComplete", result).GetValue();
               starmap.OnPathfindingComplete(result);
            });

            return false;
        }
    }
}
