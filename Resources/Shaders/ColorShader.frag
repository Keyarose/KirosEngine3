#version 330 core
out vec4 FragColor;

in vec4 vertColor;

uniform vec4 progColor;

void main()
{
	FragColor = vertColor;
}