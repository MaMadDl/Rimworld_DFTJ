using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmorRacks.Things;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace RoomApparels
{
    class CompRoomApparelsEnabler : ThingComp
    {
        public bool Enabled;

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
            Command_Toggle command_Toggle = new Command_Toggle();
            command_Toggle.hotKey = KeyBindingDefOf.Command_TogglePower;
            command_Toggle.icon = TexCommand.GatherSpotActive;
            command_Toggle.isActive = (() => Enabled);
            command_Toggle.defaultLabel = "Use This For Jobs Here".CapitalizeFirst();
            command_Toggle.activateIfAmbiguous = false;
            command_Toggle.toggleAction = () =>
             {
                 //for now change to int later
                 Enabled = !Enabled;
             };
            yield return command_Toggle;
            yield break;
        }

    }
}
