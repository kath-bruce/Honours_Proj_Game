using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using HonsProj;

public enum TutorialPartName { COMPLETING_TASK, CREW_MEMBER_MOVEMENT, CREW_SELECTION, LOSE_SCREEN, TASK_GENERATION, WIN_SCREEN }

public struct TutorialPart
{
    public string topText;
    public string bottomText;
    public string extraText;

    public TutorialPart(string top, string bottom, string extra)
    {
        topText = top;
        bottomText = bottom;
        extraText = extra;
    }
}

public class TutorialController : MonoBehaviour
{
    [SerializeField]
    VideoClip temp;

    [SerializeField]
    List<VideoClip> tutorial_gifs;
    Dictionary<TutorialPartName, TutorialPart> other_dict;

    List<TutorialPart> tutorial_texts;
    Dictionary<TutorialPartName, VideoClip> tutorial;

    int current_tutorial_part = 0;

    [SerializeField]
    GameObject prevButton;

    [SerializeField]
    GameObject nextButton;

    [SerializeField]
    GameObject pausePanel;

    // Use this for initialization
    void Start()
    {
        tutorial = new Dictionary<TutorialPartName, VideoClip>();
        tutorial_texts = new List<TutorialPart>();

#if DEBUG
        other_dict = XmlDataLoader.GetTutorialParts(@"Assets/xml files/tutorial_texts.xml");
#else
        other_dict = XmlDataLoader.GetTutorialParts(@"xml files/tutorial_texts.xml");
#endif

        for (int i = 0; i < tutorial_gifs.Count; i++)
        {
            tutorial.Add((TutorialPartName)System.Enum.Parse(typeof(TutorialPartName), tutorial_gifs[i].name, true), tutorial_gifs[i]);
            tutorial_texts.Add(other_dict[(TutorialPartName)System.Enum.Parse(typeof(TutorialPartName), tutorial_gifs[i].name, true)]);
        }

        GetComponentInChildren<GifScript>().SwitchGif(tutorial_gifs[current_tutorial_part]);

        UpdateTutorialText();

        prevButton.SetActive(false);

        pausePanel.SetActive(false);
    }

    public void NextTutorialPart()
    {
        ++current_tutorial_part;
        prevButton.SetActive(true);

        if (current_tutorial_part >= tutorial_gifs.Count)
        {
            current_tutorial_part = tutorial_gifs.Count - 1;
        }
        else if (current_tutorial_part == tutorial_gifs.Count - 1)
        {
            nextButton.SetActive(false);
            pausePanel.SetActive(true);
            GameController.INSTANCE.FinishedTutorial();
        }

        GetComponentInChildren<GifScript>().SwitchGif(tutorial_gifs[current_tutorial_part]);

        UpdateTutorialText();
    }

    public void PreviousTutorialPart()
    {
        --current_tutorial_part;
        nextButton.SetActive(true);

        if (current_tutorial_part < 0)
        {
            current_tutorial_part = 0;
        }
        else if (current_tutorial_part == 0)
        {
            prevButton.SetActive(false);
        }

        GetComponentInChildren<GifScript>().SwitchGif(tutorial_gifs[current_tutorial_part]);

        UpdateTutorialText();
    }

    void UpdateTutorialText()
    {
        foreach (Transform child in transform)
        {
            switch (child.tag)
            {
                case "TutorialTopText":
                    child.GetComponent<TMPro.TextMeshProUGUI>().text = tutorial_texts[current_tutorial_part].topText;
                    break;
                case "TutorialBottomText":
                    child.GetComponent<TMPro.TextMeshProUGUI>().text = tutorial_texts[current_tutorial_part].bottomText;
                    break;
                case "TutorialExtraText":
                    child.GetComponent<TMPro.TextMeshProUGUI>().text = tutorial_texts[current_tutorial_part].extraText;
                    break;
                default:
                    break;
            }
        }
    }
}
