using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TextAnimationScript : MonoBehaviour
{
    public TMP_Text textComponent;
    public int animationNumber=1;
    public float wobbleStrength = 10f;
    public float wobbleSpeed = 2f;


    // Update is called once per frame
    void Update()
    {


        if (textComponent != null)
        {
            textComponent.ForceMeshUpdate();
            var textInfo = textComponent.textInfo;
            if (textInfo != null)
            {
                for (int i = 0; i < textInfo.characterCount; i++)
                {
                    var charInfo = textInfo.characterInfo[i];
                    if (!charInfo.isVisible)
                    {
                        continue;
                    }

                    var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
                    Vector3 center = (verts[charInfo.vertexIndex] + verts[charInfo.vertexIndex + 2]) / 2;
                    float scale = 1f + Mathf.Sin(Time.time * wobbleSpeed + i * 0.5f) * wobbleStrength;
                    for (int j = 0; j < 4; j++)
                    {
                        var orig = verts[charInfo.vertexIndex + j];
                        if (animationNumber == 1)
                            verts[charInfo.vertexIndex + j] = orig + new Vector3(Mathf.Sin(Time.time * wobbleSpeed + orig.x * 0.01f) * wobbleStrength, 0, 0);
                        else
                            if (animationNumber == 2)
                            verts[charInfo.vertexIndex + j] = orig + new Vector3(0, Mathf.Sin(Time.time * wobbleSpeed + orig.x * 0.01f) * wobbleStrength, 0);
                        else
                            if (animationNumber == 3)
                            verts[charInfo.vertexIndex + j] = orig + new Vector3(0, Mathf.Abs(Mathf.Sin(Time.time * wobbleSpeed + i * 0.5f)) * wobbleStrength, 0);
                        else
                            if (animationNumber == 4)
                            verts[charInfo.vertexIndex + j] = center + (verts[charInfo.vertexIndex + j] - center) * scale;
                    }

                }

                for (int i = 0; i < textInfo.meshInfo.Length; i++)
                {
                    var meshInfo = textInfo.meshInfo[i];
                    meshInfo.mesh.vertices = meshInfo.vertices;
                    textComponent.UpdateGeometry(meshInfo.mesh, i);
                }
            }
        }

        return;

    }
}
