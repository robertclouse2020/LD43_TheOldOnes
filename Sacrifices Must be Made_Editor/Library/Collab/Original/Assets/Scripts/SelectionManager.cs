using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class SelectionManager : MonoBehaviour
{


    public GameObject clickedObj;

    public Material hoveredMat;
    public Material selectedMat;
    public Material defaultMat;

    public GameObject scrollContextMenuContainer;
    public GameObject panelBuildingInfo;
    public GameObject contentContainer;

    public GameObject managerPopOut;
    TextMeshProUGUI buildingTitle;
    TextMeshProUGUI buildingInfo;

    GameObject[] buildingPanels;

    public GameObject managerPanel;

    List<GameObject> managerPanels;

    public List<Sprite> managerImages;

    public Sprite noManager;

    public GameObject ImageProfilePic;

    int characterPanelCount = 0;


    BuildingStats fullBuilding;

    private void Awake()
    {
        buildingTitle = panelBuildingInfo.transform.Find("TextMeshProBuildingTitle").GetComponent<TextMeshProUGUI>();
        buildingInfo = panelBuildingInfo.transform.Find("TextMeshProBuildingInfo").GetComponent<TextMeshProUGUI>();
        buildingPanels = GameObject.FindGameObjectsWithTag("BuildingPanel");


        foreach (GameObject go in buildingPanels)
        {
            go.SetActive(false);
        }

        managerPanels = new List<GameObject>();
        managerPopOut.SetActive(false);

    }

    // Use this for initialization
    void Start()
    {
        clickedObj = null;

        scrollContextMenuContainer.SetActive(false);

        foreach (CharacterClass c in GetComponent<GameManager>().characterPool.allCharacters)
        {
            GameObject panel = Instantiate(managerPanel, contentContainer.transform);
            panel.transform.Find("ManagerName").GetComponent<TextMeshProUGUI>().text = c.name;
            panel.transform.Find("Loyalty").GetComponent<TextMeshProUGUI>().text = "Loyalty: " + c.loyalty;
            panel.transform.Find("Charisma").GetComponent<TextMeshProUGUI>().text = "Charisma: " + c.charisma;
            panel.transform.Find("Intelligence").GetComponent<TextMeshProUGUI>().text = "Intelligence: " + c.intellect;
            panel.transform.Find("Intelligence").GetComponent<TextMeshProUGUI>().text = "Intelligence: " + c.intellect;

            panel.GetComponent<ManagerPanelInfo>().character = c;

            int rand = Random.Range(0, managerImages.Count);
            Debug.Log("rand " + rand);
            c.cultistImage = managerImages[rand];

            managerPanels.Add(panel);

        }

        //ImageProfilePic.SetActive(false);
        ImageProfilePic.GetComponent<Image>().sprite = noManager;
        ImageProfilePic.transform.Find("TextMeshPro Text").GetComponent<TextMeshProUGUI>().text = "No Manager";



        foreach (GameObject go in managerPanels)
        {
            go.SetActive(false);
        }

    }


    void addManager(CharacterClass character)
    {
        if (clickedObj != null)
        {
            BuildingStats buildingStatsScript = clickedObj.GetComponent<PlotScript>().building.GetComponent<BuildingStats>();

            if (fullBuilding != null)
                Debug.Log(fullBuilding.name);
            else
                Debug.Log("full building null");

            //if (checkIfManagerBusy(c))
            //{
            //    building.manager = character;
            //}

            if (!checkIfManagerBusy(character))
            {
                buildingStatsScript.manager = character;
            }
            else
            {
                managerPopOut.SetActive(true);
                Debug.Log(character.name);
                managerPopOut.transform.Find("PanelManagerInfo").transform.Find("TextMeshProBuildingInfo").GetComponent<TextMeshProUGUI>().text = "That character is alread assigned a job. Would you like to reasign them?";
                managerPopOut.transform.Find("ButtonReassign").GetComponent<Button>().onClick.AddListener(delegate { reasign(buildingStatsScript, character); });
                managerPopOut.transform.Find("ButtonCancel").GetComponent<Button>().onClick.AddListener(delegate { cancel(); });
            }
        }
        else
        {
            return;
        }
    }

    void reasign(BuildingStats b, CharacterClass c)
    {

        b.manager = c;

        if (fullBuilding != null)
        {
            fullBuilding.manager = null;
        }
        managerPopOut.SetActive(false);


    }

    void cancel()
    {
        managerPopOut.SetActive(false);
    }


    void setManagerPic(CharacterClass character)
    {
        ImageProfilePic.GetComponent<Image>().sprite = character.cultistImage;
    }


    bool checkIfManagerBusy(CharacterClass c)
    {
        bool busy = false;


        foreach (GameObject building in GameObject.FindGameObjectsWithTag("Building"))
        {

            if (building.GetComponent<BuildingStats>())
            {
                if (c == building.GetComponent<BuildingStats>().manager)
                {
                    //building.GetComponent<BuildingStats>().manager = null;
                    busy = true;
                    fullBuilding = building.GetComponent<BuildingStats>();
                    break;
                    //return true;
                }

            }

        }

       // Debug.Log("busy: " + busy);
        return busy;
    }


    // Update is called once per frame
    void Update()
    {


        //mouse is down and not over UI object
        if (Input.GetMouseButtonDown(0)
            && !EventSystem.current.IsPointerOverGameObject())
        {

            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            //hit tagged obj
            if (hit)
            {
                if (hitInfo.transform.gameObject.tag == "Clickable")
                {
                    if (clickedObj != null)
                    {
                        ChangeMaterial(clickedObj, defaultMat);
                    }

                    clickedObj = hitInfo.transform.gameObject;
                    defaultMat = hitInfo.transform.gameObject.GetComponent<Renderer>().material;
                    ChangeMaterial(hitInfo.transform.gameObject, selectedMat);

                    //scrollContextMenuContainer.transform.position = Input.mousePosition;
                    //scrollContextMenuContainer.transform.position = 
                    //    new Vector3(Input.mousePosition.x + (scrollContextMenuContainer.GetComponent<RectTransform>().sizeDelta.x /6),
                    //    Input.mousePosition.y - (scrollContextMenuContainer.GetComponent<RectTransform>().sizeDelta.y / 5));

                }
                //hit untagged
                else
                {
                    if (clickedObj != null)
                    {
                        ChangeMaterial(clickedObj, defaultMat);
                    }
                    clickedObj = null;


                }
            }
            //no hit
            else
            {
                if (clickedObj != null)
                {
                    ChangeMaterial(clickedObj, defaultMat);
                }
                clickedObj = null;
            }
        }

        if (clickedObj == null)
        {
            scrollContextMenuContainer.SetActive(false);
        }
        else
        {
            scrollContextMenuContainer.SetActive(true);

            if (clickedObj.GetComponent<PlotScript>().building == null)
            {
                buildingTitle.text = "Plot";
                buildingInfo.text = "An empty plot of land, perfect for building on.";

                foreach (GameObject go in buildingPanels)
                {
                    go.SetActive(true);
                }

                foreach (GameObject go in managerPanels)
                {
                    go.SetActive(false);
                }

            }

            //probably a better place for this :(
            ImageProfilePic.GetComponent<Image>().sprite = noManager;
            ImageProfilePic.transform.Find("TextMeshPro Text").GetComponent<TextMeshProUGUI>().text = "No Manager";


            //setup manager context menu
            if (clickedObj.GetComponent<PlotScript>().building != null)
            {
                foreach (GameObject go in buildingPanels)
                {
                    go.SetActive(false);
                }


                //foreach (GameObject go in managerPanels)
                for (int i = 0; i < managerPanels.Count; i++)
                {
                    managerPanels[i].SetActive(true);
                    CharacterClass c = managerPanels[i].GetComponent<ManagerPanelInfo>().character;
                    //Debug.Log(c.cultistImage.name);

                    // building has no manager
                    if (clickedObj.GetComponent<PlotScript>().building.GetComponent<BuildingStats>().manager == null)
                    {
                        managerPanels[i].GetComponent<Button>().interactable = true;
                        if (managerPanels[i].GetComponent<ManagerPanelInfo>().numDelegatedEvents < 1)
                        {
                            managerPanels[i].GetComponent<Button>().onClick.AddListener(delegate { addManager(c); });
                            managerPanels[i].GetComponent<ManagerPanelInfo>().numDelegatedEvents++;
                        }
                    }
                    else if (clickedObj.GetComponent<PlotScript>().building.GetComponent<BuildingStats>().manager != null)
                    {
                        managerPanels[i].GetComponent<Button>().interactable = false;
                    }
                }

                //set image based on manager from building
                if (clickedObj.GetComponent<PlotScript>().building.GetComponent<BuildingStats>().manager != null)
                {
                    ImageProfilePic.SetActive(true);
                    setManagerPic(clickedObj.GetComponent<PlotScript>().building.GetComponent<BuildingStats>().manager);
                    ImageProfilePic.transform.Find("TextMeshPro Text").GetComponent<TextMeshProUGUI>().text = clickedObj.GetComponent<PlotScript>().building.GetComponent<BuildingStats>().manager.name;
                }

                if (clickedObj.GetComponent<PlotScript>().building.name.Contains("Obelisk"))
                {
                    buildingTitle.text = "Obelisk";
                    buildingInfo.text = "The Obelisk is a powerful but dangerous building. It will convert a lot of non-believers but the structure will draw the ire of those that would not want The Old Ones free on this realm. No manager required. ";
                }
                else if (clickedObj.GetComponent<PlotScript>().building.name.Contains("Temple"))
                {
                    buildingTitle.text = "Temple";
                    buildingInfo.text = "The Old Ones require many sacrifices. We are happy to oblige. Best manager stat: Charisma";
                }
                else if (clickedObj.GetComponent<PlotScript>().building.name.Contains("Mill"))
                {
                    buildingTitle.text = "Mill";
                    buildingInfo.text = "The Mill is where you will be making your money. Make sure you never run out of cash. Best manager stat: Intelligence";
                }
                else if (clickedObj.GetComponent<PlotScript>().building.name.Contains("Citadel"))
                {
                    buildingTitle.text = "Citadel";
                    buildingInfo.text = "We are so close! Keep this building up for one day and the Old Ones will surely rise again.";
                }

            }



        }

        //right click deselect all
        if (Input.GetMouseButtonDown(1))
        {
            if (clickedObj != null)
            {
                ChangeMaterial(clickedObj, defaultMat);
            }
            clickedObj = null;
        }

        //if (clickedObj == null)
        //{
        //    ChangeMaterial(clickedObj, defaultMat);
        //}
    }

    public void ChangeMaterial(GameObject obj, Material newMat)
    {
        List<Renderer> children = new List<Renderer>();
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {

            if (r.gameObject.tag == "Building" || r.gameObject.tag == "Effect")
            {
                //do nothing
            }
            else
            {
                children.Add(r);
            }
        }

        foreach (Renderer rend in children)
        {
            var mats = new Material[rend.materials.Length];
            for (var j = 0; j < rend.materials.Length; j++)
            {
                mats[j] = newMat;
            }
            rend.materials = mats;
        }
    }
}
