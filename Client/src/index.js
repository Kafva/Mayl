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
import EmailDisplay from '../components/EmailDisplay.vue';

var root = new Vue({
    el: "#root",
    
    data: 
    { 
        background: Bkg,
    },

    components: 
    { 
        bar: Bar,
        'email-table': EmailTable,
        'email-display': EmailDisplay
    },
})
