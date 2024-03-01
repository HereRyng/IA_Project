using System.Collections.Generic;
using UnityEngine;

public class ExplorationState<T> : NavigationState<T>
{
    Light _coneVision;
    SpiderModel _spiderModel;
    AStar<Node> _astar;
    NodeGrid _nodeGrid;
    List<Node> _path;


    public ExplorationState(NodeGrid nodeGrid, Node startNode, ISteering obsAvoid):base(obsAvoid)
    {
        _nodeGrid = nodeGrid;
        StartNode = startNode;
        _astar = new AStar<Node>();
    }
    public override void InitializedState(BaseModel model, BaseView view, FSM<T> fsm)
    {
        base.InitializedState(model, view, fsm);
        _spiderModel = (SpiderModel)model;
        _coneVision = _spiderModel._coneOfView;
    }
    public override void Awake()
    {
        base.Awake();
        _coneVision.enabled = true;
        _model.OnRun += _view.RunAnim; 
        _model.Move(Vector3.zero);
        _spiderModel._coneOfView.color = Color.yellow;

        if (StartNode == null || _nodeGrid == null)
        {

            return;
        }
        Node nodeNear = _nodeGrid.FindNearestValidNode(_model.transform);
        Pathfinding(nodeNear);
    }
    public override void Execute()
    {
       
        base.Execute();
        Vector3 astarDir = Wp.GetDir().normalized * _spiderModel._multiplierAstar;
        Vector3 avoidDir = Avoid.GetDir().normalized * _spiderModel._multiplierAvoid;
        if (_endNode!=null)
        {
            Vector3 goalNode = _endNode.transform.position;
            float endDistance = (goalNode - _model.transform.position).magnitude;
            if (endDistance <= 0.5f)
            {
                
                Pathfinding(_endNode);
                var newDir = Wp.GetDir().normalized * _spiderModel._multiplierAstar;
                astarDir = newDir;
            }
        }
   

        Vector3 dirFinal =  avoidDir+ astarDir;
        _model.Move(dirFinal);
        _model.LookDir(dirFinal);
    }
    public override void Sleep()
    {
        base.Sleep();
        _coneVision.enabled = false;
        _model.OnRun -= _view.RunAnim;
    }


    ///TODO: No repetir codigo de pathfinding reutilizar metodo (hacer mas generico)
    public void Pathfinding(Node initialNode)
    {

        StartNode?.RestartMat();
        _endNode?.RestartMat();
        StartNode = initialNode;

        while (_endNode == StartNode || _endNode == null) 
        {
            _endNode = _nodeGrid.GetRandomNode();
        }

        _path = _astar.Run(initialNode, Satisfies, GetConnections,
           GetCost, Heuristic, 500);

        if (_path != null && _path.Count > 0)
        {
            StartNode.SetColorNode(Color.white);
            _endNode.SetColorNode(Color.green);
            _spiderModel._startNode = StartNode;
            _spiderModel.GoalNode = _endNode;
            Wp.AddWaypoints(_path);
        }

    }

   



}