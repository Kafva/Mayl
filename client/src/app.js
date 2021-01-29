// Import statements will trigger webpack to include the given CSS into the resulting bundle
import './style.css'
import './nerd-fonts.min.css'

import * as Functions from './functions.js'
import Vue from 'vue'

// Assests included in the packing process will be given random names
// and we can therefore not reference them statically in the .html
import Bkg from '../assets/background.jpg'

// Set the background
new Vue({
    el: "#bkg",
    data: { background: Bkg }
});

// To use a component in our HTML we need to create a Vue() object
// which has the <component> as a child
//Vue.component('hello', require('../components/Hello.vue').default);
//new Vue({ el: "#bar"});

// To link up elements special HTML attributes prefixed with 'v-' called directives are used
// 'v-on' == @                                 is used to add eventListeners to nodes
// 'v-bind:<attrName>="vm.$data.x"  == :<name> is used to dynamically link HTML attributes

// Vue apps start with a root Vue instance from where sub 'components' can be created
var table = new Vue({ 
    // Note that root element can't have 'v-' attributes
    el: '#mailList > tbody',
    data:
    // Each attribute is added to the 'reactivity' system
    // and changes to them will be reflected in the UI
    {
        threads: null,
        //threads: [{"threadId":"17301e393da5279e","snippet":"Hey Blink, Thanks for registering for an account on Discord! Before we get started, we just need to ","emails":[{"subject":"Verify Email Address for Discord","body":"","sender":"Discord \u003Cnoreply@discordapp.com\u003E","date":"2020-06-29T23:02:35+02:00"}]}],
    }
});

//Vue.component('button-counter', {
//  data: function () {
//    return {
//      count: 0
//    }
//  },
//  template: '<button v-on:click="count++">You clicked me {{ count }} times.</button>'
//})
//
//new Vue({ el: '#components-demo' })



//(async () =>
//{
//    let x = await Functions.fetchThreads();
//    vm.$data.threads = x;
//})();

// We can access Vue components using
// vm.$el (the root element of the object)
// vm.$data
// vm.$mount(el)

// 'created' and 'destroyed' are other attributes which can be set as hooks (don't use arrow functions
// with these hooks)

