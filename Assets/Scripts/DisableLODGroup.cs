// using UnityEngine;
// using UnityEditor;
// using UnityEditor.SceneManagement;

// public class DisableLODGroup : EditorWindow
// {
//     [MenuItem("Window/Disable LOD Groups", false, priority = 30)]

//     public static void DisableAllLODGroups()
//     {
//         // Find all LODGroup components in the scene
//         LODGroup[] lodGroups = FindObjectsOfType<LODGroup>();

//         // Iterate through each LODGroup and disable the component
//         foreach (LODGroup lodGroup in lodGroups)
//         {
//             lodGroup.enabled = false;
//             // Mark the scene as dirty to ensure the change is saved
//             EditorUtility.SetDirty(lodGroup);
//         }

//         // Save the scene to apply the changes
//         EditorSceneManager.MarkAllScenesDirty();
//         Debug.Log("All LOD Groups have been disabled.");
//     }
// }
