using UnityEditor;
using UnityEngine;

public class SpriteToScriptableObjectCreator
{
    [MenuItem("Assets/Create/Scriptable Object From Sprite", false, 10)]
    private static void CreateScriptableObjectFromSprite()
    {
        Object selectedObject = Selection.activeObject;

        if (selectedObject != null && selectedObject is Sprite)
        {
            Sprite selectedSprite = (Sprite)selectedObject;
            string assetPath = AssetDatabase.GetAssetPath(selectedSprite);
            string assetName = selectedSprite.name + "Object.asset";

            ScriptableObject scriptableObject = ScriptableObject.CreateInstance<TileData>();
            AssetDatabase.CreateAsset(scriptableObject, assetPath.Replace(selectedSprite.name + ".png", assetName));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Scriptable Object created from sprite: " + assetName);
        }
        else
        {
            Debug.LogWarning("Please select a sprite in the Project window.");
        }
    }

    [MenuItem("Assets/Create/Scriptable Object From Sprite", true)]
    private static bool ValidateCreateScriptableObjectFromSprite()
    {
        Object selectedObject = Selection.activeObject;
        return selectedObject != null && selectedObject is Sprite;
    }
}
