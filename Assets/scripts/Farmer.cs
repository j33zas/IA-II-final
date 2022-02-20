using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IA2;
using System.Linq;

public enum FarmerAction
{
    Sleep,
    GrabHoe,
    GrabWateringCan,
    GrabCorn,
    LeaveHoe,
    LeaveWateringCan,
    SellCorn,
    Plant,
    Water,
    Harvest,
    BuySeed,
    NextStep,
    FailedStep,
    Win
}
public class Farmer : MonoBehaviour
{
    EventFSM<FarmerAction> _sm;

    IEnumerable<FarmerAction> _myPlan;

    Tool _currentTool;

    public bool finished = false;

    public Transform ToolPosition;

    FarmerWorldValues FWV = new FarmerWorldValues();

    FarmerStatsUI _UI;

    FarmerPlanner _myPlanner;

    public int startMoney;
    public int startEnergy;
    public int startSeeds;
    public bool startWatered;
    public string startItem;
    public float startfarmGrowth;
    public int startCorn;

    public Waypoint tentNode;
    public Waypoint farmNode;
    public Waypoint toolNode;
    public Waypoint shopNode;

    public bool reachedDestination;
    public bool doingAction;
    public float walkSpeed;
    public float turnSpeed;
    string targetTool;

    Animator _AN;

    IEnumerable<Waypoint> walkPath;

    private void Start()
    {
        _UI = GetComponent<FarmerStatsUI>();
        _AN = GetComponent<Animator>();
        _myPlanner = GetComponent<FarmerPlanner>();
        FWV.SetEnergy(startEnergy).
            SetMoney(startMoney).
            SetSeeds(startSeeds).
            SetWateredForToday(startWatered).
            SetItem(startItem).
            SetFarmG(startfarmGrowth).
            SetCorn(startCorn);
        _UI.SetenergyUI(FWV.farmerEnergy, startEnergy)
            .SetCornUI(FWV.cornStored)
            .SetItemUI(FWV.currentItem)
            .SetMoneyUI(FWV.money)
            .SetSeedUI(FWV.seedAmount);
        CalculatePlan();
    }

