using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileData))]
public class ScriptableObjectSpriteEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        TileData scriptableObject = (TileData)target;

        if (scriptableObject.TileSprite != null)
        {
            EditorGUILayout.Space();
            var sprite = scriptableObject.TileSprite;
            // assume "sprite" is your Sprite object
            var croppedTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                    (int)sprite.textureRect.y,
                                                    (int)sprite.textureRect.width,
                                                    (int)sprite.textureRect.height);
            croppedTexture.SetPixels(pixels);
            croppedTexture.Apply();
            // Display the sprite image
            EditorGUILayout.LabelField("Sprite Image:");
            Rect rect = GUILayoutUtility.GetAspectRect((float)scriptableObject.TileSprite.texture.width / scriptableObject.TileSprite.texture.height);
            EditorGUI.DrawPreviewTexture(rect, croppedTexture, null, ScaleMode.ScaleToFit);
        }
    }
}
