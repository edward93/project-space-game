using Godot;
using System;

public partial class LaserWeapon : Node2D
{
  /// <summary>
  /// Laser Color
  /// </summary>
  [Export]
  private Color LaserColor = Colors.Red;
  /// <summary>
  /// Laser bream. Should be moved into a separate script
  /// </summary>
  private Line2D _laserBeam;

  /// <summary>
  /// Ray cast for the laser beam
  /// </summary>
  private RayCast2D _laserRayCast;
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    _laserBeam = GetNode<Line2D>("RayCast2D/LaserBeam");
    _laserRayCast = GetNode<RayCast2D>("RayCast2D");

    // update laser color
    (_laserBeam.Material as ShaderMaterial).SetShaderParameter("main_color", LaserColor);

    _laserBeam.SetPointPosition(1, _laserRayCast.TargetPosition);
    _laserBeam.Width = 0;
  }

  /// <summary>
  /// Called every frame
  /// </summary>
  /// <param name="delta"></param>
  public override void _Process(double delta)
  {
    HandleWeaponFire(delta);
  }

  /// <summary>
  /// Handles weapon fire
  /// </summary>
  /// <param name="delta"></param>
  private void HandleWeaponFire(double delta)
  {
    var tween = GetTree().CreateTween();
    if (Input.IsActionPressed("Fire"))
    {
      tween.TweenProperty(_laserBeam, "width", 15, 0.1);
    }
    else
    {
      tween.TweenProperty(_laserBeam, "width", 0, 0.1);
    }
  }
}
