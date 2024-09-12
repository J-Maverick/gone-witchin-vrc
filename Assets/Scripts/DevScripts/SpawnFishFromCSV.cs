using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpawnFishFromCSV : MonoBehaviour
{
    public string csvFileName = "D:/Users/Joey/Downloads/NewFish.csv";

    [ContextMenu("Spawn Fish")]
    public void SpawnFish() {
        string[] lines = System.IO.File.ReadAllLines(csvFileName);
        foreach (string line in lines) {
            GameObject empty = new GameObject();
            empty.transform.SetParent(transform);
            if (line.Contains("- ")) {
                string[] split = line.Split("- ");
                empty.name = split[1];
            }
            else {
                empty.name = "###########" + line.ToUpper() + "###########";
                empty.SetActive(false);
            }
        }
}

}