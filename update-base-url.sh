#!/bin/bash
sed -i 's|<base href="/" />|<base href="/ConwayBlazorWASM/" />|' $1
sed -i 's|<link href="/css|<link href="/ConwayBlazorWASM/css|' $1
sed -i 's|<script src="/_content|<script src="/ConwayBlazorWASM/_content|' $1
sed -i 's|<script src="/_framework|<script src="/ConwayBlazorWASM/_framework|' $1