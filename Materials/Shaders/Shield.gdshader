shader_type canvas_item;
//render_mode skip_vertex_transform;


// main shield color
uniform vec4 main_color : source_color = vec4(0.22, 0.66, 1.0, 0.5);
// main shield highlight color
uniform vec4 shield_color: source_color = vec4(1,1,1,1);

// total shield mask (circle)
uniform sampler2D alpha_mask;
// edges of the shield
uniform sampler2D glow_gradient;
// mask for the player's ship
uniform sampler2D ship_mask;
// shield texture effect
uniform sampler2D shield_noise : repeat_enable;
// shine puls gradient
uniform sampler2D shine_gradient : repeat_enable;
// puls animation speed
uniform float pulse_speed: hint_range(0.0, 1.0, 0.01) = 0.1;

// screen texture
uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;
// how much to warp
uniform float warp_factor: hint_range(0.1, 10.0, 0.01) = 0.5;
// model center
varying vec2 center;


void vertex() {
	// texture center in the screen space
	center = (CANVAS_MATRIX * MODEL_MATRIX[3]).xy;
}

// TODO: use vec4.rgb and vec4.a separately
void fragment() {
	vec4 final_col = main_color;

	vec4 shield_noise_tex = texture(shield_noise, UV);
	final_col = mix(final_col, shield_noise_tex, shield_noise_tex.r);
	
	// mask the parts outside of the circle
	float shipMask = texture(ship_mask, UV).r;
	final_col = vec4(final_col.r, final_col.g, final_col.b, shipMask);
	
	// shield glow mask
	vec4 mask = texture(ship_mask, UV) * texture(glow_gradient, UV);
	final_col = mix(final_col, shield_color, mask.r);
	
	// assign alpha
	float alpha = texture(alpha_mask, UV).r;
	
	// shield shine
	// animate the UV x
	vec3 shineTex = texture(shine_gradient,  vec2(UV.x + TIME * pulse_speed, UV.y)).rgb /*+ texture(glow_gradient, UV).rgb */;
	// apply slowly scrolling shine
	shineTex = final_col.rgb * shineTex.rgb;
	
	// combine the shining texture with the main one
	final_col = vec4(mix(final_col.rgb, shineTex, 0.1), shineTex.r * alpha);
	
	// warping/lensing effect
	vec2 center_uv = center * SCREEN_PIXEL_SIZE;
	vec2 shift_vec = SCREEN_UV - center_uv;
	// shift from the center by warp_factor amount
	vec3 screen = texture(screen_texture, center_uv + shift_vec * warp_factor).rgb;
	final_col.rgb = mix(final_col.rgb, screen, 0.5);
	
	COLOR.rgb = final_col.rgb;
	//COLOR.a *= shineTex.r * alpha * shipMask;
	COLOR.a *= alpha;
}

void light() {
	// Called for every pixel for every light affecting the CanvasItem.
}


