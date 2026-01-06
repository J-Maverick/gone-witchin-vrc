
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Persistence;

public class GameLoadManager : UdonSharpBehaviour
{
    public FishLog fishLog;
    public ReagentTankFiller reagentTankFiller;
    public GameObject loadingScreen;
    public bool gameLoaded = false;
    public bool fishLogLoading = false;
    public int fishLogIndex = 0;
    public int fishLogCount = 0;

    public float loadStartDelay = 10f;
    public float loadTickDelay = 0.05f;
    public float loadTickTimer = 0f;

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (player.isLocal && !gameLoaded) {
            SendCustomEventDelayedSeconds(nameof(StartLoad), loadStartDelay);
        }
    }

    public void StartLoad() {
        fishLogLoading = true;
        fishLogCount = fishLog.fishLogEntries.Length;
    }


    public void Update()
    {
        if (!gameLoaded) {
            if (fishLogLoading) {
                loadTickTimer += Time.deltaTime;
                if (loadTickTimer >= loadTickDelay) {
                    loadTickTimer = 0f;
                    if (fishLogIndex < fishLogCount) {
                        float largestCaught = 0f;
                        bool largestFound = PlayerData.TryGetFloat(Networking.LocalPlayer, fishLog.fishLogEntries[fishLogIndex].fishData.name + DataKeys.LargestCaught, out largestCaught);
                        if (largestFound) {
                            fishLog.fishLogEntries[fishLogIndex].largestCaught = largestCaught;
                            fishLog.fishLogSynced.AddFish(fishLog.fishLogEntries[fishLogIndex].fishData, fishLog.fishLogEntries[fishLogIndex].largestCaught);
                            if (Networking.LocalPlayer.isMaster) {
                                reagentTankFiller.FillTankForLoad(fishLog.fishLogEntries[fishLogIndex].fishData, fishLog.fishLogEntries[fishLogIndex].largestCaught);
                            }
                        }
                        float smallestCaught = -1f;
                        bool smallestFound = PlayerData.TryGetFloat(Networking.LocalPlayer, fishLog.fishLogEntries[fishLogIndex].fishData.name + DataKeys.LargestCaught, out smallestCaught);
                        if (smallestFound) {
                            fishLog.fishLogEntries[fishLogIndex].smallestCaught = smallestCaught;
                            fishLog.fishLogSynced.AddFish(fishLog.fishLogEntries[fishLogIndex].fishData, fishLog.fishLogEntries[fishLogIndex].smallestCaught);
                        }
                        fishLogIndex++;
                    }
                    else {
                        fishLogLoading = false;
                        gameLoaded = true;
                        fishLog.UpdateText();
                        Destroy(loadingScreen);
                    }
                }
            }
        }
    }
}
