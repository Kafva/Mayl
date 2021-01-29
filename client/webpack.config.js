// To compile run:
//  npx webpack bundle --config webpack.config.js
// do not install webpack globally

// webpack is a tool which enables JS written with require() syntax to be
// compiled into a dist/ version, i.e. "a static module bundler for modern JavaScript applications"
const path = require('path');

const VueLoaderPlugin = require('vue-loader/lib/plugin')

module.exports = {
  mode: 'development',
  // One can specify each dependency and their relationship
  // or simply create a single bundle
  entry: './App.vue',
  //entry: {
  //  functions: './src/functions.js',
  //  app: {
  //    dependOn: 'functions',
  //    import: './src/app.js'
  //  },
  //},
  output: {
    path: path.resolve(__dirname) + '/dist',
    // This will produce one .js file per entrypoint instead of a single bundle.js
    filename: 'bundle.js'
  },
  // A core feature in webpack compared to alternatives like browserify + gulp is that supports bundling
  // other filetypes than .js including .css, .png, .ts etc. using 'loaders', i.e. TypeScript can be translated to .js
  module: {
    rules: [
      { test: /\.js$/, use: ['babel-loader'] },
      { test: /\.css$/, use: ['vue-style-loader', 'css-loader'] },
      { test: /\.ttf|png|jpg$/, use: ['file-loader'] },
      { test: /\.vue$/, use: ['vue-loader'] }
    ]
  },
  // Loaders aid in bundling different filetypes and Plugins 
  // support other features like optimization and asset managment
  // The VueLoaderPlugin is needed to properly handle compilation of .vue files
  plugins: [ new VueLoaderPlugin() ] 
};