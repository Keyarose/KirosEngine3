#version 330 core
out vec4 FragColor;

in vec2 uV;

uniform sampler2D texture0;
uniform vec4 aColor;

void main()
{
	FragColor = texture(texture0, uV) * aColor;
}