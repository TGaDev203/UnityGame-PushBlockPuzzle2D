using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class FindMissingScriptsInScene : EditorWindow
{
    //* -------------------- MENU ITEM --------------------

    [MenuItem("Tools/Find Missing Scripts in Scene")]
    public static void ShowWindow()
    {
        GetWindow(typeof(FindMissingScriptsInScene));
    }

    //* -------------------- GUI --------------------

    public void OnGUI()
    {
        if (GUILayout.Button("Find Missing Scripts in Current Scene"))
        {
            FindInScene();
        }
    }

    //* -------------------- FIND MISSING SCRIPTS --------------------

    private static void FindInScene()
    {
        int go_count = 0, components_count = 0, missing_count = 0;

        var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (var root in rootObjects)
        {
            var allChildren = root.GetComponentsInChildren<Transform>(true);
            foreach (var t in allChildren)
            {
                go_count++;
                var components = t.gameObject.GetComponents<Component>();

                for (int i = 0; i < components.Length; i++)
                {
                    components_count++;
                    if (components[i] == null)
                    {
                        missing_count++;
                        Debug.Log($"{t.name} has an empty script at position: {i}", t.gameObject);
                    }
                }
            }
        }

        Debug.Log($"Searched {go_count} GameObjects, {components_count} components, found {missing_count} missing");
    }
}