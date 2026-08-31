using System.Runtime.CompilerServices;

using FrooxEngine;
using FrooxEngine.FinalIK;

using Elements.Core;

using HarmonyLib;
using ResoniteModLoader;

namespace FBTCalibratorFollowHead;
//More info on creating mods can be found https://github.com/resonite-modding-group/ResoniteModLoader/wiki/Creating-Mods
public class FBTCalibratorFollowHead : ResoniteMod {
	internal const string VERSION_CONSTANT = "1.0.0"; //Changing the version here updates it in all locations needed
	public override string Name => "FBTCalibratorFollowHead";
	public override string Author => "ModernBalloonie";
	public override string Version => VERSION_CONSTANT;
	public override string Link => "https://github.com/ModernBalloonie/FBTCalibratorFollowHead";

	public override void OnEngineInit() {
		Harmony harmony = new("com.example.FBTCalibratorFollowHead");
		harmony.PatchAll();
	}

	[HarmonyPatch(typeof(FullBodyCalibrator), "OnCommonUpdate")]
	class FullBodyCalibrator_OnCommonUpdate_Patch {
		static void Postfix(FullBodyCalibrator __instance) {

			if (__instance.CalibratingPose == true) {

				// Yeah this is probably jank, but it works.

				VRIKAvatar vrIK = __instance.Slot.GetComponentInChildren<VRIKAvatar>();

				VRIK vrIKSolver = __instance.Slot.GetComponentInChildren<VRIK>();

				vrIKSolver.Solver.DefaultRootPosition.Value = float3.Zero;
				vrIKSolver.Solver.DefaultRootRotation.Value = floatQ.Identity;


				Slot calibRefSlot = vrIK.Slot;

				Slot head = __instance.Slot.FindChildInHierarchy("Visual - Head");

				var headDir = head.Parent.GlobalDirectionToLocal(head.Forward).x_z.Normalized;



				vrIK.Slot.LocalPosition = new float3 (head.LocalPosition.X,0f,head.LocalPosition.Z);

				vrIK.Slot.LocalRotation = floatQ.LookRotation(headDir,new float3(0f,1f,0f));

			}
			else if (__instance.CalibratingPose == false) {

				

			}
		}
	}
}
