using Godot;
using System;

public partial class LaserWeapon : Node2D
{
  #region properties
  private const string LASER_STATS = "Laser Stats";
  /// <summary>
  /// How strong is the laser. This will determine how much damage is done
  /// </summary>
  [ExportGroup(LASER_STATS)]
  [Export]
  private float LaserPower = 1f;
  /// <summary>
  /// Laser range
  /// </summary>
  [ExportGroup(LASER_STATS)]
  [Export]
  private float LaserRange = 10000f;
  /// <summary>
  /// Full power laser width
  /// </summary>
  [ExportGroup(LASER_STATS)]
  [Export(PropertyHint.Range, "2.0,30.0,0.1")]
  private float FullPowerLaserWidth = 15.0f;

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
  private RayCast2D _laser;

  /// <summary>
  /// Tween object for beam animation
  /// </summary>
  private Tween _beamTween;
  #endregion
  /// <summary>
  /// Called once
  /// </summary>
  public override void _Ready()
  {
    _laserBeam = GetNode<Line2D>("RayCast2D/LaserBeam");
    _laser = GetNode<RayCast2D>("RayCast2D");

    // init beam tween once
    // _beamTween = CreateTween();

    // set laser range
    _laser.TargetPosition = new Vector2(LaserRange, 0);
    _laserBeam.SetPointPosition(1, _laser.TargetPosition);
    _laserBeam.Width = 0;

    // update laser color
    (_laserBeam.Material as ShaderMaterial).SetShaderParameter("main_color", LaserColor);
  }

  /// <summary>
  /// Called every frame
  /// </summary>
  /// <param name="delta"></param>
  public override void _Process(double delta)
  {
    // handlers go here
    // weapon fire handler
    HandleWeaponFire(delta);
  }

  /// <summary>
  /// Handles weapon fire
  /// </summary>
  /// <param name="delta">GD delta</param>
  private void HandleWeaponFire(double delta)
  {
    // tween for beam animation
    // var tween = GetTree().CreateTween();
    if (Input.IsActionPressed("Fire")) FireStart(delta);
    else FireEnd(delta);
  }

  /// <summary>
  /// Starts the laser
  /// </summary>
  /// <param name="delta">GD Delta</param>
  private void FireStart(double delta)
  {
    // if (tween.IsRunning()) tween.Stop();
    var tween = CreateTween();
    // enable the ray cast
    _laser.Enabled = true;
    // animate
    tween.TweenProperty(_laserBeam, "width", FullPowerLaserWidth, 0.1);

    // check if the laser is hitting any object
    if (_laser.IsColliding())
    {
      // set the beam position to the collision point
      _laserBeam.SetPointPosition(1, ToLocal(_laser.GetCollisionPoint()));
      // TODO: calc damage
    }
    else
    {
      // if no object is being hit reset the laser beam
      _laserBeam.SetPointPosition(1, _laser.TargetPosition);
    }
  }

  /// <summary>
  /// Stops the laser
  /// </summary>
  /// <param name="delta">GD delta</param>
  private void FireEnd(double delta)
  {
    var tween = CreateTween();
    // disable the laser (ray cast)
    _laser.Enabled = false;
    // animate
    tween.TweenProperty(_laserBeam, "width", 0, 0.1);

  }
}
