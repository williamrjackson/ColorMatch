using UnityEngine;
using UnityEngine.UI;

public class ColorMatcher : MonoBehaviour 
{
	public Color matchColor = Color.red;
    [Range(0, 1)]
	public float range = .15f;
    [Range(-1, 1)]
    public float saturationOffset = 0f;
    [Range(-1, 1)]
    public float valueOffset = 0f;
    public bool realTime = false;
	public Image sourceImage;
	public Image targetImage;

    private Color[] basicColors =
    {
        Color.black,
        Color.blue,
        Color.cyan,
        Color.gray,
        Color.green,
        Color.magenta,
        Color.red,
        Color.white,
        Color.yellow
    };

    public void Start()
    {
        // Create new texture to write to. Match properties of source image.
		targetImage.sprite = Sprite.Create(new Texture2D(sourceImage.sprite.texture.width, sourceImage.sprite.texture.height), sourceImage.sprite.rect, Vector2.zero);
        targetImage.preserveAspect = true;
    }

    public void Update()
    {
        // If RealTime is checked, do this every frame
        if (realTime)
            MatchColorsInImage();
    }

	public void MatchColorsInImage()
	{
        // Go through each pixel in the source image
        for (int x = 0; x < sourceImage.sprite.texture.width; x++)
        {
            for (int y = 0; y < sourceImage.sprite.texture.height; y++)
            {
                // Get the pixel color from the source
                Color sourceColor = sourceImage.sprite.texture.GetPixel(x, y);
                // If it's in range, set the coresponding pixel in the destination black, otherwise white.
                Color destColor = ColorDifference(matchColor, sourceColor, saturationOffset, valueOffset) < range ? matchColor : Color.white;
                targetImage.sprite.texture.SetPixel(x, y, destColor);
            }
        }
        // Apply modified texture
        targetImage.sprite.texture.Apply();
	}

    public void ConvertToUnityColors()
    {
        // Go through each pixel in the source image
        for (int x = 0; x < sourceImage.sprite.texture.width; x++)
        {
            for (int y = 0; y < sourceImage.sprite.texture.height; y++)
            {
                Color sourceColor = sourceImage.sprite.texture.GetPixel(x, y);

                // Iterate over the list of colors, saving the closest to the source
                Color destColor = Color.white;
                float lowestDistance = 3f;
                foreach (Color listColor in basicColors)
                {
                    float thisDiff = ColorDifference(listColor, sourceColor);
                    if (thisDiff < lowestDistance)
                    {
                        lowestDistance = thisDiff;
                        destColor = listColor;
                    }
                }
                // set the coresponding pixel in the destination to the closest list color.
                targetImage.sprite.texture.SetPixel(x, y, destColor);
            }
        }
        // Apply modified texture
        targetImage.sprite.texture.Apply();
    }

    private float ColorDifference(Color targetColor, Color testColor, float satOffset = 0, float valOffset = 0)
	{
        // https://en.wikipedia.org/wiki/Euclidean_distance
        // https://www.codeproject.com/Articles/1172815/Finding-Nearest-Colors-using-Euclidean-Distance

        if (satOffset + valOffset != 0)
        {
            targetColor = StripSV(targetColor, satOffset, valOffset);
            testColor = StripSV(testColor, satOffset, valOffset);
        }

        float redDiff = testColor.r - targetColor.r;
		float greenDiff = testColor.g - targetColor.g;
		float blueDiff = testColor.b - targetColor.b;

        float totalDiff = redDiff * redDiff + greenDiff * greenDiff + blueDiff * blueDiff;
        return totalDiff / 3;
	}

    private Color StripSV(Color inColor, float sOffset, float vOffset)
    {
        float colorH;
        float colorS;
        float colorV;

        Color.RGBToHSV(inColor, out colorH, out colorS, out colorV);

        return Color.HSVToRGB(colorH, Mathf.Clamp01(colorS + sOffset), Mathf.Clamp01(colorV + vOffset));
    }
}
