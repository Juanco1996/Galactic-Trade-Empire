
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public static ColorManager Instance { get; private set; }

    public Color deepSpaceBlue = new Color(0.106f, 0.122f, 0.231f);
    public Color voidBlack = new Color(0.051f, 0.051f, 0.071f);
    public Color cosmicCyan = new Color(0.219f, 0.909f, 0.909f);
    public Color galacticPurple = new Color(0.494f, 0.357f, 0.831f);
    public Color nebulaOrange = new Color(0.969f, 0.623f, 0.274f);
    public Color stellarWhite = new Color(0.918f, 0.918f, 0.918f);
    public Color energeticGreen = new Color(0.274f, 0.909f, 0.431f);
    public Color warningRed = new Color(0.909f, 0.231f, 0.231f);
    public Color mutedGrey = new Color(0.631f, 0.631f, 0.698f);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}