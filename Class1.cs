using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;


namespace RoomApparels
{
//	[StaticConstructorOnStartup]
	public class Main
	{
		static Main()
		{
			new Harmony("PrisonSelector.Mod").PatchAll();
		}
	}
}
