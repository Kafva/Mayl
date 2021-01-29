// webpack is a tool which enables JS written with require() syntax to be
// compiled into a dist/ version, i.e. "a static module bundler for modern JavaScript applications"
const path = require('path');

// A core feature in webpack compared to alternatives like browserify + gulp

module.exports = {
  entry: './src/index.js',
  output: {
    path: path.resolve(__dirname, 'dist'),
    filename: 'bundle.js'
  }
};
