shader_type spatial;
render_mode unshaded;

vec3 abs_v(vec3 vector)
{
  return vec3(abs(vector.x), abs(vector.y), abs(vector.z));
}

void fragment()
{
  vec3 surface_normal = normalize((INV_VIEW_MATRIX * vec4(NORMAL, 0.0)).xyz);
  ALBEDO = abs_v(surface_normal);
}