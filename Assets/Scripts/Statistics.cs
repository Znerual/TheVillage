using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Statistics{

    public static List<HausController> getAllBuildingsFromType(string tag)
    {
        List<HausController> result = new List<HausController>();
        foreach (MonoBehaviour script in GameController.Instance.getScripts())
        {
            if (script.GetType() == typeof(HausController)) {        
                if (script.gameObject.tag == tag)
                {
                    result.Add((HausController) script);
                }
            }
        }
        return result;
    }
    public static List<character> getAllCharacterInBuildingsFromType(string tag)
    {
        List<character> result = new List<character>();
        foreach (HausController building in getAllBuildingsFromType(tag))
        {
            result.AddRange(building.getCharactersInside());
        }
        return result;
    }
    public static int getAverageAge(List<character> people)
    {
        int sum = 1;
        int i;
        for (i = 0; i < people.Count;i++) 
        {
            sum += people[i].getAge();
        }
        return (int)((float)(sum) / (i + 1));
    }
    public static int getAverageIQ(List<character> people)
    {
        int sum = 0;
        int i;
        for (i = 0; i < people.Count; i++)
        {
            sum += people[i].getIQ();
        }
        return (int)((float)(sum) / (i + 1));
    }
}