    void CalculatePlan()
    {
        #region creacion de estados
        var sleep = new State<FarmerAction>("sleep");
        var idle = new State<FarmerAction>("idle");
        var grabH = new State<FarmerAction>("grab h");
        var grabW = new State<FarmerAction>("grab w");
        var grabC = new State<FarmerAction>("grab c");
        var leaveH = new State<FarmerAction>("drop h");
        var leaveW = new State<FarmerAction>("drop w");
        var sellC = new State<FarmerAction>("sell c");
        var harvest = new State<FarmerAction>("harvest");
        var water = new State<FarmerAction>("water");
        var plant = new State<FarmerAction>("plant");
        var buy = new State<FarmerAction>("buy");
        var next = new State<FarmerAction>("next");
        var fail = new State<FarmerAction>("fail");
        var win = new State<FarmerAction>("win");
        var any = new State<FarmerAction>("any");
        #endregion

        #region OnExit estados
        sleep.OnExit += (a) =>
        {
            FWV.farmerEnergy = 100;
            FWV.hasWateredToday = false;
        };

        grabH.OnExit += (a) =>
        {
            FWV.farmerEnergy -= 10;
            FWV.currentItem = "hoe";
        };

        grabW.OnExit += (a) =>
        {
            FWV.farmerEnergy -= 10;
            FWV.currentItem = "wateringcan";
        };

        grabC.OnExit += (a) =>
        {
            FWV.farmerEnergy -= 10;
            FWV.cornStored--;
            FWV.currentItem = "corn";
        };

        harvest.OnExit += (a) =>
        {
            FWV.farmerEnergy -= 20;
            FWV.cornStored += 5;
            FWV.farmGrowth = 0;
        };

        water.OnExit += (a) =>
        {
            FWV.farmerEnergy -= 20;
            FWV.farmGrowth += .5f;
            FWV.hasWateredToday = true;
        };

        plant.OnExit += (a) =>
        {
            FWV.farmerEnergy -= 20;
            FWV.seedAmount -= 5;
            FWV.farmGrowth = 0.0001f;
        };

        sellC.OnExit += (a) =>
        {
            FWV.money += 150;
            FWV.farmerEnergy -= 15;
            FWV.currentItem = "";
        };

        leaveH.OnExit += (a) =>
        {
            FWV.currentItem = "";
        };

        leaveW.OnExit += (a) =>
        {
            FWV.currentItem = "";
        };

        buy.OnExit += (a) =>
        {
            FWV.farmerEnergy -= 10;
            FWV.seedAmount += 5;
            FWV.money -= 50;
        };
        #endregion

        #region OnEnter Estados

        sleep.OnEnter += (a) =>
        {
            walkPath = LookForPath(GetNearestNode(transform.position), tentNode);
            StartCoroutine(WalkTo(walkPath.ToList(), "sleep"));
        };

        grabH.OnEnter += (a) =>
        {
            targetTool ="hoe";
            walkPath = LookForPath(GetNearestNode(transform.position), toolNode);
            StartCoroutine(WalkTo(walkPath.ToList(), "grab"));
        };

        grabW.OnEnter += (a) =>
        {
            targetTool = "wateringcan";
            walkPath = LookForPath(GetNearestNode(transform.position), toolNode);
            StartCoroutine(WalkTo(walkPath.ToList(), "grab"));
        };

        grabC.OnEnter += (a) =>
        {
            walkPath = LookForPath(GetNearestNode(transform.position), toolNode);
            StartCoroutine(WalkTo(walkPath.ToList(), "grab"));
        };

        leaveH.OnEnter += (a) =>
        {
            targetTool = "";
            walkPath = LookForPath(GetNearestNode(transform.position), toolNode);
            StartCoroutine(WalkTo(walkPath.ToList(), "leave"));
        };

        leaveW.OnEnter += (a) =>
        {
            targetTool = "";
            walkPath = LookForPath(GetNearestNode(transform.position), toolNode);
            StartCoroutine(WalkTo(walkPath.ToList(),"leave"));
        };

        sellC.OnEnter += (a) =>
        {
            walkPath = LookForPath(GetNearestNode(transform.position), shopNode);
            StartCoroutine(WalkTo(walkPath.ToList(), "leave"));
        };

        harvest.OnEnter += (a) =>
        {
            walkPath = LookForPath(GetNearestNode(transform.position), farmNode);
            StartCoroutine(WalkTo(walkPath.ToList(), "harvest"));
        };

        water.OnEnter += (a) =>
        {
            walkPath = LookForPath(GetNearestNode(transform.position), farmNode);
            StartCoroutine(WalkTo(walkPath.ToList(), "water"));
            _currentTool.GetComponent<WateringCan>().SetProggress(FWV.farmGrowth);
        };

        plant.OnEnter += (a) =>
        {
            walkPath = LookForPath(GetNearestNode(transform.position), farmNode);
            StartCoroutine(WalkTo(walkPath.ToList(), "plant"));
        };

        buy.OnEnter += (a) =>
        {
            walkPath = LookForPath(GetNearestNode(transform.position), shopNode);
            StartCoroutine(WalkTo(walkPath.ToList(), "grab"));
        };

        fail.OnEnter += (a) =>
        {
            Debug.Log("you lose...");
        };

        win.OnEnter += (a) =>
        {
            Debug.Log("you win");
        };
        #endregion

        next.OnEnter += a =>
        {
            FarmerAction currentStep = _myPlan.FirstOrDefault();
            if (currentStep != default)
            {
                _myPlan = _myPlan.Skip(1);
                _sm.Feed(currentStep);
            }else if(!finished)
            {
                Debug.Log("End of plan! recalculating...");
                _myPlan = _myPlanner.Plan(FWV);
                _sm.Feed(_myPlan.FirstOrDefault());
            }
            else
                _sm.Feed(FarmerAction.Win);
        };

        #region state configurer
        StateConfigurer.
        Create(any).
        SetTransition(FarmerAction.NextStep, next).
        SetTransition(FarmerAction.FailedStep, idle).Done();

        StateConfigurer.
        Create(next).
        SetTransition(FarmerAction.Sleep, sleep).
        SetTransition(FarmerAction.GrabWateringCan, grabW).
        SetTransition(FarmerAction.GrabCorn, grabC).
        SetTransition(FarmerAction.GrabHoe, grabH).
        SetTransition(FarmerAction.LeaveWateringCan, leaveW).
        SetTransition(FarmerAction.LeaveHoe, leaveH).
        SetTransition(FarmerAction.SellCorn, sellC).
        SetTransition(FarmerAction.Harvest, harvest).
        SetTransition(FarmerAction.Water, water).
        SetTransition(FarmerAction.Plant, plant).
        SetTransition(FarmerAction.BuySeed, buy).
        SetTransition(FarmerAction.Win, win).Done();
        #endregion

        _sm = new EventFSM<FarmerAction>(idle, any);
    }

