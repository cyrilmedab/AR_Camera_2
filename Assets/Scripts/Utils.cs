using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Utils : MonoBehaviour
{
    public static int RealMod(int num, int mod) => num - mod * Mathf.FloorToInt((float)num / mod);


    #region UI Animations

    public static async Task<Task> FadeSprite(Image img, float target, float fadeTime)
    {
        float start = img.color.a;

        for (float time = 0f; time < fadeTime; time += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(start, target, time / fadeTime);
            SetSpriteAlpha(img, alpha);
            await Task.Yield();
        }

        return Task.CompletedTask;
    }

    public static void SetSpriteAlpha(Image img, float alpha)
    {
        Color temp = img.color;
        temp.a = alpha;
        img.color = temp;
    }

    #endregion
}
