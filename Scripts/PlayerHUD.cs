using Godot;
using System;

public partial class PlayerHUD : CanvasLayer
{
  [Export]
  private Player Player = new Player();

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
    UpdateEnergyUI();
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

    // update energy bar
    UpdateEnergyUI();
  }

  /// <summary>
  /// Update energy values on the UI
  /// </summary>
  private void UpdateEnergyUI()
  {
    var currentEnergy = Player.CurrentEnergy;
    var totalEnergy = Player.TotalEnergy;

    _energyValue.Text = $"{Player.CurrentEnergy:F2}";
    _energyBar.MaxValue = totalEnergy;
    _energyBar.Value = (double)currentEnergy;
  }
}
