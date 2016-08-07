using UnityEngine;
using System.Collections;

public class PostRenderer : MonoBehaviour {

    public bool enabled = true;

    private Material mat;

    void Start()
    {
        var shader = Shader.Find("Hidden/CRT");
        mat = new Material(shader);
        mat.hideFlags = HideFlags.HideAndDontSave;
    }

    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Vector3 screenSize = Camera.current.ViewportToScreenPoint(new Vector3(1.1f, 1.1f, 0f));

        mat.SetFloat("_GlobalTime", Time.time);
        mat.SetVector("_ScreenSize", new Vector4(screenSize.x, screenSize.y, 0f, 0f));
        Graphics.Blit(source, destination, mat);
    }

    // Will be called from camera after regular rendering is done.
    /*public void OnPostRender()
    {
        if (!enabled)
            return;

        if (!mat)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things. In this case, we just want to use
            // a blend mode that inverts destination colors.			
            var shader = Shader.Find("Hidden/CRT");
            mat = new Material(shader);
            mat.hideFlags = HideFlags.HideAndDontSave;
            // Set blend mode to invert destination colors.

            Vector3 screenSize = Camera.current.ViewportToScreenPoint(new Vector3(1.1f, 1.1f, 0f));
            Debug.Log(screenSize);

            mat.SetFloat("_GlobalTime", Time.time);
            mat.SetVector("_ScreenSize", new Vector4(screenSize.x, screenSize.y, 0f, 0f));
        }

        GL.PushMatrix();
        GL.LoadOrtho();

        // activate the first shader pass (in this case we know it is the only pass)
        mat.SetPass(0);
        // draw a quad over whole screen
        GL.Begin(GL.QUADS);
        GL.Vertex3(0, 0, 0);
        GL.Vertex3(1, 0, 0);
        GL.Vertex3(1, 1, 0);
        GL.Vertex3(0, 1, 0);
        GL.End();

        GL.PopMatrix();
    }*/
}
