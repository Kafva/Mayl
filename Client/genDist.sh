#!/bin/sh
. /usr/local/opt/nvm/nvm.sh
. /usr/local/opt/nvm/etc/bash_completion.d/nvm
nvm use node
npm run build
