using Godot;
using System;

public partial class PlayerHUD : CanvasLayer
{
  [Export]
  private RigidBody2D Player = new RigidBody2D();

  /// <summary>
  /// Speed label
  /// </summary>
  private Label _speedLabel;

  /// <summary>
  /// Called once
  /// </summary>
  public override void _Ready()
  {
    _speedLabel = GetNode<Label>("Velocity/Velocity Circle/SpeedLabel");

  }

  /// <summary>
  /// Called on each frame
  /// </summary>
  /// <param name="delta"></param>
  public override void _Process(double delta)
  {
    _speedLabel.Text = $"{Player.LinearVelocity.Length():F3} µ/s";
  }
}
