using UnityEngine;
using UnityEngine.UI;

public class CellManager : MonoBehaviour
{
    public Sprite xImage;
    public Sprite nImage;
    public Sprite sImage;
    public Sprite superImage;
    public Sprite shellImage;

    public GameObject shellCountText;

    public static CellManager instance;

    void Awake( )
    {
        instance = this;
    }
}