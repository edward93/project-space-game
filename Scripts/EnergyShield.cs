using Godot;
using System;

public partial class EnergyShield : Node2D
{

  /// <summary>
  /// Shield collider
  /// </summary>
  private CollisionShape2D _shieldCollider;

  /// <summary>
  /// Shield texture/sprite
  /// </summary>
  private Sprite2D _shieldSprite;

  /// <summary>
  /// Shield is enabled or not
  /// </summary>
  private bool _shieldEnabled = false;

  /// <summary>
  /// Init
  /// </summary>
  public override void _Ready()
  {
    _shieldSprite = GetNode<Sprite2D>("ShieldSprite");
    _shieldCollider = GetNode<CollisionShape2D>("Shield/ShieldCollider");
    _shieldEnabled = false;


  }
  /// <summary>
  /// Handle input events
  /// </summary>
  /// <param name="inputEvent">Input event</param>
  public override void _Input(InputEvent inputEvent)
  {
    if (inputEvent.IsActionPressed("Toggle Shield"))
    {
      HandleShiedInput();
    }
  }

  /// <summary>
  /// Frame process
  /// </summary>
  /// <param name="delta"></param>
  public override void _Process(double delta)
  {
  }

  /// <summary>
  /// Handles shield toggle
  /// </summary>
  private void HandleShiedInput()
  {
    // toggle shield
    if (_shieldEnabled) DisableShield();
    else EnableShield();
    // toggle shield
    _shieldEnabled = !_shieldEnabled;
  }

  /// <summary>
  /// Enables the shield
  /// </summary>
  private void EnableShield()
  {
    _shieldCollider.Disabled = false;
    _shieldSprite.Visible = false;
  }

  /// <summary>
  /// Disable the shield
  /// </summary>
  private void DisableShield()
  {
    _shieldCollider.Disabled = true;
    _shieldSprite.Visible = true;
  }
}
