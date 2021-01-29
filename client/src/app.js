import * as Functions from './functions.js'

// This import statement will trigger webpack to include style.css into the resulting bundle
import css from './style.css'

// A Vue app will attach itself to an HTML node which we can control
// with a more reactive API compared to plain JS

// To link up elements special HTML attributes prefixed with 'v-' called directives are used
// 'v-on' == @                                 is used to add eventListeners to nodes
// 'v-bind:<attrName>="vm.$data.x"  == :<name> is used to dynamically link HTML attributes

// Vue apps start with a root Vue instance from where sub 'components' can be created
var vm = new Vue({ 
    // Note that root element can't have 'v-' attributes
    el: '#mailList > tbody',
    data:
    // Each attribute is added to the 'reactivity' system
    // and changes to them will be reflected in the UI
    {
        threads: null,
        //threads: [
        //    { threadId: null, snippet: null, emails:[{sender: null, date: null}] } 
        //]
        threads: [{"threadId":"17301e393da5279e","snippet":"Hey Blink, Thanks for registering for an account on Discord! Before we get started, we just need to ","emails":[{"subject":"Verify Email Address for Discord","body":"","sender":"Discord \u003Cnoreply@discordapp.com\u003E","date":"2020-06-29T23:02:35+02:00"}]}]
    }
});


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

