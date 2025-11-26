shader_type spatial;
render_mode unshaded;

uniform vec3 albedo : source_color = vec3(.45, .36, .33);
uniform vec3 k_per_axis = vec3(.3, 1., .5);

vec3 abs_v(vec3 v)
{
  return vec3(abs(v.x), abs(v.y), abs(v.z));
}

int max_i(vec3 v)
{
  if (v.x >= v.y && v.x >= v.z)
    return 0;
  else if (v.y >= v.x && v.y >= v.z)
    return 1;
  return 2;
}

void fragment()
{
  vec3 normal = abs_v((INV_VIEW_MATRIX * vec4(NORMAL, 0.)).xyz);
  ALBEDO = albedo * k_per_axis[max_i(normal)];
}