using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;

namespace RoomApparels
{
    public class RoomApparelsSettings : ModSettings
    {
        private static Vector2 scrollPos = Vector2.zero;
        public Dictionary<string,bool> jobTypeList = new Dictionary<string, bool>();
        public bool allPossibleJobs = false;

        private List<string> jobTypes;
        private List<bool> jobBoolValue;
        public override void ExposeData()
        {            
            base.ExposeData();
            Scribe_Collections.Look(ref jobTypeList, "jobTypeList", LookMode.Value, LookMode.Value, ref jobTypes, ref jobBoolValue);
            Scribe_Values.Look(ref allPossibleJobs, "allPossibleJobs", true, true);

        }
        public void DoWindowContents(Rect inRect)
        {
            List<string> keys = jobTypeList.Keys.Reverse().OrderByDescending(x=>x).ToList();
            Listing_Standard ls = new Listing_Standard();
            Rect rect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height);
            Rect rect2 = new Rect(0f, 0f, inRect.width - 30f, inRect.height - 30f);
            Widgets.BeginScrollView(rect, ref scrollPos, rect2, true);
            ls.ColumnWidth = rect2.width ;
            ls.Begin(rect2);
            ls.CheckboxLabeled("allowAllJobs".Translate().CapitalizeFirst(), ref allPossibleJobs, null);
            for (int num = keys.Count - 1; num >= 0; num--)
            {
                bool test = jobTypeList[keys[num]];
                if (DefDatabase<WorkTypeDef>.GetNamedSilentFail(keys[num]) == null)
                {
                    jobTypeList.Remove(keys[num]);
                }
                else
                {
                    ls.CheckboxLabeled("SettingEnable".Translate(keys[num].ToString()), ref test);
                    jobTypeList[keys[num]] = test;
                }
            }

            ls.End();
            Widgets.EndScrollView();
            base.Write();
        }
    }
}
