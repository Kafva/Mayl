// To compile run:
//  npx webpack bundle --config webpack.config.js
//  <==>
//  npm run build
// do not install webpack globally

// webpack is a tool which enables JS written with require() syntax to be
// compiled into a dist/ version, i.e. "a static module bundler for modern JavaScript applications"
const path = require('path');
const VueLoaderPlugin = require('vue-loader/lib/plugin')

module.exports = {
  mode: 'development',
  // We can specify the imports inside index.js instead of listing them here
  entry: './src/index.js',
  output: {
    path: path.resolve(__dirname) + '/public/dist',
    // This will produce one .js file per entrypoint instead of a single bundle.js
    filename: 'bundle.js'
  },

 // Resolve 'vue' in imports from index.js
 resolve: {
      alias: { 'vue$': 'vue/dist/vue.esm.js', 'vue-select': 'vue-select/dist/vue-select.js' },
      extensions: ['*', '.js', '.vue', '.json']
  }, 
  // A core feature in webpack compared to alternatives like browserify + gulp is that supports bundling
  // other filetypes than .js including .css, .png, .ts etc. using 'loaders', i.e. TypeScript can be translated to .js
  module: {
    rules: [
      { test: /\.js$/, use: ['babel-loader'] },
      /* Note that vue-style-loader conflicts with the regular style loader */
      { test: /\.css$/, use: ['style-loader', 'css-loader'] },
      { test: /\.ttf$/, use: ['file-loader'] },
      { test: /\.vue$/, use: ['vue-loader'] },
      {
        test: /\.(png|svg|jpg|jpeg|gif)$/i,
        type: 'asset/resource',
      },
    ]
  },
  
  // The VueLoaderPlugin is needed to properly handle compilation of .vue files
  plugins: [ new VueLoaderPlugin() ] 
};
