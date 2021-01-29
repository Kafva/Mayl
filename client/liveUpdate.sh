#!/bin/sh
gfind . -path ./node_modules -prune -false -o -regex ".*\.\(js\|vue|html|json\)" | 
	entr -n -s 'npm run build'
