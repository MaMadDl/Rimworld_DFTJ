using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmorRacks.Things;
using RimWorld;
using Verse;
using UnityEngine;
using Verse.AI;

namespace RoomApparels
{
    class CompRoomApparelsEnabler : ThingComp
    {
        public bool Enabled;
        public bool setAllJobs;
        public Dictionary<string, bool> skillsInComp;

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            // check for general checks
            if (!(this.parent is ArmorRack) || this.parent.Faction != Faction.OfPlayer)
            {
                yield break;
            }
            //check for object specific checks
            ArmorRack rack;
            if( (rack=this.parent.SpawnedParentOrMe as ArmorRack) != null  && this.parent != rack )
            {
                yield break;
            }
            //Change this Later to get list of WorkTypes In the room
            RoomApparelGizmoEnabler command_Toggle = new RoomApparelGizmoEnabler();
            command_Toggle.hotKey = KeyBindingDefOf.Command_TogglePower;
            command_Toggle.icon = TexCommand.GatherSpotActive;
            command_Toggle.isActive = (() => Enabled);
            command_Toggle.setSkill(ref skillsInComp);
            command_Toggle.defaultLabel = "Use This For Jobs".CapitalizeFirst();
            command_Toggle.activateIfAmbiguous = false;            
            command_Toggle.toggleAction = () =>
             {
                 //for now change to int later
                 Enabled = !Enabled;
                 if (Enabled) 
                 {   
                     var assignedOwner = this.parent.AllComps.OfType<CompAssignableToPawn>().First(); 
                     foreach(var pawn in assignedOwner.AssignedPawnsForReading)
                     {
                         assignedOwner.TryUnassignPawn(pawn);
                     }
                 }
             };
            yield return command_Toggle;
            yield break;
        }

    }

    class RoomApparelGizmoEnabler : Command_Toggle
    {
        private Dictionary<string, bool> enabledSkillList;
        public void setSkill(ref Dictionary<string, bool> skillDict)
        {
            if (skillDict == null) {
                skillDict = new Dictionary<string, bool>();
            }
            enabledSkillList = skillDict;
        }

        public override IEnumerable<FloatMenuOption> RightClickFloatMenuOptions
        {
            get {
                foreach (var skill in DefDatabase<SkillDef>.AllDefsListForReading)
                {
                    string key = skill.ToString();
                    yield return new FloatMenuOption(skill.LabelCap, () => {

                        if (enabledSkillList.ContainsKey(key))
                            enabledSkillList[key] = !enabledSkillList[key];
                        else
                            enabledSkillList[key] = true;
                            
                    }
                    , ContentFinder<Texture2D>.Get("Check128")
                    , (enabledSkillList.TryGetValue(key) ? Color.green : Color.red)
                    );
                }
                yield break;
             }
        }

    }
}
