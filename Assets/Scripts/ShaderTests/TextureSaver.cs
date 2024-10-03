

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class TextureSaver : MonoBehaviour
{
    public Texture2D[] textures; // Array de texturas que se asignarán desde el editor.
    public string assetPath = "Assets/Textures/MyTextureArray.asset"; // Ruta donde se guardará el asset.

    // Método para crear y guardar el Texture2DArray como un asset.
    [ContextMenu("Create Texture2DArray Asset")]
    public void CreateTexture2DArrayAsset()
    {
        if (textures.Length == 0)
        {
            Debug.LogError("No hay texturas asignadas.");
            return;
        }

        // Crear un Texture2DArray con las dimensiones y formato de la primera textura.
        Texture2DArray textureArray = new Texture2DArray(
            textures[0].width,
            textures[0].height,
            textures.Length,
            textures[0].format,
            textures[0].mipmapCount > 1
        );

        // Rellenar el Texture2DArray con las texturas.
        for (int i = 0; i < textures.Length; i++)
        {
            for (int mip = 0; mip < textures[i].mipmapCount; mip++)
            {
                Graphics.CopyTexture(textures[i], 0, mip, textureArray, i, mip);
            }
        }

        // Guardar el Texture2DArray como un asset en el proyecto.
        AssetDatabase.CreateAsset(textureArray, assetPath);
        AssetDatabase.SaveAssets();

        Debug.Log("Texture2DArray guardado como asset en " + assetPath);
    }
}
#endif