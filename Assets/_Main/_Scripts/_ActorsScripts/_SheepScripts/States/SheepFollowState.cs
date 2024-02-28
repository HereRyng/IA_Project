using UnityEngine;

public class SheepFollowState<T> : NavigationState<T>
{
    FlockingManager _flockingManager;
    SheepModel _sheep;
    T _inputIdle;
    public float _limitDistance = 23f; /// Distancia limite de seguimiento PLAYER
    public float randomDirectionInterval = 2f; // Intervalo de tiempo para cambiar a direcci�n RANDOM
    public float multiplier = 7;
    private Vector3 _randomDirection;
    private float _randomDirectionTimer;

    public SheepFollowState(T inputIdle, FlockingManager flockingManager, float limitDistance, ISteering obsAvoid=null): base(obsAvoid)//TODO
    {
        _flockingManager = flockingManager;
        _inputIdle = inputIdle;
        _limitDistance= limitDistance;

        //_steering = steering;
    }

    public override void InitializedState(BaseModel model, BaseView view, FSM<T> fsm)
    {
        base.InitializedState(model, view, fsm);
        _sheep = (SheepModel)_model;
    }
    public override void Awake()
    {

        _model.OnRun += _view.AnimRun;
        base.Awake();
    }
    public override void Execute()
    {
   
        base.Execute();
        bool finishGame = GameManager.Instance.FinishGame;
        var distance = Vector3.Distance(_sheep.Position, _flockingManager._target.transform.position);
        var target = _flockingManager._target;

        if(_sheep.IsStop || finishGame)
        {
            _fsm.Transitions(_inputIdle);
      
        }
        else
        {
            _sheep.Move(_sheep.Front);
            if (distance <= _limitDistance)
            {
                _sheep.Icon.material.color = Color.green;
                Vector3 flockingDir = _flockingManager.RunFlockingDir().normalized;

                if (!target._allies.Contains(_sheep.gameObject))
                {
                    target._allies.Add(_sheep.gameObject);
                }
                //var avoidDir = _steering.GetDir();
                if (distance <= 3)
                {
                    _sheep.Move(Vector3.zero);
                }
                if (Avoid == null) return;
                var endDir = flockingDir + Avoid.GetDir().normalized; //TODO
                _model.LookDir(endDir);


            }
            else
            {
                _sheep.Icon.material.color = Color.white;
                if (target._allies.Contains(_sheep.gameObject))
                {
                    target._allies.Remove(_sheep.gameObject);
                }
                _randomDirectionTimer -= Time.deltaTime;
                if (_randomDirectionTimer <= 0)
                {
                    _randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
                    _randomDirectionTimer = randomDirectionInterval;
                }
                Vector3 randomDir = _randomDirection * multiplier;
                _model.LookDir(randomDir.normalized);

            }
        }

    }

    public override void Sleep()
    {
        base.Sleep();
    }


}

