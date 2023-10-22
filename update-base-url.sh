#!/bin/bash
sed -i 's|<base href="/" />|<base href="/ConwayBlazorWASM/" />|' $1
sed -i 's|<link href="/css|<link href="https://firecracker37.github.io/ConwayBlazorWASM/css|' $1
sed -i 's|<script src="/_content|<script src="https://firecracker37.github.io/ConwayBlazorWASM/_content|' $1
sed -i 's|<script src="/_framework|<script src="https://firecracker37.github.io/ConwayBlazorWASM/_framework|' $1