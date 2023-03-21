using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace RoomApparels
{
    [StaticConstructorOnStartup]
    public class RoomApparelsMain : Mod
	{
		public static RoomApparelsSettings settings;
		public RoomApparelsMain(ModContentPack content) : base(content)
		{
			new Harmony("PrisonSelector.Mod").PatchAll();
		}
		public override string SettingsCategory() => "Room Apparels Job Type Selector";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);

            //if (settings.jobTypeList == null) settings.jobTypeList = new Dictionary<string, bool>();
            //foreach (string defName in toggleablespawndef.toggleablePawns)
            //{
            //    if (!settings.jobTypeList.ContainsKey(defName) && DefDatabase<JobDef>.GetNamedSilentFail(defName) != null)
            //    {
            //        settings.jobTypeList[defName] = false;
            //    }
            //}



            settings.DoWindowContents(inRect);


        }

    }
}