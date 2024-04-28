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

  private TextureRect _directionArrow;

  /// <summary>
  /// Energy value
  /// </summary>
  private Label _energyValue;

  /// <summary>
  /// Energy bar
  /// </summary>
  private ProgressBar _energyBar;

  /// <summary>
  /// Called once
  /// </summary>
  public override void _Ready()
  {
    _speedLabel = GetNode<Label>("Velocity/Velocity Circle/SpeedLabel");
    _directionArrow = GetNode<TextureRect>("Velocity/Velocity Circle/DirectionArrow");

    _energyValue = GetNode<Label>("Energy/MarginContainer/VBoxContainer/HBoxContainer/EnergyValue");
    _energyBar = GetNode<ProgressBar>("Energy/MarginContainer/VBoxContainer/HBoxContainer/EnergyBar");

    #region energy UI
    var currentEnergy = (Player as Player).CurrentEnergy;
    var totalEnergy = (Player as Player).TotalEnergy;

    _energyValue.Text = $"{(Player as Player).CurrentEnergy}";
    _energyBar.MaxValue = totalEnergy;
    _energyBar.Value = (double)currentEnergy;
    #endregion
  }

  /// <summary>
  /// Called on each frame
  /// </summary>
  /// <param name="delta"></param>
  public override void _Process(double delta)
  {
    _speedLabel.Text = $"{Player.LinearVelocity.Length():F3} µ/s";

    // show the player's direction
    _directionArrow.Rotation = Player.Rotation;
  }
}
