// COMP30019 - Graphics and Interaction
// (c) University of Melbourne, 2022

using UnityEngine;

public class Image : MonoBehaviour
{
    [SerializeField] private int imageWidth;
    [SerializeField] private int imageHeight;
    [SerializeField] private MeshRenderer outputTo;
    
    private bool _dirty;

    private Color?[,] _image;
    private Texture2D _imageTexture;

    public int Width => this.imageWidth;
    public int Height => this.imageHeight;

    private void Awake()
    {
        this._image = new Color?[this.imageWidth, this.imageHeight];
        this._imageTexture = new Texture2D(this.imageWidth, this.imageHeight)
        {
            filterMode = FilterMode.Point
        };

        // GeneratePixelGrid();
        GenerateImage();
    }

    private void Update()
    {
        // The dirty flag ensures we only re-upload the image if it changes.
        // Otherwise, re-uploading an image each frame is expensive!
        if (this._dirty)
        {
            this._imageTexture.Apply();
            this._dirty = false;
        }
    }

    public void SetPixel(int x, int y, Color color)
    {
        this._imageTexture.SetPixel(x, y, color);
        this._dirty = true; // Re-generate next frame.
    }

    private void GenerateImage()
    {
        for (var y = 0; y < this.imageHeight; y++)
        for (var x = 0; x < this.imageWidth; x++)
            this._imageTexture.SetPixel(x, y, this._image[x, y] ?? new Color(0f, 0f, 0f, 0f));
        this._imageTexture.Apply();

        this.outputTo.material.mainTexture = this._imageTexture;
        this.outputTo.enabled = true;
    }

    private void GeneratePixelGrid()
    {
        var output = GetComponentInChildren<LinesGenerator>();
        if (!output) return;
        
        // Note that we are working with normalised coordinates here as the
        // underlying object is *scaled* according to image dimensions/FOV.
        var origin = new Vector3(-0.5f, -0.5f);

        // Vertical pixel grid lines
        for (var x = 0; x < this.imageWidth + 1; x++)
        {
            var xt = (float)x / this.imageWidth;
            var a = origin + Vector3.right * xt;
            var b = origin + Vector3.right * xt + Vector3.up;
            
            output.Line(a, b, Color.red);
        }

        // Horizontal pixel grid lines
        for (var y = 0; y < this.imageHeight + 1; y++)
        {
            var yt = (float)y / this.imageHeight;
            var a = origin + Vector3.up * yt;
            var b = origin + Vector3.up * yt + Vector3.right;
            
            output.Line(a, b, Color.red);
        }
    }
}
