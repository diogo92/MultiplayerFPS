using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
 
public class LoadingScreen : MonoBehaviour
{
    // LoadingScreen script access
	public static LoadingScreen instance = null;

	[Header("RESOURCES")]
    public Image background;
	public Image topPanel;
	public Image downPanel;
	public Image loader;
    public Text status;
	public Slider progressBar;

	[Header("SETTINGS")]
    public float animationSpeed = 1.25f;
 
    // Scene loading process
    private AsyncOperation loadingProcess;
 
	static bool ShowProgress = true;
	public bool LoadingDone = false;
    // Load a new scene


    public static void LoadScene(string sceneName)
    {
        // If there isn't a LoadingScreen, then create a new one
        if (instance == null)
        {
			instance = Instantiate(Resources.Load<GameObject>("LoadingScreen")).GetComponent<LoadingScreen>();
			 // Don't destroy loading screen while it's loading
            DontDestroyOnLoad(instance.gameObject);
        }
         
        // Enable loading screen
        instance.gameObject.SetActive(true);
        // Start loading between scenes (Background process. That's why there is an Async)
        instance.loadingProcess = SceneManager.LoadSceneAsync(sceneName);
        // Don't switch scene even after loading is completed
        instance.loadingProcess.allowSceneActivation = false;

    }
 
	public static void ActivateLoadScreen(){
		ShowProgress = false;
		DontDestroyOnLoad(instance.gameObject);
		instance.gameObject.SetActive(true);
	}

    void Awake()
	{
		instance = this;
        // Set loading screen invisible at first (panel alpha color)
        Color c = background.color;
        c.a = 0f;
		background.color = c;

		Color c2 = topPanel.color;
		c2.a = 0f;
		topPanel.color = c2;

		Color c3 = downPanel.color;
		c3.a = 0f;
		downPanel.color = c3;

		Color c4 = loader.color;
		c4.a = 0f;
		loader.color = c4;
         
		c = status.color;
        c.a = 0f;
		status.color = c;
		gameObject.SetActive (false);
    }
 
    void Update()
    {
		if (ShowProgress)
			DoLoadingAnimWithProgress ();
		else
			DoLoadingAnimWithoutProgress ();
    }

	void DoLoadingAnimWithoutProgress(){
		// If loading is complete
		if (LoadingDone)
		{
			// Fade out
			Color c = background.color;
			c.a -= animationSpeed * Time.deltaTime;
			background.color = c;

			Color c2 = topPanel.color;
			c2.a -= animationSpeed * Time.deltaTime;
			topPanel.color = c2;

			Color c3 = downPanel.color;
			c3.a -= animationSpeed * Time.deltaTime;
			downPanel.color = c3;

			Color c4 = loader.color;
			c4.a -= animationSpeed * Time.deltaTime;
			loader.color = c4;

			c = status.color;
			c.a -= animationSpeed * Time.deltaTime;
			status.color = c;

			// If fade out is complete, then disable the object
			if (c.a <= 0)
			{
				gameObject.SetActive(false);
			}
		}
		else // If loading proccess isn't completed
		{
			// Start Fade in
			Color c = background.color;
			c.a += animationSpeed * Time.deltaTime;
			background.color = c;

			Color c2 = topPanel.color;
			c2.a += animationSpeed * Time.deltaTime;
			topPanel.color = c2;

			Color c3 = downPanel.color;
			c3.a += animationSpeed * Time.deltaTime;
			downPanel.color = c3;

			Color c4 = loader.color;
			c4.a += animationSpeed * Time.deltaTime;
			loader.color = c4;

			c = status.color;
			c.a += animationSpeed * Time.deltaTime;
			status.color = c;
		}
	}

	void DoLoadingAnimWithProgress(){
		// Update loading status

		progressBar.value = loadingProcess.progress;
		status.text = Mathf.Round(progressBar.value * 100f).ToString() + "%";

		// OLD METHOD V1.0 //
		//float progress = Mathf.Clamp01(loadingProcess.progress / .9f);
		//status.text = progress * 100f + "%";

		// If loading is complete
		if (loadingProcess.isDone)
		{
			// Fade out
			Color c = background.color;
			c.a -= animationSpeed * Time.deltaTime;
			background.color = c;

			Color c2 = topPanel.color;
			c2.a -= animationSpeed * Time.deltaTime;
			topPanel.color = c2;

			Color c3 = downPanel.color;
			c3.a -= animationSpeed * Time.deltaTime;
			downPanel.color = c3;

			Color c4 = loader.color;
			c4.a -= animationSpeed * Time.deltaTime;
			loader.color = c4;

			c = status.color;
			c.a -= animationSpeed * Time.deltaTime;
			status.color = c;

			// If fade out is complete, then disable the object
			if (c.a <= 0)
			{
				gameObject.SetActive(false);
			}
		}
		else // If loading proccess isn't completed
		{
			// Start Fade in
			Color c = background.color;
			c.a += animationSpeed * Time.deltaTime;
			background.color = c;

			Color c2 = topPanel.color;
			c2.a += animationSpeed * Time.deltaTime;
			topPanel.color = c2;

			Color c3 = downPanel.color;
			c3.a += animationSpeed * Time.deltaTime;
			downPanel.color = c3;

			Color c4 = loader.color;
			c4.a += animationSpeed * Time.deltaTime;
			loader.color = c4;

			c = status.color;
			c.a += animationSpeed * Time.deltaTime;
			status.color = c;

			// If loading screen is visible
			if (c.a >= 1)
			{
				// We're good to go. New scene is on! :)
				loadingProcess.allowSceneActivation = true;
			}
		}
	}
}