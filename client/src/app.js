// Scope:
//      * Click to view body      --> (Hide table)
//      * Click to delete/archive --> (Update table)
// ----------------------------- //
// Import statements will trigger webpack to include the given CSS into the resulting bundle
import './style.css';
import './nerd-fonts.min.css';
import Vue from 'vue';

// Assests included in the packing process will be given random names
// and we can therefore not reference them statically in the .html
import Bkg from '../assets/background.jpg';

// The import statements constitute the 'registration' process for components when using webpack
import Bar from        '../components/Bar.vue';
import EmailTable from '../components/EmailTable.vue';

/* Set the background **/
var bkg = new Vue({
    el: "#bkg",
    data: { background: Bkg }
});

/* Manage the bar */
var bar = new Vue({
    el: "#bar",
    components: { bar: Bar }
});

/* Manage email display */
var emails = new Vue(
    {
        // To use a component in our HTML we need to create a Vue() object
        // which has the <component> as a child (based on its 'el:' selector)
        el: '#emails',
        components:
        {
            'email-table': EmailTable,
        }
    }
);


//var test = new Vue({
//    el: '#bar', 
//    data: { x: 1 },
//    methods: 
//    { 
//        wow: function(){ console.log("woooow"); return 3; } 
//    } 
//
//});


// Vue apps start with a root Vue instance from where sub 'components' can be imported from .vue
// files, each component is in essence its own Vue() instance
//var table = new Vue({ 
//    // Note that root element can't have 'v-' attributes
//    el: '#mailList > tbody',
//    data:
//    // Each attribute is added to the 'reactivity' system
//    // and changes to them will be reflected in the UI
//    {
//        threads: null,
//        //threads: [
//        //    {"threadId":"17301e393da5279e","snippet":"Hey Dlink","date":"2020-06-29T23:02:35+02:00"}
//        //]
//    },
//    methods:
//    {
//       displayBody: function (event) 
//        { 
//           console.log(event); 
//           return Functions.getBodyOfMessage(); 
//        } 
//    }
//});

// To use a component in our HTML we need to create a Vue() object
// which has the <component> as a child
//Vue.component('hello', require('../components/Hello.vue').default);
//new Vue({ el: "#bar"});

//(async () =>
//{
//    let x = await Functions.fetchThreads();
//    table.$data.threads = x;
//})();

// We can access Vue components using
// vm.$el (the root element of the object)
// vm.$data
// vm.$mount(el)

// 'created' and 'destroyed' are other attributes which can be set as hooks (don't use arrow functions with these hooks)

