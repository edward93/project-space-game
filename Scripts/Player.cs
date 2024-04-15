using Godot;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public partial class Player : RigidBody2D
{
  /// <summary>
  /// Instant acceleration of the object
  /// </summary>
  [Export]
  public float Acceleration = 5.0f;

  /// <summary>
  /// Angular velocity min value
  /// </summary>
  [Export]
  public float AngularVelocityMin = -0.75f;

  /// <summary>
  /// Angular velocity max value
  /// </summary>
  [Export]
  public float AngularVelocityMax = 0.75f;

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
  /// Model root node, used for rotation
  /// </summary>
  private Node2D _model;

  /// <summary>
  /// Aim line
  /// </summary>
  private Line2D _aimLine;

  /// <summary>
  /// Init
  /// </summary>
  public override void _Ready()
  {
    _model = GetNode<Node2D>("Model");
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
    // calc target acceleration (assumes near instant torque, meaning object reaches the target acceleration instantly)
    var acceleration = movementDir.Rotated(Rotation) * Acceleration;

    // rotate the node only when thurs is applied
    if (movementDir != Vector2.Zero) RotateNode(delta);

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
}
