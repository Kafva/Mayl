#!/bin/sh
gfind . -path ./node_modules -prune -false -path ./public/dist -prune -false -o -regex ".*\.\(js\|vue\|html\|json\)" | 
	entr -n -s 'npm run build'
