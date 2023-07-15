using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolState<T> : NavigationState<T>
{
    EnemyModel _enemyModel;
    AStar<Node> _astar;
    NodeGrid _nodeGrid;
    List<Node> _path;


    public EnemyPatrolState(NodeGrid nodeGrid, Node startNode, ISteering obsAvoid):base(obsAvoid)
    {
        _nodeGrid = nodeGrid;
        StartNode = startNode;
        _astar = new AStar<Node>();
    }
    public override void InitializedState(BaseModel model, BaseView view, FSM<T> fsm)
    {
        base.InitializedState(model, view, fsm);
        _enemyModel = (EnemyModel)model;
    }
    public override void Awake()
    {
        base.Awake();
        _model.OnRun += _view.AnimRun; 
        _model.Move(Vector3.zero);
        _enemyModel._coneOfView.color = Color.yellow;

        //_model.LookDir(_startNode.transform.position);

        if (StartNode != null)
        {
            Pathfinding(StartNode);
            _enemyModel.transform.LookAt(_endNode.transform);
        }
    }
    public override void Execute()
    {
        Debug.Log("Execute Patrol state");
        base.Execute();
        Vector3 astarDir = Wp.GetDir() * _enemyModel._multiplierAstar;
        Vector3 avoidDir = Avoid.GetDir() * _enemyModel._multiplierAvoid;
        if (_endNode != null)
        {
            Vector3 goalNode = _endNode.transform.position;
            Vector3 goalNodeFix = new Vector3(goalNode.x, _model.transform.position.y, goalNode.z);

            Vector3 posEnd = goalNodeFix - _model.transform.position;

            if (posEnd.magnitude < 0.2f)
            {
                Pathfinding(_endNode);
                var newDir = Wp.GetDir() * _enemyModel._multiplierAstar;
                astarDir = newDir;
            }
        }
        Vector3 dirFinal = astarDir.normalized + avoidDir.normalized;
        _model.Move(dirFinal);
        _model.LookDir(dirFinal);

    }
    public override void Sleep()
    {
        base.Sleep();
        Debug.Log("Sleep Patrol state");
        _model.OnRun -= _view.AnimRun;
    }

    ///TODO: No repetir codigo de pathfinding reutilizar metodo (hacer mas generico)
    public void Pathfinding(Node initialNode)
    {

        StartNode?.RestartMat();
        StartNode = initialNode;
        _endNode?.RestartMat();
        _endNode = _nodeGrid.GetRandomNode();


        while (_endNode == initialNode)
        {
            _endNode = _nodeGrid.GetRandomNode();
        }

        _path = _astar.Run(initialNode, Satisfies, GetConnections,
           GetCost, Heuristic, 500);

        if (_path != null && _path.Count > 0)
        {
            StartNode.SetColorNode(Color.white);
            _endNode.SetColorNode(Color.green);
            _enemyModel.GoalNode = _endNode;
            _enemyModel._startNode = StartNode;
            Wp.AddWaypoints(_path);
        }
    }

   



}