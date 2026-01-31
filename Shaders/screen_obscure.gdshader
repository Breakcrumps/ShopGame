shader_type canvas_item;

uniform float cur_feather = 2.4;

void fragment()
{
  vec2 center_dist = abs(UV - .5) * 2.;
  float max_dist = length(center_dist);

  float alpha = smoothstep(0., cur_feather, max_dist);

  COLOR = mix(vec4(0.), vec4(0, 0, 0, 1), alpha);
}