using GameNetcodeStuff;
using HarmonyLib;

namespace LethalHands.Patches
{
    [HarmonyPatch(typeof(VehicleController))]
    internal class VehicleControllerPatch
    {
        [HarmonyPatch("TakeControlOfVehicle")]
        [HarmonyPrefix]
        static void PreTakeControlOfVehicle()
        {
            LethalHands.Instance.SquareDown(false);
        }

        [HarmonyPatch("SetPassengerInCar")]
        [HarmonyPrefix]
        static void PreSetPassengerInCar(PlayerControllerB player)
        {
            if(player == GameNetworkManager.Instance.localPlayerController) LethalHands.Instance.SquareDown(false);
        }
    }
}
