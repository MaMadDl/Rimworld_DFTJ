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
using ArmorRacks.ThingComps;

namespace RoomApparels
{
	[HarmonyPatch(typeof(Pawn_JobTracker))]
	[HarmonyPatch(nameof(Pawn_JobTracker.DetermineNextJob))]
	static class RoomApparels_DetermineNextJob_InterceptorPatch
	{
		private static Dictionary<Pawn, Tuple<Job,ArmorRack>> prevJobs = new Dictionary<Pawn, Tuple<Job,ArmorRack>>();

		private static Room getRoomAtJobDestination(Job job = null)
        {
			return job?.targetA.Thing?.Position.GetRoom(Find.CurrentMap);
		}

		static void Postfix(Pawn_JobTracker __instance, ref ThinkResult __result)
		{
			Pawn pawn = __instance.pawn;
            var thinkNode = __result;
			var currJob = thinkNode.Job;
			var destRoom = getRoomAtJobDestination(currJob);
			
			if (pawn.Faction == null) return;

			var oldPawnJob = prevJobs.TryGetValue(pawn);

			if (currJob == prevJobs.TryGetValue(pawn)?.Item1) return;

			if (oldPawnJob != null && (oldPawnJob?.Item1?.def != currJob?.def || destRoom != getRoomAtJobDestination(oldPawnJob?.Item1)))
			{

				destRoom = getRoomAtJobDestination(oldPawnJob.Item1);

				if (currJob != null)pawn.jobs?.jobQueue?.EnqueueFirst(currJob);

                var injected = new ThinkResult(new Job(ArmorRacksJobDefOf.ArmorRacks_JobTransferToRack, oldPawnJob.Item2) { count = 1 }, thinkNode.SourceNode, thinkNode.Tag, false);
                __result = injected;

                prevJobs.Remove(pawn);
				return;
			}

			//change DOBill Later ///FIXME	
			if (pawn.def.race.intelligence == Intelligence.Humanlike && thinkNode.SourceNode is JobGiver_Work && currJob.def == JobDefOf.DoBill )
 			{
				// add checks based on type of racks like human and age 
				var rackLister = destRoom?.ContainedAndAdjacentThings.OfType<ArmorRack>()
															.Where(r => r.AllComps.OfType<CompRoomApparelsEnabler>().First().Enabled);
				if (rackLister?.Any() ?? true)
				{
					// Getting First Rack That's Not Reserved and has something stored
					var rack = rackLister.FirstOrFallback(r => r.InnerContainer.Count() > 0
															&& !r.MapHeld.reservationManager.IsReservedByAnyoneOf(r, Faction.OfPlayer)
															&& !r.IsForbidden(pawn));
					if (rack != null)
					{
						if (currJob != null)pawn.jobs?.jobQueue?.EnqueueFirst(currJob);

						prevJobs[pawn] = new Tuple<Job, ArmorRack>(currJob, rack);

						if (thinkNode != null)
						{

							var injected = new ThinkResult(new Job(ArmorRacksJobDefOf.ArmorRacks_JobWearRack, rack) { count = 1 }, thinkNode.SourceNode, thinkNode.Tag, false);
							__result = injected;
							

						}
						else
						{
							Log.Error("we are in a bad place");
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
}