    void ExecutePlan(FarmerPlanner planner)
    {
        _myPlan = planner.Plan(FWV);
        _sm.Feed(FarmerAction.NextStep);
    }

    void ThankYouNext()
    {
        _sm.Feed(FarmerAction.NextStep);
        _UI.SetenergyUI(FWV.farmerEnergy, startEnergy)
            .SetCornUI(FWV.cornStored)
            .SetItemUI(FWV.currentItem)
            .SetMoneyUI(FWV.money)
            .SetSeedUI(FWV.seedAmount);
    }

    private void Update()
    {
        if(_sm != null)
            _sm.Update();
        if (Input.GetKeyDown(KeyCode.Space))
            ExecutePlan(_myPlanner);
    }

    #region ble
    void UseItem()
    {
        _currentTool.UseTool();
    }

    public void PickUpItem()
    {
        var closeby = Physics.OverlapSphere(transform.position, 5, 1 << LayerMask.NameToLayer("Default"), QueryTriggerInteraction.Collide)
            .Where(a => a.GetComponent<Tool>()).Select(a=>a.GetComponent<Tool>()).Where(a=>a.toolName == targetTool).FirstOrDefault();
        if(closeby != default)
        {
            _currentTool = closeby;
            _currentTool.PickUpTool();
            _currentTool.transform.position = ToolPosition.position;
            _currentTool.transform.rotation = ToolPosition.rotation;
            _currentTool.transform.parent = ToolPosition;
        }
    }

    void DropItem()
    {
        if(_currentTool)
        {
            _currentTool.DropTool();
            _currentTool.transform.parent = null;
            _currentTool.transform.position = transform.position;
            _currentTool = null;
        }
    }

    IEnumerable<Waypoint> LookForPath(Waypoint start, Waypoint end)
    {
        return AStarNormal<Waypoint>.Run(start, end,
            (curr, goal) => Vector3.Distance(curr.transform.position, goal.transform.position),
            (curr) => curr == end,
            a => a.adyacent.Select(b => new AStarNormal<Waypoint>.Arc(b, Vector3.Distance(b.transform.position, a.transform.position))));
    }

    Waypoint GetNearestNode(Vector3 pos)
    {
        return Physics.OverlapSphere(pos, 1, 1 << LayerMask.NameToLayer("node"), QueryTriggerInteraction.Collide).Select(a => a.GetComponent<Waypoint>()).
        Aggregate(new Waypoint(), (acum,curr)=>
        {
            if (acum == null)
                acum = curr;
            else if(Vector3.Distance(acum.transform.position, transform.position) > Vector3.Distance(curr.transform.position, transform.position))
                acum = curr;
            return acum;
        });
    }
    #endregion

    IEnumerator WalkTo(IEnumerable<Waypoint> path, string animOnArrival)
    {
        var lastNode = path.LastOrDefault();
        _AN.SetBool("walking", true);
        int i = 0;
        while(i < path.Count())
        {
            var currNode = path.Skip(i).First();
            if (Vector3.Distance(currNode.transform.position, transform.position) > .025f)
            {
                Vector3 desiredDir = currNode.transform.position - transform.position;
                transform.forward = Vector3.Lerp(transform.forward, desiredDir, Time.deltaTime * turnSpeed);
                transform.position += transform.forward * Time.deltaTime * walkSpeed;
            }
            else
                i++;
            yield return new WaitForEndOfFrame();
        }
        _AN.SetBool("walking", false);
        _AN.SetTrigger(animOnArrival);
        doingAction = true;
        while (doingAction)
            yield return new WaitForEndOfFrame();
        ThankYouNext();
    }

    public void EndAction()
    {
        doingAction = false;
    }
}