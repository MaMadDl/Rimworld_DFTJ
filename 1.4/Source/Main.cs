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
            settings = GetSettings<RoomApparelsSettings>();
            
            new Harmony("PrisonSelector.Mod").PatchAll();
        }
		public override string SettingsCategory() => "Room Apparels Jobs";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);

            List<SkillDef> jobList = DefDatabase<SkillDef>.AllDefsListForReading;
            


            if (settings.jobTypeList == null) settings.jobTypeList = new Dictionary<string, bool>();
            foreach (var def in jobList)
            {
                string name = def.ToString();
                if (!settings.jobTypeList.ContainsKey(name) && DefDatabase<SkillDef>.GetNamedSilentFail(name) != null)
                {
                    settings.jobTypeList[name] = false;
                }
            }

            settings.DoWindowContents(inRect);


        }

    }
}