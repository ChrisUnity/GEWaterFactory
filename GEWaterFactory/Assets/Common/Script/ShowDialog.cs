
using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;

public class ShowDialog : Singleton<ShowDialog>
{

    [SerializeField]
    private Renderer iconRenderer;
    [SerializeField]
    private TextMesh messageText;
    [SerializeField]
    private GameObject progressBar;
    [SerializeField]
    private TextMesh progressBarText; 
    [SerializeField]
    private GameObject progressSlider; 
    [SerializeField]
    private TextMesh progressText;
    [SerializeField]
    private GameObject showTestWithBg;
    [SerializeField]
    private TextMesh showTestWithBgText;
    public int Progress { get; set; }
    public bool IsShowloadProgress { get; set; } 
    public bool IsShowTexMessage { get; set; } 
    protected override void Awake() 
    {
        base.Awake();
        iconRenderer.gameObject.SetActive(false);
        messageText.gameObject.SetActive(false);
        progressBar.gameObject.SetActive(false);
     
    }

    void FixedUpdate() 
    {
        if (IsShowloadProgress)
        {
            progressText.text = Progress + "%";
            progressSlider.transform.localScale =new Vector3( 0.01f*Progress,1,1);
            if (Progress >= 100)
            {
                IsShowloadProgress = false;
                Closeloading();
            }
        }      
    }
    public void ShowTexMessage(string message, float showTime = 2, Texture2D icon = null)
    {
        IsShowTexMessage = true;
        messageText.gameObject.SetActive(true);
        if (icon != null)
        {
            iconRenderer.gameObject.SetActive(true);
            iconRenderer.material.mainTexture = icon;
        }
        messageText.text = message;
        Invoke("HideMessage", showTime);
    }
    public void ShowTexWithBg(string message, float showTime = 2)
    {
        showTestWithBg.gameObject.SetActive(true); 
        showTestWithBgText.text = message;
        Invoke("HideMessage", showTime);
    }
    void HideMessage()
    {
        showTestWithBgText.text = ""; 
        showTestWithBg.gameObject.SetActive(false);
        messageText.text = "";
        messageText.gameObject.SetActive(false);
        iconRenderer.gameObject.SetActive(false);
        IsShowTexMessage = false;
    }
    public void ShowLoadingProgress(string message, Texture2D icon = null)
    {
        progressBar.gameObject.SetActive(true);
        if (icon != null)
        {
            iconRenderer.gameObject.SetActive(true);
            iconRenderer.material.mainTexture = icon;
        }
        progressBarText.text = message;
        IsShowloadProgress = true;
    }
    public void SetLoadingProgress(float progress)
    {
        Progress = (int)(progress*100);
    }
    void Closeloading() 
    {
        iconRenderer.gameObject.SetActive(false);
        progressBar.gameObject.SetActive(false);
    }

}

