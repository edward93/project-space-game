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
  /// Friction coefficient
  /// </summary>
  [Export]
  public float DampeningFactor = 2.0f;

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
    // RealisticAbsoluteControls(delta);
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
  /// Handles velocity/acceleration in a more realistic way.
  /// Directions are not dependent on the orientation of the player, meaning "Up" will translate the player in the -y direction of the screen
  /// </summary>
  /// <param name="delta">Processing time</param>
  private void RealisticAbsoluteControls(double delta)
  {
    // read input
    var direction = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
    // calc movement dir
    var movementDir = direction.Normalized();
    // calc target acceleration (assumes near instant torque, meaning object reaches the target acceleration instantly)
    var acceleration = movementDir * Acceleration;

    // apply dampening (Friction)
    // acceleration = acceleration.Lerp(-Velocity, (float)delta * DampeningFactor);
    acceleration = acceleration.Lerp(-LinearVelocity, (float)delta * DampeningFactor);

    // update the Velocity v = a*t (where t = 1 since we are running this per physics tic)
    // Velocity += acceleration;
    // LinearVelocity += acceleration;
    ApplyCentralForce(acceleration);
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
    // calc target acceleration (assumes near instant torque, meaning object reaches the target acceleration instantly)
    var acceleration = movementDir.Rotated(Rotation) * Acceleration;

    // rotate the node only when thurs is applied
    if (movementDir != Vector2.Zero) RotateNode(delta);

    // apply dampening (Friction)
    acceleration = acceleration.Lerp(-LinearVelocity, (float)delta * DampeningFactor);
    // update the Velocity v = a*t (where t = 1 since we are running this per physics tic)
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

    // slowly rotate the root node
    Rotation = Mathf.Lerp(Rotation, Rotation + rotation, (float)delta * RotationAccelerationFactor);
  }

  /// <summary>
  /// Realistic relative controls for character body
  /// </summary>
  /// <param name="delta">delta time</param>
  // private void RealisticRelativeControlsCharacterBody(double delta)
  // {
  //   // read input
  //   var direction = Input.GetVector("Thrust Down", "Thrust Up", "MoveLeft", "MoveRight");
  //   // calc movement dir
  //   var movementDir = direction.Normalized();
  //   // calc target acceleration (assumes near instant torque, meaning object reaches the target acceleration instantly)
  //   var acceleration = movementDir.Rotated(Rotation) * Acceleration;
  //   // GD.Print($"Rotated dir: {movementDir.Rotated(Rotation)}");
  //   // apply dampening (Friction)
  //   acceleration = acceleration.Lerp(-Velocity, (float)delta * DampeningFactor);
  //   // update the Velocity v = a*t (where t = 1 since we are running this per physics tic)
  //   Velocity += acceleration;
  // }

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
