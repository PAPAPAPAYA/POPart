//using UnityEditor;
//using UnityEngine;

//public class ClearPlayerPrefsEditor : Editor
//{
//    [MenuItem("Tools/Clear Player Preferences")]
//    public static void ClearPlayerPrefs()
//    {
//        if (EditorUtility.DisplayDialog("Clear Player Preferences",
//            "Are you sure you want to clear all player preferences? This action cannot be undone.", "Yes", "No"))
//        {
//            PlayerPrefs.DeleteAll();
//            PlayerPrefs.Save();
//            Debug.Log("Player preferences cleared.");
//        }
//    }
//}