#version 330 core
in vec2 aPosition;
in vec2 aUV;

out vec2 uV;

uniform mat4 model;
uniform mat4 proj;

void main()
{
	uV = aUV;
	gl_Position = vec4(aPosition, 0.0, 1.0) * model * proj;
}