#!/bin/sh
which node &> /dev/null || { source /usr/local/opt/nvm/nvm.sh && nvm use node ; }

gfind . -path ./node_modules -prune -false -path ./public/dist -prune -false -o -regex ".*\.\(js\|vue\|html\|json\|css\)" | 
	entr -n -s 'npm run build'
