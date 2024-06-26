using Godot;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public partial class Player : RigidBody2D
{
  private const string SPEED_AND_ACCELERATION = "Speed & Acceleration";

  /// <summary>
  /// Total energy
  /// </summary>
  public float TotalEnergy { get; set; } = 130.0f;

  /// <summary>
  /// Remaining energy
  /// </summary>
  public float CurrentEnergy { get; set; } = 90.0f;

  /// <summary>
  /// How much energy do engines consume
  /// </summary>
  [Export]
  public float EngineEnergyConsumption = 0.02f;

  /// <summary>
  /// How quickly model rotates
  /// </summary>
  [Export]
  public float RotationAccelerationFactor = 1.3f;

  /// <summary>
  /// Max length of the aim helper line
  /// </summary>
  [Export]
  public float MaxAimLength = 150.0f;

  /// <summary>
  /// Aim line offset
  /// </summary>
  [Export]
  public float AimOffset = 25.0f;
  /// <summary>
  /// Instant acceleration of the object
  /// </summary>
  [Export]
  [ExportGroup(SPEED_AND_ACCELERATION)]
  public float Acceleration = 5.0f;

  /// <summary>
  /// Angular velocity min value
  /// </summary>
  [Export]
  [ExportGroup(SPEED_AND_ACCELERATION)]
  public float AngularVelocityMin = -0.75f;

  /// <summary>
  /// Angular velocity max value
  /// </summary>
  [Export]
  [ExportGroup(SPEED_AND_ACCELERATION)]
  public float AngularVelocityMax = 0.75f;

  /// <summary>
  /// Model root node, used for rotation
  /// </summary>
  private Node2D _model;

  /// <summary>
  /// Aim line
  /// </summary>
  private Line2D _aimLine;

  /// <summary>
  /// Normalized thrust vector (input direction)
  /// </summary>
  public Vector2 _thrustVector;

  /// <summary>
  /// is energy depleted
  /// </summary>
  public bool _energyDepleted = false;

  /// <summary>
  /// Init
  /// </summary>
  public override void _Ready()
  {
    _model = GetNode<Node2D>("TransformRoot/Model");
    _aimLine = GetNode<Line2D>("HelperGuides/Aim");

    _aimLine.AddPoint(Vector2.Zero);
    _aimLine.AddPoint(Vector2.Zero);
  }

  /// <summary>
  /// Handle input events
  /// </summary>
  /// <param name="inputEvent">Input event</param>
  public override void _Input(InputEvent inputEvent)
  {
    if (inputEvent.IsActionPressed("Esc"))
    {
      GD.Print($"Esc pressed - GameState: {GetTree().Paused}");
      if (GetTree().Paused) GetTree().Quit();
      else GetTree().Paused = true;
    }
  }

  /// <summary>
  /// Physics process
  /// </summary>
  /// <param name="delta"></param>
  public override void _PhysicsProcess(double delta)
  {
    RealisticRelativeControls(delta);
  }

  /// <summary>
  /// Frame process
  /// </summary>
  /// <param name="delta"></param>
  public override void _Process(double delta)
  {
    DrawAimLine(delta);
  }

  /// <summary>
  /// Realistic relative controls for Rigid body
  /// </summary>
  /// <param name="delta"></param>
  private void RealisticRelativeControls(double delta)
  {
    // read input
    var direction = Input.GetVector("Thrust Down", "Thrust Up", "MoveLeft", "MoveRight");
    // calc movement dir
    var movementDir = direction.Normalized();

    // lower the sideway thrusters power
    movementDir = new Vector2(movementDir.X, movementDir.Y * 0.3f);
    // update thrust vector
    MovementDirToThrustVector(movementDir);

    // calc energy consumption
    CalculateEnergyConsumedByEngines((float)delta);

    // calc target acceleration (assumes near instant torque, meaning object reaches the target acceleration instantly)
    var acceleration = _thrustVector.Rotated(Rotation) * Acceleration;

    // rotate the node only when thrust is applied
    if (_thrustVector != Vector2.Zero) RotateNode(delta);

    // update the linear velocity
    LinearVelocity += acceleration;
  }

  /// <summary>
  /// Slowly rotates the node towards the aim point
  /// </summary>
  /// <param name="delta"></param>
  private void RotateNode(double delta)
  {
    // calc how much to rotate
    var rotation = GetAngleTo(ToGlobal(_aimLine.GetPointPosition(1)));
    AngularVelocity = Mathf.Clamp(rotation, AngularVelocityMin, AngularVelocityMax);
  }

  /// <summary>
  /// Draws the aim line
  /// </summary>
  /// <param name="delta"></param>
  private void DrawAimLine(double delta)
  {
    // mouse current position (in local transform)
    var mousePosition = ToLocal(GetGlobalMousePosition());
    // origin point (local 0,0)
    var originPoint = Vector2.Zero;

    // calc end point position
    var line = mousePosition - originPoint;

    if (line.Length() > MaxAimLength)
    {
      line = line.Normalized() * MaxAimLength;
    }

    // calc start point
    var startPoint = originPoint + line.Normalized() * AimOffset;
    // calc new end point
    var endPoint = originPoint + line;
    // update end position
    _aimLine.SetPointPosition(1, endPoint);
    // update start position
    _aimLine.SetPointPosition(0, startPoint);
  }

  /// <summary>
  /// Calculate engine energy consumption
  /// </summary>
  /// <param name="delta">delta</param>
  private void CalculateEnergyConsumedByEngines(float delta)
  {
    // thrust/engin intensity
    var thrustIntensity = _thrustVector.Length();

    // update current energy
    CurrentEnergy -= EngineEnergyConsumption * thrustIntensity * delta;

    // if no energy is left emit 'EnergyDepleted' signal
    if (CurrentEnergy <= 0)
    {
      CurrentEnergy = 0;
      EmitSignal(SignalName.EnergyDepleted);
    }
  }

  /// <summary>
  /// Converts movement dir vector (input) to thrust vector (used for moving the player)
  /// </summary>
  /// <param name="movementDir">Input dir vector</param>
  private void MovementDirToThrustVector(Vector2 movementDir)
  {
    if (!_energyDepleted)
    {
      _thrustVector = movementDir;
    }
    else
    {
      // turn off the engines
      _thrustVector = Vector2.Zero;
    }
  }

  #region signals
  [Signal]
  public delegate void EnergyDepletedEventHandler();

  /// <summary>
  /// Handle energy deleted event
  /// </summary>
  public void OnEnergyDepleted()
  {
    _energyDepleted = true;
  }
  #endregion
}
