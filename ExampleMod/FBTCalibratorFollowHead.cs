using System.Runtime.CompilerServices;

using FrooxEngine;
using FrooxEngine.FinalIK;

using Elements.Core;

using HarmonyLib;
using ResoniteModLoader;

namespace FBTCalibratorFollowHead;
//More info on creating mods can be found https://github.com/resonite-modding-group/ResoniteModLoader/wiki/Creating-Mods
public class FBTCalibratorFollowHead : ResoniteMod {
	internal const string VERSION_CONSTANT = "1.0.2"; //Changing the version here updates it in all locations needed
	public override string Name => "FBTCalibratorFollowHead";
	public override string Author => "ModernBalloonie";
	public override string Version => VERSION_CONSTANT;
	public override string Link => "https://github.com/ModernBalloonie/FBTCalibratorFollowHead";

	[AutoRegisterConfigKey]
	private static readonly ModConfigurationKey<bool> MOD_ENABLED = new("modEnabled", "Mod enabled", () => true);

	[Range(-0.25f, 0.25f)]
	[AutoRegisterConfigKey]
	private static readonly ModConfigurationKey<float> FORWARD_BACK_OFFSET = new("fbOffset", "Forward Back Offset (Changes the offset of calibration reference)", () => 0f);

	public override void OnEngineInit() {
		Harmony harmony = new("ModernBalloonie.FBTCalibratorFollowHead");
		harmony.PatchAll();
	}

	[HarmonyPatch(typeof(FullBodyCalibrator), "OnCommonUpdate")]
	class FullBodyCalibrator_OnCommonUpdate_Patch {
		static void Postfix(FullBodyCalibrator __instance) {

			if (__instance.CalibratingPose == true && MOD_ENABLED.Value) {

				// Yeah this is probably jank, but it works.

				VRIKAvatar vrIK = __instance.Slot.GetComponentInChildren<VRIKAvatar>();

				VRIK vrIKSolver = __instance.Slot.GetComponentInChildren<VRIK>();

				vrIKSolver.Solver.DefaultRootPosition.Value = float3.Zero;
				vrIKSolver.Solver.DefaultRootRotation.Value = floatQ.Identity;


				Slot calibRefSlot = vrIK.Slot;

				Slot head = __instance.Slot.FindChildInHierarchy("Visual - Head");

				float3 headDir = head.Parent.GlobalDirectionToLocal(head.Forward).x_z.Normalized;

				vrIK.Slot.LocalPosition = new float3 (head.LocalPosition.X,0f,head.LocalPosition.Z) + headDir * FORWARD_BACK_OFFSET.Value; // Offset to move forward and backwards to tweak tracking position

				vrIK.Slot.LocalRotation = floatQ.LookRotation(headDir,new float3(0f,1f,0f));

			}
		}
	}
}
