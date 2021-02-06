#!/bin/sh
gfind . -path ./node_modules -prune -false -path ./public/dist -prune -false -o -regex ".*\.\(js\|vue\|html\|json\|css\)" | 
	entr -n -s 'npm run build'
