#version 330 core
in vec3 aPosition;
in vec2 aUV;

out vec2 uV;

uniform mat4 model;
uniform mat4 view;
uniform mat4 proj;

void main(void)
{
	uV = aUV;

	gl_Position = vec4(aPosition, 1.0f) * model * view * proj;
}