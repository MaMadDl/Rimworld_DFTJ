using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using RimWorld;
using HarmonyLib;
using UnityEngine;
using Verse.AI.Group;
using ArmorRacks.Things;
using ArmorRacks.DefOfs;

namespace RoomApparels
{
	[HarmonyPatch(typeof(Pawn_JobTracker))]
	[HarmonyPatch(nameof(Pawn_JobTracker.DetermineNextJob))]
	static class RoomApparels_DetermineNextJob_InterceptorPatch
	{
		static void Postfix(Pawn_JobTracker __instance, ref ThinkResult __result)
		{
            Pawn pawn = __instance.pawn;
            var thinkNode = __result;
			var currJob = thinkNode.Job;
			var destRoom = currJob?.targetA.Thing?.Position.GetRoom(Find.CurrentMap);
						
			if (pawn.Faction == null) return;

			//change DOBill Later
			if (pawn.def.race.intelligence == Intelligence.Humanlike && thinkNode.SourceNode is JobGiver_Work && currJob.def == JobDefOf.DoBill )
 			{
 				var rackLister = destRoom?.ContainedAndAdjacentThings.OfType<ArmorRack>().Where(r => r.AllComps.OfType<CompRoomApparelsEnabler>().First().Enabled);
				if (rackLister?.Any() ?? true)
				{
					// Getting First Rack That's Not Reserved and has something stored
					var rack = rackLister.FirstOrFallback(r => r.InnerContainer.Count() > 0 && !r.MapHeld.reservationManager.IsReservedByAnyoneOf(r, Faction.OfPlayer));
					
					if (currJob != null) pawn.jobs?.jobQueue?.EnqueueFirst(currJob);
					if (thinkNode != null)
					{
						var injected = new ThinkResult(new Job(ArmorRacksJobDefOf.ArmorRacks_JobWearRack,rack) { count = 1 }, thinkNode.SourceNode, thinkNode.Tag, false);
						__result = injected;
                    }
                    else
                    {
						pawn.jobs?.StartJob(new Job(
							ArmorRacksJobDefOf.ArmorRacks_JobWearRack, rack)
						{ count = 1 },
							JobCondition.InterruptOptional,
							resumeCurJobAfterwards: true,
							cancelBusyStances: false,
							keepCarryingThingOverride: true);
                    }
				}
			}

        }
	}
}
