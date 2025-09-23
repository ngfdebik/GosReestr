# Vue js-xlsx ![Current version](https://img.shields.io/badge/dynamic/json.svg?label=version&url=https%3A%2F%2Fraw.githubusercontent.com%2Fmagr0s%2Fvue-js-xlsx%2Fmaster%2Fpackage.json&query=version&colorB=orange&style=flat-square) 

![Vue.js version](https://img.shields.io/badge/dynamic/json.svg?label=vue.js&url=https%3A%2F%2Fraw.githubusercontent.com%2Fmagr0s%2Fvue-js-xlsx%2Fmaster%2Fpackage.json&query=dependencies.vue&colorB=blue&style=flat-square)
![xlsx version](https://img.shields.io/badge/dynamic/json.svg?label=xlsx&url=https%3A%2F%2Fraw.githubusercontent.com%2Fmagr0s%2Fvue-js-xlsx%2Fmaster%2Fpackage.json&query=dependencies.xlsx&colorB=blue&style=flat-square)
![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?&style=flat-square)

Vue.js plugin for [SheetJs js-xlsx](http://sheetjs.com/)

## Install

#### NPM
```
npm i vue-js-xlsx --save
```

## Usage

#### mount with global
```js
import VueXlsx from 'vue-js-xlsx'
Vue.use(VueXlsx)
```
#### mount with nuxt.js/ssr
```js
// plugins/vue-js-xlsx.js
import VueXlsx from 'vue-js-xlsx'
Vue.use(VueXlsx)

// nuxt.config.js
{
  ...
  plugins: [{
    src: '~plugins/vue-js-xlsx.js',
    ssr: false
  }]
  ...
}
```
Once installed, the plugin add $xlsx to Vue.prototype to make him easily accessibles in every components.

## Documentation

See [SheetJs](https://github.com/sheetjs/js-xlsx)

**Methods**

```js
/**
 * Generates different types of JS objects
 * @param {Blob} data
 * @param {Object} options
 * - parsingOpts {Object} See https://github.com/sheetjs/js-xlsx#parsing-options
 * - sheetIndex {Number} Select the sheet number you want to convert. Default: 0
 * - Others options. See https://github.com/sheetjs/js-xlsx#json
 * @returns {Array} array of objects

 */
const jsonData = this.$xlsx.toJson(data, options)
```

## Bonus
[Vue FileReader component](https://github.com/magr0s/vue-filereader)

## Development

#### Compiles and hot-reloads for development
```
npm run serve
```

#### Compiles and minifies for production
```
npm run build:lib
```

#### Lints and fixes files
```
npm run lint
```
### Author

[magr0s](https://github.com/magr0s)

### License

[MIT](https://github.com/magr0s/vue-js-xlsx/blob/master/LICENSE)
