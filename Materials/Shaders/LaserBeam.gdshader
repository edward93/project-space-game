shader_type canvas_item;
uniform vec4 main_color : source_color = vec4(0.22, 0.66, 1.0, 1.0);
uniform sampler2D glow_gradient;
uniform sampler2D beam_central_part;
uniform vec4 laser_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	vec4 final_col = main_color;
	
	// Sample the gradient texture
    float gradientValue = texture(glow_gradient, UV).r;
	
	// add glow color
	final_col = vec4(final_col.rgb, gradientValue * main_color.a);
	
	float laser_color_mask = texture(beam_central_part, UV).r;
	// add central bright white laser color
	final_col = mix(final_col, laser_color, laser_color_mask);
	
	COLOR = final_col;
}

void light() {
	// Called for every pixel for every light affecting the CanvasItem.
}
