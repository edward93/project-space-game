using Godot;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public partial class Player : CharacterBody2D
{
  [Export]
  public float Speed = 40.0f;
  /// <summary>
  /// Instant acceleration of the object
  /// </summary>
  [Export]
  public float Acceleration = 5.0f;
  /// <summary>
  /// Friction coefficient
  /// </summary>
  [Export]
  public float DampeningFactor = 2.0f;

  /// <summary>
  /// How quickly model rotates
  /// </summary>
  [Export]
  public float RotationAccelerationFactor = 2.0f;

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
    _aimLine = GetNode<Line2D>("MovementGuideSystem/Aim");

    _aimLine.AddPoint(Vector2.Zero);
    _aimLine.AddPoint(Vector2.Zero);
  }

  /// <summary>
  /// Handle input events
  /// </summary>
  /// <param name="inputEvent">Input event</param>
  public override void _Input(InputEvent inputEvent)
  {
    if (inputEvent is InputEventMouseMotion)
    {
      // var mouseMovement = inputEvent as InputEventMouseMotion;

      // _aimLine.SetPointPosition(1, ToLocal(mouseMovement.Position));

      // GD.Print($" aim line start: {_aimLine.GetPointPosition(0)} End: {_aimLine.GetPointPosition(1)}");

      // _model.Rotation = Mathf.LerpAngle(_model.Rotation, modelDir.AngleTo(mouseMovement.GlobalPosition - _model.GlobalPosition), RotationAccelerationFactor);
      // _model.Rotate(modelDir.AngleTo(mouseMovement.GlobalPosition - _model.GlobalPosition));

      // _model.GlobalRotation = Mathf.LerpAngle(_model.GlobalRotation, _model.GetAngleTo(mouseMovement.GlobalPosition) + Mathf.Pi/2, RotationAccelerationFactor);

      // Rotate(GetAngleTo(mouseMovement.GlobalPosition) + Mathf.Pi/2);
      // very simple way of rotating
      // LookAt(mouseMovement.GlobalPosition);

      // GD.Print(Mathf.RadToDeg(_model.GetAngleTo(mouseMovement.GlobalPosition)));
      // GD.Print($"Angle: {Mathf.RadToDeg(modelDir.AngleTo(mouseMovement.GlobalPosition - _model.GlobalPosition))}");
      // // mouseMovement.Position
      // GD.Print($"Vector pointing from player to mouse: {mouseMovement.GlobalPosition - _model.GlobalPosition}");
      // GD.Print($"_model Global Rotation: {_model.Rotation:F3} mouse Global Rotation: ({mouseMovement.GlobalPosition.X:F3}, {mouseMovement.GlobalPosition.Y:F3})");
    }
    if (inputEvent.IsActionPressed("Esc"))
    {
      if (GetTree().Paused) GetTree().Quit();
      else GetTree().Paused = true;
    }
  }

  public override void _PhysicsProcess(double delta)
  {
    // read input
    var direction = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
    // calc movement dir
    var movementDir = direction.Normalized();

    RealisticControls(movementDir, delta);
    // GD.Print($"Pos: {Position} G Pos: {GlobalPosition}");
    MoveAndSlide();
  }

  public override void _Process(double delta)
  {
    DrawAimLine(delta);
  }

  /// <summary>
  /// Handles velocity/acceleration in a more realistic way
  /// </summary>
  /// <param name="movementDir">Direction of the movement (comes from the controller)</param>
  /// <param name="delta">Processing time</param>
  private void RealisticControls(Vector2 movementDir, double delta)
  {
    // calc target acceleration (assumes near instant torque, meaning object reaches the target acceleration instantly)
    var acceleration = movementDir * Acceleration;

    // apply dampening (Friction)
    acceleration = acceleration.Lerp(-Velocity, (float)delta * DampeningFactor);

    // update the Velocity v = a*t (where t = 1 since we are running this per physics tic)
    Velocity += acceleration;

    // GD.Print($"Velocity: ({Velocity.X:F3}, {Velocity.Y:F3}) Acceleration: ({acceleration.X:F3}, {acceleration.Y:F3})");
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
  /// More arcade like velocity/acceleration controls (const acceleration)
  /// </summary>
  /// <param name="movementDir">Direction of the movement (comes from the controller)</param>
  /// <param name="delta">Processing time</param>
  private void ArcadeControls(Vector2 movementDir, double delta)
  {
    // calc acceleration
    var acceleration = (float)delta * Acceleration;
    // calc target velocity
    var targetVelocity = movementDir * Speed;
    // update velocity
    Velocity = Velocity.MoveToward(targetVelocity, acceleration);
  }
}
