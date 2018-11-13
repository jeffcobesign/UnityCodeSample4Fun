using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWS;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainController : Singleton<MainController> {

	private GameObject crossSceneControl;
	public PathManager _blackHolePath;
	public splineMove _ss;
	public Transform _plane;
	public Transform _planepoint;
	private bool _moving;
	public Image _shipChatImage;
	public Text _shipChatText;
	public Image[] _saturnChatImage;
	public Text[] _saturnChatText;
	public GameObject[] _saturnAlien;

    public Image[] _moonChatImage;
    public Text[] _moonChatText;
    public GameObject[] _moonAlien;

    public Image[] _plutoChatImage;
    public Text[] _plutoChatText;
    public GameObject[] _plutoAlien;

    public Animator[] _saturnAlienAnimator;
	public AudioSource _talksound;
	public AudioSource _BGsound;
	public AudioClip _BG1;
	public AudioClip _BG2;
	public AudioClip _BG3;
	public AudioClip origenalBGM;
	public GameObject _ship;
	public GameObject _wing;
	public Transform _wholePlayer;
	public GameObject _blackhole;
	public GameObject _blacklight;
	public Transform _crashPoint;
	public Transform _crashPoint2;
	public GameObject _white;
	public GameObject _UIloadToGameScene;
	public Transform _earthPoint;
	public GameObject _light;

    public GameObject[] introduce;

    public Text _context;
	public Text _opContext;
	public GameObject _cam2;
	public Image _logo;
	public Image _gamelogo;
	public PathManager moonpathContainer;
	public PathManager plutopathContainer;
	public PathManager saturnpathContainer;
	public AudioSource _talksound2;
	public AudioSource _talksound3;

    private int whichplant = 0;
	private string ToWhichGameSceneName;
	private int ToWhichGameScene;
	// Use this for initialization


	//PressButton use collider on LMHeadMountedRig->TrackHeadSet->plane button->plane->Xxxx->Xxxx_border02<colliderOfBorder02>
	//set plane button<planeButtonVScrossSceneCarrier> ().gotoWhichPlanet : 0 Moon, 1Pluto,2 Saturn
	//start ship on plane button->planeAreYouSure-> 3dText-YES<>script3dTextYes
	//call  plane button<planeButtonVScrossSceneCarrier> ().startjourney ()  ref to here .ToMoon()
	void Start () {
		crossSceneControl = GameObject.Find ("DataCarrier");
		AudioSource[] sxsss = crossSceneControl.GetComponents<AudioSource> ();
		_BGsound = sxsss [0];

		_UIloadToGameScene.SetActive (false);
		turnOffBtn();	//ToStar btn's trigger off
		Scene sceneName = SceneManager.GetActiveScene();
		Debug.Log("Get current Scene Name on Start @ MainController.cs");
		switch (sceneName.name) 
		{
		case "TheBegining":
			{
				Debug.Log ("call the Start Function in case <TheBegining>");
				_moving = true;
				_plane.gameObject.SetActive (true);
				StartCoroutine(Opening());
				for (int SMDCBOOLS = 0; SMDCBOOLS < 3; SMDCBOOLS++) 
				{
					crossSceneControl.GetComponent<sceneManageronDCarrier>().panelButtonStates [SMDCBOOLS] = false;
					//Debug.Log ("BOOL[SMDCBOOLS] = "+sceneManageronDCarrier.panelButtonStates [SMDCBOOLS]);
				}
				//ArriveSaturn ();
				break;}
		case "CommonScene":
			{
				_moving = true;
				//skip white screen by not calling "StartCoroutine(Opening());"
				//set ship to Origin position for trip
				_white.GetComponent<Renderer>().material.DOFade (1f, 0.1f);	//Show whiteBall to VR camera
				StartCoroutine(whiteFadeOutAfterLoadScene());
				turnOnBtn ();
				_BGsound.Play ();
				break;
			}
		case "BlackHole":
			{
				//close white Screen
				_moving = false;
				_plane.gameObject.SetActive (false);	//hide control panel for view reason
				_BGsound.Play ();
				// make sure btn to stars have no function at all 

				StartCoroutine(whiteFadeOutAfterLoadScene());
				//set value to withcplant in SaturnEnd(), then call SaturnEnd()
				//whichplant=crossSceneControl.GetComponent<sceneManageronDCarrier>().backFromWhichSceneToBlackHole;
				//SaturnEnd ();
				//Debug.Log ("Load to BlackHole and set witchplant to " + whichplant);
				break;
			}
		}
	}

	void turnOffBtn(){
		GameObject[] btnNeedToTurnOff = GameObject.FindGameObjectsWithTag("NeedShutDownBtn");
		foreach (GameObject btn in btnNeedToTurnOff)
		{
			btn.GetComponent<panelBTNcolliderControl> ().enabled = false;
			btn.GetComponent<CapsuleCollider> ().enabled = false;
		}
	}

	void turnOnBtn(){
		GameObject[] btnNeedToTurnOff = GameObject.FindGameObjectsWithTag("NeedShutDownBtn");
		foreach (GameObject btn in btnNeedToTurnOff)
		{
			btn.GetComponent<panelBTNcolliderControl> ().enabled = true;
			btn.GetComponent<CapsuleCollider> ().enabled = true;
		}
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.J)) {
			GameObject panelControl = GameObject.Find ("plane button");
			panelControl.GetComponent<planeButtonVScrossSceneCarrier> ().gotoWhichPlanet = 0;
			ToMoon();
		}if (Input.GetKeyDown(KeyCode.K)) {
			GameObject panelControl = GameObject.Find ("plane button");
			panelControl.GetComponent<planeButtonVScrossSceneCarrier> ().gotoWhichPlanet = 1;
			ToPluto();
		}if (Input.GetKeyDown(KeyCode.L)) {
			GameObject panelControl = GameObject.Find ("plane button");
			panelControl.GetComponent<planeButtonVScrossSceneCarrier> ().gotoWhichPlanet = 2;
			ToSaturn();
		}
	}

	#region Load to Game Scene
	//Call function to switch to game scene, push button to set trigger in there
	//LMHeadMountedRig->TrackHeadSet->plane button->Moon->Moon_border02 Base->Moon_border02
	//On Unpress() -> DataCarrier 's function: Ready to Moon
	public void loadToGameScene(){
		//crossSceneControl.GetComponent<sceneManageronDCarrier> ().LoadGameScene (gameSceneID);
		//crossSceneControl.GetComponent<sceneManageronDCarrier> ().LoadGameScene ();
		_white.GetComponent<Renderer>().material.DOFade (1f, 0.5f);
		StartCoroutine(LoadingGameScene());
  	}

	private IEnumerator LoadingGameScene()
	{
		//fade to white, switch camera, show loading progress bar, set dataCarrier, load GameScene

		_white.GetComponent<Renderer>().material.DOFade (1f, 3f);

		_cam2.GetComponent<Camera> ().DOColor (Color.white, 0.1f);
		_UIloadToGameScene.SetActive (true);
		yield return new WaitForSeconds(1F);
		_cam2.SetActive (true);
		//_opContext.gameObject.SetActive(false);
		//show loading bar

		//ToWhichGameScene = crossSceneControl.GetComponent<sceneManageronDCarrier> ().codeNameForGameScene;
		switch(ToWhichGameScene){
		//0 moon; 1 pluto; 2 Saturn
		case 0:{ToWhichGameSceneName = "Moon";break;}
		case 1:{ToWhichGameSceneName = "Pluto";break;}
		case 2:{ToWhichGameSceneName = "Saturn";break;}

		default:{ToWhichGameSceneName = "";break;}
		}
		yield return new WaitForSeconds(0.5F);
		StartCoroutine(LoadNewScene(ToWhichGameSceneName));

	}
	IEnumerator LoadNewScene(string sceneName) {

		// Start an asynchronous operation to load the scene that was passed to the LoadNewScene coroutine.
		AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
		Slider sliderBar = _UIloadToGameScene.transform.GetChild (0).gameObject.GetComponent<Slider>();
		// While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
		while (!async.isDone)
		{
			float progress = Mathf.Clamp01(async.progress / 0.9f);
			sliderBar.value = progress;
			//loadingText.text = progress * 100f + "%";
			yield return null;

		}

	}

	#endregion

	#region Game Scene back to space scene
	private IEnumerator whiteFadeOutAfterLoadScene()
	{
		//load to commmonScene, set whiteBall show there,fade it here
		_white.GetComponent<Renderer>().material.DOFade (0f, 1f);
		_opContext.gameObject.SetActive(false);
		yield return new WaitForSeconds(1F);
		_cam2.SetActive (false);
		//
		// call black ho
	}
	#endregion

	private IEnumerator Opening()
	{
        //Set Langage
		LanguageController.Instance.SetLanguage(LanguageEnum.Chinese);

        //Canvas_StartLogo-Image(1)
        _gamelogo.DOFade (1f, 2F);
		yield return new WaitForSeconds(6F);
		_gamelogo.DOFade (0f, 2F);
		yield return new WaitForSeconds(2F);

		//_cam2 = Camer_Start,"_c" means "C"
		_cam2.GetComponent<Camera> ().DOColor (Color.white, 2f);
		yield return new WaitForSeconds(2F);

		//Canvas_StartLogo-Image
		_logo.DOFade (1f, 2F);
		yield return new WaitForSeconds(6F);
		_logo.DOFade (0f, 2F);
		yield return new WaitForSeconds(2F);
		_cam2.SetActive (false);
		//_cam2.GetComponent<Camera> ().DOColor (Color.white, 0.1f);


		_white.GetComponent<Renderer>().material.DOFade (0f, 1f);
		_BGsound.clip = origenalBGM;
		_BGsound.Play ();

        //_moving = false;

        // add text
        _opContext.text = LanguageController.Instance.GetOpenText();

		Sequence mySequence2 = DOTween.Sequence();
		mySequence2.Append(_opContext.DOFade(1f,0.1f))
			.Append(_opContext.transform.DOLocalMoveY(400,13f))
			.Append (_opContext.DOFade(0f,0.1f))
			.AppendCallback(turnOnBtn)
			.AppendCallback(GameStart)
			;
		mySequence2.Play ();

        foreach (GameObject alien in _saturnAlien)
        {
            alien.SetActive(true);
        }

    }

	private void GameStart(){
		_moving = false;

	}

	public void ResetPlane(){
		_plane.position = _planepoint.position;
		_plane.rotation = _planepoint.rotation;
	}

	public void ToSaturn(){
		
		if (!_moving){
			_ss.pathContainer = saturnpathContainer;
			_ss.speed = 500;
			_moving = true;
			_ss.StartMove ();
			whichplant = 1;
			turnOffBtn ();
			ToWhichGameScene = 2; //with planeButtonVScrossSceneCarrier
		}
	}


    public void ToMoon()
    {

        if (!_moving)
        {
			_ss.pathContainer = moonpathContainer;
			_ss.speed = 20;
            _moving = true;
            _ss.StartMove();
			whichplant = 2;
			turnOffBtn ();
			ToWhichGameScene = 0;	//with planeButtonVScrossSceneCarrier
        }
    }

    public void ToPluto()
    {

        if (!_moving)
        {
			_ss.pathContainer = plutopathContainer;
			_ss.speed = 500;
            _moving = true;
            _ss.StartMove();
            whichplant = 3;
			turnOffBtn ();
			ToWhichGameScene = 1;	//with planeButtonVScrossSceneCarrier
        }
    }

	#region FastArrival Group
	public void ToMoonFast()
	{
		Debug.Log ("ToMoonFast");
		if (!_moving)
		{
			_ss.pathContainer = moonpathContainer;
			_ss.speed = 10000;
			_moving = true;
			_ss.StartMove();
			whichplant = 2;

			ToWhichGameScene = 0;	//with planeButtonVScrossSceneCarrier
		}
	}

	public void ToPlutoFast()
	{
		Debug.Log ("ToPlutoFast");
		if (!_moving)
		{
			_ss.pathContainer = plutopathContainer;
			_ss.speed = 10000;
			_moving = true;
			_ss.StartMove();
			whichplant = 3;

			ToWhichGameScene = 1;	//with planeButtonVScrossSceneCarrier
		}
	}

	public void ToSaturnFast(){
		Debug.Log ("ToSaturnFast");
		if (!_moving){
			_ss.pathContainer = saturnpathContainer;
			_ss.speed = 10000;
			_moving = true;
			_ss.StartMove ();
			whichplant = 1;

			ToWhichGameScene = 2; //with planeButtonVScrossSceneCarrier
		}
	}

	public void fastToBlackHole(){
		StartCoroutine(CallSaturnEnd());
	}

	IEnumerator CallSaturnEnd()
	{
		yield return new WaitForSeconds (2f);
		SaturnEnd ();
		//StopCoroutine (CallSaturnEnd ());
	}

	#endregion

    public void ArriveSaturn(){
		ResetPlane ();
		StartTalkingSaturn ();
	}

	public void Light(){
        bool isLight = _light.activeInHierarchy;
        _light.SetActive(!isLight);
        /*switch (whichplant)
        {
            case 1:
                break;
            case 2:
                //introduce[1].SetActive(isLight);
                break;
            case 3:
                //introduce[2].SetActive(isLight);
                break;
        }*/
	}



	public void InforntBlackHole(){
		_blackhole.SetActive (true);
		_BGsound.clip = _BG2;
		_BGsound.Play ();
		Sequence mySequence = DOTween.Sequence();
        string[] blackHoleText = LanguageController.Instance.GetBlackHoleText();
		//effect and dialuge
        mySequence
            .Append(_wholePlayer.DOShakeRotation(4f))
            .Append(_wholePlayer.DOShakePosition(4f))
            .Append(_shipChatImage.DOFade(1f, 0.5f))
            .Append(_shipChatText.DOText(blackHoleText[0], 3f))
            .Append(_shipChatText.DOText("", 0.1f))
            .Append(_shipChatText.DOText(blackHoleText[1], 3f))
            .Append(_shipChatText.DOText("", 0.1f))
            .Append(_shipChatText.DOText(blackHoleText[2], 3f))
            .Append(_shipChatText.DOText("", 0.1f))
            .Append(_shipChatImage.DOFade(0f, 0.5f))

            ;               
		mySequence.Play ();

		//set ship to CrashPoint
		Sequence mySequence2 = DOTween.Sequence();
		mySequence2.Append(_wholePlayer.DOMove(_crashPoint2.position,20f))
			.Append (_wholePlayer.DOMove(_blackhole.transform.position,0.5f))
			.AppendCallback(IntoBlackHole)
			;
		mySequence2.Play ();

	}

	public void IntoBlackHole(){
		_wing.SetActive (false);
		_ship.SetActive (false);
		_blacklight.SetActive (true);
		InBlackHole();

	}

	public void InBlackHole(){
		Sequence mySequence = DOTween.Sequence();
		mySequence
			.Append (_wholePlayer.DOShakeRotation (2f))
			.Append (_wholePlayer.DOShakePosition (2f))
			.Append (_white.GetComponent<Renderer> ().material.DOFade (1f, 1f))
			.AppendCallback(BackToEarth);
		mySequence.Play ();
		//yield return new WaitForSeconds(5F);

	}

	public void BackToEarth(){
		_blacklight.SetActive (false);
		_BGsound.clip = _BG3;
		_BGsound.Play ();
		_wholePlayer.position = _earthPoint.position;
        _wholePlayer.eulerAngles = new Vector3(0,0,0);
		_blackhole.SetActive (false);
		_wing.SetActive (true);
		Sequence mySequence = DOTween.Sequence();
		mySequence
			.Append (_white.GetComponent<Renderer>().material.DOFade (0f, 1f))
			.Append (_shipChatImage.DOFade (1f, 0.5f))
			.Append (_shipChatText.DOText (LanguageController.Instance.GetBlackHoleText()[3], 3f))
			.Append (_shipChatText.DOText ("", 0.1f))
			.Append (_shipChatImage.DOFade (0f, 0.5f))
			.Append (_context.DOFade (1f, 0.5f))
			;
		mySequence.Play ();
		Invoke ("EndGame", 10f);
	}

	public void EndGame(){
		Sequence mySequence = DOTween.Sequence();
		mySequence.Append (_context.DOFade (0f, 0.5f))
			.Append (_white.GetComponent<Renderer>().material.DOFade (1f, 1f))
			.AppendCallback(ShowLogo)
			;
		mySequence.Play ();
    }

	public void ShowLogo(){
        _cam2.SetActive (true);
		_logo.DOFade (1f, 5f);
        _ship.SetActive(true);
        _white.GetComponent<Renderer>().material.DOFade(0f, 5f);
        Invoke ("HideLogo", 14f);
	}

	public void HideLogo(){
		_logo.DOFade (0f, 5f).OnComplete (Close);        
        /*_ship.SetActive(true);
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(_white.GetComponent<Renderer>().material.DOFade(0f, 5f))            
            .AppendCallback(SetCamera2)
            ;
        mySequence.Play();
        _moving = false;*/
    }

    public void SetCamera2()
    {
        _cam2.SetActive(false);
    }

	public void Close(){
		Application.Quit ();
	}

	public void StartTalkingSaturn(){
		//1 moon, 2 Saturn, 3 pluto
		foreach (Animator alienAnimator in _saturnAlienAnimator) {
			alienAnimator.SetTrigger ("Talk");
		}
		_light.SetActive (true);

        switch (whichplant)
        {
            case 1:
                string[] Saturntext = LanguageController.Instance.GetSaturnText();
                Sequence mySequence1 = DOTween.Sequence();
                mySequence1.Append(_shipChatImage.DOFade(1f, 0.5f))
                .Append(_shipChatText.DOText(Saturntext[0], 3f))
                .Append(_shipChatText.DOText("", 0.1f))
                .Append(_shipChatText.DOText(Saturntext[1], 3f))
                .Append(_shipChatText.DOText("", 0.1f))
                .Append(_shipChatImage.DOFade(0f, 0.5f))
                .Append(_saturnChatImage[0].DOFade(1f, 0.5f))
                .Append(_talksound.DOFade(1f, 0.1f))
                .Append(_saturnChatText[0].DOText(Saturntext[2], 3f, true, ScrambleMode.Numerals))
                .Append(_talksound.DOFade(0f, 0.1f))
                .Append(_saturnChatText[0].DOText("", 0.1f))
                .Append(_saturnChatImage[0].DOFade(0f, 0.5f))
                .Append(_shipChatImage.DOFade(1f, 0.5f))
                .Append(_shipChatText.DOText(Saturntext[3], 3f))
                .Append(_shipChatImage.DOFade(2f, 0.5f))
                .Append(_saturnChatImage[1].DOFade(1f, 0.5f))
                .Append(_talksound.DOFade(1f, 0.1f))
                .Append(_saturnChatText[1].DOText(Saturntext[4], 3f, true, ScrambleMode.Numerals))
                .Append(_talksound.DOFade(0f, 0.1f))
                .Append(_saturnChatText[1].DOText("", 0.1f))
                .Append(_saturnChatImage[1].DOFade(0f, 0.5f))
				.AppendCallback(loadToGameScene);
				//.AppendCallback(SaturnEnd);
                mySequence1.Play();
                break;
            case 2:
                //introduce[1].SetActive(true);
                string[] moontext = LanguageController.Instance.GetMoonText();
                Sequence mySequence2 = DOTween.Sequence();
                mySequence2.Append(_shipChatImage.DOFade(1f, 0.5f))
                .Append(_shipChatText.DOText(moontext[0], 3f))
                .Append(_shipChatText.DOText("", 0.1f))
                .Append(_shipChatText.DOText(moontext[1], 3f))
                .Append(_shipChatText.DOText("", 0.1f))
                .Append(_shipChatText.DOText(moontext[2], 3f))
                .Append(_shipChatText.DOText("", 0.1f))
                .Append(_shipChatImage.DOFade(0f, 0.5f))

                .Append(_moonChatImage[0].DOFade(1f, 0.5f))
				.Append(_talksound2.DOFade(1f, 0.1f))
                .Append(_moonChatText[0].DOText(moontext[3], 3f, true, ScrambleMode.Numerals))
                .Append(_moonChatText[0].DOText("", 0.1f))
                .Append(_moonChatImage[0].DOFade(0f, 0.5f))
                .Append(_talksound2.DOFade(0f, 0.1f))

                .Append(_shipChatImage.DOFade(1f, 0.5f))
                .Append(_shipChatText.DOText(moontext[4], 3f))
                .Append(_shipChatText.DOText("", 0.1f))
                .Append(_shipChatImage.DOFade(0f, 0.5f))

                .Append(_talksound2.DOFade(1f, 0.1f))
                .Append(_moonChatImage[1].DOFade(1f, 0.5f))
				.Append(_moonChatText[1].DOText(moontext[5], 3f, true, ScrambleMode.Numerals))                
                .Append(_moonChatText[1].DOText("", 0.1f))
                .Append(_moonChatImage[1].DOFade(0f, 0.5f))
                .Append(_moonChatImage[0].DOFade(1f, 0.5f))
                .Append(_moonChatText[0].DOText(moontext[6], 3f, true, ScrambleMode.Numerals))
                .Append(_moonChatText[0].DOText("", 0.1f))
                .Append(_moonChatImage[0].DOFade(0f, 0.5f))

                .Append(_talksound2.DOFade(0f, 0.1f))

                .Append(_shipChatImage.DOFade(1f, 0.5f))
                .Append(_shipChatText.DOText(moontext[7], 3f))
                .Append(_shipChatText.DOText("", 0.1f))
                .Append(_shipChatImage.DOFade(0f, 0.5f))
				.AppendCallback(loadToGameScene);
                //.AppendCallback(SaturnEnd);
                mySequence2.Play();
                break;

            case 3:
                string[] plutotext = LanguageController.Instance.GetPlutoText();

                //introduce[2].SetActive(true);
                Sequence mySequence3 = DOTween.Sequence();
                mySequence3.Append(_shipChatImage.DOFade(1f, 0.5f))
                .Append(_shipChatText.DOText(plutotext[0], 3f))
                .Append(_shipChatText.DOText("", 0.1f))
                .Append(_shipChatText.DOText(plutotext[1], 3f))
                .Append(_shipChatText.DOText("", 0.1f))
                .Append(_shipChatImage.DOFade(0f, 0.5f))
                .Append(_plutoChatImage[0].DOFade(1f, 0.5f))                                
				.Append(_talksound3.DOFade(1f, 0.1f))
                .Append(_plutoChatText[0].DOText(plutotext[2], 3f, true, ScrambleMode.Numerals))
				.Append(_talksound3.DOFade(0f, 0.1f))
				.Append(_plutoChatText[0].DOText("", 0.1f))
				.Append(_plutoChatImage[0].DOFade(0f, 0.5f)) 
                .Append(_plutoChatImage[1].DOFade(1f, 0.5f))
				.Append(_talksound3.DOFade(1f, 0.1f))
                .Append(_plutoChatText[1].DOText(plutotext[3], 3f, true, ScrambleMode.Numerals))
				.Append(_talksound3.DOFade(0f, 0.1f))
				.Append(_plutoChatText[1].DOText("", 0.1f))
                .Append(_plutoChatImage[1].DOFade(0f, 0.5f))
                .Append(_shipChatImage.DOFade(1f, 0.5f))
                .Append(_shipChatText.DOText(plutotext[4], 3f))
				.Append(_shipChatText.DOText("", 0.1f))
                .Append(_shipChatImage.DOFade(0f, 0.5f))
                .Append(_plutoChatImage[2].DOFade(1f, 0.5f))
				.Append(_talksound3.DOFade(1f, 0.1f))
                .Append(_plutoChatText[2].DOText(plutotext[5], 3f, true, ScrambleMode.Numerals)) 
				.Append(_plutoChatText[2].DOText("", 0.1f))
				.Append(_plutoChatImage[2].DOFade(0f, 0.5f)) 
                .Append(_plutoChatImage[3].DOFade(1f, 0.5f))
                .Append(_plutoChatText[3].DOText(plutotext[6], 3f, true, ScrambleMode.Numerals))
				.Append(_plutoChatText[3].DOText("", 0.1f))
				.Append(_plutoChatImage[3].DOFade(0f, 0.5f))
				.Append(_talksound3.DOFade(0f, 0.1f))
                .AppendCallback(loadToGameScene);
                //.AppendCallback(SaturnEnd);
                mySequence3.Play();
                break;
        }
		
	}

	public void SaturnEnd(){
		//foreach (GameObject alien in _saturnAlien) {
		//	alien.SetActive (false);
		//}
		_light.SetActive (false);
        switch (whichplant)
        {
            case 1:
                string[] Saturntext = LanguageController.Instance.GetSaturnText();
                Sequence mySequence1 = DOTween.Sequence();
                mySequence1.Append(_shipChatImage.DOFade(1f, 0.5f))
                    .Append(_shipChatText.DOText("", 0.1f))
                    .Append(_shipChatText.DOText(Saturntext[5], 3f))
                    .Append(_shipChatText.DOText("", 0.1f))
                    .Append(_shipChatImage.DOFade(0f, 0.5f))


                    .Append(_saturnChatImage[0].DOFade(1f, 0.5f))
                    .Append(_talksound.DOFade(1f, 0.1f))
                    .Append(_saturnChatText[0].DOText(Saturntext[6], 3f, true, ScrambleMode.Numerals))
                    .Append(_talksound.DOFade(0f, 0.1f))
                    .Append(_saturnChatText[0].DOText("", 0.1f))
                    .Append(_saturnChatImage[0].DOFade(0f, 0.5f))

                    .Append(_shipChatImage.DOFade(1f, 0.5f))
                    .Append(_shipChatText.DOText(Saturntext[7], 3f))
                    .Append(_shipChatText.DOText("", 0.1f))
                    .Append(_shipChatImage.DOFade(0f, 0.5f))
                    .AppendCallback(ToBlackHole);
                ;
                mySequence1.Play();
                break;
            case 2:
                string[] moontext = LanguageController.Instance.GetMoonText();
                Sequence mySequence2 = DOTween.Sequence();
                mySequence2.Append(_shipChatImage.DOFade(1f, 0.5f))
                    .Append(_shipChatText.DOText(moontext[7], 3f))
                    .Append(_shipChatText.DOText("", 0.1f))
                    .Append(_shipChatImage.DOFade(0f, 0.5f))
                    .AppendCallback(ToBlackHole);
                ;
                mySequence2.Play();
                break;
            case 3:
                string[] plutotext = LanguageController.Instance.GetPlutoText();
                Sequence mySequence3 = DOTween.Sequence();
                mySequence3.Append(_shipChatImage.DOFade(1f, 0.5f))
                    .Append(_shipChatText.DOText(plutotext[7], 3f))
                    .Append(_shipChatText.DOText("", 0.1f))
                    .Append(_shipChatText.DOText(plutotext[8], 3f))
                    .Append(_shipChatText.DOText("", 0.1f))
                    .Append(_shipChatImage.DOFade(0f, 0.5f))
                    .AppendCallback(ToBlackHole);
                ;
                mySequence3.Play();
                break;
        }
		
	}

	public void ToBlackHole(){
		Sequence mySequence = DOTween.Sequence();
		mySequence.Append (_wholePlayer.DOLookAt(_blackhole.transform.position,3f))
			.Append (_wholePlayer.DOMove(_crashPoint.position,6f))
			.AppendCallback(InforntBlackHole)
			;
			mySequence.Play ();
	}

	public void SlowDown(){
		_ss.speed = 100f;
	}


}
