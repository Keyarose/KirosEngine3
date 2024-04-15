#version 330 core

out vec4 outputColor;

in vec2 uV;

uniform sampler2D texture0;
uniform vec4 aTextColor;

void main()
{
	outputColor = texture(texture0, uV) * aTextColor;
}